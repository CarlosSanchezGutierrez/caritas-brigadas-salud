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

public sealed class P3SyncListEventsEndpointIntegrationTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task ListEventsEndpoint_WhenNoAuthenticationHeaders_ReturnsUnauthorized()
    {
        await using var factory = CreateFactory($"p3-sync-list-events-api-unauthorized-{Guid.NewGuid():N}");
        using var client = factory.CreateClient();

        var organizationId = Guid.NewGuid();
        var syncBatchId = Guid.NewGuid();

        using var response = await client.GetAsync(
            $"/api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}/events",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ListEventsEndpoint_WhenAuthenticatedWithSyncReadPermission_ReturnsEventsWithoutPayloadJson()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var factory = CreateFactory($"p3-sync-list-events-api-{Guid.NewGuid():N}");

        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var brigadeId = Guid.NewGuid();
        var syncBatchId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        var sensitivePayload = JsonSerializer.Serialize(
            new CreatePatientRequest
            {
                PatientFolio = "PAT-LIST-EVENTS-001",
                FirstName = "SensitiveFirstNameShouldNotLeak",
                PaternalLastName = "SensitivePaternalLastNameShouldNotLeak",
                MaternalLastName = "SensitiveMaternalLastNameShouldNotLeak",
                ApproximateAge = 42,
                Sex = "female",
                Phone = "8180000000",
                Municipality = "Monterrey",
                Colony = "Centro",
                IsPartialRecord = false
            },
            JsonOptions);

        await SeedSyncBatchWithSensitiveEventAsync(
            factory,
            organizationId,
            userId,
            brigadeId,
            syncBatchId,
            deviceId,
            patientId,
            sensitivePayload,
            cancellationToken);

        using var client = factory.CreateClient();

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}/events?pageNumber=1&pageSize=10");

        AddDevelopmentAuthHeaders(
            request,
            organizationId,
            userId,
            roles: "SUPER_ADMIN",
            permissions: "sync-batches.read");

        using var response = await client.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.DoesNotContain("payloadJson", responseBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("SensitiveFirstNameShouldNotLeak", responseBody, StringComparison.Ordinal);
        Assert.DoesNotContain("SensitivePaternalLastNameShouldNotLeak", responseBody, StringComparison.Ordinal);
        Assert.DoesNotContain("SensitiveMaternalLastNameShouldNotLeak", responseBody, StringComparison.Ordinal);
        Assert.DoesNotContain("8180000000", responseBody, StringComparison.Ordinal);

        using var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;

        Assert.True(root.GetProperty("success").GetBoolean());

        var data = root.GetProperty("data");

        Assert.Equal(1, data.GetProperty("pageNumber").GetInt32());
        Assert.Equal(10, data.GetProperty("pageSize").GetInt32());
        Assert.Equal(1, data.GetProperty("totalCount").GetInt32());

        var items = data.GetProperty("items");

        Assert.Equal(1, items.GetArrayLength());

        var item = items[0];

        Assert.Equal(syncBatchId, item.GetProperty("syncBatchId").GetGuid());
        Assert.Equal(organizationId, item.GetProperty("organizationId").GetGuid());
        Assert.Equal("001-patient-list-events-api", item.GetProperty("localEventId").GetString());
        Assert.Equal(SyncEntityType.Patient, item.GetProperty("entityType").GetString());
        Assert.Equal(SyncOperation.Create, item.GetProperty("operation").GetString());
        Assert.Equal(SyncEventStatus.Pending, item.GetProperty("status").GetString());
        Assert.Equal(patientId, item.GetProperty("entityId").GetGuid());
        Assert.True(item.GetProperty("isPending").GetBoolean());
        Assert.False(item.GetProperty("isAccepted").GetBoolean());
        Assert.False(item.GetProperty("isRejected").GetBoolean());
        Assert.False(item.GetProperty("isConflict").GetBoolean());

        Assert.False(item.TryGetProperty("payloadJson", out _));
        Assert.False(item.TryGetProperty("payload", out _));

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CaritasDbContext>();

        var syncEvent = await dbContext.SyncEvents.SingleAsync(cancellationToken);

        Assert.Contains(
            "SensitiveFirstNameShouldNotLeak",
            syncEvent.PayloadJson,
            StringComparison.Ordinal);

        Assert.Equal(SyncEventStatus.Pending, syncEvent.Status);
    }

    [Fact]
    public async Task ListEventsEndpoint_WhenBatchBelongsToAnotherOrganization_ReturnsNotFound()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var factory = CreateFactory($"p3-sync-list-events-api-tenant-{Guid.NewGuid():N}");

        var organizationId = Guid.NewGuid();
        var otherOrganizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var brigadeId = Guid.NewGuid();
        var syncBatchId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        await SeedSyncBatchWithSensitiveEventAsync(
            factory,
            organizationId,
            userId,
            brigadeId,
            syncBatchId,
            deviceId,
            patientId,
            """{"patientFolio":"PAT-TENANT-001","firstName":"TenantSensitiveName"}""",
            cancellationToken);

        using var client = factory.CreateClient();

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/v1/organizations/{otherOrganizationId}/sync-batches/{syncBatchId}/events");

        AddDevelopmentAuthHeaders(
            request,
            otherOrganizationId,
            userId,
            roles: "SUPER_ADMIN",
            permissions: "sync-batches.read");

        using var response = await client.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.DoesNotContain("TenantSensitiveName", responseBody, StringComparison.Ordinal);
        Assert.Contains("Sync batch was not found.", responseBody, StringComparison.Ordinal);
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
                });
            });
    }

    private static async Task SeedSyncBatchWithSensitiveEventAsync(
        WebApplicationFactory<Program> factory,
        Guid organizationId,
        Guid userId,
        Guid brigadeId,
        Guid syncBatchId,
        Guid deviceId,
        Guid patientId,
        string payloadJson,
        CancellationToken cancellationToken)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CaritasDbContext>();

        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;

        dbContext.Organizations.Add(new Organization(
            organizationId,
            "Caritas Monterrey P3 API List Sync Events"));

        dbContext.Users.Add(new User(
            userId,
            organizationId,
            "Medico API List Sync Events",
            "medico.api.list.sync.events@caritas.local"));

        dbContext.Brigades.Add(new Brigade(
            brigadeId,
            organizationId,
            "Brigada P3 API List Sync Events",
            "medical",
            DateOnly.FromDateTime(now.UtcDateTime),
            municipality: "Monterrey",
            colony: "Centro",
            locationText: "Caritas Monterrey"));

        dbContext.SyncBatches.Add(new SyncBatch(
            syncBatchId,
            organizationId,
            deviceId,
            userId,
            now,
            brigadeId,
            eventsCount: 1));

        dbContext.SyncEvents.Add(new SyncEvent(
            Guid.NewGuid(),
            syncBatchId,
            organizationId,
            "001-patient-list-events-api",
            SyncEntityType.Patient,
            SyncOperation.Create,
            payloadJson,
            patientId,
            createdAtDevice: now.AddSeconds(-30),
            receivedAtServer: now.AddSeconds(1),
            idempotencyKey: $"org:{organizationId:N}:device:{deviceId:N}:event:001-patient-list-events-api"));

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
        request.Headers.Add("X-Dev-Name", "P3 API List Sync Events Test User");
        request.Headers.Add("X-Dev-Email", "p3.api.list.sync.events@caritas.local");
    }
}
