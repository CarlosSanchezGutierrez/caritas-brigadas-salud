using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class PatientGuardianTests
{
    [Fact]
    public void Constructor_WithPresentGuardian_ShouldCreateGuardian()
    {
        var patientId = Guid.NewGuid();

        var guardian = new PatientGuardian(
            Guid.NewGuid(),
            patientId,
            " Edit Gutiérrez ",
            " Madre ",
            " 8112345678 ",
            "INE",
            " ABC123 ",
            isPresent: true,
            isPrimary: true);

        Assert.Equal(patientId, guardian.PatientId);
        Assert.Equal("Edit Gutiérrez", guardian.FullName);
        Assert.Equal("Madre", guardian.Relationship);
        Assert.Equal("8112345678", guardian.Phone);
        Assert.Equal("INE", guardian.IdentificationType);
        Assert.Equal("ABC123", guardian.IdentificationValue);
        Assert.True(guardian.IsPresent);
        Assert.Null(guardian.AbsenceReason);
        Assert.True(guardian.IsPrimary);
    }

    [Fact]
    public void Constructor_WithPresentGuardianAndBlankName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new PatientGuardian(
                Guid.NewGuid(),
                Guid.NewGuid(),
                " ",
                "Madre",
                isPresent: true));
    }

    [Fact]
    public void Constructor_WithAbsentGuardianAndReason_ShouldCreateAbsentGuardianRecord()
    {
        var guardian = new PatientGuardian(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            null,
            isPresent: false,
            absenceReason: "Menor acudió sin tutor durante brigada");

        Assert.False(guardian.IsPresent);
        Assert.Equal("Menor acudió sin tutor durante brigada", guardian.AbsenceReason);
        Assert.Null(guardian.FullName);
    }

    [Fact]
    public void Constructor_WithAbsentGuardianAndNoReason_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new PatientGuardian(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                null,
                isPresent: false,
                absenceReason: " "));
    }

    [Fact]
    public void Constructor_WithEmptyPatientId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new PatientGuardian(
                Guid.NewGuid(),
                Guid.Empty,
                "Tutor",
                "Padre"));
    }

    [Fact]
    public void MarkAsPrimary_ShouldSetPrimary()
    {
        var guardian = new PatientGuardian(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Tutor",
            "Padre");

        guardian.MarkAsPrimary();

        Assert.True(guardian.IsPrimary);
    }

    [Fact]
    public void ClearPrimary_ShouldUnsetPrimary()
    {
        var guardian = new PatientGuardian(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Tutor",
            "Padre",
            isPrimary: true);

        guardian.ClearPrimary();

        Assert.False(guardian.IsPrimary);
    }
}
