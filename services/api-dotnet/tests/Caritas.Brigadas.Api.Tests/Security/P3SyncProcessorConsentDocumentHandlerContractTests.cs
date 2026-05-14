using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncProcessorConsentDocumentHandlerContractTests
{
    [Fact]
    public void SyncBatchProcessor_HandlesConsentDocumentCreateOnly()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var requiredTokens = new[]
        {
            "HandleConsentDocumentEventAsync",
            "syncEvent.EntityType == SyncEntityType.ConsentDocument",
            "syncEvent.Operation != SyncOperation.Create",
            "consent_document_operation_not_implemented",
            "JsonSerializer.Deserialize<CreateConsentDocumentRequest>",
            "CreateConsentDocumentForSync",
            "SetConsentPropertyIfExists",
            "_dbContext.Set<ConsentDocument>().Add(consentDocument)",
            "syncEvent.Accept(",
            "consentDocument.Id",
            "consent_document_patient_not_found",
            "consent_document_visit_not_found",
            "consent_document_signed_by_user_not_found",
            "consent_document_id_already_exists",
            "consent_document_duplicate_patient_visit_type_version",
            "consent_document_duplicate_patient_visit_type_version_in_pending_batch",
            "acceptedConsentDocumentIdsInBatch",
            "acceptedConsentDocumentKeysInBatch",
            "reserved only after successful ConsentDocument construction",
            "return 5;",
            "return 6;"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor consent document handler");

        var forbiddenTokens = new[]
        {
            "_dbContext.MedicalReferrals.Add",
            "_dbContext.MedicationDeliveries.Add"
        };

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ConsentDocumentHandlerBaseline_DefinesConsentDocumentOnlyScope()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_PROCESSOR_CONSENT_DOCUMENT_HANDLER_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Processor Consent Document Handler Baseline",
            "EntityType: consent_document",
            "Operation: create",
            "parse PayloadJson as CreateConsentDocumentRequest",
            "require SignatureDataUrl",
            "preserve DocumentTextSnapshot as the legal text snapshot",
            "preserve SignatureDataUrl as the captured signature evidence",
            "processor response must not expose SignatureDataUrl",
            "consent_document update is not implemented in P3-18",
            "document_signature standalone sync is not implemented in P3-18",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync processor consent document handler baseline");
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

    private static string GetInfrastructurePath(params string[] segments)
    {
        return Path.Combine(
            new[] { FindRepositoryRoot(), "services", "api-dotnet", "src", "Caritas.Brigadas.Infrastructure" }
                .Concat(segments)
                .ToArray());
    }

    private static string GetDocPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "docs",
            "backend",
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