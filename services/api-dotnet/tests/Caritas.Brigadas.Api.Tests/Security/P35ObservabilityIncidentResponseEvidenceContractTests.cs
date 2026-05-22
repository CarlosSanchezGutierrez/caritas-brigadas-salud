using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P35ObservabilityIncidentResponseEvidenceContractTests
{
    [Fact]
    public void ObservabilityIncidentResponseEvidenceContract_DefinesRequiredOperationalConstraints()
    {
        var baseline = File.ReadAllText(GetRepoPath(
            "docs",
            "operations",
            "P3_5_OBSERVABILITY_INCIDENT_RESPONSE_EVIDENCE_CONTRACT_BASELINE.md"));

        var contract = File.ReadAllText(GetRepoPath(
            "docs",
            "operations",
            "P3_5_OBSERVABILITY_INCIDENT_RESPONSE_EVIDENCE_CONTRACT.md"));

        Assert.Contains("A production system without observability is operationally blind", baseline, StringComparison.Ordinal);
        Assert.Contains("Liveness health check", baseline, StringComparison.Ordinal);
        Assert.Contains("Readiness health check", baseline, StringComparison.Ordinal);
        Assert.Contains("Database connectivity health check", baseline, StringComparison.Ordinal);
        Assert.Contains("Structured logs", baseline, StringComparison.Ordinal);
        Assert.Contains("Correlation id", baseline, StringComparison.Ordinal);
        Assert.Contains("Dashboard or equivalent operational view", baseline, StringComparison.Ordinal);
        Assert.Contains("Incident runbook", baseline, StringComparison.Ordinal);
        Assert.Contains("Security incident requirements", baseline, StringComparison.Ordinal);
        Assert.Contains("Mobile incident requirements", baseline, StringComparison.Ordinal);

        Assert.Contains("Status: BLOCKED", contract, StringComparison.Ordinal);
        Assert.Contains("Production readiness requires observable behavior", contract, StringComparison.Ordinal);
        Assert.Contains("Health check evidence", contract, StringComparison.Ordinal);
        Assert.Contains("Structured logging evidence", contract, StringComparison.Ordinal);
        Assert.Contains("Metrics evidence", contract, StringComparison.Ordinal);
        Assert.Contains("Tracing and correlation evidence", contract, StringComparison.Ordinal);
        Assert.Contains("Monitoring stack decision", contract, StringComparison.Ordinal);
        Assert.Contains("Alerting evidence", contract, StringComparison.Ordinal);
        Assert.Contains("Incident response evidence", contract, StringComparison.Ordinal);
        Assert.Contains("Production observability readiness", contract, StringComparison.Ordinal);
    }

    [Fact]
    public void ObservabilityIncidentResponseVerifier_IsWiredIntoGovernance()
    {
        var governance = File.ReadAllText(GetRepoPath(
            "scripts",
            "validate-repo-governance-baseline.ps1"));

        Assert.Contains("verify-p3-5-observability-incident-response-evidence-contract.ps1", governance, StringComparison.Ordinal);
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
