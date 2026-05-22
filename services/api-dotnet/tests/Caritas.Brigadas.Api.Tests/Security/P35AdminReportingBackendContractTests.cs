using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P35AdminReportingBackendContractTests
{
    [Fact]
    public void AdminReportingBackendContract_DefinesRequiredReportingConstraints()
    {
        var baseline = File.ReadAllText(GetRepoPath(
            "docs",
            "operations",
            "P3_5_ADMIN_REPORTING_BACKEND_CONTRACT_BASELINE.md"));

        var contract = File.ReadAllText(GetRepoPath(
            "docs",
            "operations",
            "P3_5_ADMIN_REPORTING_BACKEND_CONTRACT.md"));

        Assert.Contains("Web Admin and reporting users must never connect directly to SQL Server", baseline, StringComparison.Ordinal);
        Assert.Contains("Web Admin -> HTTPS -> API -> SQL Server", baseline, StringComparison.Ordinal);
        Assert.Contains("Administrative reporting goal", baseline, StringComparison.Ordinal);
        Assert.Contains("Daily patient counts", baseline, StringComparison.Ordinal);
        Assert.Contains("Daily service counts", baseline, StringComparison.Ordinal);
        Assert.Contains("Export requirements", baseline, StringComparison.Ordinal);
        Assert.Contains("Privacy requirements", baseline, StringComparison.Ordinal);
        Assert.Contains("Audit requirements", baseline, StringComparison.Ordinal);
        Assert.Contains("Data quality requirements", baseline, StringComparison.Ordinal);

        Assert.Contains("Status: BLOCKED", contract, StringComparison.Ordinal);
        Assert.Contains("Reporting roles and permissions", contract, StringComparison.Ordinal);
        Assert.Contains("Dashboard metric evidence", contract, StringComparison.Ordinal);
        Assert.Contains("Report endpoint evidence", contract, StringComparison.Ordinal);
        Assert.Contains("Export evidence", contract, StringComparison.Ordinal);
        Assert.Contains("Privacy controls", contract, StringComparison.Ordinal);
        Assert.Contains("Audit logging evidence", contract, StringComparison.Ordinal);
        Assert.Contains("Data quality indicators", contract, StringComparison.Ordinal);
        Assert.Contains("Reporting API requirements", contract, StringComparison.Ordinal);
        Assert.Contains("Production reporting readiness", contract, StringComparison.Ordinal);
    }

    [Fact]
    public void AdminReportingBackendVerifier_IsWiredIntoGovernance()
    {
        var governance = File.ReadAllText(GetRepoPath(
            "scripts",
            "validate-repo-governance-baseline.ps1"));

        Assert.Contains("verify-p3-5-admin-reporting-backend-contract.ps1", governance, StringComparison.Ordinal);
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
