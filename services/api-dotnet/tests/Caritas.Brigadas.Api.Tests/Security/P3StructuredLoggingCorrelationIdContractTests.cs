using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3StructuredLoggingCorrelationIdContractTests
{
    [Fact]
    public void CorrelationIdMiddleware_ValidatesIncomingCorrelationId()
    {
        var source = File.ReadAllText(GetMiddlewarePath("CorrelationIdMiddleware.cs"));

        var requiredTokens = new[]
        {
            "public const string HeaderName = \"X-Correlation-Id\";",
            "MaxCorrelationIdLength",
            "IsValidCorrelationId",
            "IsAllowedCorrelationIdCharacter",
            "char.IsAsciiLetterOrDigit",
            "value is '-' or '_' or '.' or ':'",
            "context.Items[HeaderName] = correlationId;",
            "context.Response.Headers[HeaderName] = correlationId;",
            "return context.TraceIdentifier;"
        };

        AssertRequiredTokens(source, requiredTokens, "CorrelationIdMiddleware");
    }

    [Fact]
    public void RequestTelemetryMiddleware_UsesStructuredScopeAndCorrelationId()
    {
        var source = File.ReadAllText(GetMiddlewarePath("RequestTelemetryMiddleware.cs"));

        var requiredTokens = new[]
        {
            "using Caritas.Brigadas.Api.Extensions;",
            "context.GetCorrelationId()",
            "SanitizePath(context.Request.Path)",
            "BeginScope(new Dictionary<string, object?>",
            "[\"CorrelationId\"]",
            "[\"RequestId\"]",
            "[\"HttpMethod\"]",
            "[\"EndpointRoute\"]",
            "[\"StatusCode\"]",
            "[\"ElapsedMilliseconds\"]",
            "StatusCodes.Status500InternalServerError",
            "StatusCodes.Status400BadRequest",
            "LogInformation",
            "LogWarning",
            "LogError"
        };

        AssertRequiredTokens(source, requiredTokens, "RequestTelemetryMiddleware");
    }

    [Fact]
    public void RequestTelemetryMiddleware_SanitizesSensitiveRoutes()
    {
        var source = File.ReadAllText(GetMiddlewarePath("RequestTelemetryMiddleware.cs"));

        var requiredTokens = new[]
        {
            "SensitivePathSegments",
            "patients",
            "patient-visits",
            "service-encounters",
            "form-responses",
            "consent-documents",
            "sync-batches",
            "/api/v1/[sensitive-resource]"
        };

        AssertRequiredTokens(source, requiredTokens, "RequestTelemetryMiddleware sensitive route sanitization");

        Assert.DoesNotContain("Request.QueryString", source, StringComparison.Ordinal);
        Assert.DoesNotContain("Request.Body", source, StringComparison.Ordinal);
        Assert.DoesNotContain("PayloadJson", source, StringComparison.Ordinal);
    }

    [Fact]
    public void StructuredLoggingCorrelationIdBaseline_DefinesProductionScope()
    {
        var source = File.ReadAllText(GetOperationsDocPath("P3_STRUCTURED_LOGGING_CORRELATION_ID_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Structured Logging and Correlation ID Baseline",
            "X-Correlation-Id",
            "MaxCorrelationIdLength",
            "safe ASCII characters",
            "CorrelationId",
            "RequestId",
            "HttpMethod",
            "EndpointRoute",
            "StatusCode",
            "ElapsedMilliseconds",
            "/api/v1/[sensitive-resource]",
            "Information for successful responses below 400",
            "Warning for responses from 400 to 499",
            "Error for responses 500 or greater",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 structured logging baseline");
    }

    [Fact]
    public void StructuredLoggingCorrelationIdVerifier_RequiresRuntimeEvidence()
    {
        var source = File.ReadAllText(GetScriptPath("verify-p3-structured-logging-correlation-id.ps1"));

        var requiredTokens = new[]
        {
            "P3 structured logging and correlation id verification passed.",
            "P3_STRUCTURED_LOGGING_CORRELATION_ID_BASELINE.md",
            "CorrelationIdMiddleware.cs",
            "RequestTelemetryMiddleware.cs",
            "HttpContextExtensions.cs",
            "context.GetCorrelationId()",
            "BeginScope(new Dictionary<string, object?>",
            "/api/v1/[sensitive-resource]"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 structured logging verifier");
    }

    [Fact]
    public void RepositoryGovernanceBaseline_RunsStructuredLoggingCorrelationIdVerifier()
    {
        var source = File.ReadAllText(GetScriptPath("validate-repo-governance-baseline.ps1"));

        Assert.Contains(
            "verify-p3-structured-logging-correlation-id.ps1",
            source,
            StringComparison.Ordinal);
    }

    private static void AssertRequiredTokens(
        string source,
        IReadOnlyCollection<string> requiredTokens,
        string label)
    {
        var failures = requiredTokens
            .Where(token => !source.Contains(token, StringComparison.Ordinal))
            .Select(token => $"{label} is missing required token: {token}")
            .ToArray();

        Assert.True(
            failures.Length == 0,
            $"{label} is incomplete." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    private static string GetMiddlewarePath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            "Middleware",
            fileName);
    }

    private static string GetOperationsDocPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "docs",
            "operations",
            fileName);
    }

    private static string GetScriptPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "scripts",
            fileName);
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
