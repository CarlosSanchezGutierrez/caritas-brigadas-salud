using System.Text.Json;
using Caritas.Brigadas.Application.Sync;
using Caritas.Brigadas.Contracts.Sync;
using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Sync;

public sealed class SyncBatchWriteRepository : ISyncBatchWriteRepository
{
    private readonly CaritasDbContext _dbContext;

    public SyncBatchWriteRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SyncBatchSummaryDto> CreateAsync(
        Guid organizationId,
        CreateSyncBatchRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.UserId == Guid.Empty)
        {
            throw new DomainException("User id is required.");
        }

        if (request.BrigadeId == Guid.Empty)
        {
            throw new DomainException("Brigade id is required.");
        }

        if (string.IsNullOrWhiteSpace(request.PayloadJson))
        {
            throw new DomainException("Payload JSON is required.");
        }

        ValidateJson(request.PayloadJson);

        var organizationExists = await _dbContext.Organizations
            .AsNoTracking()
            .AnyAsync(
                organization =>
                    organization.Id == organizationId &&
                    !organization.IsDeleted,
                cancellationToken);

        if (!organizationExists)
        {
            throw new KeyNotFoundException("Organization was not found.");
        }

        var userExists = await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(
                user =>
                    user.Id == request.UserId &&
                    user.OrganizationId == organizationId &&
                    !user.IsDeleted,
                cancellationToken);

        if (!userExists)
        {
            throw new KeyNotFoundException("User was not found in this organization.");
        }

        var brigadeExists = await _dbContext.Brigades
            .AsNoTracking()
            .AnyAsync(
                brigade =>
                    brigade.Id == request.BrigadeId &&
                    brigade.OrganizationId == organizationId &&
                    !brigade.IsDeleted,
                cancellationToken);

        if (!brigadeExists)
        {
            throw new KeyNotFoundException("Brigade was not found in this organization.");
        }

        var deviceIdForConstructor = request.DeviceId ?? Guid.Empty;

        var batch = new SyncBatch(
            id: Guid.NewGuid(),
            organizationId: organizationId,
            userId: request.UserId,
            brigadeId: request.BrigadeId,
            deviceId: deviceIdForConstructor,
            startedAt: request.StartedAt ?? DateTimeOffset.UtcNow,
            eventsCount: request.EventsCount ?? InferEventsCount(request.PayloadJson));

        _dbContext.SyncBatches.Add(batch);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SyncBatchSummaryDto
        {
            Id = batch.Id,
            OrganizationId = batch.OrganizationId,
            UserId = batch.UserId,
            BrigadeId = batch.BrigadeId,
            DeviceId = batch.DeviceId,
            EventsCount = batch.EventsCount,
            Status = batch.Status.ToString(),
            StartedAt = batch.StartedAt,
            CompletedAt = batch.CompletedAt,
            ErrorSummary = batch.ErrorSummary,
            IsCompleted = batch.IsCompleted
        };
    }

    private static void ValidateJson(string payloadJson)
    {
        try
        {
            using var _ = JsonDocument.Parse(payloadJson);
        }
        catch (JsonException exception)
        {
            throw new DomainException($"Payload JSON is invalid: {exception.Message}");
        }
    }

    private static int InferEventsCount(string payloadJson)
    {
        using var document = JsonDocument.Parse(payloadJson);

        if (document.RootElement.ValueKind == JsonValueKind.Array)
        {
            return document.RootElement.GetArrayLength();
        }

        if (document.RootElement.ValueKind == JsonValueKind.Object &&
            document.RootElement.TryGetProperty("events", out var events) &&
            events.ValueKind == JsonValueKind.Array)
        {
            return events.GetArrayLength();
        }

        if (document.RootElement.ValueKind == JsonValueKind.Object &&
            document.RootElement.TryGetProperty("items", out var items) &&
            items.ValueKind == JsonValueKind.Array)
        {
            return items.GetArrayLength();
        }

        return 1;
    }
}
