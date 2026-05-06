using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class MedicalReferralTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateReferral()
    {
        var organizationId = Guid.NewGuid();
        var encounterId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var referredByUserId = Guid.NewGuid();

        var referral = new MedicalReferral(
            Guid.NewGuid(),
            organizationId,
            encounterId,
            patientId,
            " ref-001 ",
            " Requiere valoración externa ",
            " Hospital Universitario ",
            " Alta ",
            referredByUserId);

        Assert.Equal(organizationId, referral.OrganizationId);
        Assert.Equal(encounterId, referral.EncounterId);
        Assert.Equal(patientId, referral.PatientId);
        Assert.Equal("REF-001", referral.ReferralFolio);
        Assert.Equal("Requiere valoración externa", referral.ReferralReason);
        Assert.Equal("Hospital Universitario", referral.DestinationInstitution);
        Assert.Equal("alta", referral.Priority);
        Assert.Equal(referredByUserId, referral.ReferredByUserId);
        Assert.Equal(MedicalReferralStatus.Created, referral.Status);
        Assert.True(referral.IsCreated);
    }

    [Fact]
    public void Constructor_WithEmptyReason_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new MedicalReferral(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                "REF-001",
                " "));
    }

    [Fact]
    public void UpdateDetails_WhenCreated_ShouldUpdateReferral()
    {
        var referral = CreateReferral();

        referral.UpdateDetails(
            "Nueva razón",
            "Clínica externa",
            "Media");

        Assert.Equal("Nueva razón", referral.ReferralReason);
        Assert.Equal("Clínica externa", referral.DestinationInstitution);
        Assert.Equal("media", referral.Priority);
    }

    [Fact]
    public void AttachProviderSignature_ShouldSetSignatureId()
    {
        var referral = CreateReferral();
        var signatureId = Guid.NewGuid();

        referral.AttachProviderSignature(signatureId);

        Assert.Equal(signatureId, referral.ProviderSignatureId);
    }

    [Fact]
    public void Complete_ShouldSetCompletedStatus()
    {
        var referral = CreateReferral();

        referral.Complete();

        Assert.Equal(MedicalReferralStatus.Completed, referral.Status);
        Assert.True(referral.IsCompleted);
    }

    [Fact]
    public void Cancel_AfterCompleted_ShouldThrowDomainException()
    {
        var referral = CreateReferral();

        referral.Complete();

        Assert.Throws<DomainException>(referral.Cancel);
    }

    private static MedicalReferral CreateReferral()
    {
        return new MedicalReferral(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "REF-001",
            "Razón de referencia");
    }
}
