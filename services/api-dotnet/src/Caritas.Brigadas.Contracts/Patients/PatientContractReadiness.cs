namespace Caritas.Brigadas.Contracts.Patients;

public static class PatientContractReadiness
{
    public const string BackendProductionReadiness = "BLOCKED_PENDING_REAL_EVIDENCE";

    public const bool PatientCoreRequiredForFinalSystem = true;

    public const bool OfflineFirstRequiredForFinalSystem = true;

    public const bool LongitudinalHistoryRequiredForFinalSystem = true;

    public const bool DashboardsRequiredForFinalSystem = true;

    public const bool AnalyticsRequiredForFinalSystem = true;

    public const string PatientCoreContractStatus = "PATIENT_CORE_CONTRACT_HARDENED_PENDING_PERSISTENCE_AND_ENDPOINT_PROOF";

    public static readonly string[] RequiredOfflineCreateFields =
    [
        nameof(CreatePatientRequest.LocalPatientId),
        nameof(CreatePatientRequest.ClientOperationId),
        nameof(CreatePatientRequest.IdempotencyKey),
        nameof(CreatePatientRequest.SyncStatus),
        nameof(CreatePatientRequest.DataCaptureSource)
    ];

    public static readonly string[] RequiredLongitudinalLinkFields =
    [
        nameof(CreatePatientRequest.SourceBrigadeId),
        nameof(CreatePatientRequest.PatientFolio)
    ];

    public static readonly string[] RequiredFlexibleIdentityFields =
    [
        nameof(CreatePatientRequest.FirstName),
        nameof(CreatePatientRequest.PaternalLastName),
        nameof(CreatePatientRequest.MaternalLastName),
        nameof(CreatePatientRequest.BirthDate),
        nameof(CreatePatientRequest.ApproximateAge),
        nameof(CreatePatientRequest.Curp),
        nameof(CreatePatientRequest.Phone),
        nameof(CreatePatientRequest.IsMigrant),
        nameof(CreatePatientRequest.IsPartialRecord),
        nameof(CreatePatientRequest.PartialRecordReason)
    ];
}