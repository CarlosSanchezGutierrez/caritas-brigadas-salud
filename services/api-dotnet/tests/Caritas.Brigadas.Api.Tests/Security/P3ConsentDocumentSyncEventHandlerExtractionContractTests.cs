using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3ConsentDocumentSyncEventHandlerExtractionContractTests
{
    [Fact]
    public void ConsentDocumentSyncEventHandler_OwnsConsentDocumentCreateBehavior()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "ConsentDocumentSyncEventHandler.cs"));

        var requiredTokens = new[]
        {
            "internal sealed class ConsentDocumentSyncEventHandler",
            "public async Task HandleAsync",
            "SyncPayloadReader.TryReadObject",
            "out CreateConsentDocumentRequest? request",
            "CreateConsentDocumentForSync",
            "SetConsentPropertyIfExists",
            "_dbContext.Set<ConsentDocument>().Add(consentDocument)",
            "syncEvent.Accept(",
            "consentDocument.Id",
            "consent_document_operation_not_implemented",
            "consent_document_patient_not_found",
            "consent_document_visit_not_found",
            "consent_document_signed_by_user_not_found",
            "consent_document_id_already_exists",
            "consent_document_duplicate_patient_visit_type_version",
            "consent_document_duplicate_patient_visit_type_version_in_pending_batch",
            "acceptedConsentDocumentIdsInBatch",
            "acceptedConsentDocumentKeysInBatch",
            "reserved only after successful ConsentDocument construction",
            "reserved atomically",
            "consentDocumentIdReserved",
            "consentDocumentKeyReserved",
            "acceptedConsentDocumentIdsInBatch.Remove(consentDocumentId)",
            "SignatureDataUrl",
            "DocumentTextSnapshot",
            "GuardianFullName",
            "GuardianRelationship",
            "BindingFlags.Instance",
            "property.SetValue(instance, value)"
        };

        AssertRequiredTokens(source, requiredTokens, "ConsentDocumentSyncEventHandler");
    }

    [Fact]
    public void SyncBatchProcessor_DelegatesConsentDocumentCreateToConsentDocumentSyncEventHandler()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var requiredTokens = new[]
        {
            "private readonly ConsentDocumentSyncEventHandler _consentDocumentSyncEventHandler;",
            "_consentDocumentSyncEventHandler = new ConsentDocumentSyncEventHandler(dbContext, PayloadJsonOptions);",
            "    await _consentDocumentSyncEventHandler.HandleAsync(",
            "await _consentDocumentSyncEventHandler.HandleAsync("
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor consent document handler extraction");

        var forbiddenTokens = new[]
        {
            "out CreateConsentDocumentRequest? request",
            "CreateConsentDocumentForSync",
            "SetConsentPropertyIfExists",
            "_dbContext.Set<ConsentDocument>().Add(consentDocument)",
            "consent_document_operation_not_implemented",
            "consent_document_patient_not_found",
            "consent_document_visit_not_found",
            "consent_document_signed_by_user_not_found",
            "consent_document_id_already_exists",
            "consent_document_duplicate_patient_visit_type_version",
            "consent_document_duplicate_patient_visit_type_version_in_pending_batch",
            "consentDocumentIdReserved",
            "consentDocumentKeyReserved",
            "acceptedConsentDocumentIdsInBatch.Remove(consentDocumentId)"
        };

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ConsentDocumentHandlerExtractionBaseline_DefinesSixthHandlerExtraction()
    {
        var source = File.ReadAllText(GetDocPath("P3_CONSENT_DOCUMENT_SYNC_EVENT_HANDLER_EXTRACTION_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Consent Document Sync Event Handler Extraction Baseline",
            "ConsentDocumentSyncEventHandler must own consent_document/create payload parsing",
            "SyncBatchProcessor must not directly create ConsentDocument",
            "SyncBatchProcessor must not directly parse CreateConsentDocumentRequest",
            "SyncBatchProcessor must not contain CreateConsentDocumentForSync",
            "SyncBatchProcessor must not contain SetConsentPropertyIfExists",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 consent document sync event handler extraction baseline");
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
