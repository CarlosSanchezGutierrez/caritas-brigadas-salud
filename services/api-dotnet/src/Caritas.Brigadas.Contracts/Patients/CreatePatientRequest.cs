using System.ComponentModel.DataAnnotations;

namespace Caritas.Brigadas.Contracts.Patients;

public sealed record CreatePatientRequest
{
    [MaxLength(50)]
    public string? PatientFolio { get; init; }

    [MaxLength(150)]
    public string? FirstName { get; init; }

    [MaxLength(150)]
    public string? PaternalLastName { get; init; }

    [MaxLength(150)]
    public string? MaternalLastName { get; init; }

    public DateOnly? BirthDate { get; init; }

    [Range(0, 130)]
    public int? ApproximateAge { get; init; }

    [MaxLength(30)]
    public string? Sex { get; init; }

    [MaxLength(30)]
    public string? Curp { get; init; }

    [MaxLength(50)]
    public string? Phone { get; init; }

    [MaxLength(500)]
    public string? AddressLine { get; init; }

    [MaxLength(150)]
    public string? Municipality { get; init; }

    [MaxLength(150)]
    public string? Colony { get; init; }

    [MaxLength(200)]
    public string? Community { get; init; }

    public bool IsMigrant { get; init; }

    public bool IsPartialRecord { get; init; }

    [MaxLength(500)]
    public string? PartialRecordReason { get; init; }

    [MaxLength(1000)]
    public string? NotesAdmin { get; init; }

    public Guid? SourceBrigadeId { get; init; }

    [MaxLength(100)]
    public string? LocalPatientId { get; init; }

    [MaxLength(100)]
    public string? ClientOperationId { get; init; }

    [MaxLength(100)]
    public string? IdempotencyKey { get; init; }

    [MaxLength(50)]
    public string? SyncStatus { get; init; }

    [MaxLength(100)]
    public string? DataCaptureSource { get; init; }
}