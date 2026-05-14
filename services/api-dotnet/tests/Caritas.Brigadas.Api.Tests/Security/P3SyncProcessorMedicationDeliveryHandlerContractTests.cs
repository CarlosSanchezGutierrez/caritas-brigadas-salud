using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncProcessorMedicationDeliveryHandlerContractTests
{
    [Fact]
    public void SyncBatchProcessor_HandlesMedicationDeliveryCreateOnly()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var requiredTokens = new[]
        {
            "HandleMedicationDeliveryEventAsync",
            "syncEvent.EntityType == SyncEntityType.MedicationDelivery",
            "syncEvent.Operation != SyncOperation.Create",
            "medication_delivery_operation_not_implemented",
            "JsonSerializer.Deserialize<CreateMedicationDeliveryRequest>",
            "new MedicationDelivery(",
            "medicationDelivery.MarkDelivered(",
            "_dbContext.Set<MedicationDelivery>().Add(medicationDelivery)",
            "syncEvent.Accept(",
            "medicationDelivery.Id",
            "medication_delivery_encounter_not_found",
            "medication_delivery_brigade_mismatch",
            "medication_delivery_patient_not_found",
            "medication_delivery_delivered_by_user_not_found",
            "medication_delivery_signature_not_supported_until_document_signature_handler",
            "medication_delivery_id_already_exists",
            "medication_delivery_duplicate_in_pending_batch",
            "acceptedMedicationDeliveryIdsInBatch",
            "reserved only after successful MedicationDelivery construction and optional delivered transition",
            "return 7;",
            "return 8;"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor medication delivery handler");
    }

    [Fact]
    public void MedicationDeliveryHandlerBaseline_DefinesMedicationDeliveryOnlyScope()
    {
        var source = File.ReadAllText(GetDocPath("P3_SYNC_PROCESSOR_MEDICATION_DELIVERY_HANDLER_BASELINE.md"));

        var requiredTokens = new[]
        {
            "P3 Sync Processor Medication Delivery Handler Baseline",
            "EntityType: medication_delivery",
            "Operation: create",
            "parse PayloadJson as CreateMedicationDeliveryRequest",
            "derive PatientId from ServiceEncounter.PatientId, not from payload trust",
            "reject SignatureId until the document_signature handler exists",
            "support optional delivered transition only when MarkAsDelivered is true and DeliveredByUserId is provided",
            "medication_delivery update is not implemented in P3-20",
            "medication_delivery cancel is not implemented in P3-20",
            "inventory decrement/reservation is not implemented in P3-20",
            "Acceptance criteria"
        };

        AssertRequiredTokens(source, requiredTokens, "P3 sync processor medication delivery handler baseline");
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