using System.Net;
using System.Globalization;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Caritas.Brigadas.Api.Health;
using Microsoft.AspNetCore.HttpOverrides;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Api.Middleware;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

const string CorsPolicyName = "ConfiguredOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto |
        ForwardedHeaders.XForwardedHost;

    var knownProxies = builder.Configuration
        .GetSection("ReverseProxy:ForwardedHeaders:KnownProxies")
        .Get<string[]>() ?? [];

    foreach (var knownProxy in knownProxies)
    {
        if (IPAddress.TryParse(knownProxy, out var address))
        {
            options.KnownProxies.Add(address);
        }
    }

    var knownNetworks = builder.Configuration
        .GetSection("ReverseProxy:ForwardedHeaders:KnownIPNetworks")
        .Get<string[]>() ?? [];

    foreach (var knownNetwork in knownNetworks)
    {
        var parts = knownNetwork.Split(
            '/',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length != 2)
        {
            continue;
        }

        if (!IPAddress.TryParse(parts[0], out var prefix))
        {
            continue;
        }

        if (!int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var prefixLength))
        {
            continue;
        }

        options.KnownIPNetworks.Add(new System.Net.IPNetwork(prefix, prefixLength));
    }
});

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile(
        "appsettings.Local.json",
        optional: true,
        reloadOnChange: true);
}

builder.ValidateProductionConfiguration();

var maxRequestBodyBytes = builder.Configuration.GetValue<long?>(
    "Security:MaxRequestBodyBytes") ?? 5L * 1024L * 1024L;

builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
    options.Limits.MaxRequestBodySize = maxRequestBodyBytes;
});

var rateLimitingEnabled = builder.Configuration.GetValue(
    "Security:RateLimiting:Enabled",
    true);

var rateLimitingPermitLimit = builder.Configuration.GetValue(
    "Security:RateLimiting:PermitLimit",
    100);

var rateLimitingWindowMinutes = builder.Configuration.GetValue(
    "Security:RateLimiting:WindowMinutes",
    1);

var rateLimitingQueueLimit = builder.Configuration.GetValue(
    "Security:RateLimiting:QueueLimit",
    0);

builder.Services.AddCurrentUserContext();
builder.Services.AddAuditLogging();
builder.Services.AddClinicalWriteAudit();
builder.Services.AddOperationalWriteAudit();
builder.Services.AddCaritasAuthenticationOptions(builder.Configuration);
builder.Services.AddConfiguredAuthentication(builder.Configuration, builder.Environment);
builder.Services.AddPermissionAuthorization();
builder.Services.AddOrganizationAccessEnforcement();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = false;
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var details = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .SelectMany(entry => entry.Value!.Errors.Select(error => new ApiErrorDetail(
                entry.Key,
                string.IsNullOrWhiteSpace(error.ErrorMessage)
                    ? "Invalid value."
                    : error.ErrorMessage,
                ApiErrorCodes.ValidationError)))
            .ToArray();

        var response = ApiErrorResponse.Create(
            ApiErrorCodes.ValidationError,
            "Validation failed.",
            context.HttpContext.TraceIdentifier,
            details);

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>() ?? [];

    options.AddPolicy(CorsPolicyName, policy =>
    {
        if (allowedOrigins.Length == 0)
        {
            policy
                .WithOrigins(
                    "http://localhost:3000",
                    "https://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod();

            return;
        }

        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

if (rateLimitingEnabled)
{
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        {
            var partitionKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip";

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey,
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = rateLimitingPermitLimit,
                    Window = TimeSpan.FromMinutes(rateLimitingWindowMinutes),
                    QueueLimit = rateLimitingQueueLimit,
                    AutoReplenishment = true
                });
        });
    });
}

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCaritasSwagger();
builder.Services.AddProblemDetails();
builder.Services
    .AddHealthChecks()
    .AddCheck(
        "api-live",
        () => HealthCheckResult.Healthy("API process is running."),
        tags: new[] { "live" })
    .AddCheck<DatabaseConnectivityHealthCheck>(
        "database",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "ready" });

var app = builder.Build();

app.UseForwardedHeaders();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestTelemetryMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

if (rateLimitingEnabled)
{
    app.UseRateLimiter();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

var enableSwaggerInDevelopment = builder.Configuration
    .GetValue("Features:EnableSwaggerInDevelopment", true);

if (app.Environment.IsDevelopment() && enableSwaggerInDevelopment)
{
    app.UseCaritasSwagger();
}
app.UseHttpsRedirection();

app.UseCors(CorsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks(
    "/health/live",
    new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("live"),
        ResponseWriter = HealthCheckResponseWriter.WriteAsync
    })
.WithName("HealthLive")
.WithTags("Health");

app.MapHealthChecks(
    "/health/ready",
    new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = HealthCheckResponseWriter.WriteAsync
    })
.WithName("HealthReady")
.WithTags("Health");

app.MapGet("/", (HttpContext httpContext) =>
{
    var payload = new
    {
        service = "caritas-brigadas-api",
        name = "Cáritas Brigadas de Salud API",
        status = "running",
        environment = app.Environment.EnvironmentName,
        timestampUtc = DateTimeOffset.UtcNow
    };

    return Results.Ok(ApiResponse<object>.Ok(
        payload,
        httpContext.GetCorrelationId()));
})
.WithName("Root")
.WithTags("System")
.Produces<ApiResponse<object>>(StatusCodes.Status200OK);

app.Run();

public partial class Program
{
}
