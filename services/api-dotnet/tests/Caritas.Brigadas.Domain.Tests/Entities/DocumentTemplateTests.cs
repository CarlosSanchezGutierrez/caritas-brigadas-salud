using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class DocumentTemplateTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateTemplate()
    {
        var template = new DocumentTemplate(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DocumentType.PrivacyNotice,
            " Aviso de privacidad ",
            "2025.1",
            contentText: "Contenido",
            requiresPatientSignature: true);

        Assert.Equal(DocumentType.PrivacyNotice, template.DocumentType);
        Assert.Equal("Aviso de privacidad", template.Title);
        Assert.Equal("2025.1", template.Version);
        Assert.True(template.RequiresPatientSignature);
        Assert.True(template.RequiresAnySignature);
        Assert.True(template.IsActive);
    }

    [Fact]
    public void Constructor_WithInvalidEffectiveDates_ShouldThrowDomainException()
    {
        var start = DateTimeOffset.UtcNow;
        var end = start.AddMinutes(-1);

        Assert.Throws<DomainException>(() =>
            new DocumentTemplate(
                Guid.NewGuid(),
                Guid.NewGuid(),
                DocumentType.Other,
                "Doc",
                "1.0",
                effectiveFrom: start,
                effectiveTo: end));
    }

    [Fact]
    public void Approve_ShouldSetApprovalMetadata()
    {
        var template = new DocumentTemplate(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DocumentType.GeneralInformedConsent,
            "Consentimiento",
            "1.0");

        var userId = Guid.NewGuid();
        var approvedAt = DateTimeOffset.UtcNow;

        template.Approve(userId, approvedAt);

        Assert.Equal(userId, template.ApprovedByUserId);
        Assert.Equal(approvedAt, template.ApprovedAt);
    }
}
