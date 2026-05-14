using System.Collections.Generic;
using System.Text.Json;
using Caritas.Brigadas.Contracts.ConsentDocuments;
using Caritas.Brigadas.Contracts.FormResponses;
using Caritas.Brigadas.Contracts.MedicalReferrals;
using Caritas.Brigadas.Contracts.MedicationDeliveries;
using Caritas.Brigadas.Contracts.Patients;
using Caritas.Brigadas.Contracts.PatientVisits;
using Caritas.Brigadas.Contracts.ServiceEncounters;
using Caritas.Brigadas.Contracts.Sync;
using Caritas.Brigadas.Contracts.VitalSigns;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Caritas.Brigadas.Infrastructure.Sync;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Integration;

public sealed class P3ClinicalSyncEndToEndIntegrationTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task SyncBatchProcessor_ProcessesCompleteClinicalOfflineBatchEndToEnd()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var dbContext = CreateDbContext();

        var seed = await SeedCompleteClinicalBatchAsync(
            dbContext,
            reverseEventInsertionOrder: false,
            cancellationToken);

        var processor = new SyncBatchProcessor(dbContext);

        var result = await processor.ProcessAsync(
            seed.OrganizationId,
            seed.SyncBatchId,
            cancellationToken);

        await AssertCompletedClinicalBatchAsync(
            dbContext,
            result,
            cancellationToken);
    }

    [Fact]
    public async Task SyncBatchProcessor_ProcessesOutOfOrderClinicalOfflineBatchUsingSyncProcessingOrder()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var dbContext = CreateDbContext();

        var seed = await SeedCompleteClinicalBatchAsync(
            dbContext,
            reverseEventInsertionOrder: true,
            cancellationToken);

        var processor = new SyncBatchProcessor(dbContext);

        var result = await processor.ProcessAsync(
            seed.OrganizationId,
            seed.SyncBatchId,
            cancellationToken);

        await AssertCompletedClinicalBatchAsync(
            dbContext,
            result,
            cancellationToken);
    }

    private static async Task<ClinicalSyncSeed> SeedCompleteClinicalBatchAsync(
        CaritasDbContext dbContext,
        bool reverseEventInsertionOrder,
        CancellationToken cancellationToken)
    {
        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var brigadeId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        var formTemplateId = Guid.NewGuid();
        var syncBatchId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();

        var patientId = Guid.NewGuid();
        var visitId = Guid.NewGuid();
        var encounterId = Guid.NewGuid();
        var vitalSignsId = Guid.NewGuid();
        var formResponseId = Guid.NewGuid();
        var consentDocumentId = Guid.NewGuid();
        var medicalReferralId = Guid.NewGuid();
        var medicationDeliveryId = Guid.NewGuid();

        var now = DateTimeOffset.UtcNow;

        dbContext.Organizations.Add(new Organization(
            organizationId,
            "Caritas Monterrey P3 Sync E2E"));

        dbContext.Users.Add(new User(
            userId,
            organizationId,
            "Medico Sync E2E",
            "medico.sync.e2e@caritas.local"));

        dbContext.Brigades.Add(new Brigade(
            brigadeId,
            organizationId,
            "Brigada P3 Sync E2E",
            "medical",
            DateOnly.FromDateTime(now.UtcDateTime),
            municipality: "Monterrey",
            colony: "Centro",
            locationText: "Caritas Monterrey"));

        dbContext.Services.Add(new Service(
            serviceId,
            organizationId,
            ServiceCode.GeneralMedicine,
            "Medicina general",
            "clinical",
            requiresConsent: true,
            requiresClinicalNotes: true,
            requiresFollowUpOption: true,
            requiresReferralOption: true));

        dbContext.BrigadeServices.Add(new BrigadeService(
            Guid.NewGuid(),
            brigadeId,
            serviceId,
            capacityEstimate: 100,
            assignedLeadUserId: userId));

        dbContext.FormTemplates.Add(new FormTemplate(
            formTemplateId,
            organizationId,
            serviceId,
            "GENERAL_MEDICINE_INTAKE",
            "Consulta general inicial",
            "1.0.0",
            """{"type":"object","properties":{"chiefComplaint":{"type":"string"}}}"""));

        var syncBatch = new SyncBatch(
            syncBatchId,
            organizationId,
            deviceId,
            userId,
            now,
            brigadeId,
            eventsCount: 8);

        dbContext.SyncBatches.Add(syncBatch);

        var events = new List<SyncEvent>
        {
            CreateEvent(
                syncBatchId,
                organizationId,
                "001-patient",
                SyncEntityType.Patient,
                patientId,
                new CreatePatientRequest
                {
                    PatientFolio = "PAT-E2E-001",
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
                now.AddSeconds(1)),
            CreateEvent(
                syncBatchId,
                organizationId,
                "002-visit",
                SyncEntityType.PatientVisit,
                visitId,
                new CreatePatientVisitRequest
                {
                    VisitFolio = "VIS-E2E-001",
                    PatientId = patientId,
                    BrigadeId = brigadeId,
                    ArrivalTime = now.AddMinutes(1),
                    RegisteredByUserId = userId,
                    CreatedOffline = true,
                    DeviceId = deviceId
                },
                now.AddSeconds(2)),
            CreateEvent(
                syncBatchId,
                organizationId,
                "003-encounter",
                SyncEntityType.ServiceEncounter,
                encounterId,
                new CreateServiceEncounterRequest
                {
                    EncounterFolio = "ENC-E2E-001",
                    VisitId = visitId,
                    ServiceCode = ServiceCode.GeneralMedicine,
                    ProviderUserId = userId,
                    StartedAt = now.AddMinutes(2),
                    CreatedOffline = true,
                    DeviceId = deviceId
                },
                now.AddSeconds(3)),
            CreateEvent(
                syncBatchId,
                organizationId,
                "004-vital-signs",
                SyncEntityType.VitalSigns,
                vitalSignsId,
                new CreateVitalSignsRecordRequest
                {
                    PatientId = patientId,
                    VisitId = visitId,
                    EncounterId = encounterId,
                    MeasuredByUserId = userId,
                    MeasuredAt = now.AddMinutes(3),
                    SystolicBloodPressureMmHg = 120,
                    DiastolicBloodPressureMmHg = 80,
                    HeartRateBpm = 72,
                    RespiratoryRatePerMinute = 16,
                    TemperatureCelsius = 36.7m,
                    OxygenSaturationPercent = 98,
                    WeightKg = 68.5m,
                    HeightCm = 165.0m,
                    Source = "offline-ipad",
                    Notes = "Paciente estable",
                    DeviceId = deviceId
                },
                now.AddSeconds(4)),
            CreateEvent(
                syncBatchId,
                organizationId,
                "005-form-response",
                SyncEntityType.FormResponse,
                formResponseId,
                new CreateFormResponseRequest
                {
                    EncounterId = encounterId,
                    FormTemplateId = formTemplateId,
                    ResponseJson = """{"chiefComplaint":"dolor de cabeza","durationDays":2}""",
                    SubmittedByUserId = userId,
                    SubmittedAt = now.AddMinutes(4),
                    CreatedOffline = true,
                    DeviceId = deviceId
                },
                now.AddSeconds(5)),
            CreateEvent(
                syncBatchId,
                organizationId,
                "006-consent",
                SyncEntityType.ConsentDocument,
                consentDocumentId,
                new CreateConsentDocumentRequest
                {
                    PatientId = patientId,
                    VisitId = visitId,
                    ConsentType = "privacy_notice",
                    DocumentVersion = "1.0.0",
                    DocumentTextSnapshot = "Aviso de privacidad firmado para atencion medica de brigada.",
                    SignatureDataUrl = "data:image/png;base64,UElORy1TSUdOQVRVUkU=",
                    GuardianFullName = "Tutor Responsable",
                    GuardianRelationship = "Familiar",
                    SignedByUserId = userId,
                    SignedAt = now.AddMinutes(5),
                    CreatedOffline = true,
                    DeviceId = deviceId
                },
                now.AddSeconds(6)),
            CreateEvent(
                syncBatchId,
                organizationId,
                "007-referral",
                SyncEntityType.MedicalReferral,
                medicalReferralId,
                new CreateMedicalReferralRequest
                {
                    EncounterId = encounterId,
                    ReferralFolio = "REF-E2E-001",
                    DestinationInstitution = "Hospital General",
                    ReferralReason = "Valoracion por especialidad",
                    Priority = "normal",
                    ReferredByUserId = userId,
                    CreatedOffline = true,
                    DeviceId = deviceId
                },
                now.AddSeconds(7)),
            CreateEvent(
                syncBatchId,
                organizationId,
                "008-medication",
                SyncEntityType.MedicationDelivery,
                medicationDeliveryId,
                new CreateMedicationDeliveryRequest
                {
                    EncounterId = encounterId,
                    MedicationName = "Paracetamol",
                    Presentation = "Tabletas 500mg",
                    Quantity = "1 caja",
                    LotNumber = "LOT-E2E-001",
                    ExpirationDate = DateOnly.FromDateTime(now.AddYears(1).UtcDateTime),
                    Instructions = "Tomar cada 8 horas por dolor.",
                    DeliveredByUserId = userId,
                    ReceivedByName = "Maria Lopez Garcia",
                    MarkAsDelivered = true,
                    CreatedOffline = true,
                    DeviceId = deviceId
                },
                now.AddSeconds(8))
        };

        if (reverseEventInsertionOrder)
        {
            events.Reverse();
        }

        dbContext.SyncEvents.AddRange(events);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ClinicalSyncSeed(
            organizationId,
            syncBatchId);
    }

    private static async Task AssertCompletedClinicalBatchAsync(
        CaritasDbContext dbContext,
        ProcessSyncBatchResultDto result,
        CancellationToken cancellationToken)
    {
        Assert.True(result.Completed);
        Assert.Equal(8, result.PendingEventsProcessed);
        Assert.Equal(8, result.AcceptedCount);
        Assert.Equal(0, result.RejectedCount);
        Assert.Equal(0, result.ConflictCount);

        Assert.Equal(1, await dbContext.Patients.CountAsync(cancellationToken));
        Assert.Equal(1, await dbContext.PatientVisits.CountAsync(cancellationToken));
        Assert.Equal(1, await dbContext.ServiceEncounters.CountAsync(cancellationToken));
        Assert.Equal(1, await dbContext.VitalSignsRecords.CountAsync(cancellationToken));
        Assert.Equal(1, await dbContext.FormResponses.CountAsync(cancellationToken));
        Assert.Equal(1, await dbContext.ConsentDocuments.CountAsync(cancellationToken));
        Assert.Equal(1, await dbContext.MedicalReferrals.CountAsync(cancellationToken));
        Assert.Equal(1, await dbContext.MedicationDeliveries.CountAsync(cancellationToken));

        var processedEvents = await dbContext.SyncEvents
            .OrderBy(syncEvent => syncEvent.LocalEventId)
            .ToArrayAsync(cancellationToken);

        Assert.All(
            processedEvents,
            syncEvent => Assert.Equal(SyncEventStatus.Accepted, syncEvent.Status));

        var completedBatch = await dbContext.SyncBatches.SingleAsync(cancellationToken);

        Assert.Equal(SyncBatchStatus.Completed, completedBatch.Status);
        Assert.Equal(8, completedBatch.AcceptedCount);
        Assert.Equal(0, completedBatch.RejectedCount);
        Assert.Equal(0, completedBatch.ConflictCount);
    }

    private static CaritasDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CaritasDbContext>()
            .UseInMemoryDatabase($"p3-clinical-sync-e2e-{Guid.NewGuid():N}")
            .EnableSensitiveDataLogging()
            .Options;

        return new CaritasDbContext(options);
    }

    private static SyncEvent CreateEvent<TPayload>(
        Guid syncBatchId,
        Guid organizationId,
        string localEventId,
        string entityType,
        Guid entityId,
        TPayload payload,
        DateTimeOffset receivedAtServer)
    {
        return new SyncEvent(
            Guid.NewGuid(),
            syncBatchId,
            organizationId,
            localEventId,
            entityType,
            SyncOperation.Create,
            JsonSerializer.Serialize(payload, JsonOptions),
            entityId,
            createdAtDevice: receivedAtServer.AddSeconds(-30),
            receivedAtServer: receivedAtServer,
            idempotencyKey: $"p3-e2e-{localEventId}");
    }

    private sealed record ClinicalSyncSeed(
        Guid OrganizationId,
        Guid SyncBatchId);
}
