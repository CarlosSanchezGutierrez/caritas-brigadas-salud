using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Infrastructure.Sync;

internal static class SyncProcessingOrder
{
    public static int GetOrder(SyncEvent syncEvent)
    {
        if (syncEvent.EntityType == SyncEntityType.Patient &&
            syncEvent.Operation == SyncOperation.Create)
        {
            return 0;
        }

        if (syncEvent.EntityType == SyncEntityType.PatientVisit &&
            syncEvent.Operation == SyncOperation.Create)
        {
            return 1;
        }

        if (syncEvent.EntityType == SyncEntityType.ServiceEncounter &&
            syncEvent.Operation == SyncOperation.Create)
        {
            return 2;
        }

        if (syncEvent.EntityType == SyncEntityType.VitalSigns &&
            syncEvent.Operation == SyncOperation.Create)
        {
            return 3;
        }

        if (syncEvent.EntityType == SyncEntityType.FormResponse &&
            syncEvent.Operation == SyncOperation.Create)
        {
            return 4;
        }

        if (syncEvent.EntityType == SyncEntityType.ConsentDocument &&
            syncEvent.Operation == SyncOperation.Create)
        {
            return 5;
        }

        if (syncEvent.EntityType == SyncEntityType.MedicalReferral &&
            syncEvent.Operation == SyncOperation.Create)
        {
            return 6;
        }

        if (syncEvent.EntityType == SyncEntityType.MedicationDelivery &&
            syncEvent.Operation == SyncOperation.Create)
        {
            return 7;
        }

        return 8;
    }
}