using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SecurityProductReadinessGapAuditContractTests
{
    [Fact]
    public void SecurityProductReadinessGapAuditBaseline_DefinesRequiredGapScope()
    {
        var source = File.ReadAllText(GetOperationsDocPath("P3_SECURITY_PRODUCT_READINESS_GAP_AUDIT_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Security and Product Readiness Gap Audit Baseline",
            "REQUIRED_BEFORE_FRONTEND",
            "REQUIRED_BEFORE_STAGING",
            "REQUIRED_BEFORE_PRODUCTION",
            "OWNED_BY_INFRASTRUCTURE",
            "rate limiting",
            "dependency scanning",
            "secret scanning",
            "penetration testing",
            "SQL Server VM connectivity",
            "SQL Server least privilege",
            "network ACL and firewall rules",
            "deny-by-default traffic posture",
            "TLS between backend and SQL Server",
            "patient signature",
            "social security / insurance fields",
            "emergency contact fields",
            "OpenAPI/frontend contract readiness",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 security and product readiness gap audit baseline");
    }

    [Fact]
    public void SecurityProductReadinessGapAudit_DefinesFrontendStagingAndProductionBlockers()
    {
        var source = File.ReadAllText(GetOperationsDocPath("P3_SECURITY_PRODUCT_READINESS_GAP_AUDIT.md"));

        var requiredTokens = new[]
        {
            "P3 Security and Product Readiness Gap Audit",
            "Backend production readiness conclusion: NOT_PRODUCTION_READY",
            "Frontend readiness conclusion: NOT_READY_FOR_FULL_FRONTEND",
            "Phase plan",
            "Security gap matrix",
            "Product and medical workflow gap matrix",
            "What is unnecessary right now",
            "SQL Server VM interpretation",
            "Frontend readiness decision",
            "Production readiness decision",
            "Recommended immediate next PRs",
            "Rate limiting",
            "Dependency Review",
            "Static analysis",
            "Secret scanning",
            "Penetration testing",
            "SQL least privilege",
            "Network ACL/firewall",
            "Deny-by-default traffic posture",
            "Patient signature",
            "Privacy notice consent",
            "Social security / insurance",
            "Emergency contact",
            "Migrant/incomplete data handling",
            "OpenAPI contract",
            "Grafana dashboards",
            "The SQL Server VM does not create the backend."
        };

        AssertRequiredTokens(source, requiredTokens, "P3 security and product readiness gap audit");
    }

    [Fact]
    public void SecurityProductReadinessGapAuditVerifier_RequiresClosureAndGovernanceReferences()
    {
        var source = File.ReadAllText(GetScriptPath("verify-p3-security-product-readiness-gap-audit.ps1"));

        var requiredTokens = new[]
        {
            "P3 security and product readiness gap audit verification passed.",
            "P3_SECURITY_PRODUCT_READINESS_GAP_AUDIT_BASELINE.md",
            "P3_SECURITY_PRODUCT_READINESS_GAP_AUDIT.md",
            "P3_BACKEND_PRODUCTION_READINESS_CLOSURE_REPORT.md",
            "repository governance baseline"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 security and product readiness gap audit verifier");
    }

    [Fact]
    public void RepositoryGovernanceBaseline_RunsSecurityProductReadinessGapAuditVerifier()
    {
        var source = File.ReadAllText(GetScriptPath("validate-repo-governance-baseline.ps1"));

        Assert.Contains(
            "verify-p3-security-product-readiness-gap-audit.ps1",
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
