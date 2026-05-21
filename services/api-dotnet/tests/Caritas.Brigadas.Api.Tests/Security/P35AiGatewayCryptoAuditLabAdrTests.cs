using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P35AiGatewayCryptoAuditLabAdrTests
{
    [Fact]
    public void AiGatewayCryptoAuditLabAdr_DefinesRequiredDeferralAndSafetyConstraints()
    {
        var baseline = File.ReadAllText(GetRepoPath(
            "docs",
            "operations",
            "P3_5_AI_GATEWAY_CRYPTO_AUDIT_LAB_ADR_BASELINE.md"));

        var adr = File.ReadAllText(GetRepoPath(
            "docs",
            "architecture",
            "ADR_P3_5_10_AI_GATEWAY_CRYPTO_AUDIT_LAB.md"));

        Assert.Contains("AI Gateway and crypto audit work must be disabled by default", baseline, StringComparison.Ordinal);
        Assert.Contains("The AI Gateway is deferred", baseline, StringComparison.Ordinal);
        Assert.Contains("No PHI processing", baseline, StringComparison.Ordinal);
        Assert.Contains("No patient data prompts", baseline, StringComparison.Ordinal);
        Assert.Contains("No autonomous medical advice", baseline, StringComparison.Ordinal);
        Assert.Contains("Feature flag", baseline, StringComparison.Ordinal);
        Assert.Contains("Kill switch", baseline, StringComparison.Ordinal);
        Assert.Contains("Crypto audit and blockchain work is deferred", baseline, StringComparison.Ordinal);
        Assert.Contains("No PHI on-chain", baseline, StringComparison.Ordinal);
        Assert.Contains("Neither AI Gateway nor blockchain is required for production MVP", baseline, StringComparison.Ordinal);

        Assert.Contains("DEFERRED", adr, StringComparison.Ordinal);
        Assert.Contains("AI Gateway and crypto audit lab work are deferred and disabled by default", adr, StringComparison.Ordinal);
        Assert.Contains("Neither AI Gateway nor blockchain is required for production MVP", adr, StringComparison.Ordinal);
        Assert.Contains("Current AI Gateway state: DISABLED", adr, StringComparison.Ordinal);
        Assert.Contains("AI Gateway future approval checklist", adr, StringComparison.Ordinal);
        Assert.Contains("Kill switch", adr, StringComparison.Ordinal);
        Assert.Contains("Current crypto audit lab state: DISABLED FOR PRODUCTION CLINICAL WORKFLOW", adr, StringComparison.Ordinal);
        Assert.Contains("Blockchain is not required for production MVP", adr, StringComparison.Ordinal);
        Assert.Contains("Patient PHI on-chain", adr, StringComparison.Ordinal);
        Assert.Contains("Production MVP dependency", adr, StringComparison.Ordinal);
        Assert.Contains("NOT REQUIRED", adr, StringComparison.Ordinal);
    }

    [Fact]
    public void AiGatewayCryptoAuditLabVerifier_IsWiredIntoGovernance()
    {
        var governance = File.ReadAllText(GetRepoPath(
            "scripts",
            "validate-repo-governance-baseline.ps1"));

        Assert.Contains("verify-p3-5-ai-gateway-crypto-audit-lab-adr.ps1", governance, StringComparison.Ordinal);
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
