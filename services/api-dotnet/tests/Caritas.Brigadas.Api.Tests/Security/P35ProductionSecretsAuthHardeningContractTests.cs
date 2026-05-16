using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P35ProductionSecretsAuthHardeningContractTests
{
    [Fact]
    public void ProductionSecretsAuthHardeningContract_DefinesRequiredProductionAuthConstraints()
    {
        var baseline = File.ReadAllText(GetRepoPath(
            "docs",
            "operations",
            "P3_5_PRODUCTION_SECRETS_AUTH_HARDENING_CONTRACT_BASELINE.md"));

        var contract = File.ReadAllText(GetRepoPath(
            "docs",
            "operations",
            "P3_5_PRODUCTION_SECRETS_AUTH_HARDENING_CONTRACT.md"));

        Assert.Contains("Production must not depend on development-only authentication", baseline, StringComparison.Ordinal);
        Assert.Contains("Development authentication headers", baseline, StringComparison.Ordinal);
        Assert.Contains("Hardcoded secrets", baseline, StringComparison.Ordinal);
        Assert.Contains("SQL credentials in mobile apps", baseline, StringComparison.Ordinal);
        Assert.Contains("Azure Key Vault", baseline, StringComparison.Ordinal);
        Assert.Contains("AWS Secrets Manager", baseline, StringComparison.Ordinal);
        Assert.Contains("HashiCorp Vault", baseline, StringComparison.Ordinal);
        Assert.Contains("Production authentication must use a real token-based provider", baseline, StringComparison.Ordinal);
        Assert.Contains("Authorization must remain server-enforced", baseline, StringComparison.Ordinal);
        Assert.Contains("iOS and Android must not contain production secrets", baseline, StringComparison.Ordinal);

        Assert.Contains("Status: BLOCKED", contract, StringComparison.Ordinal);
        Assert.Contains("SQL credentials in clients", contract, StringComparison.Ordinal);
        Assert.Contains("Secret provider selected", contract, StringComparison.Ordinal);
        Assert.Contains("OIDC authority", contract, StringComparison.Ordinal);
        Assert.Contains("Authentication provider decision", contract, StringComparison.Ordinal);
        Assert.Contains("Bootstrap admin process", contract, StringComparison.Ordinal);
        Assert.Contains("Break-glass process", contract, StringComparison.Ordinal);
        Assert.Contains("App Store release config separation", contract, StringComparison.Ordinal);
        Assert.Contains("Play Store release config separation", contract, StringComparison.Ordinal);
        Assert.Contains("No secrets in frontend bundle", contract, StringComparison.Ordinal);
        Assert.Contains("AI Gateway keys must not exist in production", contract, StringComparison.Ordinal);
        Assert.Contains("Secrets readiness", contract, StringComparison.Ordinal);
        Assert.Contains("Auth readiness", contract, StringComparison.Ordinal);
    }

    [Fact]
    public void ProductionSecretsAuthHardeningVerifier_IsWiredIntoGovernance()
    {
        var governance = File.ReadAllText(GetRepoPath(
            "scripts",
            "validate-repo-governance-baseline.ps1"));

        Assert.Contains("verify-p3-5-production-secrets-auth-hardening-contract.ps1", governance, StringComparison.Ordinal);
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
