using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3ProductionObservabilityBaselineContractTests
{
    [Fact]
    public void ProductionObservabilityBaseline_DefinesBlockedObservabilityScope()
    {
        var source = File.ReadAllText(GetOperationsDocPath("P3_PRODUCTION_OBSERVABILITY_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Production Observability Baseline",
            "Production observability status: blocked.",
            "health endpoint",
            "structured application logs",
            "request correlation identifier",
            "error correlation identifier",
            "database connectivity signal",
            "authentication failure visibility",
            "authorization failure visibility",
            "sync processing failure visibility",
            "critical exception visibility",
            "rate limiting visibility",
            "Required health signals",
            "Required logging posture",
            "Required tracing posture",
            "Required metrics posture",
            "Required alerting posture",
            "Required incident response evidence",
            "Required deployment monitoring checklist",
            "Required follow-up workstreams",
            "P3-26E health endpoint and deployment smoke implementation",
            "P3-26F structured logging and correlation id implementation",
            "raw PayloadJson",
            "patient names",
            "connection strings",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 production observability baseline");
    }

    [Fact]
    public void ProductionObservabilityVerifier_RequiresDeploymentAndSmokeEvidence()
    {
        var source = File.ReadAllText(GetScriptPath("verify-p3-production-observability-baseline.ps1"));

        var requiredTokens = new[]
        {
            "P3 production observability baseline verification passed.",
            "P3_PRODUCTION_OBSERVABILITY_BASELINE.md",
            "P3_PRODUCTION_DEPLOYMENT_READINESS_BASELINE.md",
            "P3_SQLSERVER_INTEGRATION_SMOKE_TEST_BASELINE.md",
            "Production observability status: blocked.",
            "health endpoint",
            "structured application logs",
            "request correlation identifier",
            "database connectivity signal",
            "verify-p3-production-observability-baseline.ps1"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 production observability verifier");
    }

    [Fact]
    public void RepositoryGovernanceBaseline_RunsProductionObservabilityVerifier()
    {
        var source = File.ReadAllText(GetScriptPath("validate-repo-governance-baseline.ps1"));

        Assert.Contains(
            "verify-p3-production-observability-baseline.ps1",
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
