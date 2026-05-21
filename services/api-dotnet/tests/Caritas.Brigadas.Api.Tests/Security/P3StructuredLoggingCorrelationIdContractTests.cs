using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3StructuredLoggingCorrelationIdContractTests
{
    [Fact]
    public void Program_RegistersCorrelationIdBeforeRequestTelemetry()
    {
        var program = File.ReadAllText(GetRepoPath(
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            "Program.cs"));

        Assert.Contains("app.UseMiddleware<CorrelationIdMiddleware>();", program, StringComparison.Ordinal);
        Assert.Contains("app.UseMiddleware<RequestTelemetryMiddleware>();", program, StringComparison.Ordinal);

        var correlationIndex = program.IndexOf(
            "app.UseMiddleware<CorrelationIdMiddleware>();",
            StringComparison.Ordinal);

        var telemetryIndex = program.IndexOf(
            "app.UseMiddleware<RequestTelemetryMiddleware>();",
            StringComparison.Ordinal);

        Assert.True(correlationIndex >= 0, "CorrelationIdMiddleware registration was not found.");
        Assert.True(telemetryIndex >= 0, "RequestTelemetryMiddleware registration was not found.");
        Assert.True(
            correlationIndex < telemetryIndex,
            "CorrelationIdMiddleware must run before RequestTelemetryMiddleware.");
    }

    [Fact]
    public void CorrelationIdMiddleware_ValidatesAndPropagatesCorrelationId()
    {
        var correlation = File.ReadAllText(GetRepoPath(
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            "Middleware",
            "CorrelationIdMiddleware.cs"));

        AssertRequiredTokens(
            correlation,
            new[]
            {
                "public const string HeaderName = \"X-Correlation-Id\";",
                "MaxCorrelationIdLength",
                "context.Items[HeaderName] = correlationId;",
                "context.Response.Headers[HeaderName] = correlationId;",
                "GetOrCreateCorrelationId",
                "IsValidCorrelationId",
                "IsAllowedCorrelationIdCharacter",
                "context.TraceIdentifier"
            },
            "CorrelationIdMiddleware");
    }

    [Fact]
    public void RequestTelemetryMiddleware_UsesStructuredScopeAndPropagatedCorrelationId()
    {
        var telemetry = File.ReadAllText(GetRepoPath(
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            "Middleware",
            "RequestTelemetryMiddleware.cs"));

        AssertRequiredTokens(
            telemetry,
            new[]
            {
                "using Caritas.Brigadas.Api.Extensions;",
                "using Microsoft.AspNetCore.Mvc.Controllers;",
                "context.GetCorrelationId()",
                "SanitizeForLog(context.GetCorrelationId())",
                "_logger.BeginScope(scopeProperties)",
                "\"CorrelationId\"",
                "\"RequestId\"",
                "\"HttpMethod\"",
                "\"EndpointRoute\"",
                "\"StatusCode\"",
                "\"ElapsedMilliseconds\"",
                "_logger.LogError",
                "_logger.LogWarning",
                "_logger.LogInformation"
            },
            "RequestTelemetryMiddleware");

        Assert.DoesNotContain("context.TraceIdentifier);", telemetry, StringComparison.Ordinal);
    }

    [Fact]
    public void RequestTelemetryMiddleware_ClassifiesEndpointTemplatesInsteadOfLoggingRawPaths()
    {
        var telemetry = File.ReadAllText(GetRepoPath(
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            "Middleware",
            "RequestTelemetryMiddleware.cs"));

        AssertRequiredTokens(
            telemetry,
            new[]
            {
                "GetSafeEndpointRouteForLog(context.GetEndpoint())",
                "ControllerActionDescriptor",
                "ClassifyEndpointTemplateForLog",
                "SensitiveEndpointTokens",
                "/api/v1/[sensitive-resource]",
                "/api/v1/organizations/[id]",
                "/api/v1/organizations/[id]/reports/[segment]",
                "/api/v1/audit-logs",
                "/api/v1/[endpoint]"
            },
            "RequestTelemetryMiddleware endpoint classification");

        Assert.DoesNotContain("SanitizePath(context.Request.Path)", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("SensitivePathSegments", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("rawPath.Split", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("return SanitizeForLog(rawPath);", telemetry, StringComparison.Ordinal);
    }

    [Fact]
    public void RequestTelemetryMiddleware_UsesSafeHttpMethodClassification()
    {
        var telemetry = File.ReadAllText(GetRepoPath(
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            "Middleware",
            "RequestTelemetryMiddleware.cs"));

        AssertRequiredTokens(
            telemetry,
            new[]
            {
                "GetSafeHttpMethodForLog(context.Request.Method)",
                "if (string.IsNullOrWhiteSpace(method))",
                "HttpMethods.IsGet(method)",
                "HttpMethods.IsPost(method)",
                "HttpMethods.IsPut(method)",
                "HttpMethods.IsPatch(method)",
                "HttpMethods.IsDelete(method)",
                "HttpMethods.IsHead(method)",
                "HttpMethods.IsOptions(method)",
                "HttpMethods.IsTrace(method)",
                "HttpMethods.IsConnect(method)",
                "return \"UNKNOWN\";"
            },
            "RequestTelemetryMiddleware HTTP method classification");

        Assert.DoesNotContain("NormalizeHttpMethodForLog", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("SanitizeForLog(NormalizeHttpMethodForLog(context.Request.Method))", telemetry, StringComparison.Ordinal);
        Assert.DoesNotContain("var normalizedMethod = method.Trim().ToUpperInvariant();", telemetry, StringComparison.Ordinal);
    }

    private static void AssertRequiredTokens(
        string source,
        IReadOnlyCollection<string> requiredTokens,
        string label)
    {
        var missingTokens = requiredTokens
            .Where(token => !source.Contains(token, StringComparison.Ordinal))
            .ToArray();

        Assert.True(
            missingTokens.Length == 0,
            label + " is incomplete." + Environment.NewLine +
            string.Join(
                Environment.NewLine,
                missingTokens.Select(token => $"{label} is missing required token: {token}")));
    }

    private static string GetRepoPath(params string[] parts)
    {
        return Path.Combine(
            new[]
            {
                FindRepositoryRoot()
            }.Concat(parts).ToArray());
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, ".git")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Repository root with .git directory was not found.");
    }
}
