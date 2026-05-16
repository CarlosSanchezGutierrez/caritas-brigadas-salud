using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P35BackendProductionClosureAuditTests
{
    [Fact]
    public void BackendProductionClosureAudit_DefinesRequiredClosureDecision()
    {
        var audit = File.ReadAllText(GetRepoPath(
            "docs",
            "operations",
            "P3_5_BACKEND_PRODUCTION_CLOSURE_AUDIT.md"));

        Assert.Contains("Status: BLOCKED FOR PRODUCTION", audit, StringComparison.Ordinal);
        Assert.Contains("Documentation alone is not production evidence", audit, StringComparison.Ordinal);
        Assert.Contains("P3.5 contract inventory", audit, StringComparison.Ordinal);
        Assert.Contains("P3.5-01", audit, StringComparison.Ordinal);
        Assert.Contains("P3.5-02", audit, StringComparison.Ordinal);
        Assert.Contains("P3.5-03", audit, StringComparison.Ordinal);
        Assert.Contains("P3.5-04", audit, StringComparison.Ordinal);
        Assert.Contains("P3.5-05", audit, StringComparison.Ordinal);
        Assert.Contains("P3.5-06", audit, StringComparison.Ordinal);
        Assert.Contains("P3.5-07", audit, StringComparison.Ordinal);
        Assert.Contains("P3.5-08", audit, StringComparison.Ordinal);
        Assert.Contains("P3.5-09", audit, StringComparison.Ordinal);
        Assert.Contains("P3.5-10", audit, StringComparison.Ordinal);
        Assert.Contains("Required evidence before production", audit, StringComparison.Ordinal);
        Assert.Contains("Overengineering control", audit, StringComparison.Ordinal);
        Assert.Contains("App Store and Play Store implications", audit, StringComparison.Ordinal);
        Assert.Contains("Backend production readiness: BLOCKED", audit, StringComparison.Ordinal);
        Assert.Contains("P3.6-01 Staging environment evidence", audit, StringComparison.Ordinal);
        Assert.Contains("P4 Frontend/Web Admin/iOS/Android implementation", audit, StringComparison.Ordinal);
    }

    [Fact]
    public void BackendProductionClosureAuditVerifier_IsWiredIntoGovernance()
    {
        var governance = File.ReadAllText(GetRepoPath(
            "scripts",
            "validate-repo-governance-baseline.ps1"));

        Assert.Contains("verify-p3-5-backend-production-closure-audit.ps1", governance, StringComparison.Ordinal);
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
