using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncBackendReadinessChecklistContractTests
{
    [Fact]
    public void SyncBackendReadinessChecklist_DefinesClosedP3SyncScope()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_BACKEND_READINESS_CHECKLIST.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Backend Readiness Checklist",
            "Backend sync readiness status: ready for next backend workstream.",
            "Processor-level coverage closed",
            "API-level coverage closed",
            "Privacy coverage closed",
            "Tenant boundary coverage closed",
            "Governance and CI coverage closed",
            "Required evidence files",
            "Explicit non-goals",
            "Next backend workstreams",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync backend readiness checklist");
    }

    [Fact]
    public void SyncBackendReadinessVerifier_RequiresProcessorAndApiEvidence()
    {
        var source = File.ReadAllText(GetScriptPath("verify-p3-sync-backend-readiness-checklist.ps1"));

        var requiredTokens = new[]
        {
            "P3 sync backend readiness checklist verification passed.",
            "P3ClinicalSyncEndToEndIntegrationTests.cs",
            "P3SyncProcessEndpointIntegrationTests.cs",
            "P3SyncCreateBatchEndpointIntegrationTests.cs",
            "P3SyncListEventsEndpointIntegrationTests.cs",
            "P3SyncTenantBoundaryEndpointIntegrationTests.cs",
            "P3_SYNC_PROCESS_ENDPOINT_API_REGRESSION_BASELINE.md",
            "P3_SYNC_CREATE_BATCH_ENDPOINT_API_REGRESSION_BASELINE.md",
            "P3_SYNC_LIST_EVENTS_ENDPOINT_API_REGRESSION_BASELINE.md",
            "P3_SYNC_TENANT_BOUNDARY_ENDPOINT_API_REGRESSION_BASELINE.md",
            "P3_ZERO_TECHNICAL_DEBT_SYNC_PROCESSOR_BASELINE.md"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync backend readiness verifier");
    }

    [Fact]
    public void RepositoryGovernanceBaseline_RunsSyncBackendReadinessVerifier()
    {
        var source = File.ReadAllText(GetScriptPath("validate-repo-governance-baseline.ps1"));

        Assert.Contains(
            "verify-p3-sync-backend-readiness-checklist.ps1",
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

    private static string GetDocPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "docs",
            "backend",
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
