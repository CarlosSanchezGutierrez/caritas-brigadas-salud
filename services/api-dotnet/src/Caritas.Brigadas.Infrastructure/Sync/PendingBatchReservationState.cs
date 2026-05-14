namespace Caritas.Brigadas.Infrastructure.Sync;

internal sealed class PendingBatchReservationState
{
    public ISet<string> AcceptedPatientFoliosInBatch { get; } = new HashSet<string>(StringComparer.Ordinal);

    public ISet<string> AcceptedVisitFoliosInBatch { get; } = new HashSet<string>(StringComparer.Ordinal);

    public ISet<Guid> AcceptedVitalSignsIdsInBatch { get; } = new HashSet<Guid>();

    public ISet<string> AcceptedEncounterFoliosInBatch { get; } = new HashSet<string>(StringComparer.Ordinal);

    public ISet<string> AcceptedEncounterVisitServiceKeysInBatch { get; } = new HashSet<string>(StringComparer.Ordinal);

    public ISet<Guid> AcceptedFormResponseIdsInBatch { get; } = new HashSet<Guid>();

    public ISet<string> AcceptedFormResponseEncounterTemplateKeysInBatch { get; } = new HashSet<string>(StringComparer.Ordinal);

    public ISet<Guid> AcceptedConsentDocumentIdsInBatch { get; } = new HashSet<Guid>();

    public ISet<string> AcceptedConsentDocumentKeysInBatch { get; } = new HashSet<string>(StringComparer.Ordinal);

    public ISet<Guid> AcceptedMedicalReferralIdsInBatch { get; } = new HashSet<Guid>();

    public ISet<string> AcceptedMedicalReferralFoliosInBatch { get; } = new HashSet<string>(StringComparer.Ordinal);

    public ISet<Guid> AcceptedMedicationDeliveryIdsInBatch { get; } = new HashSet<Guid>();
}