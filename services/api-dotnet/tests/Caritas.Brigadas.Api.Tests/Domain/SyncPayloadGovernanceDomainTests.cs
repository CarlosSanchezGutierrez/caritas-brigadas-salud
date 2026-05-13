using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Domain;

public sealed class SyncPayloadGovernanceDomainTests
{
    [Theory]
    [InlineData(SyncEntityType.Patient)]
    [InlineData(SyncEntityType.PatientVisit)]
    [InlineData(SyncEntityType.ServiceEncounter)]
    [InlineData(SyncEntityType.VitalSigns)]
    [InlineData(SyncEntityType.FormResponse)]
    [InlineData(SyncEntityType.ConsentDocument)]
    [InlineData(SyncEntityType.DocumentSignature)]
    [InlineData(SyncEntityType.MedicalReferral)]
    [InlineData(SyncEntityType.MedicationDelivery)]
    [InlineData(SyncEntityType.MediaRelease)]
    public void Constructor_AcceptsAllowedEntityTypes(string entityType)
    {
        var syncEvent = CreateValid(entityType, SyncOperation.Create);

        Assert.Equal(entityType, syncEvent.EntityType);
    }

    [Theory]
    [InlineData(SyncOperation.Create)]
    [InlineData(SyncOperation.Update)]
    [InlineData(SyncOperation.Void)]
    [InlineData(SyncOperation.Sign)]
    [InlineData(SyncOperation.Sync)]
    public void Constructor_AcceptsAllowedOperations(string operation)
    {
        var syncEvent = CreateValid(SyncEntityType.Patient, operation);

        Assert.Equal(operation, syncEvent.Operation);
    }

    [Fact]
    public void Constructor_RejectsUnknownEntityType()
    {
        Assert.Throws<DomainException>(() =>
            CreateValid("arbitrary_table", SyncOperation.Create));
    }

    [Fact]
    public void Constructor_RejectsUnknownOperation()
    {
        Assert.Throws<DomainException>(() =>
            CreateValid(SyncEntityType.Patient, "drop_database"));
    }

    [Fact]
    public void SyncEntityType_AllowedListIsExplicitAndStable()
    {
        var expected = new[]
        {
            "consent_document",
            "document_signature",
            "form_response",
            "media_release",
            "medical_referral",
            "medication_delivery",
            "patient",
            "patient_visit",
            "service_encounter",
            "vital_signs"
        };

        Assert.Equal(expected, SyncEntityType.Allowed.Order(StringComparer.Ordinal).ToArray());
    }

    [Fact]
    public void SyncOperation_AllowedListIsExplicitAndStable()
    {
        var expected = new[]
        {
            "create",
            "sign",
            "sync",
            "update",
            "void"
        };

        Assert.Equal(expected, SyncOperation.Allowed.Order(StringComparer.Ordinal).ToArray());
    }

    private static SyncEvent CreateValid(string entityType, string operation)
    {
        return new SyncEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "local-event-001",
            entityType,
            operation,
            """{""value"":true}""",
            idempotencyKey: $"idem-{Guid.NewGuid():N}");
    }
}