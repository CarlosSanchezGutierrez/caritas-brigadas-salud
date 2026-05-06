using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class PatientGuardian : AuditableEntity
{
    private const int MaxFullNameLength = 250;
    private const int MaxRelationshipLength = 100;
    private const int MaxPhoneLength = 50;
    private const int MaxIdentificationTypeLength = 100;
    private const int MaxIdentificationValueLength = 150;
    private const int MaxAbsenceReasonLength = 500;

    private PatientGuardian()
    {
    }

    public PatientGuardian(
        Guid id,
        Guid patientId,
        string? fullName,
        string? relationship,
        string? phone = null,
        string? identificationType = null,
        string? identificationValue = null,
        bool isPresent = true,
        string? absenceReason = null,
        bool isPrimary = false)
        : base(id)
    {
        PatientId = RequireGuid(patientId, nameof(patientId));
        IsPresent = isPresent;
        IsPrimary = isPrimary;

        UpdateDetails(
            fullName,
            relationship,
            phone,
            identificationType,
            identificationValue,
            isPresent,
            absenceReason);
    }

    public Guid PatientId { get; private set; }

    public string? FullName { get; private set; }

    public string? Relationship { get; private set; }

    public string? Phone { get; private set; }

    public string? IdentificationType { get; private set; }

    public string? IdentificationValue { get; private set; }

    public bool IsPresent { get; private set; }

    public string? AbsenceReason { get; private set; }

    public bool IsPrimary { get; private set; }

    public void UpdateDetails(
        string? fullName,
        string? relationship,
        string? phone,
        string? identificationType,
        string? identificationValue,
        bool isPresent,
        string? absenceReason)
    {
        if (isPresent && string.IsNullOrWhiteSpace(fullName))
        {
            throw new DomainException("Guardian full name is required when guardian is present.");
        }

        if (!isPresent && string.IsNullOrWhiteSpace(absenceReason))
        {
            throw new DomainException("Absence reason is required when guardian is not present.");
        }

        FullName = NormalizeOptional(fullName, nameof(fullName), MaxFullNameLength);
        Relationship = NormalizeOptional(relationship, nameof(relationship), MaxRelationshipLength);
        Phone = NormalizeOptional(phone, nameof(phone), MaxPhoneLength);
        IdentificationType = NormalizeOptional(identificationType, nameof(identificationType), MaxIdentificationTypeLength);
        IdentificationValue = NormalizeOptional(identificationValue, nameof(identificationValue), MaxIdentificationValueLength);
        IsPresent = isPresent;
        AbsenceReason = isPresent
            ? null
            : NormalizeRequired(absenceReason!, nameof(absenceReason), MaxAbsenceReasonLength);
    }

    public void MarkAsPrimary()
    {
        IsPrimary = true;
    }

    public void ClearPrimary()
    {
        IsPrimary = false;
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
