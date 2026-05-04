using Caritas.Brigadas.Domain.Common;

namespace Caritas.Brigadas.Domain.Entities;

public sealed class ExportJob : Entity
{
    private const int MaxExportTypeLength = 100;
    private const int MaxFileUrlLength = 500;
    private const int MaxErrorMessageLength = 4000;

    private ExportJob()
    {
        ExportType = string.Empty;
        Status = ExportJobStatus.Pending;
        RequestedAt = DateTimeOffset.UtcNow;
    }

    public ExportJob(
        Guid id,
        Guid organizationId,
        Guid requestedByUserId,
        string exportType,
        DateTimeOffset requestedAt,
        string? filtersJson = null,
        bool includesIdentifiableData = false)
        : base(id)
    {
        OrganizationId = RequireGuid(organizationId, nameof(organizationId));
        RequestedByUserId = RequireGuid(requestedByUserId, nameof(requestedByUserId));
        ExportType = NormalizeRequired(exportType, nameof(exportType), MaxExportTypeLength).ToLowerInvariant();
        FiltersJson = NormalizeOptionalJson(filtersJson);
        IncludesIdentifiableData = includesIdentifiableData;
        Status = ExportJobStatus.Pending;
        RequestedAt = requestedAt;
    }

    public Guid OrganizationId { get; private set; }

    public Guid RequestedByUserId { get; private set; }

    public string ExportType { get; private set; }

    public string? FiltersJson { get; private set; }

    public bool IncludesIdentifiableData { get; private set; }

    public string? FileUrl { get; private set; }

    public string Status { get; private set; }

    public DateTimeOffset RequestedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public string? ErrorMessage { get; private set; }

    public bool IsPending => Status == ExportJobStatus.Pending;

    public bool IsCompleted => Status == ExportJobStatus.Completed;

    public bool IsFailed => Status == ExportJobStatus.Failed;

    public void MarkProcessing()
    {
        if (Status != ExportJobStatus.Pending)
        {
            throw new DomainException("Only pending export jobs can be marked as processing.");
        }

        Status = ExportJobStatus.Processing;
    }

    public void Complete(string fileUrl, DateTimeOffset completedAt)
    {
        if (Status != ExportJobStatus.Pending && Status != ExportJobStatus.Processing)
        {
            throw new DomainException("Only pending or processing export jobs can be completed.");
        }

        FileUrl = NormalizeRequired(fileUrl, nameof(fileUrl), MaxFileUrlLength);
        CompletedAt = completedAt;
        ErrorMessage = null;
        Status = ExportJobStatus.Completed;
    }

    public void Fail(string errorMessage, DateTimeOffset completedAt)
    {
        if (Status == ExportJobStatus.Completed)
        {
            throw new DomainException("Completed export jobs cannot be failed.");
        }

        ErrorMessage = NormalizeRequired(errorMessage, nameof(errorMessage), MaxErrorMessageLength);
        CompletedAt = completedAt;
        Status = ExportJobStatus.Failed;
    }

    public void Cancel()
    {
        if (Status == ExportJobStatus.Completed)
        {
            throw new DomainException("Completed export jobs cannot be cancelled.");
        }

        Status = ExportJobStatus.Cancelled;
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

    private static string? NormalizeOptionalJson(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }
}

public static class ExportJobStatus
{
    public const string Pending = "pending";
    public const string Processing = "processing";
    public const string Completed = "completed";
    public const string Failed = "failed";
    public const string Cancelled = "cancelled";
}

public static class ExportType
{
    public const string DailyReport = "daily_report";
    public const string OperationalDetailed = "operational_detailed";
    public const string Analytical = "analytical";
    public const string Patients = "patients";
    public const string Encounters = "encounters";
}
