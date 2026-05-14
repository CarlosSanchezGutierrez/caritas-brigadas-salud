using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncProcessorMedicalReferralHandlerContractTests
{
    [Fact]
    public void SyncBatchProcessor_HandlesMedicalReferralCreateOnly()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var requiredTokens = new[]
        {
            "HandleMedicalReferralEventAsync",
            "syncEvent.EntityType == SyncEntityType.MedicalReferral",
            "syncEvent.Operation != SyncOperation.Create",
            "medical_referral_operation_not_implemented",
            "JsonSerializer.Deserialize<CreateMedicalReferralRequest>",
            "new MedicalReferral(",
            "_dbContext.Set<MedicalReferral>().Add(medicalReferral)",
            "syncEvent.Accept(",
            "medicalReferral.Id",
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
            "medicalReferralIdReserved",
            "medicalReferralFolioReserved",
            "acceptedMedicalReferralIdsInBatch.Remove(medicalReferralId)",
            "reserved only after successful MedicalReferral construction and reserved atomically",
            "Medical referral id duplicate checks include soft-deleted rows because primary key uniqueness is not filtered by IsDeleted",
            "Medical referral folio duplicate checks include soft-deleted rows because database unique index is not filtered by IsDeleted",
            "return 6;",
            "return 7;"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor medical referral handler");

        var forbiddenTokens = new[]
        {
            "_dbContext.MedicationDeliveries.Add"
        };

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void MedicalReferralHandlerBaseline_DefinesMedicalReferralOnlyScope()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_PROCESSOR_MEDICAL_REFERRAL_HANDLER_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Processor Medical Referral Handler Baseline",
            "EntityType: medical_referral",
            "Operation: create",
            "parse PayloadJson as CreateMedicalReferralRequest",
            "derive PatientId from ServiceEncounter.PatientId, not from payload trust",
            "reject ProviderSignatureId until the document_signature handler exists",
            "ReferralFolio is the stable traceability key for printed/PDF passes",
            "medical_referral update is not implemented in P3-19",
            "medical_referral complete is not implemented in P3-19",
            "medical_referral cancel is not implemented in P3-19",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync processor medical referral handler baseline");
    }


    [Fact]
    public void MedicalReferral_FolioDuplicateCheck_IncludesSoftDeletedRows()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        Assert.DoesNotMatch(
            "referral\\.Id == medicalReferralId[\\s\\S]{0,180}!referral\\.IsDeleted",
            source);

        Assert.DoesNotMatch(
            "referral\\.ReferralFolio == normalizedReferralFolio[\\s\\S]{0,180}!referral\\.IsDeleted",
            source);

        Assert.Contains(
            "Medical referral folio duplicate checks include soft-deleted rows because database unique index is not filtered by IsDeleted",
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