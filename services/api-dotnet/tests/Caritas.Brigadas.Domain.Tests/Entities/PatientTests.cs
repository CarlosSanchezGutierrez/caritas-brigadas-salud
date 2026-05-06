using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Domain.Enums;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class PatientTests
{
    [Fact]
    public void Constructor_WithMinimalData_ShouldCreatePatient()
    {
        var organizationId = Guid.NewGuid();

        var patient = new Patient(
            Guid.NewGuid(),
            organizationId,
            " pat-001 ");

        Assert.Equal(organizationId, patient.OrganizationId);
        Assert.Equal("PAT-001", patient.PatientFolio);
        Assert.Null(patient.FullNameNormalized);
        Assert.Equal(Sex.NotSpecified, patient.Sex);
        Assert.False(patient.IsMinor);
        Assert.True(patient.IsActive);
    }

    [Fact]
    public void Constructor_WithName_ShouldNormalizeFullName()
    {
        var patient = new Patient(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "PAT-001",
            " Carlos ",
            " Sánchez ",
            " Gutiérrez ");

        Assert.Equal("Carlos", patient.FirstName);
        Assert.Equal("Sánchez", patient.PaternalLastName);
        Assert.Equal("Gutiérrez", patient.MaternalLastName);
        Assert.Equal("CARLOS SÁNCHEZ GUTIÉRREZ", patient.FullNameNormalized);
    }

    [Fact]
    public void Constructor_WithEmptyOrganizationId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Patient(Guid.NewGuid(), Guid.Empty, "PAT-001"));
    }

    [Fact]
    public void Constructor_WithEmptyFolio_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Patient(Guid.NewGuid(), Guid.NewGuid(), " "));
    }

    [Fact]
    public void UpdateDemographics_WithApproximateAgeUnder18_ShouldMarkMinor()
    {
        var patient = new Patient(Guid.NewGuid(), Guid.NewGuid(), "PAT-001");

        patient.UpdateDemographics(null, 12, Sex.Male);

        Assert.True(patient.IsMinor);
        Assert.Equal(12, patient.ApproximateAge);
        Assert.Equal(Sex.Male, patient.Sex);
    }

    [Fact]
    public void UpdateDemographics_WithApproximateAge18_ShouldNotMarkMinor()
    {
        var patient = new Patient(Guid.NewGuid(), Guid.NewGuid(), "PAT-001");

        patient.UpdateDemographics(null, 18, Sex.Female);

        Assert.False(patient.IsMinor);
    }

    [Fact]
    public void UpdateDemographics_WithFutureBirthDate_ShouldThrowDomainException()
    {
        var patient = new Patient(Guid.NewGuid(), Guid.NewGuid(), "PAT-001");
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        Assert.Throws<DomainException>(() =>
            patient.UpdateDemographics(futureDate, null, Sex.NotSpecified));
    }

    [Fact]
    public void UpdateSensitiveIdentifiers_ShouldNormalizeCurp()
    {
        var patient = new Patient(Guid.NewGuid(), Guid.NewGuid(), "PAT-001");

        patient.UpdateSensitiveIdentifiers(" abcd1234 ", " 8112345678 ");

        Assert.Equal("ABCD1234", patient.Curp);
        Assert.Equal("8112345678", patient.Phone);
    }

    [Fact]
    public void UpdateLocation_WithBlankValues_ShouldStoreNulls()
    {
        var patient = new Patient(Guid.NewGuid(), Guid.NewGuid(), "PAT-001");

        patient.UpdateLocation(" ", "", null, "   ");

        Assert.Null(patient.AddressLine);
        Assert.Null(patient.Municipality);
        Assert.Null(patient.Colony);
        Assert.Null(patient.Community);
    }

    [Fact]
    public void MarkAsPartialRecord_WithReason_ShouldSetPartialRecord()
    {
        var patient = new Patient(Guid.NewGuid(), Guid.NewGuid(), "PAT-001");

        patient.MarkAsPartialRecord("Paciente no puede proporcionar información completa");

        Assert.True(patient.IsPartialRecord);
        Assert.Equal("Paciente no puede proporcionar información completa", patient.PartialRecordReason);
    }

    [Fact]
    public void MarkAsPartialRecord_WithBlankReason_ShouldThrowDomainException()
    {
        var patient = new Patient(Guid.NewGuid(), Guid.NewGuid(), "PAT-001");

        Assert.Throws<DomainException>(() =>
            patient.MarkAsPartialRecord(" "));
    }

    [Fact]
    public void ClearPartialRecord_ShouldResetPartialRecord()
    {
        var patient = new Patient(Guid.NewGuid(), Guid.NewGuid(), "PAT-001");

        patient.MarkAsPartialRecord("Registro parcial");
        patient.ClearPartialRecord();

        Assert.False(patient.IsPartialRecord);
        Assert.Null(patient.PartialRecordReason);
    }
}
