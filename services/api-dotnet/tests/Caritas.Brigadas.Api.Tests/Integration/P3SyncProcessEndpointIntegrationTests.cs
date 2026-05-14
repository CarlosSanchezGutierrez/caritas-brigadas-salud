using System.Net;
using System.Text.Json;
using Caritas.Brigadas.Contracts.Patients;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Integration;

public sealed class P3SyncProcessEndpointIntegrationTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task ProcessEndpoint_WhenNoAuthenticationHeaders_ReturnsUnauthorized()
    {
        await using var factory = CreateFactory($"p3-sync-api-unauthorized-{Guid.NewGuid():N}");
        using var client = factory.CreateClient();

        var organizationId = Guid.NewGuid();
        var syncBatchId = Guid.NewGuid();

        using var response = await client.PostAsync(
            $"/api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}/process",
            content: null,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProcessEndpoint_WhenAuthenticatedWithSyncWritePermission_ProcessesPendingBatch()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var factory = CreateFactory($"p3-sync-api-process-{Guid.NewGuid():N}");

        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var brigadeId = Guid.NewGuid();
        var syncBatchId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();

        await SeedSinglePatientSyncBatchAsync(
            factory,
            organizationId,
            userId,
            brigadeId,
            syncBatchId,
            deviceId,
            cancellationToken);

        using var client = factory.CreateClient();

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}/process");

        AddDevelopmentAuthHeaders(
            request,
            organizationId,
            userId,
            roles: "SUPER_ADMIN",
            permissions: "sync-batches.write");

        using var response = await client.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;

        Assert.True(root.GetProperty("success").GetBoolean());
        Assert.Equal("Sync batch processed successfully.", root.GetProperty("message").GetString());

        var data = root.GetProperty("data");

        Assert.True(data.GetProperty("completed").GetBoolean());
        Assert.Equal(1, data.GetProperty("pendingEventsProcessed").GetInt32());
        Assert.Equal(1, data.GetProperty("acceptedCount").GetInt32());
        Assert.Equal(0, data.GetProperty("rejectedCount").GetInt32());
        Assert.Equal(0, data.GetProperty("conflictCount").GetInt32());

        var batch = data.GetProperty("batch");

        Assert.Equal("completed", batch.GetProperty("status").GetString());
        Assert.Equal(1, batch.GetProperty("acceptedCount").GetInt32());
        Assert.Equal(0, batch.GetProperty("rejectedCount").GetInt32());
        Assert.Equal(0, batch.GetProperty("conflictCount").GetInt32());

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CaritasDbContext>();

        Assert.Equal(1, await dbContext.Patients.CountAsync(cancellationToken));
        Assert.Equal(1, await dbContext.SyncEvents.CountAsync(cancellationToken));

        var syncEvent = await dbContext.SyncEvents.SingleAsync(cancellationToken);
        Assert.Equal(SyncEventStatus.Accepted, syncEvent.Status);

        var completedBatch = await dbContext.SyncBatches.SingleAsync(cancellationToken);
        Assert.Equal(SyncBatchStatus.Completed, completedBatch.Status);
        Assert.Equal(1, completedBatch.AcceptedCount);
        Assert.Equal(0, completedBatch.RejectedCount);
        Assert.Equal(0, completedBatch.ConflictCount);
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
                        ["ConnectionStrings:SqlServer"] = "Server=(localdb)\\MSSQLLocalDB;Database=CaritasP324A;Trusted_Connection=True;TrustServerCertificate=True;",
                        ["Features:EnableSwaggerInDevelopment"] = "false",
                        ["Security:RateLimiting:Enabled"] = "false"
                    });
                });

                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<CaritasDbContext>>();
                    services.RemoveAll<CaritasDbContext>();

                    services.AddDbContext<CaritasDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(databaseName);
                    });
                });
            });
    }

    private static async Task SeedSinglePatientSyncBatchAsync(
        WebApplicationFactory<Program> factory,
        Guid organizationId,
        Guid userId,
        Guid brigadeId,
        Guid syncBatchId,
        Guid deviceId,
        CancellationToken cancellationToken)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CaritasDbContext>();

        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var patientId = Guid.NewGuid();

        dbContext.Organizations.Add(new Organization(
            organizationId,
            "Caritas Monterrey P3 API Sync Process"));

        dbContext.Users.Add(new User(
            userId,
            organizationId,
            "Medico API Sync Process",
            "medico.api.sync.process@caritas.local"));

        dbContext.Brigades.Add(new Brigade(
            brigadeId,
            organizationId,
            "Brigada P3 API Sync Process",
            "medical",
            DateOnly.FromDateTime(now.UtcDateTime),
            municipality: "Monterrey",
            colony: "Centro",
            locationText: "Caritas Monterrey"));

        var syncBatch = new SyncBatch(
            syncBatchId,
            organizationId,
            deviceId,
            userId,
            now,
            brigadeId,
            eventsCount: 1);

        dbContext.SyncBatches.Add(syncBatch);

        dbContext.SyncEvents.Add(new SyncEvent(
            Guid.NewGuid(),
            syncBatchId,
            organizationId,
            "001-patient-api",
            SyncEntityType.Patient,
            SyncOperation.Create,
            JsonSerializer.Serialize(
                new CreatePatientRequest
                {
                    PatientFolio = "PAT-API-001",
                    FirstName = "Maria",
                    PaternalLastName = "Lopez",
                    MaternalLastName = "Garcia",
                    ApproximateAge = 42,
                    Sex = "female",
                    Phone = "8180000000",
                    Municipality = "Monterrey",
                    Colony = "Centro",
                    IsPartialRecord = false
                },
                JsonOptions),
            patientId,
            createdAtDevice: now.AddSeconds(-30),
            receivedAtServer: now.AddSeconds(1),
            idempotencyKey: "p3-api-process-patient"));

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
        request.Headers.Add("X-Dev-Name", "P3 API Sync Process Test User");
        request.Headers.Add("X-Dev-Email", "p3.api.sync.process@caritas.local");
    }
}
