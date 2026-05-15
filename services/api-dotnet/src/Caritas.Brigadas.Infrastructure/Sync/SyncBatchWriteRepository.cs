using System.Security.Cryptography;
using System.Text;
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
    private const int MaxClientInstanceIdLength = 150;
    private const int MaxIdempotencyKeyLength = 250;

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
        if (organizationId == Guid.Empty)
        {
            throw new DomainException("Organization id is required.");
        }

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

        var clientInstanceId = NormalizeClientInstanceId(request.ClientInstanceId);

        if (!request.DeviceId.HasValue && string.IsNullOrWhiteSpace(clientInstanceId))
        {
            throw new DomainException("Client instance id is required when device id is not provided.");
        }

        var syncPayloadEvents = ExtractSyncPayloadEvents(request.PayloadJson);

        if (request.EventsCount.HasValue &&
            request.EventsCount.Value != syncPayloadEvents.Count)
        {
            throw new DomainException("Events count does not match payload event count.");
        }

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

        var batch = new SyncBatch(
            id: Guid.NewGuid(),
            organizationId: organizationId,
            userId: request.UserId,
            brigadeId: request.BrigadeId,
            deviceId: request.DeviceId,
            startedAt: request.StartedAt ?? DateTimeOffset.UtcNow,
            eventsCount: syncPayloadEvents.Count);

        var events = syncPayloadEvents
            .Select(item => new SyncEvent(
                id: Guid.NewGuid(),
                syncBatchId: batch.Id,
                organizationId: organizationId,
                localEventId: item.LocalEventId,
                entityType: item.EntityType,
                operation: item.Operation,
                payloadJson: item.PayloadJson,
                entityId: item.EntityId,
                createdAtDevice: item.CreatedAtDevice,
                idempotencyKey: BuildIdempotencyKey(
                    organizationId,
                    request.UserId,
                    request.BrigadeId,
                    request.DeviceId,
                    clientInstanceId,
                    item.LocalEventId)))
            .ToArray();

        var duplicateKeysInsideBatch = events
            .GroupBy(syncEvent => syncEvent.IdempotencyKey, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();

        if (duplicateKeysInsideBatch.Length > 0)
        {
            throw new DomainException("Payload contains duplicate sync event idempotency keys.");
        }

        var eventKeys = events
            .Select(syncEvent => syncEvent.IdempotencyKey)
            .ToArray();

        var existingEvents = await _dbContext.SyncEvents
            .AsNoTracking()
            .Where(syncEvent =>
                syncEvent.OrganizationId == organizationId &&
                eventKeys.Contains(syncEvent.IdempotencyKey))
            .Select(syncEvent => new
            {
                syncEvent.IdempotencyKey,
                syncEvent.SyncBatchId
            })
            .ToArrayAsync(cancellationToken);

        if (existingEvents.Length > 0)
        {
            var existingKeySet = existingEvents
                .Select(syncEvent => syncEvent.IdempotencyKey)
                .ToHashSet(StringComparer.Ordinal);

            if (existingKeySet.Count == events.Length)
            {
                var existingBatchIds = existingEvents
                    .Select(syncEvent => syncEvent.SyncBatchId)
                    .Distinct()
                    .ToArray();

                if (existingBatchIds.Length == 1)
                {
                    var existingBatch = await _dbContext.SyncBatches
                        .AsNoTracking()
                        .SingleOrDefaultAsync(
                            syncBatch =>
                                syncBatch.Id == existingBatchIds[0] &&
                                syncBatch.OrganizationId == organizationId,
                            cancellationToken);

                    if (existingBatch is not null)
                    {
                        return ToSummaryDto(existingBatch);
                    }
                }
            }

            throw new InvalidOperationException("Payload contains sync events that were already submitted in a different batch.");
        }

        _dbContext.SyncBatches.Add(batch);
        _dbContext.SyncEvents.AddRange(events);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToSummaryDto(batch);
    }

    private static SyncBatchSummaryDto ToSummaryDto(SyncBatch batch)
    {
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

    private static IReadOnlyCollection<SyncPayloadEventItem> ExtractSyncPayloadEvents(string payloadJson)
    {
        try
        {
            using var document = JsonDocument.Parse(payloadJson);

            var eventElements = GetEventElements(document.RootElement);

            return eventElements
                .Select(ParseSyncPayloadEvent)
                .ToArray();
        }
        catch (JsonException exception)
        {
            throw new DomainException($"Payload JSON is invalid: {exception.Message}");
        }
    }

    private static IReadOnlyCollection<JsonElement> GetEventElements(JsonElement root)
    {
        if (root.ValueKind == JsonValueKind.Array)
        {
            return root.EnumerateArray().ToArray();
        }

        if (root.ValueKind == JsonValueKind.Object &&
            root.TryGetProperty("events", out var events) &&
            events.ValueKind == JsonValueKind.Array)
        {
            return events.EnumerateArray().ToArray();
        }

        if (root.ValueKind == JsonValueKind.Object &&
            root.TryGetProperty("items", out var items) &&
            items.ValueKind == JsonValueKind.Array)
        {
            return items.EnumerateArray().ToArray();
        }

        throw new DomainException("Payload JSON must contain an events array.");
    }

    private static SyncPayloadEventItem ParseSyncPayloadEvent(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            throw new DomainException("Each sync event must be a JSON object.");
        }

        var localEventId = GetRequiredString(element, "localEventId");
        var entityType = GetRequiredString(element, "entityType");
        var operation = GetRequiredString(element, "operation");
        var payloadJson = GetRequiredRawJson(element, "payload");

        Guid? entityId = null;

        if (element.TryGetProperty("entityId", out var entityIdElement) &&
            entityIdElement.ValueKind != JsonValueKind.Null)
        {
            if (entityIdElement.ValueKind != JsonValueKind.String ||
                !Guid.TryParse(entityIdElement.GetString(), out var parsedEntityId))
            {
                throw new DomainException("entityId must be a valid GUID when provided.");
            }

            entityId = parsedEntityId;
        }

        DateTimeOffset? createdAtDevice = null;

        if (element.TryGetProperty("createdAtDevice", out var createdAtDeviceElement) &&
            createdAtDeviceElement.ValueKind != JsonValueKind.Null)
        {
            if (createdAtDeviceElement.ValueKind != JsonValueKind.String ||
                !DateTimeOffset.TryParse(createdAtDeviceElement.GetString(), out var parsedCreatedAtDevice))
            {
                throw new DomainException("createdAtDevice must be a valid date/time when provided.");
            }

            createdAtDevice = parsedCreatedAtDevice;
        }

        return new SyncPayloadEventItem(
            localEventId,
            entityType,
            operation,
            payloadJson,
            entityId,
            createdAtDevice);
    }

    private static string GetRequiredString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property) ||
            property.ValueKind != JsonValueKind.String ||
            string.IsNullOrWhiteSpace(property.GetString()))
        {
            throw new DomainException($"{propertyName} is required.");
        }

        return property.GetString()!.Trim();
    }

    private static string GetRequiredRawJson(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property) ||
            property.ValueKind == JsonValueKind.Null ||
            property.ValueKind == JsonValueKind.Undefined)
        {
            throw new DomainException($"{propertyName} is required.");
        }

        return property.GetRawText();
    }

    private static string BuildIdempotencyKey(
        Guid organizationId,
        Guid userId,
        Guid brigadeId,
        Guid? deviceId,
        string? clientInstanceId,
        string localEventId)
    {
        if (deviceId.HasValue)
        {
            var deviceKey = $"org:{organizationId:N}:device:{deviceId.Value:N}:event:{localEventId}";
            return EnsureIdempotencyKeyLength(deviceKey);
        }

        var rawKey = $"org:{organizationId:N}:user:{userId:N}:brigade:{brigadeId:N}:client:{clientInstanceId}:event:{localEventId}";
        return BuildHashedIdempotencyKey(organizationId, rawKey);
    }

    private static string BuildHashedIdempotencyKey(Guid organizationId, string rawKey)
    {
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawKey))).ToLowerInvariant();
        var idempotencyKey = $"org:{organizationId:N}:sync:{hash}";

        return EnsureIdempotencyKeyLength(idempotencyKey);
    }

    private static string EnsureIdempotencyKeyLength(string idempotencyKey)
    {
        if (idempotencyKey.Length > MaxIdempotencyKeyLength)
        {
            throw new DomainException($"Idempotency key cannot exceed {MaxIdempotencyKeyLength} characters.");
        }

        return idempotencyKey;
    }

    private static string? NormalizeClientInstanceId(string? clientInstanceId)
    {
        if (string.IsNullOrWhiteSpace(clientInstanceId))
        {
            return null;
        }

        var normalized = clientInstanceId.Trim();

        if (normalized.Length > MaxClientInstanceIdLength)
        {
            throw new DomainException($"Client instance id cannot exceed {MaxClientInstanceIdLength} characters.");
        }

        return normalized;
    }

    private sealed record SyncPayloadEventItem(
        string LocalEventId,
        string EntityType,
        string Operation,
        string PayloadJson,
        Guid? EntityId,
        DateTimeOffset? CreatedAtDevice);
}
