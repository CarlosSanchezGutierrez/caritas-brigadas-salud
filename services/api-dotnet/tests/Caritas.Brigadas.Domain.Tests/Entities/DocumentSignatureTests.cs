using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Domain.Enums;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class DocumentSignatureTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateSignature()
    {
        var signature = new DocumentSignature(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DocumentSignatureRole.Patient,
            DateTimeOffset.UtcNow,
            patientId: Guid.NewGuid(),
            signedByName: "Paciente",
            signatureFileUrl: "encrypted/signature.png",
            signatureHash: "HASH123");

        Assert.Equal(DocumentSignatureRole.Patient, signature.SignedByRole);
        Assert.Equal("Paciente", signature.SignedByName);
        Assert.Equal("encrypted/signature.png", signature.SignatureFileUrl);
        Assert.Equal("HASH123", signature.SignatureHash);
        Assert.Equal(SyncStatus.Synced, signature.SyncStatus);
    }

    [Fact]
    public void Constructor_WhenCreatedOffline_ShouldSetPendingSync()
    {
        var signature = new DocumentSignature(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DocumentSignatureRole.Guardian,
            DateTimeOffset.UtcNow,
            createdOffline: true,
            deviceId: Guid.NewGuid());

        Assert.Equal(SyncStatus.Pending, signature.SyncStatus);
    }
}
