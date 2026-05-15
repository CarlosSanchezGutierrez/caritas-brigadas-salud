using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3ProductionReadinessFinalBlockerMatrixContractTests
{
    [Fact]
    public void ProductionReadinessFinalBlockerMatrixBaseline_DefinesFinalProductionReadinessScope()
    {
        var source = File.ReadAllText(GetOperationsDocPath("P3_PRODUCTION_READINESS_FINAL_BLOCKER_MATRIX_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Production Readiness Final Blocker Matrix Baseline",
            "Production readiness status: blocked.",
            "READY",
            "BLOCKED",
            "CONDITIONAL",
            "WAIVED_WITH_APPROVAL",
            "NOT_APPLICABLE",
            "Required blocker categories",
            "Required matrix fields",
            "Required final decision",
            "Hard blockers",
            "SQL Server smoke test",
            "deployment health smoke",
            "request telemetry",
            "incident response runbook",
            "privacy/data handling evidence",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 production readiness final blocker matrix baseline");
    }

    [Fact]
    public void ProductionReadinessFinalBlockerMatrixTemplate_ContainsRequiredBlockerRows()
    {
        var source = File.ReadAllText(GetOperationsTemplatePath("PRODUCTION_READINESS_FINAL_BLOCKER_MATRIX_TEMPLATE.md"));

        var requiredTokens = new[]
        {
            "Production Readiness Final Blocker Matrix",
            "Production readiness status: BLOCKED",
            "Final go/no-go decision: PENDING",
            "Blocker ID",
            "Category",
            "Blocker description",
            "Required evidence",
            "Current status",
            "Owner",
            "Approver",
            "Evidence link",
            "Exit criterion",
            "Risk if unresolved",
            "Target resolution date",
            "Final decision",
            "P3J-001",
            "Repository governance",
            "P3J-006",
            "SQL Server smoke test",
            "P3J-011",
            "Deployment health smoke",
            "P3J-014",
            "Request telemetry",
            "request telemetry evidence",
            "Request telemetry fields are present and sanitized",
            "P3J-015",
            "Production observability",
            "P3J-017",
            "Incident response runbook",
            "P3J-020",
            "Privacy/data handling evidence",
            "GO",
            "NO-GO",
            "CONDITIONAL-GO",
            "ROLLBACK"
        };

        AssertRequiredTokens(source, requiredTokens, "production readiness final blocker matrix template");
    }

    [Fact]
    public void ProductionReadinessFinalBlockerMatrixVerifier_RequiresReleaseEvidenceReferences()
    {
        var source = File.ReadAllText(GetScriptPath("verify-p3-production-readiness-final-blocker-matrix.ps1"));

        var requiredTokens = new[]
        {
            "P3 production readiness final blocker matrix verification passed.",
            "P3_PRODUCTION_READINESS_FINAL_BLOCKER_MATRIX_BASELINE.md",
            "PRODUCTION_READINESS_FINAL_BLOCKER_MATRIX_TEMPLATE.md",
            "P3_PRODUCTION_DEPLOYMENT_READINESS_BASELINE.md",
            "DEPLOYMENT_EVIDENCE_RECORD_TEMPLATE.md",
            "INCIDENT_RESPONSE_RECORD_TEMPLATE.md",
            "repository governance baseline",
            "Request telemetry",
            "request telemetry evidence"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 production readiness final blocker matrix verifier");
    }

    [Fact]
    public void RepositoryGovernanceBaseline_RunsProductionReadinessFinalBlockerMatrixVerifier()
    {
        var source = File.ReadAllText(GetScriptPath("validate-repo-governance-baseline.ps1"));

        Assert.Contains(
            "verify-p3-production-readiness-final-blocker-matrix.ps1",
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
