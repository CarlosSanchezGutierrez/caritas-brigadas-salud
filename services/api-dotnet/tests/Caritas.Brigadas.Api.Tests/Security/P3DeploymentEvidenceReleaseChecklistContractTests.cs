using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3DeploymentEvidenceReleaseChecklistContractTests
{
    [Fact]
    public void DeploymentEvidenceBaseline_DefinesReleaseChecklistScope()
    {
        var source = File.ReadAllText(GetOperationsDocPath("P3_DEPLOYMENT_EVIDENCE_RELEASE_CHECKLIST_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Deployment Evidence and Release Checklist Baseline",
            "Production go-live remains blocked",
            "Required release identity evidence",
            "Required pre-deployment evidence",
            "Required database deployment evidence",
            "Required smoke evidence",
            "Required security evidence",
            "Required observability evidence",
            "Required rollback evidence",
            "Required approval evidence",
            "git commit SHA",
            "migration script checksum",
            "SQL Server smoke command",
            "deployment health smoke command",
            "explicit go/no-go decision",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 deployment evidence baseline");
    }

    [Fact]
    public void DeploymentEvidenceTemplate_ContainsRequiredReleaseEvidenceSections()
    {
        var source = File.ReadAllText(GetOperationsTemplatePath("DEPLOYMENT_EVIDENCE_RECORD_TEMPLATE.md"));

        var requiredTokens = new[]
        {
            "Deployment Evidence Record",
            "Release identity",
            "Pre-deployment verification",
            "Database deployment evidence",
            "Smoke test evidence",
            "Security evidence",
            "Observability evidence",
            "Rollback evidence",
            "Approval evidence",
            "Final release decision",
            "Git commit SHA",
            "Migration script checksum",
            "SQL Server smoke command",
            "Deployment health smoke command",
            "/health/live status",
            "/health/ready status",
            "No X-Dev-* authentication in production",
            "Explicit HTTPS CORS origins",
            "Security:RateLimiting:Enabled",
            "Explicit go/no-go decision",
            "GO",
            "NO-GO",
            "ROLLBACK"
        };

        AssertRequiredTokens(source, requiredTokens, "deployment evidence record template");
    }

    [Fact]
    public void DeploymentEvidenceVerifier_RequiresReadinessAndObservabilityReferences()
    {
        var source = File.ReadAllText(GetScriptPath("verify-p3-deployment-evidence-release-checklist.ps1"));

        var requiredTokens = new[]
        {
            "P3 deployment evidence and release checklist verification passed.",
            "P3_DEPLOYMENT_EVIDENCE_RELEASE_CHECKLIST_BASELINE.md",
            "DEPLOYMENT_EVIDENCE_RECORD_TEMPLATE.md",
            "P3_PRODUCTION_DEPLOYMENT_READINESS_BASELINE.md",
            "P3_PRODUCTION_OBSERVABILITY_BASELINE.md",
            "git commit SHA",
            "migration script checksum",
            "SQL Server smoke command",
            "deployment health smoke command"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 deployment evidence verifier");
    }

    [Fact]
    public void RepositoryGovernanceBaseline_RunsDeploymentEvidenceVerifier()
    {
        var source = File.ReadAllText(GetScriptPath("validate-repo-governance-baseline.ps1"));

        Assert.Contains(
            "verify-p3-deployment-evidence-release-checklist.ps1",
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
