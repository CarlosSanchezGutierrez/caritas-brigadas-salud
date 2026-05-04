using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class MedicationDeliveryTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateMedicationDelivery()
    {
        var organizationId = Guid.NewGuid();
        var encounterId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        var delivery = new MedicationDelivery(
            Guid.NewGuid(),
            organizationId,
            encounterId,
            patientId,
            " Paracetamol ",
            " Tabletas ",
            " 10 piezas ",
            " LOTE123 ",
            DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6)),
            " Tomar cada 8 horas ");

        Assert.Equal(organizationId, delivery.OrganizationId);
        Assert.Equal(encounterId, delivery.EncounterId);
        Assert.Equal(patientId, delivery.PatientId);
        Assert.Equal("Paracetamol", delivery.MedicationName);
        Assert.Equal("Tabletas", delivery.Presentation);
        Assert.Equal("10 piezas", delivery.Quantity);
        Assert.Equal("LOTE123", delivery.LotNumber);
        Assert.Equal("Tomar cada 8 horas", delivery.Instructions);
        Assert.Equal(MedicationDeliveryStatus.Created, delivery.Status);
        Assert.False(delivery.IsDelivered);
    }

    [Fact]
    public void Constructor_WithEmptyMedicationName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new MedicationDelivery(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                " "));
    }

    [Fact]
    public void Constructor_WithExpiredMedication_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new MedicationDelivery(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Paracetamol",
                expirationDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1))));
    }

    [Fact]
    public void UpdateMedicationDetails_WhenCreated_ShouldUpdateFields()
    {
        var delivery = CreateDelivery();

        delivery.UpdateMedicationDetails(
            "Ibuprofeno",
            "Cápsulas",
            "5 piezas",
            "L2",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
            "Tomar con alimentos");

        Assert.Equal("Ibuprofeno", delivery.MedicationName);
        Assert.Equal("Cápsulas", delivery.Presentation);
        Assert.Equal("5 piezas", delivery.Quantity);
        Assert.Equal("L2", delivery.LotNumber);
        Assert.Equal("Tomar con alimentos", delivery.Instructions);
    }

    [Fact]
    public void MarkDelivered_ShouldSetDeliveryMetadata()
    {
        var delivery = CreateDelivery();
        var userId = Guid.NewGuid();
        var signatureId = Guid.NewGuid();

        delivery.MarkDelivered(
            userId,
            "Paciente",
            signatureId);

        Assert.Equal(MedicationDeliveryStatus.Delivered, delivery.Status);
        Assert.True(delivery.IsDelivered);
        Assert.Equal(userId, delivery.DeliveredByUserId);
        Assert.Equal("Paciente", delivery.ReceivedByName);
        Assert.Equal(signatureId, delivery.SignatureId);
    }

    [Fact]
    public void Cancel_AfterDelivered_ShouldThrowDomainException()
    {
        var delivery = CreateDelivery();

        delivery.MarkDelivered(Guid.NewGuid(), "Paciente", null);

        Assert.Throws<DomainException>(delivery.Cancel);
    }

    private static MedicationDelivery CreateDelivery()
    {
        return new MedicationDelivery(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Paracetamol",
            expirationDate: DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6)));
    }
}
