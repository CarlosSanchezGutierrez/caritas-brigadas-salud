using System.ComponentModel.DataAnnotations;

namespace Caritas.Brigadas.Contracts.PatientVisits;

public sealed record CreatePatientVisitRequest
{
    [MaxLength(50)]
    public string? VisitFolio { get; init; }

    public Guid PatientId { get; init; }

    public Guid BrigadeId { get; init; }

    public DateTimeOffset? ArrivalTime { get; init; }

    public Guid? RegisteredByUserId { get; init; }

    public bool CreatedOffline { get; init; }

    public Guid? DeviceId { get; init; }
}
