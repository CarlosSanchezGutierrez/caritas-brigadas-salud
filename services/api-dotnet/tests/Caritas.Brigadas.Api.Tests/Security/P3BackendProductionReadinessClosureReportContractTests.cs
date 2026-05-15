using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3BackendProductionReadinessClosureReportContractTests
{
    [Fact]
    public void BackendProductionReadinessClosureBaseline_DefinesHonestReadinessScope()
    {
        var source = File.ReadAllText(GetOperationsDocPath("P3_BACKEND_PRODUCTION_READINESS_CLOSURE_REPORT_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Backend Production Readiness Closure Report Baseline",
            "NOT_PRODUCTION_READY",
            "CONDITIONALLY_READY_FOR_STAGING",
            "READY_FOR_PRODUCTION_WITH_EVIDENCE",
            "PRODUCTION_READY_APPROVED",
            "Required completed work summary",
            "Required implemented backend capabilities",
            "Required blockers",
            "Required final blocker matrix interpretation",
            "Required next actions",
            "Required executive summary",
            "Required technical summary",
            "P3-26K does not approve production go-live",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 backend production readiness closure report baseline");
    }

    [Fact]
    public void BackendProductionReadinessClosureReport_StatesNotProductionReadyUntilEvidenceExists()
    {
        var source = File.ReadAllText(GetOperationsDocPath("P3_BACKEND_PRODUCTION_READINESS_CLOSURE_REPORT.md"));

        var requiredTokens = new[]
        {
            "P3 Backend Production Readiness Closure Report",
            "Readiness conclusion: NOT_PRODUCTION_READY",
            "Go-live decision: NO-GO",
            "CONDITIONALLY_READY_FOR_STAGING",
            "The backend is not production-ready yet.",
            "Completed P3-26 work summary",
            "Implemented backend capabilities",
            "Evidence currently available in repository",
            "Remaining hard blockers",
            "Final blocker matrix interpretation",
            "Recommended next actions",
            "Technical summary",
            "Executive conclusion",
            "SQL Server smoke evidence",
            "Deployment health smoke evidence",
            "Production JWT configuration",
            "Production CORS configuration",
            "Production AllowedHosts",
            "Production secrets source",
            "Backup and restore validation",
            "Rollback validation",
            "Observability validation",
            "Incident response drill",
            "The remaining work is environment validation, operational evidence, and formal approval."
        };

        AssertRequiredTokens(source, requiredTokens, "P3 backend production readiness closure report");
    }

    [Fact]
    public void BackendProductionReadinessClosureVerifier_RequiresReadinessEvidenceReferences()
    {
        var source = File.ReadAllText(GetScriptPath("verify-p3-backend-production-readiness-closure-report.ps1"));

        var requiredTokens = new[]
        {
            "P3 backend production readiness closure report verification passed.",
            "P3_BACKEND_PRODUCTION_READINESS_CLOSURE_REPORT_BASELINE.md",
            "P3_BACKEND_PRODUCTION_READINESS_CLOSURE_REPORT.md",
            "P3_PRODUCTION_DEPLOYMENT_READINESS_BASELINE.md",
            "DEPLOYMENT_EVIDENCE_RECORD_TEMPLATE.md",
            "PRODUCTION_READINESS_FINAL_BLOCKER_MATRIX_TEMPLATE.md",
            "NOT_PRODUCTION_READY",
            "CONDITIONALLY_READY_FOR_STAGING"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 backend production readiness closure report verifier");
    }

    [Fact]
    public void RepositoryGovernanceBaseline_RunsBackendProductionReadinessClosureReportVerifier()
    {
        var source = File.ReadAllText(GetScriptPath("validate-repo-governance-baseline.ps1"));

        Assert.Contains(
            "verify-p3-backend-production-readiness-closure-report.ps1",
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
