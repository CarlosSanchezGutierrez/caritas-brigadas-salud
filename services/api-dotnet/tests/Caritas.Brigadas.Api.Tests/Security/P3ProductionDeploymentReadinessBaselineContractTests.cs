using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3ProductionDeploymentReadinessBaselineContractTests
{
    [Fact]
    public void ProductionDeploymentReadinessBaseline_DefinesBlockedGoLiveScope()
    {
        var source = File.ReadAllText(GetOperationsDocPath("P3_PRODUCTION_DEPLOYMENT_READINESS_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Production Deployment Readiness Baseline",
            "Production go-live status: blocked.",
            "P3-26B authentication and authorization hardening",
            "P3-26C SQL Server integration smoke test",
            "no automatic database migrations during API startup",
            "SQL Server migration scripts generated as idempotent SQL",
            "separate runtime and migration database users",
            "minimum privilege for the runtime user",
            "no local development headers in production authentication flows",
            "no development authentication mode in production",
            "no localhost CORS origins in production",
            "secrets stored outside source control",
            "Required deployment evidence",
            "Required environment configuration",
            "Required security posture",
            "Required database deployment posture",
            "Required operational posture",
            "Explicit non-goals",
            "Required follow-up workstreams",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 production deployment readiness baseline");
    }

    [Fact]
    public void ProductionDeploymentReadinessVerifier_RequiresSyncAndDatabaseEvidence()
    {
        var source = File.ReadAllText(GetScriptPath("verify-p3-production-deployment-readiness-baseline.ps1"));

        var requiredTokens = new[]
        {
            "P3 production deployment readiness baseline verification passed.",
            "P3_PRODUCTION_DEPLOYMENT_READINESS_BASELINE.md",
            "P3_SYNC_BACKEND_READINESS_CHECKLIST.md",
            "deployment-baseline.md",
            "validate-database-deployment-baseline.ps1",
            "verify.yml",
            "Production go-live status: blocked.",
            "P3-26B authentication and authorization hardening",
            "P3-26C SQL Server integration smoke test"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 production deployment readiness verifier");
    }

    [Fact]
    public void RepositoryGovernanceBaseline_RunsProductionDeploymentReadinessVerifier()
    {
        var source = File.ReadAllText(GetScriptPath("validate-repo-governance-baseline.ps1"));

        Assert.Contains(
            "verify-p3-production-deployment-readiness-baseline.ps1",
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
