using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P35MobileApiOfflineReadinessContractTests
{
    [Fact]
    public void MobileApiOfflineReadinessContract_DefinesRequiredMobileAndApiConstraints()
    {
        var baseline = File.ReadAllText(GetRepoPath(
        "docs",
        "operations",
        "P3_5_MOBILE_API_OFFLINE_READINESS_CONTRACT_BASELINE.md"));

        var contract = File.ReadAllText(GetRepoPath(
            "docs",
            "operations",
            "P3_5_MOBILE_API_OFFLINE_READINESS_CONTRACT.md"));

        Assert.Contains("Mobile and web clients must never connect directly to SQL Server", baseline, StringComparison.Ordinal);
        Assert.Contains("iOS -> HTTPS -> API -> SQL Server", baseline, StringComparison.Ordinal);
        Assert.Contains("Android -> HTTPS -> API -> SQL Server", baseline, StringComparison.Ordinal);
        Assert.Contains("Web Admin -> HTTPS -> API -> SQL Server", baseline, StringComparison.Ordinal);
        Assert.Contains("Mobile-first production goal", baseline, StringComparison.Ordinal);
        Assert.Contains("Offline queue", baseline, StringComparison.Ordinal);
        Assert.Contains("Idempotent sync", baseline, StringComparison.Ordinal);
        Assert.Contains("Retry-safe sync", baseline, StringComparison.Ordinal);
        Assert.Contains("Conflict-aware sync", baseline, StringComparison.Ordinal);
        Assert.Contains("App Store and Play Store readiness requirements", baseline, StringComparison.Ordinal);

        Assert.Contains("Status: BLOCKED", contract, StringComparison.Ordinal);
        Assert.Contains("Clients must never connect directly to SQL Server", contract, StringComparison.Ordinal);
        Assert.Contains("Mobile-first goal", contract, StringComparison.Ordinal);
        Assert.Contains("API readiness evidence", contract, StringComparison.Ordinal);
        Assert.Contains("Offline sync evidence", contract, StringComparison.Ordinal);
        Assert.Contains("Conflict handling evidence", contract, StringComparison.Ordinal);
        Assert.Contains("Retry and idempotency evidence", contract, StringComparison.Ordinal);
        Assert.Contains("Mobile local storage evidence", contract, StringComparison.Ordinal);
        Assert.Contains("Web admin API readiness evidence", contract, StringComparison.Ordinal);
        Assert.Contains("Production mobile/API readiness", contract, StringComparison.Ordinal);
    }

    [Fact]
    public void MobileApiOfflineReadinessVerifier_IsWiredIntoGovernance()
    {
        var governance = File.ReadAllText(GetRepoPath(
            "scripts",
            "validate-repo-governance-baseline.ps1"));

        Assert.Contains("verify-p3-5-mobile-api-offline-readiness-contract.ps1", governance, StringComparison.Ordinal);
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
