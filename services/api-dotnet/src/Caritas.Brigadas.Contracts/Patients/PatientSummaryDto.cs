namespace Caritas.Brigadas.Contracts.Patients;

public sealed record PatientSummaryDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public string PatientFolio { get; init; } = string.Empty;

    public string? FirstName { get; init; }

    public string? PaternalLastName { get; init; }

    public string? MaternalLastName { get; init; }

    public string? FullNameNormalized { get; init; }

    public DateOnly? BirthDate { get; init; }

    public int? ApproximateAge { get; init; }

    public string Sex { get; init; } = string.Empty;

    public string? Curp { get; init; }

    public string? Phone { get; init; }

    public string? Municipality { get; init; }

    public string? Colony { get; init; }

    public string? Community { get; init; }

    public bool IsMinor { get; init; }

    public bool IsMigrant { get; init; }

    public bool IsPartialRecord { get; init; }

    public string? PartialRecordReason { get; init; }

    public string Status { get; init; } = string.Empty;

    public bool IsActive { get; init; }
}
