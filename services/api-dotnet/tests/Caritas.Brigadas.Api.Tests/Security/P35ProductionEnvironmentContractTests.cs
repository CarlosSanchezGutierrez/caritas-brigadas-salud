using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P35ProductionEnvironmentContractTests
{
    [Fact]
    public void ProductionEnvironmentContract_DefinesRequiredBackendProductionConstraints()
    {
        var baseline = File.ReadAllText(GetRepoPath(
        "docs",
        "operations",
        "P3_5_PRODUCTION_ENVIRONMENT_CONTRACT_BASELINE.md"));

        var contract = File.ReadAllText(GetRepoPath(
            "docs",
            "operations",
            "P3_5_PRODUCTION_ENVIRONMENT_CONTRACT.md"));

        Assert.Contains("Clients must never connect directly to SQL Server", baseline, StringComparison.Ordinal);
        Assert.Contains("Migrations must not run automatically at API startup", baseline, StringComparison.Ordinal);
        Assert.Contains("Production must not use development-only headers or bypasses", baseline, StringComparison.Ordinal);
        Assert.Contains("No SQL credentials", baseline, StringComparison.Ordinal);
        Assert.Contains("No embedded production secrets", baseline, StringComparison.Ordinal);
        Assert.Contains("OWASP baseline test plan", baseline, StringComparison.Ordinal);
        Assert.Contains("The AI Gateway must remain disabled until a dedicated ADR exists", baseline, StringComparison.Ordinal);
        Assert.Contains("Blockchain must not be required for production MVP", baseline, StringComparison.Ordinal);

        Assert.Contains("Status: BLOCKED", contract, StringComparison.Ordinal);
        Assert.Contains("Direct client-to-database access is forbidden", contract, StringComparison.Ordinal);
        Assert.Contains("Application user has minimum privileges", contract, StringComparison.Ordinal);
        Assert.Contains("Connection string is stored as a secret", contract, StringComparison.Ordinal);
        Assert.Contains("Production must use real token-based authentication", contract, StringComparison.Ordinal);
        Assert.Contains("Encrypted local storage for sensitive records", contract, StringComparison.Ordinal);
        Assert.Contains("Export audit logs", contract, StringComparison.Ordinal);
        Assert.Contains("Database connectivity health check", contract, StringComparison.Ordinal);
        Assert.Contains("AI Gateway is deferred", contract, StringComparison.Ordinal);
        Assert.Contains("Blockchain is deferred", contract, StringComparison.Ordinal);
    }

    [Fact]
    public void ProductionEnvironmentContractVerifier_IsWiredIntoGovernance()
    {
        var governance = File.ReadAllText(GetRepoPath(
            "scripts",
            "validate-repo-governance-baseline.ps1"));

        Assert.Contains("verify-p3-5-production-environment-contract.ps1", governance, StringComparison.Ordinal);
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
