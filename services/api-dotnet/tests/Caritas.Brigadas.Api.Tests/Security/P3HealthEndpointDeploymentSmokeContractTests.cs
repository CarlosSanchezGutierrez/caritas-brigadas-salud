using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3HealthEndpointDeploymentSmokeContractTests
{
    [Fact]
    public void Program_MapsLiveAndReadyHealthEndpointsWithJsonWriter()
    {
        var source = File.ReadAllText(GetApiSourcePath("Program.cs"));

        var requiredTokens = new[]
        {
            "AddCheck(",
            "\"api-live\"",
            "DatabaseConnectivityHealthCheck",
            "MapHealthChecks(",
            "\"/health/live\"",
            "\"/health/ready\"",
            "Predicate = check => check.Tags.Contains(\"live\")",
            "Predicate = check => check.Tags.Contains(\"ready\")",
            "ResponseWriter = HealthCheckResponseWriter.WriteAsync"
        };

        AssertRequiredTokens(source, requiredTokens, "Program.cs health endpoint implementation");
    }

    [Fact]
    public void HealthCheckResponseWriter_UsesSanitizedJsonAndCorrelationId()
    {
        var source = File.ReadAllText(GetHealthSourcePath("HealthCheckResponseWriter.cs"));

        var requiredTokens = new[]
        {
            "HealthCheckResponseWriter",
            "context.GetCorrelationId()",
            "status",
            "timestampUtc",
            "correlationId",
            "totalDurationMilliseconds",
            "checks",
            "durationMilliseconds",
            "JsonSerializer.SerializeAsync"
        };

        AssertRequiredTokens(source, requiredTokens, "HealthCheckResponseWriter");
    }

    [Fact]
    public void DatabaseConnectivityHealthCheck_UsesDbContextCanConnectAsync()
    {
        var source = File.ReadAllText(GetHealthSourcePath("DatabaseConnectivityHealthCheck.cs"));

        var requiredTokens = new[]
        {
            "DatabaseConnectivityHealthCheck",
            "CaritasDbContext",
            "CanConnectAsync",
            "Database connectivity check passed.",
            "Database connectivity check failed.",
            "HealthCheckResult.Healthy",
            "HealthCheckResult.Unhealthy"
        };

        AssertRequiredTokens(source, requiredTokens, "DatabaseConnectivityHealthCheck");
    }

    [Fact]
    public void HealthEndpointIntegrationTests_ValidateNoSensitiveHealthLeakage()
    {
        var source = File.ReadAllText(GetIntegrationTestPath("P3HealthEndpointIntegrationTests.cs"));

        var requiredTokens = new[]
        {
            "P3HealthEndpointIntegrationTests",
            "LiveHealthEndpoint_ReturnsJsonWithoutAuthentication",
            "ReadyHealthEndpoint_ReturnsDatabaseConnectivitySignalWithoutSensitiveData",
            "/health/live",
            "/health/ready",
            "X-Correlation-Id",
            "api-live",
            "database",
            "Database connectivity check passed.",
            "Assert.DoesNotContain(\"PayloadJson\", responseBody, StringComparison.OrdinalIgnoreCase)"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 health endpoint integration tests");
    }

    [Fact]
    public void DeploymentHealthSmokeScript_ValidatesLiveReadyAndRoot()
    {
        var source = File.ReadAllText(GetScriptPath("run-p3-deployment-health-smoke.ps1"));

        var requiredTokens = new[]
        {
            "CARITAS_DEPLOYMENT_SMOKE_BASE_URL",
            "health/live",
            "health/ready",
            "caritas-brigadas-api",
            "ConnectionStrings",
            "PayloadJson",
            "P3 deployment health smoke test passed."
        };

        AssertRequiredTokens(source, requiredTokens, "P3 deployment health smoke script");
    }

    [Fact]
    public void HealthEndpointDeploymentSmokeBaseline_DefinesImplementationScope()
    {
        var source = File.ReadAllText(GetOperationsDocPath("P3_HEALTH_ENDPOINT_DEPLOYMENT_SMOKE_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Health Endpoint and Deployment Smoke Baseline",
            "GET /health/live",
            "GET /health/ready",
            "DatabaseConnectivityHealthCheck",
            "HealthCheckResponseWriter.WriteAsync",
            "CARITAS_DEPLOYMENT_SMOKE_BASE_URL",
            "Production go-live remains blocked",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 health endpoint baseline");
    }

    [Fact]
    public void RepositoryGovernanceBaseline_RunsHealthEndpointDeploymentSmokeVerifier()
    {
        var source = File.ReadAllText(GetScriptPath("validate-repo-governance-baseline.ps1"));

        Assert.Contains(
            "verify-p3-health-endpoint-deployment-smoke.ps1",
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

    private static string GetApiSourcePath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            fileName);
    }

    private static string GetHealthSourcePath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "services",
            "api-dotnet",
            "src",
            "Caritas.Brigadas.Api",
            "Health",
            fileName);
    }

    private static string GetIntegrationTestPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "services",
            "api-dotnet",
            "tests",
            "Caritas.Brigadas.Api.Tests",
            "Integration",
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
