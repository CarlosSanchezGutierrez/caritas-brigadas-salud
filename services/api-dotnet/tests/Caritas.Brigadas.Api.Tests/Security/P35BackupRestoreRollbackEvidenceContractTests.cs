using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P35BackupRestoreRollbackEvidenceContractTests
{
    [Fact]
    public void BackupRestoreRollbackEvidenceContract_DefinesRequiredRecoveryConstraints()
    {
        var baseline = File.ReadAllText(GetRepoPath(
            "docs",
            "operations",
            "P3_5_BACKUP_RESTORE_ROLLBACK_EVIDENCE_CONTRACT_BASELINE.md"));

        var contract = File.ReadAllText(GetRepoPath(
            "docs",
            "operations",
            "P3_5_BACKUP_RESTORE_ROLLBACK_EVIDENCE_CONTRACT.md"));

        Assert.Contains("A system is not production-ready until recovery has been tested", baseline, StringComparison.Ordinal);
        Assert.Contains("Documentation without a restore test is not recovery evidence", baseline, StringComparison.Ordinal);
        Assert.Contains("Backups are encrypted", baseline, StringComparison.Ordinal);
        Assert.Contains("Restore is tested", baseline, StringComparison.Ordinal);
        Assert.Contains("RTO is defined", baseline, StringComparison.Ordinal);
        Assert.Contains("RPO is defined", baseline, StringComparison.Ordinal);
        Assert.Contains("Sync and offline recovery requirements", baseline, StringComparison.Ordinal);
        Assert.Contains("Mobile recovery requirements", baseline, StringComparison.Ordinal);

        Assert.Contains("Status: BLOCKED", contract, StringComparison.Ordinal);
        Assert.Contains("A backup that has not been restored is only an assumption", contract, StringComparison.Ordinal);
        Assert.Contains("Production readiness requires restore evidence", contract, StringComparison.Ordinal);
        Assert.Contains("Recovery Time Objective", contract, StringComparison.Ordinal);
        Assert.Contains("Recovery Point Objective", contract, StringComparison.Ordinal);
        Assert.Contains("Deployment rollback", contract, StringComparison.Ordinal);
        Assert.Contains("Migration rollback", contract, StringComparison.Ordinal);
        Assert.Contains("SQL Server disaster recovery", contract, StringComparison.Ordinal);
        Assert.Contains("Offline sync recovery", contract, StringComparison.Ordinal);
        Assert.Contains("Evidence record template", contract, StringComparison.Ordinal);
        Assert.Contains("Production recovery readiness", contract, StringComparison.Ordinal);
    }

    [Fact]
    public void BackupRestoreRollbackVerifier_IsWiredIntoGovernance()
    {
        var governance = File.ReadAllText(GetRepoPath(
            "scripts",
            "validate-repo-governance-baseline.ps1"));

        Assert.Contains("verify-p3-5-backup-restore-rollback-evidence-contract.ps1", governance, StringComparison.Ordinal);
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
