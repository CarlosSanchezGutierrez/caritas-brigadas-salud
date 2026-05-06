namespace Caritas.Brigadas.Contracts.Reports;

public sealed record OrganizationReportSummaryDto
{
    public Guid OrganizationId { get; init; }

    public DateTimeOffset GeneratedAtUtc { get; init; }

    public int UsersCount { get; init; }

    public int RolesCount { get; init; }

    public int PermissionsCount { get; init; }

    public int RolePermissionsCount { get; init; }

    public int ServicesCount { get; init; }

    public int CommunitiesCount { get; init; }

    public int MobileUnitsCount { get; init; }

    public int BrigadesCount { get; init; }

    public int BrigadeServiceAssignmentsCount { get; init; }

    public int PatientsCount { get; init; }

    public int PatientVisitsCount { get; init; }

    public int ServiceEncountersCount { get; init; }

    public int FormTemplatesCount { get; init; }

    public int FormResponsesCount { get; init; }

    public int ConsentDocumentsCount { get; init; }

    public int ClinicalRecordsCount { get; init; }
}
