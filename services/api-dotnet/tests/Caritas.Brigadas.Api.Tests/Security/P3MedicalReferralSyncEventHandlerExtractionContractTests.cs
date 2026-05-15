using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3MedicalReferralSyncEventHandlerExtractionContractTests
{
    [Fact]
    public void MedicalReferralSyncEventHandler_OwnsMedicalReferralCreateBehavior()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "MedicalReferralSyncEventHandler.cs"));

        var requiredTokens = new[]
        {
            "internal sealed class MedicalReferralSyncEventHandler",
            "public async Task HandleAsync",
            "SyncPayloadReader.TryReadObject",
            "out CreateMedicalReferralRequest? request",
            "new MedicalReferral(",
            "_dbContext.Set<MedicalReferral>().Add(medicalReferral)",
            "syncEvent.Accept(",
            "medicalReferral.Id",
            "medical_referral_operation_not_implemented",
            "medical_referral_encounter_not_found",
            "medical_referral_brigade_mismatch",
            "medical_referral_patient_not_found",
            "medical_referral_referred_by_user_not_found",
            "medical_referral_provider_signature_not_supported_until_document_signature_handler",
            "medical_referral_id_already_exists",
            "medical_referral_folio_already_exists",
            "medical_referral_folio_duplicate_in_pending_batch",
            "acceptedMedicalReferralIdsInBatch",
            "acceptedMedicalReferralFoliosInBatch",
            "GenerateSyncMedicalReferralFolio",
            "private static string GenerateSyncMedicalReferralFolio",
            "medicalReferralIdReserved",
            "medicalReferralFolioReserved",
            "acceptedMedicalReferralIdsInBatch.Remove(medicalReferralId)",
            "reserved only after successful MedicalReferral construction and reserved atomically",
            "Medical referral id duplicate checks include soft-deleted rows because primary key uniqueness is not filtered by IsDeleted",
            "Medical referral folio duplicate checks include soft-deleted rows because database unique index is not filtered by IsDeleted"
        };

        AssertRequiredTokens(source, requiredTokens, "MedicalReferralSyncEventHandler");
    }

    [Fact]
    public void SyncBatchProcessor_DelegatesMedicalReferralCreateToMedicalReferralSyncEventHandler()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var requiredTokens = new[]
        {
            "private readonly MedicalReferralSyncEventHandler _medicalReferralSyncEventHandler;",
            "_medicalReferralSyncEventHandler = new MedicalReferralSyncEventHandler(dbContext, PayloadJsonOptions);",
            "await _medicalReferralSyncEventHandler.HandleAsync(",
            "await _medicalReferralSyncEventHandler.HandleAsync("
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor medical referral handler extraction");

        var forbiddenTokens = new[]
        {
            "out CreateMedicalReferralRequest? request",
            "new MedicalReferral(",
            "_dbContext.Set<MedicalReferral>().Add(medicalReferral)",
            "medical_referral_operation_not_implemented",
            "medical_referral_encounter_not_found",
            "medical_referral_brigade_mismatch",
            "medical_referral_patient_not_found",
            "medical_referral_referred_by_user_not_found",
            "medical_referral_provider_signature_not_supported_until_document_signature_handler",
            "medical_referral_id_already_exists",
            "medical_referral_folio_already_exists",
            "medical_referral_folio_duplicate_in_pending_batch",
            "GenerateSyncMedicalReferralFolio",
            "medicalReferralIdReserved",
            "medicalReferralFolioReserved",
            "acceptedMedicalReferralIdsInBatch.Remove(medicalReferralId)"
        };

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void MedicalReferralHandlerExtractionBaseline_DefinesSeventhHandlerExtraction()
    {
        var source = File.ReadAllText(GetDocPath("P3_MEDICAL_REFERRAL_SYNC_EVENT_HANDLER_EXTRACTION_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Medical Referral Sync Event Handler Extraction Baseline",
            "MedicalReferralSyncEventHandler must own medical_referral/create payload parsing",
            "SyncBatchProcessor must not directly construct MedicalReferral",
            "SyncBatchProcessor must not directly parse CreateMedicalReferralRequest",
            "SyncBatchProcessor must not contain GenerateSyncMedicalReferralFolio",
            "Traceability requirement",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 medical referral sync event handler extraction baseline");
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
