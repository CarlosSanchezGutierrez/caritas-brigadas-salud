using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class CryptoIntegrityRecordTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateIntegrityRecord()
    {
        var organizationId = Guid.NewGuid();
        var entityId = Guid.NewGuid();
        var createdByUserId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;

        var record = new CryptoIntegrityRecord(
            Guid.NewGuid(),
            organizationId,
            " AuditEvent ",
            entityId,
            " sha256 ",
            " HASH1 ",
            createdAt,
            createdByUserId,
            previousHash: "HASH0",
            chainKey: "audit-chain");

        Assert.Equal(organizationId, record.OrganizationId);
        Assert.Equal("auditevent", record.EntityType);
        Assert.Equal(entityId, record.EntityId);
        Assert.Equal(HashAlgorithmName.Sha256, record.HashAlgorithm);
        Assert.Equal("HASH1", record.PayloadHash);
        Assert.Equal("HASH0", record.PreviousHash);
        Assert.Equal("audit-chain", record.ChainKey);
        Assert.Equal(createdAt, record.CreatedAt);
        Assert.Equal(createdByUserId, record.CreatedByUserId);
        Assert.Equal(CryptoIntegrityStatus.Created, record.Status);
        Assert.True(record.HasChainLink);
    }

    [Fact]
    public void Constructor_WithEmptyEntityId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new CryptoIntegrityRecord(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "audit_event",
                Guid.Empty,
                HashAlgorithmName.Sha256,
                "HASH1",
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void MarkVerified_ShouldSetVerificationMetadata()
    {
        var record = CreateRecord();
        var userId = Guid.NewGuid();
        var verifiedAt = DateTimeOffset.UtcNow;

        record.MarkVerified(userId, verifiedAt);

        Assert.Equal(CryptoIntegrityStatus.Verified, record.Status);
        Assert.Equal(userId, record.VerifiedByUserId);
        Assert.Equal(verifiedAt, record.VerifiedAt);
        Assert.True(record.IsVerified);
    }

    [Fact]
    public void MarkFailed_ShouldSetFailureMetadata()
    {
        var record = CreateRecord();
        var userId = Guid.NewGuid();
        var verifiedAt = DateTimeOffset.UtcNow;

        record.MarkFailed(userId, verifiedAt, "Hash mismatch");

        Assert.Equal(CryptoIntegrityStatus.Failed, record.Status);
        Assert.Equal(userId, record.VerifiedByUserId);
        Assert.Equal(verifiedAt, record.VerifiedAt);
        Assert.Equal("Hash mismatch", record.VerificationError);
        Assert.True(record.IsFailed);
    }

    [Fact]
    public void Void_WhenCreated_ShouldSetVoided()
    {
        var record = CreateRecord();

        record.Void();

        Assert.Equal(CryptoIntegrityStatus.Voided, record.Status);
    }

    [Fact]
    public void Void_AfterVerified_ShouldThrowDomainException()
    {
        var record = CreateRecord();

        record.MarkVerified(Guid.NewGuid(), DateTimeOffset.UtcNow);

        Assert.Throws<DomainException>(record.Void);
    }

    private static CryptoIntegrityRecord CreateRecord()
    {
        return new CryptoIntegrityRecord(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "audit_event",
            Guid.NewGuid(),
            HashAlgorithmName.Sha256,
            "HASH1",
            DateTimeOffset.UtcNow);
    }
}
