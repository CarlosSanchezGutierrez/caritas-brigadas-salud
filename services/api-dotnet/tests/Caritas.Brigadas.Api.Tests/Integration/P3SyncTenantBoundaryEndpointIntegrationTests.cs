using System.Net;
using System.Text.Json;
using Caritas.Brigadas.Application.Sync;
using Caritas.Brigadas.Contracts.Patients;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Caritas.Brigadas.Infrastructure.Sync;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Integration;

public sealed class P3SyncTenantBoundaryEndpointIntegrationTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task GetByIdEndpoint_WhenBatchBelongsToAnotherOrganization_ReturnsNotFoundWithoutLeakingPayload()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var factory = CreateFactory($"p3-sync-tenant-get-{Guid.NewGuid():N}");

        var owningOrganizationId = Guid.NewGuid();
        var otherOrganizationId = Guid.NewGuid();
        var owningUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var owningBrigadeId = Guid.NewGuid();
        var otherBrigadeId = Guid.NewGuid();
        var syncBatchId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        await SeedCrossTenantSyncBatchAsync(
            factory,
            owningOrganizationId,
            otherOrganizationId,
            owningUserId,
            otherUserId,
            owningBrigadeId,
            otherBrigadeId,
            syncBatchId,
            deviceId,
            patientId,
            cancellationToken);

        using var client = factory.CreateClient();

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/v1/organizations/{otherOrganizationId}/sync-batches/{syncBatchId}");

        AddDevelopmentAuthHeaders(
            request,
            otherOrganizationId,
            otherUserId,
            roles: "SUPER_ADMIN",
            permissions: "sync-batches.read");

        using var response = await client.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Sync batch was not found.", responseBody, StringComparison.Ordinal);
        Assert.DoesNotContain(syncBatchId.ToString(), responseBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("TenantBoundarySensitiveNameShouldNotLeak", responseBody, StringComparison.Ordinal);
        Assert.DoesNotContain("TenantBoundarySensitivePhoneShouldNotLeak", responseBody, StringComparison.Ordinal);
        Assert.DoesNotContain("payloadJson", responseBody, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ProcessEndpoint_WhenBatchBelongsToAnotherOrganization_ReturnsNotFoundAndDoesNotProcess()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var factory = CreateFactory($"p3-sync-tenant-process-{Guid.NewGuid():N}");

        var owningOrganizationId = Guid.NewGuid();
        var otherOrganizationId = Guid.NewGuid();
        var owningUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var owningBrigadeId = Guid.NewGuid();
        var otherBrigadeId = Guid.NewGuid();
        var syncBatchId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        await SeedCrossTenantSyncBatchAsync(
            factory,
            owningOrganizationId,
            otherOrganizationId,
            owningUserId,
            otherUserId,
            owningBrigadeId,
            otherBrigadeId,
            syncBatchId,
            deviceId,
            patientId,
            cancellationToken);

        using var client = factory.CreateClient();

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/v1/organizations/{otherOrganizationId}/sync-batches/{syncBatchId}/process");

        AddDevelopmentAuthHeaders(
            request,
            otherOrganizationId,
            otherUserId,
            roles: "SUPER_ADMIN",
            permissions: "sync-batches.write");

        using var response = await client.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Sync batch was not found.", responseBody, StringComparison.Ordinal);
        Assert.DoesNotContain("TenantBoundarySensitiveNameShouldNotLeak", responseBody, StringComparison.Ordinal);
        Assert.DoesNotContain("TenantBoundarySensitivePhoneShouldNotLeak", responseBody, StringComparison.Ordinal);
        Assert.DoesNotContain("payloadJson", responseBody, StringComparison.OrdinalIgnoreCase);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CaritasDbContext>();

        Assert.Equal(0, await dbContext.Patients.CountAsync(cancellationToken));

        var batch = await dbContext.SyncBatches.SingleAsync(cancellationToken);
        Assert.Equal(owningOrganizationId, batch.OrganizationId);
        Assert.Equal(SyncBatchStatus.Received, batch.Status);
        Assert.Equal(1, batch.EventsCount);
        Assert.Equal(0, batch.AcceptedCount);
        Assert.Equal(0, batch.RejectedCount);
        Assert.Equal(0, batch.ConflictCount);

        var syncEvent = await dbContext.SyncEvents.SingleAsync(cancellationToken);
        Assert.Equal(owningOrganizationId, syncEvent.OrganizationId);
        Assert.Equal(SyncEventStatus.Pending, syncEvent.Status);
        Assert.Contains("TenantBoundarySensitiveNameShouldNotLeak", syncEvent.PayloadJson, StringComparison.Ordinal);
    }

    private static WebApplicationFactory<Program> CreateFactory(string databaseName)
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("environment", "Development");

                builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                {
                    configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Authentication:Mode"] = "Development",
                        ["ConnectionStrings:SqlServer"] = string.Empty,
                        ["Features:EnableSwaggerInDevelopment"] = "false",
                        ["Security:RateLimiting:Enabled"] = "false"
                    });
                });

                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<CaritasDbContext>>();
                    services.RemoveAll<DbContextOptions>();
                    services.RemoveAll<IDbContextOptionsConfiguration<CaritasDbContext>>();
                    services.RemoveAll<CaritasDbContext>();

                    services.AddDbContext<CaritasDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(databaseName);
                    });

                    services.AddScoped<ISyncBatchReadRepository, SyncBatchReadRepository>();
                    services.AddScoped<ISyncBatchProcessor, SyncBatchProcessor>();
                });
            });
    }

    private static async Task SeedCrossTenantSyncBatchAsync(
        WebApplicationFactory<Program> factory,
        Guid owningOrganizationId,
        Guid otherOrganizationId,
        Guid owningUserId,
        Guid otherUserId,
        Guid owningBrigadeId,
        Guid otherBrigadeId,
        Guid syncBatchId,
        Guid deviceId,
        Guid patientId,
        CancellationToken cancellationToken)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CaritasDbContext>();

        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;

        dbContext.Organizations.AddRange(
            new Organization(
                owningOrganizationId,
                "Caritas Monterrey P3 Tenant Owner"),
            new Organization(
                otherOrganizationId,
                "Caritas Monterrey P3 Tenant Other"));

        dbContext.Users.AddRange(
            new User(
                owningUserId,
                owningOrganizationId,
                "Owner User",
                "owner.tenant.boundary@caritas.local"),
            new User(
                otherUserId,
                otherOrganizationId,
                "Other User",
                "other.tenant.boundary@caritas.local"));

        dbContext.Brigades.AddRange(
            new Brigade(
                owningBrigadeId,
                owningOrganizationId,
                "Owner Brigade",
                "medical",
                DateOnly.FromDateTime(now.UtcDateTime),
                municipality: "Monterrey",
                colony: "Centro",
                locationText: "Owner Caritas Monterrey"),
            new Brigade(
                otherBrigadeId,
                otherOrganizationId,
                "Other Brigade",
                "medical",
                DateOnly.FromDateTime(now.UtcDateTime),
                municipality: "San Pedro",
                colony: "Centro",
                locationText: "Other Caritas Monterrey"));

        dbContext.SyncBatches.Add(new SyncBatch(
            syncBatchId,
            owningOrganizationId,
            deviceId,
            owningUserId,
            now,
            owningBrigadeId,
            eventsCount: 1));

        var sensitivePayload = JsonSerializer.Serialize(
            new CreatePatientRequest
            {
                PatientFolio = "PAT-TENANT-BOUNDARY-001",
                FirstName = "TenantBoundarySensitiveNameShouldNotLeak",
                PaternalLastName = "TenantBoundarySensitiveLastNameShouldNotLeak",
                ApproximateAge = 42,
                Sex = "female",
                Phone = "TenantBoundarySensitivePhoneShouldNotLeak",
                Municipality = "Monterrey",
                Colony = "Centro",
                IsPartialRecord = false
            },
            JsonOptions);

        dbContext.SyncEvents.Add(new SyncEvent(
            Guid.NewGuid(),
            syncBatchId,
            owningOrganizationId,
            "001-patient-tenant-boundary-api",
            SyncEntityType.Patient,
            SyncOperation.Create,
            sensitivePayload,
            patientId,
            createdAtDevice: now.AddSeconds(-30),
            receivedAtServer: now.AddSeconds(1),
            idempotencyKey: $"org:{owningOrganizationId:N}:device:{deviceId:N}:event:001-patient-tenant-boundary-api"));

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void AddDevelopmentAuthHeaders(
        HttpRequestMessage request,
        Guid organizationId,
        Guid userId,
        string roles,
        string permissions)
    {
        request.Headers.Add("X-Dev-User-Id", userId.ToString());
        request.Headers.Add("X-Dev-Organization-Id", organizationId.ToString());
        request.Headers.Add("X-Dev-Roles", roles);
        request.Headers.Add("X-Dev-Permissions", permissions);
        request.Headers.Add("X-Dev-Name", "P3 API Tenant Boundary Test User");
        request.Headers.Add("X-Dev-Email", "p3.api.tenant.boundary@caritas.local");
    }
}
