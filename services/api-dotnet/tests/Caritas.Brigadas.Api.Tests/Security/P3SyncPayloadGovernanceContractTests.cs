using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncPayloadGovernanceContractTests
{
    [Fact]
    public void SyncPayloadGovernanceBaseline_DefinesPayloadAllowlistAndSafeDiagnostics()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_PAYLOAD_GOVERNANCE_PROCESSOR_CONTRACT_BASELINE.md"));

        var requiredTokens = new[]
        {
            "PayloadJson is sensitive and untrusted",
            "EntityType must come from an explicit allowlist",
            "Operation must come from an explicit allowlist",
            "unknown EntityType must be rejected",
            "unknown Operation must be rejected",
            "raw PayloadJson must not be logged",
            "Allowed EntityType values",
            "Allowed Operation values",
            "Processor contract expectations",
            "Payload validation expectations",
            "Safe diagnostics",
            "Forbidden diagnostics by default"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync payload governance baseline");
    }

    [Fact]
    public void SyncEvent_DefinesEntityAndOperationAllowlists()
    {
        var source = File.ReadAllText(GetDomainPath("Entities", "SyncEvent.cs"));

        var requiredTokens = new[]
        {
            "public static class SyncEntityType",
            "public static class SyncOperation",
            "public static readonly IReadOnlySet<string> Allowed",
            "SyncEntityType.IsAllowed",
            "SyncOperation.IsAllowed",
            "patient",
            "patient_visit",
            "service_encounter",
            "vital_signs",
            "form_response",
            "consent_document",
            "document_signature",
            "medical_referral",
            "medication_delivery",
            "media_release"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncEvent allowlist contract");
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

    private static string GetDomainPath(params string[] segments)
    {
        return Path.Combine(
            new[] { FindRepositoryRoot(), "services", "api-dotnet", "src", "Caritas.Brigadas.Domain" }
                .Concat(segments)
                .ToArray());
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
