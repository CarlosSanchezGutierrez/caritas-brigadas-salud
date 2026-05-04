using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class CryptoIntegrityRecord : Entity
{
    private const int MaxEntityTypeLength = 100;
    private const int MaxHashAlgorithmLength = 100;
    private const int MaxHashLength = 256;
    private const int MaxChainKeyLength = 150;
    private const int MaxVerificationErrorLength = 4000;

    private CryptoIntegrityRecord()
    {
        EntityType = string.Empty;
        HashAlgorithm = string.Empty;
        PayloadHash = string.Empty;
        Status = CryptoIntegrityStatus.Created;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public CryptoIntegrityRecord(
        Guid id,
        Guid organizationId,
        string entityType,
        Guid entityId,
        string hashAlgorithm,
        string payloadHash,
        DateTimeOffset createdAt,
        Guid? createdByUserId = null,
        string? previousHash = null,
        string? chainKey = null)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        EntityType = NormalizeRequired(entityType, nameof(entityType), MaxEntityTypeLength).ToLowerInvariant();
        EntityId = RequireGuid(entityId, nameof(entityId));
        HashAlgorithm = NormalizeRequired(hashAlgorithm, nameof(hashAlgorithm), MaxHashAlgorithmLength).ToUpperInvariant();
        PayloadHash = NormalizeRequired(payloadHash, nameof(payloadHash), MaxHashLength);
        PreviousHash = NormalizeOptional(previousHash, nameof(previousHash), MaxHashLength);
        ChainKey = NormalizeOptional(chainKey, nameof(chainKey), MaxChainKeyLength);
        CreatedAt = createdAt;
        CreatedByUserId = createdByUserId;
        Status = CryptoIntegrityStatus.Created;
    }

    public Guid OrganizationId { get; private set; }

    public string EntityType { get; private set; }

    public Guid EntityId { get; private set; }

    public string HashAlgorithm { get; private set; }

    public string PayloadHash { get; private set; }

    public string? PreviousHash { get; private set; }

    public string? ChainKey { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public Guid? CreatedByUserId { get; private set; }

    public string Status { get; private set; }

    public DateTimeOffset? VerifiedAt { get; private set; }

    public Guid? VerifiedByUserId { get; private set; }

    public string? VerificationError { get; private set; }

    public bool IsVerified => Status == CryptoIntegrityStatus.Verified;

    public bool IsFailed => Status == CryptoIntegrityStatus.Failed;

    public bool HasChainLink => !string.IsNullOrWhiteSpace(PreviousHash);

    public void MarkVerified(Guid verifiedByUserId, DateTimeOffset verifiedAt)
    {
        if (Status == CryptoIntegrityStatus.Voided)
        {
            throw new DomainException("Voided integrity records cannot be verified.");
        }

        VerifiedByUserId = RequireGuid(verifiedByUserId, nameof(verifiedByUserId));
        VerifiedAt = verifiedAt;
        VerificationError = null;
        Status = CryptoIntegrityStatus.Verified;
    }

    public void MarkFailed(Guid verifiedByUserId, DateTimeOffset verifiedAt, string verificationError)
    {
        if (Status == CryptoIntegrityStatus.Voided)
        {
            throw new DomainException("Voided integrity records cannot be marked as failed.");
        }

        VerifiedByUserId = RequireGuid(verifiedByUserId, nameof(verifiedByUserId));
        VerifiedAt = verifiedAt;
        VerificationError = NormalizeRequired(verificationError, nameof(verificationError), MaxVerificationErrorLength);
        Status = CryptoIntegrityStatus.Failed;
    }

    public void Void()
    {
        if (Status == CryptoIntegrityStatus.Verified)
        {
            throw new DomainException("Verified integrity records cannot be voided.");
        }

        Status = CryptoIntegrityStatus.Voided;
    }

    private static Guid RequireGuid(Guid value, string fieldName)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException($"{fieldName} cannot be empty.");
        }

        return value;
    }

    private static string NormalizeRequired(string value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{fieldName} is required.");
        }

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new DomainException($"{fieldName} cannot exceed {maxLength} characters.");
        }

        return normalized;
    }

    private static string? NormalizeOptional(string? value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new DomainException($"{fieldName} cannot exceed {maxLength} characters.");
        }

        return normalized;
    }
}

public static class CryptoIntegrityStatus
{
    public const string Created = "created";
    public const string Verified = "verified";
    public const string Failed = "failed";
    public const string Voided = "voided";
}

public static class HashAlgorithmName
{
    public const string Sha256 = "SHA256";
    public const string Sha384 = "SHA384";
    public const string Sha512 = "SHA512";
}
