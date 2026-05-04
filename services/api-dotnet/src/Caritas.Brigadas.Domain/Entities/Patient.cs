using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Enums;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class Patient : AuditableEntity
{
    private const int MaxFolioLength = 50;
    private const int MaxNameLength = 150;
    private const int MaxFullNameNormalizedLength = 400;
    private const int MaxCurpLength = 30;
    private const int MaxPhoneLength = 50;
    private const int MaxAddressLength = 500;
    private const int MaxMunicipalityLength = 150;
    private const int MaxColonyLength = 150;
    private const int MaxCommunityLength = 200;
    private const int MaxPartialRecordReasonLength = 500;
    private const int MaxAdminNotesLength = 1000;

    private Patient()
    {
        PatientFolio = string.Empty;
        Sex = Sex.NotSpecified;
        Status = PatientStatus.Active;
    }

    public Patient(
        Guid id,
        Guid organizationId,
        string patientFolio,
        string? firstName = null,
        string? paternalLastName = null,
        string? maternalLastName = null,
        DateOnly? birthDate = null,
        int? approximateAge = null,
        Sex sex = Sex.NotSpecified)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        PatientFolio = NormalizeRequired(patientFolio, nameof(patientFolio), MaxFolioLength).ToUpperInvariant();
        Status = PatientStatus.Active;

        UpdateIdentity(firstName, paternalLastName, maternalLastName);
        UpdateDemographics(birthDate, approximateAge, sex);
    }

    public Guid OrganizationId { get; private set; }

    public string PatientFolio { get; private set; }

    public string? FirstName { get; private set; }

    public string? PaternalLastName { get; private set; }

    public string? MaternalLastName { get; private set; }

    public string? FullNameNormalized { get; private set; }

    public DateOnly? BirthDate { get; private set; }

    public int? ApproximateAge { get; private set; }

    public Sex Sex { get; private set; }

    public string? Curp { get; private set; }

    public string? Phone { get; private set; }

    public string? AddressLine { get; private set; }

    public string? Municipality { get; private set; }

    public string? Colony { get; private set; }

    public string? Community { get; private set; }

    public bool IsMinor { get; private set; }

    public bool IsMigrant { get; private set; }

    public bool IsPartialRecord { get; private set; }

    public string? PartialRecordReason { get; private set; }

    public string? NotesAdmin { get; private set; }

    public string Status { get; private set; }

    public bool IsActive => Status == PatientStatus.Active;

    public void UpdateIdentity(
        string? firstName,
        string? paternalLastName,
        string? maternalLastName)
    {
        FirstName = NormalizeOptional(firstName, nameof(firstName), MaxNameLength);
        PaternalLastName = NormalizeOptional(paternalLastName, nameof(paternalLastName), MaxNameLength);
        MaternalLastName = NormalizeOptional(maternalLastName, nameof(maternalLastName), MaxNameLength);
        FullNameNormalized = BuildNormalizedFullName(FirstName, PaternalLastName, MaternalLastName);
    }

    public void UpdateDemographics(
        DateOnly? birthDate,
        int? approximateAge,
        Sex sex)
    {
        if (approximateAge.HasValue && approximateAge.Value < 0)
        {
            throw new DomainException("Approximate age cannot be negative.");
        }

        if (birthDate.HasValue && birthDate.Value > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new DomainException("Birth date cannot be in the future.");
        }

        BirthDate = birthDate;
        ApproximateAge = approximateAge;
        Sex = sex;
        IsMinor = DetermineMinorStatus(birthDate, approximateAge);
    }

    public void UpdateSensitiveIdentifiers(
        string? curp,
        string? phone)
    {
        Curp = NormalizeOptional(curp, nameof(curp), MaxCurpLength)?.ToUpperInvariant();
        Phone = NormalizeOptional(phone, nameof(phone), MaxPhoneLength);
    }

    public void UpdateLocation(
        string? addressLine,
        string? municipality,
        string? colony,
        string? community)
    {
        AddressLine = NormalizeOptional(addressLine, nameof(addressLine), MaxAddressLength);
        Municipality = NormalizeOptional(municipality, nameof(municipality), MaxMunicipalityLength);
        Colony = NormalizeOptional(colony, nameof(colony), MaxColonyLength);
        Community = NormalizeOptional(community, nameof(community), MaxCommunityLength);
    }

    public void MarkAsMigrant()
    {
        IsMigrant = true;
    }

    public void ClearMigrantFlag()
    {
        IsMigrant = false;
    }

    public void MarkAsPartialRecord(string reason)
    {
        PartialRecordReason = NormalizeRequired(reason, nameof(reason), MaxPartialRecordReasonLength);
        IsPartialRecord = true;
    }

    public void ClearPartialRecord()
    {
        IsPartialRecord = false;
        PartialRecordReason = null;
    }

    public void UpdateAdminNotes(string? notesAdmin)
    {
        NotesAdmin = NormalizeOptional(notesAdmin, nameof(notesAdmin), MaxAdminNotesLength);
    }

    public void Activate()
    {
        Status = PatientStatus.Active;
    }

    public void Deactivate()
    {
        Status = PatientStatus.Inactive;
    }

    private static bool DetermineMinorStatus(DateOnly? birthDate, int? approximateAge)
    {
        if (birthDate.HasValue)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - birthDate.Value.Year;

            if (birthDate.Value > today.AddYears(-age))
            {
                age--;
            }

            return age < 18;
        }

        if (approximateAge.HasValue)
        {
            return approximateAge.Value < 18;
        }

        return false;
    }

    private static string? BuildNormalizedFullName(
        string? firstName,
        string? paternalLastName,
        string? maternalLastName)
    {
        var parts = new[]
            {
                firstName,
                paternalLastName,
                maternalLastName
            }
            .Where(part => !string.IsNullOrWhiteSpace(part))
            .Select(part => part!.Trim());

        var fullName = string.Join(" ", parts);

        if (string.IsNullOrWhiteSpace(fullName))
        {
            return null;
        }

        if (fullName.Length > MaxFullNameNormalizedLength)
        {
            throw new DomainException($"Full name cannot exceed {MaxFullNameNormalizedLength} characters.");
        }

        return fullName.ToUpperInvariant();
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

public static class PatientStatus
{
    public const string Active = "active";
    public const string Inactive = "inactive";
}
