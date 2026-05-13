using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Persistence;

public sealed class EfSyncIdempotencyContractTests
{
    [Fact]
    public void SyncEvent_MapsRequiredIdempotencyKey()
    {
        var entity = CreateModel().FindEntityType(typeof(SyncEvent));

        Assert.NotNull(entity);

        var property = entity!.FindProperty(nameof(SyncEvent.IdempotencyKey));

        Assert.NotNull(property);
        Assert.False(property!.IsNullable);
        Assert.Equal(250, property.GetMaxLength());
    }

    [Fact]
    public void SyncEvent_HasCrossBatchIdempotencyUniqueIndex()
    {
        var entity = CreateModel().FindEntityType(typeof(SyncEvent));

        Assert.NotNull(entity);

        var uniqueIndexes = entity!
            .GetIndexes()
            .Where(index => index.IsUnique)
            .Select(index => string.Join(", ", index.Properties.Select(property => property.Name)))
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Contains("OrganizationId, IdempotencyKey", uniqueIndexes);
        Assert.Contains("SyncBatchId, LocalEventId", uniqueIndexes);
    }

    [Fact]
    public void SyncEvent_IdempotencyGuardrailIsNotOnlyBatchScoped()
    {
        var entity = CreateModel().FindEntityType(typeof(SyncEvent));

        Assert.NotNull(entity);

        var hasCrossBatchGuardrail = entity!
            .GetIndexes()
            .Any(index =>
                index.IsUnique &&
                index.Properties.Select(property => property.Name).SequenceEqual(new[]
                {
                    nameof(SyncEvent.OrganizationId),
                    nameof(SyncEvent.IdempotencyKey)
                }));

        Assert.True(
            hasCrossBatchGuardrail,
            "SyncEvent must have a unique idempotency guardrail outside SyncBatchId-only scope.");
    }

    private static Microsoft.EntityFrameworkCore.Metadata.IModel CreateModel()
    {
        var options = new DbContextOptionsBuilder<CaritasDbContext>()
            .UseSqlServer("Server=localhost;Database=Caritas_ModelOnly;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        using var dbContext = new CaritasDbContext(options);

        return dbContext.Model;
    }
}