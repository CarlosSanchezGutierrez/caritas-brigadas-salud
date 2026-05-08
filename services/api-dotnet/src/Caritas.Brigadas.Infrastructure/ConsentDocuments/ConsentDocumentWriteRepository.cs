using System.Reflection;
using Caritas.Brigadas.Application.ConsentDocuments;
using Caritas.Brigadas.Contracts.ConsentDocuments;
using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.ConsentDocuments;

public sealed class ConsentDocumentWriteRepository : IConsentDocumentWriteRepository
{
    private readonly CaritasDbContext _dbContext;

    public ConsentDocumentWriteRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ConsentDocumentSummaryDto> CreateAsync(
        Guid organizationId,
        CreateConsentDocumentRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.PatientId == Guid.Empty)
        {
            throw new DomainException("Patient id is required.");
        }

        if (string.IsNullOrWhiteSpace(request.ConsentType))
        {
            throw new DomainException("Consent type is required.");
        }

        if (string.IsNullOrWhiteSpace(request.DocumentVersion))
        {
            throw new DomainException("Document version is required.");
        }

        var organizationExists = await _dbContext.Organizations
            .AsNoTracking()
            .AnyAsync(
                organization =>
                    organization.Id == organizationId &&
                    !organization.IsDeleted,
                cancellationToken);

        if (!organizationExists)
        {
            throw new KeyNotFoundException("Organization was not found.");
        }

        var patientExists = await _dbContext.Patients
            .AsNoTracking()
            .AnyAsync(
                patient =>
                    patient.Id == request.PatientId &&
                    patient.OrganizationId == organizationId &&
                    !patient.IsDeleted,
                cancellationToken);

        if (!patientExists)
        {
            throw new KeyNotFoundException("Patient was not found in this organization.");
        }

        if (request.VisitId.HasValue)
        {
            var visitExists = await _dbContext.PatientVisits
                .AsNoTracking()
                .AnyAsync(
                    visit =>
                        visit.Id == request.VisitId.Value &&
                        visit.OrganizationId == organizationId &&
                        visit.PatientId == request.PatientId &&
                        !visit.IsDeleted,
                    cancellationToken);

            if (!visitExists)
            {
                throw new KeyNotFoundException("Patient visit was not found in this organization.");
            }
        }

        if (request.SignedByUserId.HasValue)
        {
            var userExists = await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.Id == request.SignedByUserId.Value &&
                        user.OrganizationId == organizationId &&
                        !user.IsDeleted,
                    cancellationToken);

            if (!userExists)
            {
                throw new KeyNotFoundException("Signed by user was not found in this organization.");
            }
        }

        var normalizedConsentType = request.ConsentType.Trim().ToUpperInvariant();
        var normalizedVersion = request.DocumentVersion.Trim();

        var existingDocuments = await _dbContext.Set<ConsentDocument>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var duplicateExists = existingDocuments.Any(document =>
            GetGuidProperty(document, "OrganizationId") == organizationId &&
            GetGuidProperty(document, "PatientId") == request.PatientId &&
            (
                GetGuidNullableProperty(document, "VisitId") == request.VisitId ||
                GetGuidNullableProperty(document, "PatientVisitId") == request.VisitId
            ) &&
            string.Equals(
                GetStringProperty(document, "ConsentType") ?? GetStringProperty(document, "DocumentType"),
                normalizedConsentType,
                StringComparison.OrdinalIgnoreCase) &&
            string.Equals(
                GetStringProperty(document, "DocumentVersion") ?? GetStringProperty(document, "Version"),
                normalizedVersion,
                StringComparison.OrdinalIgnoreCase) &&
            !GetBoolProperty(document, "IsDeleted"));

        if (duplicateExists)
        {
            throw new InvalidOperationException("This patient already has this consent document version registered.");
        }

        var now = DateTimeOffset.UtcNow;

        var consentDocument = CreateConsentDocument(
            organizationId,
            request,
            normalizedConsentType,
            normalizedVersion,
            now);

        _dbContext.Set<ConsentDocument>().Add(consentDocument);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(consentDocument);
    }

    private static ConsentDocument CreateConsentDocument(
        Guid organizationId,
        CreateConsentDocumentRequest request,
        string normalizedConsentType,
        string normalizedVersion,
        DateTimeOffset now)
    {
        var document = (ConsentDocument)Activator.CreateInstance(
            typeof(ConsentDocument),
            nonPublic: true)!;

        SetPropertyIfExists(document, "Id", Guid.NewGuid());
        SetPropertyIfExists(document, "OrganizationId", organizationId);
        SetPropertyIfExists(document, "PatientId", request.PatientId);
        SetPropertyIfExists(document, "VisitId", request.VisitId);
        SetPropertyIfExists(document, "PatientVisitId", request.VisitId);
        SetPropertyIfExists(document, "ConsentType", normalizedConsentType);
        SetPropertyIfExists(document, "DocumentType", normalizedConsentType);
        SetPropertyIfExists(document, "DocumentVersion", normalizedVersion);
        SetPropertyIfExists(document, "Version", normalizedVersion);
        SetPropertyIfExists(document, "DocumentTextSnapshot", request.DocumentTextSnapshot);
        SetPropertyIfExists(document, "TextSnapshot", request.DocumentTextSnapshot);
        SetPropertyIfExists(document, "ContentSnapshot", request.DocumentTextSnapshot);
        SetPropertyIfExists(document, "SignatureDataUrl", request.SignatureDataUrl);
        SetPropertyIfExists(document, "SignatureBase64", request.SignatureDataUrl);
        SetPropertyIfExists(document, "SignatureImageDataUrl", request.SignatureDataUrl);
        SetPropertyIfExists(document, "GuardianFullName", request.GuardianFullName);
        SetPropertyIfExists(document, "GuardianRelationship", request.GuardianRelationship);
        SetPropertyIfExists(document, "SignedByUserId", request.SignedByUserId);
        SetPropertyIfExists(document, "CapturedByUserId", request.SignedByUserId);
        SetPropertyIfExists(document, "CreatedByUserId", request.SignedByUserId);
        SetPropertyIfExists(document, "SignedAt", request.SignedAt ?? now);
        SetPropertyIfExists(document, "CapturedAt", request.SignedAt ?? now);
        SetPropertyIfExists(document, "CreatedAt", now);
        SetPropertyIfExists(document, "CreatedOffline", request.CreatedOffline);
        SetPropertyIfExists(document, "DeviceId", request.DeviceId);
        SetPropertyIfExists(document, "SyncStatus", "Synced");
        SetPropertyIfExists(document, "IsDeleted", false);

        return document;
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

    private static void SetPropertyIfExists(
        object instance,
        string propertyName,
        object? value)
    {
        var property = instance
            .GetType()
            .GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (property is null || !property.CanWrite)
        {
            return;
        }

        if (value is null)
        {
            property.SetValue(instance, null);
            return;
        }

        var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        if (targetType.IsEnum && value is string stringValue)
        {
            var parsed = Enum.Parse(targetType, stringValue, ignoreCase: true);
            property.SetValue(instance, parsed);
            return;
        }

        property.SetValue(instance, value);
    }
}
