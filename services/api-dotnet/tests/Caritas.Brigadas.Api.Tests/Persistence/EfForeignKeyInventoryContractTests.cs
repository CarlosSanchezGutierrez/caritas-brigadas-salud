using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Persistence;

public sealed class EfForeignKeyInventoryContractTests
{
    private static readonly string[] AllowedPackages =
    {
        "P2-05-core-security",
        "P2-06-brigades",
        "P2-07-clinical",
        "P2-08-forms-documents-sync"
    };

    [Fact]
    public void ForeignKeyInventory_HasNoDuplicateRelationships()
    {
        var duplicates = GetCandidateForeignKeys()
            .GroupBy(candidate => new
            {
                candidate.DependentClrType,
                candidate.DependentPropertyName,
                candidate.PrincipalClrType,
                candidate.PrincipalKeyName
            })
            .Where(group => group.Count() > 1)
            .Select(group =>
                $"{group.Key.DependentClrType.Name}.{group.Key.DependentPropertyName} -> {group.Key.PrincipalClrType.Name}.{group.Key.PrincipalKeyName}")
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            duplicates.Length == 0,
            "Foreign key inventory must not contain duplicate relationships." +
            Environment.NewLine +
            string.Join(Environment.NewLine, duplicates));
    }

    [Fact]
    public void ForeignKeyInventory_UsesKnownP2Packages()
    {
        var failures = GetCandidateForeignKeys()
            .Where(candidate => !AllowedPackages.Contains(candidate.Package, StringComparer.Ordinal))
            .Select(candidate => $"{candidate.Describe()} uses unknown package '{candidate.Package}'.")
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            failures.Length == 0,
            "Every candidate foreign key must be assigned to a known P2 package." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void ForeignKeyInventory_ReferencesMappedEntitiesAndProperties()
    {
        var model = CreateModel();
        var failures = new List<string>();

        foreach (var candidate in GetCandidateForeignKeys())
        {
            var dependentEntityType = model.FindEntityType(candidate.DependentClrType);
            var principalEntityType = model.FindEntityType(candidate.PrincipalClrType);

            if (dependentEntityType is null)
            {
                failures.Add($"{candidate.Describe()} dependent entity is not mapped.");
                continue;
            }

            if (principalEntityType is null)
            {
                failures.Add($"{candidate.Describe()} principal entity is not mapped.");
                continue;
            }

            var dependentProperty = dependentEntityType.FindProperty(candidate.DependentPropertyName);

            if (dependentProperty is null)
            {
                failures.Add($"{candidate.Describe()} dependent property is not mapped.");
                continue;
            }
            if (candidate.IsRequired)
            {
                if (dependentProperty.ClrType != typeof(Guid))
                {
                    failures.Add($"{candidate.Describe()} is marked required, so dependent property must be non-nullable Guid.");
                }

                if (dependentProperty.IsNullable)
                {
                    failures.Add($"{candidate.Describe()} is marked required, so EF property must be non-nullable.");
                }
            }
            else
            {
                if (dependentProperty.ClrType != typeof(Guid?))
                {
                    failures.Add($"{candidate.Describe()} is marked optional, so dependent property must be nullable Guid?.");
                }

                if (!dependentProperty.IsNullable)
                {
                    failures.Add($"{candidate.Describe()} is marked optional, so EF property must be nullable.");
                }
            }
var principalProperty = principalEntityType.FindProperty(candidate.PrincipalKeyName);

            if (principalProperty is null)
            {
                failures.Add($"{candidate.Describe()} principal key property is not mapped.");
                continue;
            }

            if (principalProperty.ClrType != typeof(Guid))
            {
                failures.Add($"{candidate.Describe()} principal key must be Guid.");
            }

            if (string.IsNullOrWhiteSpace(candidate.Rationale))
            {
                failures.Add($"{candidate.Describe()} must have a rationale.");
            }
        }

        Assert.True(
            failures.Count == 0,
            "Foreign key inventory references invalid EF model metadata." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void ForeignKeyInventory_DoesNotAssumeCascadeDelete()
    {
        var failures = GetCandidateForeignKeys()
            .Where(candidate => candidate.AllowedDeleteBehavior is not
                CandidateDeleteBehavior.Restrict and not
                CandidateDeleteBehavior.NoAction and not
                CandidateDeleteBehavior.ClientNoAction)
            .Select(candidate => $"{candidate.Describe()} uses unsafe delete behavior {candidate.AllowedDeleteBehavior}.")
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            failures.Length == 0,
            "Candidate foreign keys must default to non-destructive delete behavior." +
            Environment.NewLine +
            string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void CoreSecurityPackage_ContainsExpectedRelationships()
    {
        var expected = new[]
        {
            "Role.OrganizationId -> Organization.Id",
            "User.OrganizationId -> Organization.Id",
            "UserRole.OrganizationId -> Organization.Id",
            "UserRole.UserId -> User.Id",
            "UserRole.RoleId -> Role.Id",
            "RolePermission.RoleId -> Role.Id",
            "RolePermission.PermissionId -> Permission.Id",
            "Service.OrganizationId -> Organization.Id"
        };

        AssertPackageContains("P2-05-core-security", expected);
    }

    [Fact]
    public void BrigadesPackage_ContainsExpectedRelationships()
    {
        var expected = new[]
        {
            "Community.OrganizationId -> Organization.Id",
            "MobileUnit.OrganizationId -> Organization.Id",
            "Brigade.OrganizationId -> Organization.Id",
            "Brigade.CommunityId -> Community.Id",
            "Brigade.MobileUnitId -> MobileUnit.Id",
            "BrigadeService.BrigadeId -> Brigade.Id",
            "BrigadeService.ServiceId -> Service.Id"
        };

        AssertPackageContains("P2-06-brigades", expected);
    }

    [Fact]
    public void ClinicalPackage_ContainsExpectedRelationships()
    {
        var expected = new[]
        {
            "Patient.OrganizationId -> Organization.Id",
            "PatientGuardian.PatientId -> Patient.Id",
            "PatientVisit.OrganizationId -> Organization.Id",
            "PatientVisit.PatientId -> Patient.Id",
            "PatientVisit.BrigadeId -> Brigade.Id",
            "ServiceEncounter.OrganizationId -> Organization.Id",
            "ServiceEncounter.PatientId -> Patient.Id",
            "ServiceEncounter.VisitId -> PatientVisit.Id",
            "ServiceEncounter.BrigadeId -> Brigade.Id",
            "ServiceEncounter.ServiceId -> Service.Id",
            "MedicalReferral.OrganizationId -> Organization.Id",
            "MedicalReferral.PatientId -> Patient.Id",
            "MedicalReferral.EncounterId -> ServiceEncounter.Id",
            "MedicationDelivery.OrganizationId -> Organization.Id",
            "MedicationDelivery.PatientId -> Patient.Id",
            "MedicationDelivery.EncounterId -> ServiceEncounter.Id"
        };

        AssertPackageContains("P2-07-clinical", expected);
    }

    [Fact]
    public void FormsDocumentsSyncPackage_ContainsExpectedRelationships()
    {
        var expected = new[]
        {
            "FormTemplate.OrganizationId -> Organization.Id",
            "FormTemplate.ServiceId -> Service.Id",
            "FormResponse.OrganizationId -> Organization.Id",
            "FormResponse.FormTemplateId -> FormTemplate.Id",
            "FormResponse.EncounterId -> ServiceEncounter.Id",
            "DocumentTemplate.OrganizationId -> Organization.Id",
            "DocumentTemplate.AppliesToServiceId -> Service.Id",
            "DocumentSignature.OrganizationId -> Organization.Id",
            "DocumentSignature.DocumentTemplateId -> DocumentTemplate.Id",
            "DocumentSignature.PatientId -> Patient.Id",
            "DocumentSignature.VisitId -> PatientVisit.Id",
            "DocumentSignature.EncounterId -> ServiceEncounter.Id",
            "MediaRelease.OrganizationId -> Organization.Id",
            "MediaRelease.PatientId -> Patient.Id",
            "MediaRelease.VisitId -> PatientVisit.Id",
            "SyncBatch.OrganizationId -> Organization.Id",
            "SyncBatch.BrigadeId -> Brigade.Id",            "SyncEvent.OrganizationId -> Organization.Id",
            "SyncEvent.SyncBatchId -> SyncBatch.Id"
        };

        AssertPackageContains("P2-08-forms-documents-sync", expected);
    }

    [Fact]
    public void DeferredForeignKeyInventory_ContainsDeviceRelationshipsRequiringPolicyDecision()
    {
        var deferred = GetDeferredForeignKeys()
            .Select(candidate => candidate.Describe())
            .Order(StringComparer.Ordinal)
            .ToArray();

        var expected = new[]
        {
            "SyncBatch.DeviceId -> Device.Id"
        };

        Assert.Equal(expected, deferred);
    }

    private static IReadOnlyCollection<CandidateForeignKey> GetDeferredForeignKeys()
    {
        return new[]
        {
            CandidateForeignKey.Optional<SyncBatch, Device>(
                nameof(SyncBatch.DeviceId),
                "policy-decision-required",
                "Sync batches may carry offline, revoked, or not-yet-synced device identifiers and require an explicit policy decision before becoming real FKs.")
        };
    }
    private static void AssertPackageContains(
        string package,
        IReadOnlyCollection<string> expectedRelationships)
    {
        var actual = GetCandidateForeignKeys()
            .Where(candidate => candidate.Package == package)
            .Select(candidate => candidate.Describe())
            .Order(StringComparer.Ordinal)
            .ToArray();

        var missing = expectedRelationships
            .Except(actual, StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            missing.Length == 0,
            $"{package} is missing expected relationships." +
            Environment.NewLine +
            string.Join(Environment.NewLine, missing));
    }

    private static IReadOnlyCollection<CandidateForeignKey> GetCandidateForeignKeys()
    {
        return new[]
        {
            CandidateForeignKey.Required<Role, Organization>(
                nameof(Role.OrganizationId),
                "P2-05-core-security",
                "Roles are tenant-owned security records."),
            CandidateForeignKey.Required<User, Organization>(
                nameof(User.OrganizationId),
                "P2-05-core-security",
                "Users are tenant-owned identity records."),
            CandidateForeignKey.Required<UserRole, Organization>(
                nameof(UserRole.OrganizationId),
                "P2-05-core-security",
                "User-role assignments occur inside a tenant boundary."),
            CandidateForeignKey.Required<UserRole, User>(
                nameof(UserRole.UserId),
                "P2-05-core-security",
                "User-role assignments must point to an existing user."),
            CandidateForeignKey.Required<UserRole, Role>(
                nameof(UserRole.RoleId),
                "P2-05-core-security",
                "User-role assignments must point to an existing role."),
            CandidateForeignKey.Required<RolePermission, Role>(
                nameof(RolePermission.RoleId),
                "P2-05-core-security",
                "Role-permission grants must point to an existing role."),
            CandidateForeignKey.Required<RolePermission, Permission>(
                nameof(RolePermission.PermissionId),
                "P2-05-core-security",
                "Role-permission grants must point to an existing permission."),
            CandidateForeignKey.Required<Service, Organization>(
                nameof(Service.OrganizationId),
                "P2-05-core-security",
                "Services are tenant-owned catalog records."),

            CandidateForeignKey.Required<Community, Organization>(
                nameof(Community.OrganizationId),
                "P2-06-brigades",
                "Communities are tenant-owned operational records."),
            CandidateForeignKey.Required<MobileUnit, Organization>(
                nameof(MobileUnit.OrganizationId),
                "P2-06-brigades",
                "Mobile units are tenant-owned operational records."),
            CandidateForeignKey.Required<Brigade, Organization>(
                nameof(Brigade.OrganizationId),
                "P2-06-brigades",
                "Brigades are tenant-owned operational records."),
            CandidateForeignKey.Optional<Brigade, Community>(
                nameof(Brigade.CommunityId),
                "P2-06-brigades",
                "A brigade can optionally be tied to a known community."),
            CandidateForeignKey.Optional<Brigade, MobileUnit>(
                nameof(Brigade.MobileUnitId),
                "P2-06-brigades",
                "A brigade can optionally use a registered mobile unit."),
            CandidateForeignKey.Required<BrigadeService, Brigade>(
                nameof(BrigadeService.BrigadeId),
                "P2-06-brigades",
                "Brigade services belong to a brigade."),
            CandidateForeignKey.Required<BrigadeService, Service>(
                nameof(BrigadeService.ServiceId),
                "P2-06-brigades",
                "Brigade services reference enabled service catalog entries."),

            CandidateForeignKey.Required<Patient, Organization>(
                nameof(Patient.OrganizationId),
                "P2-07-clinical",
                "Patients are tenant-owned clinical records."),
            CandidateForeignKey.Required<PatientGuardian, Patient>(
                nameof(PatientGuardian.PatientId),
                "P2-07-clinical",
                "Guardians belong to a patient."),
            CandidateForeignKey.Required<PatientVisit, Organization>(
                nameof(PatientVisit.OrganizationId),
                "P2-07-clinical",
                "Visits are tenant-owned clinical records."),
            CandidateForeignKey.Required<PatientVisit, Patient>(
                nameof(PatientVisit.PatientId),
                "P2-07-clinical",
                "Visits must point to an existing patient."),
            CandidateForeignKey.Required<PatientVisit, Brigade>(
                nameof(PatientVisit.BrigadeId),
                "P2-07-clinical",
                "Visits must occur inside a known brigade."),
            CandidateForeignKey.Required<ServiceEncounter, Organization>(
                nameof(ServiceEncounter.OrganizationId),
                "P2-07-clinical",
                "Encounters are tenant-owned clinical records."),
            CandidateForeignKey.Required<ServiceEncounter, Patient>(
                nameof(ServiceEncounter.PatientId),
                "P2-07-clinical",
                "Encounters must point to the attended patient."),
            CandidateForeignKey.Required<ServiceEncounter, PatientVisit>(
                nameof(ServiceEncounter.VisitId),
                "P2-07-clinical",
                "Encounters belong to a patient visit."),
            CandidateForeignKey.Required<ServiceEncounter, Brigade>(
                nameof(ServiceEncounter.BrigadeId),
                "P2-07-clinical",
                "Encounters happen inside a brigade."),
            CandidateForeignKey.Required<ServiceEncounter, Service>(
                nameof(ServiceEncounter.ServiceId),
                "P2-07-clinical",
                "Encounters reference a service catalog entry."),
            CandidateForeignKey.Required<MedicalReferral, Organization>(
                nameof(MedicalReferral.OrganizationId),
                "P2-07-clinical",
                "Referrals are tenant-owned clinical records."),
            CandidateForeignKey.Required<MedicalReferral, Patient>(
                nameof(MedicalReferral.PatientId),
                "P2-07-clinical",
                "Referrals belong to a patient."),
            CandidateForeignKey.Required<MedicalReferral, ServiceEncounter>(
                nameof(MedicalReferral.EncounterId),
                "P2-07-clinical",
                "Referrals originate from an encounter."),
            CandidateForeignKey.Required<MedicationDelivery, Organization>(
                nameof(MedicationDelivery.OrganizationId),
                "P2-07-clinical",
                "Medication deliveries are tenant-owned clinical records."),
            CandidateForeignKey.Required<MedicationDelivery, Patient>(
                nameof(MedicationDelivery.PatientId),
                "P2-07-clinical",
                "Medication deliveries belong to a patient."),
            CandidateForeignKey.Required<MedicationDelivery, ServiceEncounter>(
                nameof(MedicationDelivery.EncounterId),
                "P2-07-clinical",
                "Medication deliveries originate from an encounter."),

            CandidateForeignKey.Required<FormTemplate, Organization>(
                nameof(FormTemplate.OrganizationId),
                "P2-08-forms-documents-sync",
                "Form templates are tenant-owned form records."),
            CandidateForeignKey.Required<FormTemplate, Service>(
                nameof(FormTemplate.ServiceId),
                "P2-08-forms-documents-sync",
                "Form templates are attached to a service."),
            CandidateForeignKey.Required<FormResponse, Organization>(
                nameof(FormResponse.OrganizationId),
                "P2-08-forms-documents-sync",
                "Form responses are tenant-owned form records."),
            CandidateForeignKey.Required<FormResponse, FormTemplate>(
                nameof(FormResponse.FormTemplateId),
                "P2-08-forms-documents-sync",
                "Form responses use a known form template."),
            CandidateForeignKey.Required<FormResponse, ServiceEncounter>(
                nameof(FormResponse.EncounterId),
                "P2-08-forms-documents-sync",
                "Form responses belong to a service encounter."),
            CandidateForeignKey.Required<DocumentTemplate, Organization>(
                nameof(DocumentTemplate.OrganizationId),
                "P2-08-forms-documents-sync",
                "Document templates are tenant-owned document records."),
            CandidateForeignKey.Optional<DocumentTemplate, Service>(
                nameof(DocumentTemplate.AppliesToServiceId),
                "P2-08-forms-documents-sync",
                "Document templates can optionally apply to a service."),
            CandidateForeignKey.Required<DocumentSignature, Organization>(
                nameof(DocumentSignature.OrganizationId),
                "P2-08-forms-documents-sync",
                "Document signatures are tenant-owned document records."),
            CandidateForeignKey.Required<DocumentSignature, DocumentTemplate>(
                nameof(DocumentSignature.DocumentTemplateId),
                "P2-08-forms-documents-sync",
                "Document signatures must reference a template."),
            CandidateForeignKey.Optional<DocumentSignature, Patient>(
                nameof(DocumentSignature.PatientId),
                "P2-08-forms-documents-sync",
                "Document signatures can optionally reference a patient."),
            CandidateForeignKey.Optional<DocumentSignature, PatientVisit>(
                nameof(DocumentSignature.VisitId),
                "P2-08-forms-documents-sync",
                "Document signatures can optionally reference a visit."),
            CandidateForeignKey.Optional<DocumentSignature, ServiceEncounter>(
                nameof(DocumentSignature.EncounterId),
                "P2-08-forms-documents-sync",
                "Document signatures can optionally reference an encounter."),
            CandidateForeignKey.Required<MediaRelease, Organization>(
                nameof(MediaRelease.OrganizationId),
                "P2-08-forms-documents-sync",
                "Media releases are tenant-owned document records."),
            CandidateForeignKey.Required<MediaRelease, Patient>(
                nameof(MediaRelease.PatientId),
                "P2-08-forms-documents-sync",
                "Media releases belong to a patient."),
            CandidateForeignKey.Optional<MediaRelease, PatientVisit>(
                nameof(MediaRelease.VisitId),
                "P2-08-forms-documents-sync",
                "Media releases can optionally reference a visit."),
            CandidateForeignKey.Required<SyncBatch, Organization>(
                nameof(SyncBatch.OrganizationId),
                "P2-08-forms-documents-sync",
                "Sync batches are tenant-owned sync records."),
            CandidateForeignKey.Optional<SyncBatch, Brigade>(
                nameof(SyncBatch.BrigadeId),
                "P2-08-forms-documents-sync",
                "Sync batches can optionally be tied to a brigade."),
            CandidateForeignKey.Required<SyncEvent, Organization>(
                nameof(SyncEvent.OrganizationId),
                "P2-08-forms-documents-sync",
                "Sync events are tenant-owned sync records."),
            CandidateForeignKey.Required<SyncEvent, SyncBatch>(
                nameof(SyncEvent.SyncBatchId),
                "P2-08-forms-documents-sync",
                "Sync events belong to a sync batch.")
        };
    }

    private static IModel CreateModel()
    {
        var options = new DbContextOptionsBuilder<CaritasDbContext>()
            .UseSqlServer("Server=localhost;Database=Caritas_ModelOnly;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        using var dbContext = new CaritasDbContext(options);

        return dbContext.Model;
    }

    private sealed record CandidateForeignKey(
        Type DependentClrType,
        string DependentPropertyName,
        Type PrincipalClrType,
        string PrincipalKeyName,
        bool IsRequired,
        string Package,
        CandidateDeleteBehavior AllowedDeleteBehavior,
        string Rationale)
    {
        public static CandidateForeignKey Required<TDependent, TPrincipal>(
            string dependentPropertyName,
            string package,
            string rationale)
        {
            return new CandidateForeignKey(
                typeof(TDependent),
                dependentPropertyName,
                typeof(TPrincipal),
                "Id",
                true,
                package,
                CandidateDeleteBehavior.NoAction,
                rationale);
        }

        public static CandidateForeignKey Optional<TDependent, TPrincipal>(
            string dependentPropertyName,
            string package,
            string rationale)
        {
            return new CandidateForeignKey(
                typeof(TDependent),
                dependentPropertyName,
                typeof(TPrincipal),
                "Id",
                false,
                package,
                CandidateDeleteBehavior.NoAction,
                rationale);
        }

        public string Describe()
        {
            return $"{DependentClrType.Name}.{DependentPropertyName} -> {PrincipalClrType.Name}.{PrincipalKeyName}";
        }
    }

    private enum CandidateDeleteBehavior
    {
        Restrict,
        NoAction,
        ClientNoAction
    }
}