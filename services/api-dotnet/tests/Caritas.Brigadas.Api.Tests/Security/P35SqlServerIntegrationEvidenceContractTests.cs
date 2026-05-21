using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P35SqlServerIntegrationEvidenceContractTests
{
    [Fact]
    public void SqlServerIntegrationEvidenceContract_DefinesRequiredProductionSqlConstraints()
    {
        var baseline = File.ReadAllText(GetRepoPath(
        "docs",
        "operations",
        "P3_5_SQLSERVER_INTEGRATION_EVIDENCE_CONTRACT_BASELINE.md"));

        var contract = File.ReadAllText(GetRepoPath(
            "docs",
            "operations",
            "P3_5_SQLSERVER_INTEGRATION_EVIDENCE_CONTRACT.md"));

        Assert.Contains("SQL Server is not the backend", baseline, StringComparison.Ordinal);
        Assert.Contains("Client -> HTTPS -> API -> SQL Server", baseline, StringComparison.Ordinal);
        Assert.Contains("No connection string, password, token, certificate private key, or secret value may be committed", baseline, StringComparison.Ordinal);
        Assert.Contains("The runtime API login should not own schema migrations by default", baseline, StringComparison.Ordinal);
        Assert.Contains("Migrations must not run automatically at API startup in production", baseline, StringComparison.Ordinal);
        Assert.Contains("SQL Server should not be publicly exposed", baseline, StringComparison.Ordinal);
        Assert.Contains("Encrypt=True", baseline, StringComparison.Ordinal);
        Assert.Contains("Restore test evidence", baseline, StringComparison.Ordinal);

        Assert.Contains("Status: BLOCKED", contract, StringComparison.Ordinal);
        Assert.Contains("SQL Server is the database, not the backend", contract, StringComparison.Ordinal);
        Assert.Contains("Direct SQL Server access from clients is forbidden", contract, StringComparison.Ordinal);
        Assert.Contains("No plaintext SQL passwords", contract, StringComparison.Ordinal);
        Assert.Contains("No secrets in mobile apps", contract, StringComparison.Ordinal);
        Assert.Contains("No secrets in web frontend bundles", contract, StringComparison.Ordinal);
        Assert.Contains("The runtime API login must be minimum privilege", contract, StringComparison.Ordinal);
        Assert.Contains("Production migrations must not run automatically at API startup", contract, StringComparison.Ordinal);
        Assert.Contains("SQL Server not publicly exposed", contract, StringComparison.Ordinal);
        Assert.Contains("TrustServerCertificate must be explicitly approved", contract, StringComparison.Ordinal);
        Assert.Contains("SQL smoke test evidence", contract, StringComparison.Ordinal);
    }

    [Fact]
    public void SqlServerIntegrationEvidenceVerifier_IsWiredIntoGovernance()
    {
        var governance = File.ReadAllText(GetRepoPath(
            "scripts",
            "validate-repo-governance-baseline.ps1"));

        Assert.Contains("verify-p3-5-sqlserver-integration-evidence-contract.ps1", governance, StringComparison.Ordinal);
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
