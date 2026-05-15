using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3OperationalIncidentResponseRunbookContractTests
{
    [Fact]
    public void OperationalIncidentResponseBaseline_DefinesIncidentRunbookScope()
    {
        var source = File.ReadAllText(GetOperationsDocPath("P3_OPERATIONAL_INCIDENT_RESPONSE_RUNBOOK_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Operational Incident Response Runbook Baseline",
            "Production go-live remains blocked",
            "SEV-1 Critical",
            "SEV-2 High",
            "SEV-3 Medium",
            "SEV-4 Low",
            "incident commander",
            "technical owner",
            "communications owner",
            "database owner",
            "security/privacy owner",
            "business owner",
            "detection timestamp UTC",
            "acknowledgement timestamp UTC",
            "correlation ids",
            "request ids",
            "rollback decision",
            "privacy/legal escalation status",
            "Postmortem is required",
            "follow-up PR or issue reference",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 operational incident response baseline");
    }

    [Fact]
    public void IncidentResponseTemplate_ContainsRequiredIncidentEvidenceSections()
    {
        var source = File.ReadAllText(GetOperationsTemplatePath("INCIDENT_RESPONSE_RECORD_TEMPLATE.md"));

        var requiredTokens = new[]
        {
            "Incident Response Record",
            "Incident identity",
            "Severity classification",
            "SEV-1 Critical",
            "SEV-2 High",
            "SEV-3 Medium",
            "SEV-4 Low",
            "Incident commander",
            "Technical owner",
            "Communications owner",
            "Database owner",
            "Security/privacy owner",
            "Business owner",
            "Detection timestamp UTC",
            "Acknowledgement timestamp UTC",
            "Correlation ids",
            "Request ids",
            "Health endpoint status",
            "Database connectivity status",
            "Authentication failure rate",
            "Authorization failure rate",
            "Sync rejection rate",
            "Rollback decision",
            "Privacy/legal escalation status",
            "Postmortem required",
            "Follow-up PR or issue reference",
            "OPEN",
            "MITIGATED",
            "RESOLVED",
            "POSTMORTEM_REQUIRED",
            "CLOSED"
        };

        AssertRequiredTokens(source, requiredTokens, "incident response record template");
    }

    [Fact]
    public void OperationalIncidentResponseVerifier_RequiresReadinessObservabilityAndGovernanceReferences()
    {
        var source = File.ReadAllText(GetScriptPath("verify-p3-operational-incident-response-runbook.ps1"));

        var requiredTokens = new[]
        {
            "P3 operational incident response runbook verification passed.",
            "P3_OPERATIONAL_INCIDENT_RESPONSE_RUNBOOK_BASELINE.md",
            "INCIDENT_RESPONSE_RECORD_TEMPLATE.md",
            "P3_PRODUCTION_DEPLOYMENT_READINESS_BASELINE.md",
            "P3_PRODUCTION_OBSERVABILITY_BASELINE.md",
            "DEPLOYMENT_EVIDENCE_RECORD_TEMPLATE.md",
            "incident response record template",
            "repository governance baseline"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 operational incident response verifier");
    }

    [Fact]
    public void RepositoryGovernanceBaseline_RunsOperationalIncidentResponseVerifier()
    {
        var source = File.ReadAllText(GetScriptPath("validate-repo-governance-baseline.ps1"));

        Assert.Contains(
            "verify-p3-operational-incident-response-runbook.ps1",
            source,
            StringComparison.Ordinal);
    }

    private static void AssertRequiredTokens(
        string source,
        IReadOnlyCollection<string> requiredTokens,
        string label)
    {
        var failures = requiredTokens
            .Where(token => !source.Contains(token, StringComparison.Ordinal))
            .Select(token => $"{label} is missing required token: {token}")
            .ToArray();

        Assert.True(
            failures.Length == 0,
            $"{label} is incomplete." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    private static string GetOperationsDocPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "docs",
            "operations",
            fileName);
    }

    private static string GetOperationsTemplatePath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "docs",
            "operations",
            "templates",
            fileName);
    }

    private static string GetScriptPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "scripts",
            fileName);
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
