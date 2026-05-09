using System.Reflection;
using Caritas.Brigadas.Application.ConsentDocuments;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.ConsentDocuments;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.ConsentDocuments;

public sealed class ConsentDocumentReadRepository : IConsentDocumentReadRepository
{
    private readonly CaritasDbContext _dbContext;

    public ConsentDocumentReadRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedResponse<ConsentDocumentSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("Organization id is required.", nameof(organizationId));
        }

        ArgumentNullException.ThrowIfNull(pagination);

        var pageNumber = pagination.NormalizedPageNumber;
        var pageSize = pagination.NormalizedPageSize;

        var query = _dbContext.Set<ConsentDocument>()
            .AsNoTracking()
            .Where(document =>
                EF.Property<Guid>(document, "OrganizationId") == organizationId &&
                !EF.Property<bool>(document, "IsDeleted"));

        var totalCount = await query.CountAsync(cancellationToken);

        var documents = await query
            .OrderByDescending(document => EF.Property<Guid>(document, "Id"))
            .Skip(pagination.Skip)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        var items = documents
            .Select(MapToDto)
            .ToArray();

        return new PaginatedResponse<ConsentDocumentSummaryDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ConsentDocumentSummaryDto?> GetByIdAsync(
        Guid consentDocumentId,
        CancellationToken cancellationToken = default)
    {
        if (consentDocumentId == Guid.Empty)
        {
            throw new ArgumentException("Consent document id is required.", nameof(consentDocumentId));
        }

        var document = await _dbContext.Set<ConsentDocument>()
            .AsNoTracking()
            .Where(item =>
                EF.Property<Guid>(item, "Id") == consentDocumentId &&
                !EF.Property<bool>(item, "IsDeleted"))
            .SingleOrDefaultAsync(cancellationToken);

        return document is null
            ? null
            : MapToDto(document);
    }

    private static ConsentDocumentSummaryDto MapToDto(ConsentDocument document)
    {
        return new ConsentDocumentSummaryDto
        {
            Id = GetGuidProperty(document, "Id"),
            OrganizationId = GetGuidProperty(document, "OrganizationId"),
            PatientId = GetGuidProperty(document, "PatientId"),
            VisitId =
                GetGuidNullableProperty(document, "VisitId") ??
                GetGuidNullableProperty(document, "PatientVisitId"),
            ConsentType =
                GetStringProperty(document, "ConsentType") ??
                GetStringProperty(document, "DocumentType") ??
                string.Empty,
            DocumentVersion =
                GetStringProperty(document, "DocumentVersion") ??
                GetStringProperty(document, "Version") ??
                string.Empty,
            DocumentTextSnapshot =
                GetNullableStringProperty(document, "DocumentTextSnapshot") ??
                GetNullableStringProperty(document, "TextSnapshot") ??
                GetNullableStringProperty(document, "ContentSnapshot"),
            SignatureDataUrl =
                GetNullableStringProperty(document, "SignatureDataUrl") ??
                GetNullableStringProperty(document, "SignatureBase64") ??
                GetNullableStringProperty(document, "SignatureImageDataUrl"),
            GuardianFullName = GetNullableStringProperty(document, "GuardianFullName"),
            GuardianRelationship = GetNullableStringProperty(document, "GuardianRelationship"),
            SignedByUserId =
                GetGuidNullableProperty(document, "SignedByUserId") ??
                GetGuidNullableProperty(document, "CapturedByUserId") ??
                GetGuidNullableProperty(document, "CreatedByUserId"),
            SignedAt =
                GetDateTimeOffsetNullableProperty(document, "SignedAt") ??
                GetDateTimeOffsetNullableProperty(document, "CapturedAt") ??
                GetDateTimeOffsetNullableProperty(document, "CreatedAt"),
            CreatedOffline = GetBoolProperty(document, "CreatedOffline"),
            DeviceId = GetGuidNullableProperty(document, "DeviceId"),
            SyncStatus =
                GetStringProperty(document, "SyncStatus") ??
                string.Empty,
            IsDeleted = GetBoolProperty(document, "IsDeleted")
        };
    }

    private static Guid GetGuidProperty(object instance, string propertyName)
    {
        var value = GetPropertyValue(instance, propertyName);
        return value is Guid guid ? guid : Guid.Empty;
    }

    private static Guid? GetGuidNullableProperty(object instance, string propertyName)
    {
        var value = GetPropertyValue(instance, propertyName);
        return value is Guid guid ? guid : null;
    }

    private static string? GetStringProperty(object instance, string propertyName)
    {
        return GetPropertyValue(instance, propertyName)?.ToString();
    }

    private static string? GetNullableStringProperty(object instance, string propertyName)
    {
        var value = GetPropertyValue(instance, propertyName)?.ToString();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static bool GetBoolProperty(object instance, string propertyName, bool defaultValue = false)
    {
        var value = GetPropertyValue(instance, propertyName);
        return value is bool boolean ? boolean : defaultValue;
    }

    private static DateTimeOffset? GetDateTimeOffsetNullableProperty(object instance, string propertyName)
    {
        var value = GetPropertyValue(instance, propertyName);
        return value is DateTimeOffset dateTimeOffset ? dateTimeOffset : null;
    }

    private static object? GetPropertyValue(object instance, string propertyName)
    {
        return instance
            .GetType()
            .GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?.GetValue(instance);
    }
}