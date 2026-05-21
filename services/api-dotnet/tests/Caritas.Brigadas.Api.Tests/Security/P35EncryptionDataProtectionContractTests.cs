using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P35EncryptionDataProtectionContractTests
{
    [Fact]
    public void EncryptionDataProtectionContract_DefinesRequiredProtectionConstraints()
    {
        var baseline = File.ReadAllText(GetRepoPath(
            "docs",
            "operations",
            "P3_5_ENCRYPTION_DATA_PROTECTION_CONTRACT_BASELINE.md"));

        var contract = File.ReadAllText(GetRepoPath(
            "docs",
            "operations",
            "P3_5_ENCRYPTION_DATA_PROTECTION_CONTRACT.md"));

        Assert.Contains("The project must not claim full end-to-end encryption", baseline, StringComparison.Ordinal);
        Assert.Contains("Full end-to-end encryption is not the default architecture", baseline, StringComparison.Ordinal);
        Assert.Contains("Encryption in transit", baseline, StringComparison.Ordinal);
        Assert.Contains("Encryption at rest", baseline, StringComparison.Ordinal);
        Assert.Contains("Backup encryption", baseline, StringComparison.Ordinal);
        Assert.Contains("Mobile local storage encryption", baseline, StringComparison.Ordinal);
        Assert.Contains("Field-level encryption decision", baseline, StringComparison.Ordinal);
        Assert.Contains("No PHI on-chain", baseline, StringComparison.Ordinal);
        Assert.Contains("AI Gateway must remain disabled", baseline, StringComparison.Ordinal);

        Assert.Contains("Status: BLOCKED", contract, StringComparison.Ordinal);
        Assert.Contains("Do not claim full end-to-end encryption", contract, StringComparison.Ordinal);
        Assert.Contains("Data classification matrix", contract, StringComparison.Ordinal);
        Assert.Contains("Field-level protection decision", contract, StringComparison.Ordinal);
        Assert.Contains("Mobile local storage encryption", contract, StringComparison.Ordinal);
        Assert.Contains("Raw clinical request bodies", contract, StringComparison.Ordinal);
        Assert.Contains("Export audit logging", contract, StringComparison.Ordinal);
        Assert.Contains("Key management", contract, StringComparison.Ordinal);
        Assert.Contains("De-identification decision", contract, StringComparison.Ordinal);
        Assert.Contains("No PHI on-chain", contract, StringComparison.Ordinal);
        Assert.Contains("Production data protection readiness", contract, StringComparison.Ordinal);
    }

    [Fact]
    public void EncryptionDataProtectionVerifier_IsWiredIntoGovernance()
    {
        var governance = File.ReadAllText(GetRepoPath(
            "scripts",
            "validate-repo-governance-baseline.ps1"));

        Assert.Contains("verify-p3-5-encryption-data-protection-contract.ps1", governance, StringComparison.Ordinal);
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
