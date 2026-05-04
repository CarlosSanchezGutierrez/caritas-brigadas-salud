using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class AiRequestLog : Entity
{
    private const int MaxModuleLength = 100;
    private const int MaxPurposeLength = 250;
    private const int MaxProviderLength = 100;
    private const int MaxModelLength = 150;
    private const int MaxHashLength = 128;
    private const int MaxErrorMessageLength = 4000;

    private AiRequestLog()
    {
        Module = string.Empty;
        Purpose = string.Empty;
        Status = AiRequestStatus.Requested;
        RequestedAt = DateTimeOffset.UtcNow;
    }

    public AiRequestLog(
        Guid id,
        Guid organizationId,
        Guid requestedByUserId,
        string module,
        string purpose,
        DateTimeOffset requestedAt,
        string? provider = null,
        string? model = null,
        string? promptHash = null,
        string? inputHash = null,
        bool containsSensitiveData = false)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        RequestedByUserId = RequireGuid(requestedByUserId, nameof(requestedByUserId));
        Module = NormalizeRequired(module, nameof(module), MaxModuleLength).ToLowerInvariant();
        Purpose = NormalizeRequired(purpose, nameof(purpose), MaxPurposeLength);
        Provider = NormalizeOptional(provider, nameof(provider), MaxProviderLength)?.ToLowerInvariant();
        Model = NormalizeOptional(model, nameof(model), MaxModelLength);
        PromptHash = NormalizeOptional(promptHash, nameof(promptHash), MaxHashLength);
        InputHash = NormalizeOptional(inputHash, nameof(inputHash), MaxHashLength);
        ContainsSensitiveData = containsSensitiveData;
        RequestedAt = requestedAt;
        Status = AiRequestStatus.Requested;
    }

    public Guid OrganizationId { get; private set; }

    public Guid RequestedByUserId { get; private set; }

    public string Module { get; private set; }

    public string Purpose { get; private set; }

    public string? Provider { get; private set; }

    public string? Model { get; private set; }

    public string? PromptHash { get; private set; }

    public string? InputHash { get; private set; }

    public string? OutputHash { get; private set; }

    public bool ContainsSensitiveData { get; private set; }

    public string Status { get; private set; }

    public DateTimeOffset RequestedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public string? ErrorMessage { get; private set; }

    public bool IsCompleted => Status == AiRequestStatus.Completed;

    public bool IsFailed => Status == AiRequestStatus.Failed;

    public bool IsBlocked => Status == AiRequestStatus.Blocked;

    public void MarkCompleted(DateTimeOffset completedAt, string? outputHash = null)
    {
        if (Status == AiRequestStatus.Blocked)
        {
            throw new DomainException("Blocked AI requests cannot be completed.");
        }

        if (Status == AiRequestStatus.Failed)
        {
            throw new DomainException("Failed AI requests cannot be completed.");
        }

        CompletedAt = completedAt;
        OutputHash = NormalizeOptional(outputHash, nameof(outputHash), MaxHashLength);
        ErrorMessage = null;
        Status = AiRequestStatus.Completed;
    }

    public void MarkFailed(DateTimeOffset completedAt, string errorMessage)
    {
        if (Status == AiRequestStatus.Completed)
        {
            throw new DomainException("Completed AI requests cannot be marked as failed.");
        }

        CompletedAt = completedAt;
        ErrorMessage = NormalizeRequired(errorMessage, nameof(errorMessage), MaxErrorMessageLength);
        Status = AiRequestStatus.Failed;
    }

    public void MarkBlocked(DateTimeOffset completedAt, string reason)
    {
        if (Status == AiRequestStatus.Completed)
        {
            throw new DomainException("Completed AI requests cannot be blocked.");
        }

        CompletedAt = completedAt;
        ErrorMessage = NormalizeRequired(reason, nameof(reason), MaxErrorMessageLength);
        Status = AiRequestStatus.Blocked;
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

public static class AiRequestStatus
{
    public const string Requested = "requested";
    public const string Completed = "completed";
    public const string Failed = "failed";
    public const string Blocked = "blocked";
}

public static class AiModule
{
    public const string AdministrativeAssistant = "administrative_assistant";
    public const string ReportSummary = "report_summary";
    public const string DataQuality = "data_quality";
    public const string Documentation = "documentation";
    public const string Analytics = "analytics";
}
