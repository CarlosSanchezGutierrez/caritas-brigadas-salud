using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class P3SyncProcessorMedicationDeliveryHandlerContractTests
{
    [Fact]
    public void SyncBatchProcessor_HandlesMedicationDeliveryCreateOnly()
    {
        var source =
            File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs")) +
            File.ReadAllText(GetInfrastructurePath("Sync", "SyncProcessingOrder.cs")) +
            File.ReadAllText(GetInfrastructurePath("Sync", "MedicationDeliverySyncEventHandler.cs"));

        var requiredTokens = new[]
        {
            "await _medicationDeliverySyncEventHandler.HandleAsync(",
            "MedicationDeliverySyncEventHandler",
            "syncEvent.EntityType == SyncEntityType.MedicationDelivery",
            "syncEvent.Operation != SyncOperation.Create",
            "medication_delivery_operation_not_implemented",
            "out CreateMedicationDeliveryRequest? request",
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
            "Medication delivery id duplicate checks include globally duplicated ids because primary key uniqueness is not tenant-scoped",
            "Non-delivered medication receipt metadata is preserved through constructor fields instead of silently dropped",
            "request.MarkAsDelivered ? null : request.DeliveredByUserId",
            "request.MarkAsDelivered ? null : request.ReceivedByName",
            "return 7;",
            "return 8;"
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor medication delivery handler");
    }

    [Fact]
    public void SyncBatchProcessor_DoesNotContainDirectMedicationDeliveryLogicAfterExtraction()
    {
        var source = File.ReadAllText(GetInfrastructurePath("Sync", "SyncBatchProcessor.cs"));

        var requiredTokens = new[]
        {
            "private readonly MedicationDeliverySyncEventHandler _medicationDeliverySyncEventHandler;",
            "_medicationDeliverySyncEventHandler = new MedicationDeliverySyncEventHandler(dbContext, PayloadJsonOptions);",
            "await _medicationDeliverySyncEventHandler.HandleAsync(",
            "await _medicationDeliverySyncEventHandler.HandleAsync("
        };

        AssertRequiredTokens(source, requiredTokens, "SyncBatchProcessor medication delivery wrapper");

        var forbiddenTokens = new[]
        {
            "out CreateMedicationDeliveryRequest? request",
            "new MedicationDelivery(",
            "medicationDelivery.MarkDelivered(",
            "_dbContext.Set<MedicationDelivery>().Add(medicationDelivery)",
            "medication_delivery_operation_not_implemented",
            "medication_delivery_encounter_not_found",
            "medication_delivery_brigade_mismatch",
            "medication_delivery_patient_not_found",
            "medication_delivery_delivered_by_user_not_found",
            "medication_delivery_signature_not_supported_until_document_signature_handler",
            "medication_delivery_id_already_exists",
            "medication_delivery_duplicate_in_pending_batch",
            "request.MarkAsDelivered ? null : request.DeliveredByUserId",
            "request.MarkAsDelivered ? null : request.ReceivedByName"
        };

        foreach (var token in forbiddenTokens)
        {
            Assert.DoesNotContain(token, source, StringComparison.Ordinal);
        }
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
            "reserve pending-batch medication delivery id only after successful MedicationDelivery construction and optional delivered transition",
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
