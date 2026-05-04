namespace Caritas.Brigadas.Domain.Enums;

public enum SyncStatus
{
    Pending = 0,
    Syncing = 1,
    Synced = 2,
    Failed = 3,
    Conflict = 4
}
