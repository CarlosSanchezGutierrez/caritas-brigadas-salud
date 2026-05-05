using System.Text.Json.Serialization;
using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Api.Middleware;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Infrastructure;
using Microsoft.AspNetCore.Mvc;

const string CorsPolicyName = "ConfiguredOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCurrentUserContext();
builder.Services.AddCaritasAuthenticationOptions(builder.Configuration);
builder.Services.AddDevelopmentAuthentication(builder.Environment);
builder.Services.AddPermissionAuthorization();
builder.Services.AddOrganizationAccessEnforcement();

builder.Configuration.AddJsonFile(
    "appsettings.Local.json",
    optional: true,
    reloadOnChange: true);

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

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCaritasSwagger();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

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

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

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





