using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Caritas.Brigadas.Application.Sync;
using Caritas.Brigadas.Contracts.Patients;
using Caritas.Brigadas.Contracts.Sync;
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

public sealed class P3SyncCreateBatchEndpointIntegrationTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task CreateEndpoint_WhenNoAuthenticationHeaders_ReturnsUnauthorized()
    {
        await using var factory = CreateFactory($"p3-sync-create-api-unauthorized-{Guid.NewGuid():N}");
        using var client = factory.CreateClient();

        var organizationId = Guid.NewGuid();

        using var response = await client.PostAsJsonAsync(
            $"/api/v1/organizations/{organizationId}/sync-batches",
            new CreateSyncBatchRequest(),
            JsonOptions,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateEndpoint_WhenAuthenticatedWithSyncWritePermission_CreatesBatchAndEvents()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var factory = CreateFactory($"p3-sync-create-api-{Guid.NewGuid():N}");

        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var brigadeId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var startedAt = DateTimeOffset.UtcNow;

        await SeedOrganizationUserAndBrigadeAsync(
            factory,
            organizationId,
            userId,
            brigadeId,
            cancellationToken);

        using var client = factory.CreateClient();

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/v1/organizations/{organizationId}/sync-batches");

        AddDevelopmentAuthHeaders(
            request,
            organizationId,
            userId,
            roles: "SUPER_ADMIN",
            permissions: "sync-batches.write");

        var payloadJson = JsonSerializer.Serialize(
            new
            {
                events = new object[]
                {
                    new
                    {
                        localEventId = "001-patient-create-api",
                        entityType = SyncEntityType.Patient,
                        operation = SyncOperation.Create,
                        entityId = patientId,
                        createdAtDevice = startedAt.AddSeconds(-30),
                        payload = new CreatePatientRequest
                        {
                            PatientFolio = "PAT-CREATE-API-001",
                            FirstName = "Maria",
                            PaternalLastName = "Lopez",
                            MaternalLastName = "Garcia",
                            ApproximateAge = 42,
                            Sex = "female",
                            Phone = "8180000000",
                            Municipality = "Monterrey",
                            Colony = "Centro",
                            IsPartialRecord = false
                        }
                    }
                }
            },
            JsonOptions);

        var createRequest = new CreateSyncBatchRequest
        {
            UserId = userId,
            BrigadeId = brigadeId,
            DeviceId = deviceId,
            PayloadJson = payloadJson,
            EventsCount = 1,
            StartedAt = startedAt
        };

        request.Content = JsonContent.Create(
            createRequest,
            options: JsonOptions);

        using var response = await client.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        using var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;

        Assert.True(root.GetProperty("success").GetBoolean());
        Assert.Equal("Sync batch received successfully.", root.GetProperty("message").GetString());

        var data = root.GetProperty("data");

        var createdBatchId = data.GetProperty("id").GetGuid();

        Assert.NotEqual(Guid.Empty, createdBatchId);
        Assert.Equal(organizationId, data.GetProperty("organizationId").GetGuid());
        Assert.Equal(userId, data.GetProperty("userId").GetGuid());
        Assert.Equal(brigadeId, data.GetProperty("brigadeId").GetGuid());
        Assert.Equal(deviceId, data.GetProperty("deviceId").GetGuid());
        Assert.Equal(1, data.GetProperty("eventsCount").GetInt32());
        Assert.Equal("received", data.GetProperty("status").GetString());
        Assert.False(data.GetProperty("isCompleted").GetBoolean());

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CaritasDbContext>();

        Assert.Equal(1, await dbContext.SyncBatches.CountAsync(cancellationToken));
        Assert.Equal(1, await dbContext.SyncEvents.CountAsync(cancellationToken));
        Assert.Equal(0, await dbContext.Patients.CountAsync(cancellationToken));

        var batch = await dbContext.SyncBatches.SingleAsync(cancellationToken);

        Assert.Equal(createdBatchId, batch.Id);
        Assert.Equal(SyncBatchStatus.Received, batch.Status);
        Assert.Equal(organizationId, batch.OrganizationId);
        Assert.Equal(userId, batch.UserId);
        Assert.Equal(brigadeId, batch.BrigadeId);
        Assert.Equal(deviceId, batch.DeviceId);
        Assert.Equal(1, batch.EventsCount);
        Assert.Equal(0, batch.AcceptedCount);
        Assert.Equal(0, batch.RejectedCount);
        Assert.Equal(0, batch.ConflictCount);

        var syncEvent = await dbContext.SyncEvents.SingleAsync(cancellationToken);

        Assert.Equal(createdBatchId, syncEvent.SyncBatchId);
        Assert.Equal(organizationId, syncEvent.OrganizationId);
        Assert.Equal("001-patient-create-api", syncEvent.LocalEventId);
        Assert.Equal(SyncEntityType.Patient, syncEvent.EntityType);
        Assert.Equal(SyncOperation.Create, syncEvent.Operation);
        Assert.Equal(patientId, syncEvent.EntityId);
        Assert.Equal(SyncEventStatus.Pending, syncEvent.Status);
        Assert.Contains(
            $"org:{organizationId:N}:device:{deviceId:N}:event:001-patient-create-api",
            syncEvent.IdempotencyKey,
            StringComparison.Ordinal);
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

                    services.AddScoped<ISyncBatchWriteRepository, SyncBatchWriteRepository>();
                });
            });
    }

    private static async Task SeedOrganizationUserAndBrigadeAsync(
        WebApplicationFactory<Program> factory,
        Guid organizationId,
        Guid userId,
        Guid brigadeId,
        CancellationToken cancellationToken)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CaritasDbContext>();

        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;

        dbContext.Organizations.Add(new Organization(
            organizationId,
            "Caritas Monterrey P3 API Create Sync Batch"));

        dbContext.Users.Add(new User(
            userId,
            organizationId,
            "Medico API Create Sync Batch",
            "medico.api.create.sync.batch@caritas.local"));

        dbContext.Brigades.Add(new Brigade(
            brigadeId,
            organizationId,
            "Brigada P3 API Create Sync Batch",
            "medical",
            DateOnly.FromDateTime(now.UtcDateTime),
            municipality: "Monterrey",
            colony: "Centro",
            locationText: "Caritas Monterrey"));

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
        request.Headers.Add("X-Dev-Name", "P3 API Create Sync Batch Test User");
        request.Headers.Add("X-Dev-Email", "p3.api.create.sync.batch@caritas.local");
    }
}
