using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class MedicationDelivery : AuditableEntity
{
    private const int MaxMedicationNameLength = 250;
    private const int MaxPresentationLength = 150;
    private const int MaxQuantityLength = 100;
    private const int MaxLotNumberLength = 100;
    private const int MaxInstructionsLength = 1000;
    private const int MaxReceivedByNameLength = 250;

    private MedicationDelivery()
    {
        MedicationName = string.Empty;
        Status = MedicationDeliveryStatus.Created;
    }

    public MedicationDelivery(
        Guid id,
        Guid organizationId,
        Guid encounterId,
        Guid patientId,
        string medicationName,
        string? presentation = null,
        string? quantity = null,
        string? lotNumber = null,
        DateOnly? expirationDate = null,
        string? instructions = null,
        Guid? deliveredByUserId = null,
        string? receivedByName = null,
        Guid? signatureId = null)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        EncounterId = RequireGuid(encounterId, nameof(encounterId));
        PatientId = RequireGuid(patientId, nameof(patientId));
        MedicationName = NormalizeRequired(medicationName, nameof(medicationName), MaxMedicationNameLength);
        Presentation = NormalizeOptional(presentation, nameof(presentation), MaxPresentationLength);
        Quantity = NormalizeOptional(quantity, nameof(quantity), MaxQuantityLength);
        LotNumber = NormalizeOptional(lotNumber, nameof(lotNumber), MaxLotNumberLength);
        ExpirationDate = expirationDate;
        Instructions = NormalizeOptional(instructions, nameof(instructions), MaxInstructionsLength);
        DeliveredByUserId = deliveredByUserId;
        ReceivedByName = NormalizeOptional(receivedByName, nameof(receivedByName), MaxReceivedByNameLength);
        SignatureId = signatureId;
        Status = MedicationDeliveryStatus.Created;

        ValidateExpirationDate(expirationDate);
    }

    public Guid OrganizationId { get; private set; }

    public Guid EncounterId { get; private set; }

    public Guid PatientId { get; private set; }

    public string MedicationName { get; private set; }

    public string? Presentation { get; private set; }

    public string? Quantity { get; private set; }

    public string? LotNumber { get; private set; }

    public DateOnly? ExpirationDate { get; private set; }

    public string? Instructions { get; private set; }

    public Guid? DeliveredByUserId { get; private set; }

    public string? ReceivedByName { get; private set; }

    public Guid? SignatureId { get; private set; }

    public string Status { get; private set; }

    public bool IsDelivered => Status == MedicationDeliveryStatus.Delivered;

    public void UpdateMedicationDetails(
        string medicationName,
        string? presentation,
        string? quantity,
        string? lotNumber,
        DateOnly? expirationDate,
        string? instructions)
    {
        if (Status == MedicationDeliveryStatus.Cancelled || Status == MedicationDeliveryStatus.Delivered)
        {
            throw new DomainException("Delivered or cancelled medication records cannot be updated.");
        }

        MedicationName = NormalizeRequired(medicationName, nameof(medicationName), MaxMedicationNameLength);
        Presentation = NormalizeOptional(presentation, nameof(presentation), MaxPresentationLength);
        Quantity = NormalizeOptional(quantity, nameof(quantity), MaxQuantityLength);
        LotNumber = NormalizeOptional(lotNumber, nameof(lotNumber), MaxLotNumberLength);
        ExpirationDate = expirationDate;
        Instructions = NormalizeOptional(instructions, nameof(instructions), MaxInstructionsLength);

        ValidateExpirationDate(expirationDate);
    }

    public void MarkDelivered(
        Guid deliveredByUserId,
        string? receivedByName,
        Guid? signatureId)
    {
        if (Status == MedicationDeliveryStatus.Cancelled)
        {
            throw new DomainException("Cancelled medication records cannot be marked as delivered.");
        }

        DeliveredByUserId = RequireGuid(deliveredByUserId, nameof(deliveredByUserId));
        ReceivedByName = NormalizeOptional(receivedByName, nameof(receivedByName), MaxReceivedByNameLength);
        SignatureId = signatureId;
        Status = MedicationDeliveryStatus.Delivered;
    }

    public void Cancel()
    {
        if (Status == MedicationDeliveryStatus.Delivered)
        {
            throw new DomainException("Delivered medication records cannot be cancelled.");
        }

        Status = MedicationDeliveryStatus.Cancelled;
    }

    private static void ValidateExpirationDate(DateOnly? expirationDate)
    {
        if (!expirationDate.HasValue)
        {
            return;
        }

        var minimumValidDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        if (expirationDate.Value < minimumValidDate)
        {
            throw new DomainException("Expired medication cannot be registered for delivery.");
        }
    }

    private static Guid RequireGuid(Guid value, string fieldName)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException($"{fieldName} cannot be empty.");
        }

        return value;
    }

    private static string NormalizeRequired(string value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{fieldName} is required.");
        }

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new DomainException($"{fieldName} cannot exceed {maxLength} characters.");
        }

        return normalized;
    }

    private static string? NormalizeOptional(string? value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new DomainException($"{fieldName} cannot exceed {maxLength} characters.");
        }

        return normalized;
    }
}

public static class MedicationDeliveryStatus
{
    public const string Created = "created";
    public const string Delivered = "delivered";
    public const string Cancelled = "cancelled";
}
