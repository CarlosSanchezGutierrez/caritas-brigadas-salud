namespace Caritas.Brigadas.Contracts.Sync;

public sealed record ProcessSyncBatchResultDto
{
    public SyncBatchSummaryDto Batch { get; init; } = new();

    public int PendingEventsProcessed { get; init; }

    public int AcceptedCount { get; init; }

    public int RejectedCount { get; init; }

    public int ConflictCount { get; init; }

    public bool Completed { get; init; }

    public string Message { get; init; } = string.Empty;
}