using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;
using Caritas.Brigadas.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Caritas.Brigadas.Infrastructure.Persistence;

public sealed class CaritasDbContext : DbContext
{
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    public DbSet<ConsentDocument> ConsentDocuments { get; set; } = null!;

    public CaritasDbContext(DbContextOptions<CaritasDbContext> options)
        : base(options)
    {
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Service> Services => Set<Service>();

    public DbSet<Community> Communities => Set<Community>();
    public DbSet<MobileUnit> MobileUnits => Set<MobileUnit>();
    public DbSet<Brigade> Brigades => Set<Brigade>();
    public DbSet<BrigadeService> BrigadeServices => Set<BrigadeService>();

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<PatientGuardian> PatientGuardians => Set<PatientGuardian>();
    public DbSet<PatientVisit> PatientVisits => Set<PatientVisit>();
    public DbSet<ServiceEncounter> ServiceEncounters => Set<ServiceEncounter>();
    public DbSet<MedicalReferral> MedicalReferrals => Set<MedicalReferral>();
    public DbSet<MedicationDelivery> MedicationDeliveries => Set<MedicationDelivery>();
    public DbSet<VitalSignsRecord> VitalSignsRecords => Set<VitalSignsRecord>();

    public DbSet<FormTemplate> FormTemplates => Set<FormTemplate>();
    public DbSet<FormResponse> FormResponses => Set<FormResponse>();
    public DbSet<DocumentTemplate> DocumentTemplates => Set<DocumentTemplate>();
    public DbSet<DocumentSignature> DocumentSignatures => Set<DocumentSignature>();
    public DbSet<MediaRelease> MediaReleases => Set<MediaRelease>();

    public DbSet<SyncBatch> SyncBatches => Set<SyncBatch>();
    public DbSet<SyncEvent> SyncEvents => Set<SyncEvent>();

    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
    public DbSet<ExportJob> ExportJobs => Set<ExportJob>();
    public DbSet<AiRequestLog> AiRequestLogs => Set<AiRequestLog>();
    public DbSet<CryptoIntegrityRecord> CryptoIntegrityRecords => Set<CryptoIntegrityRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());

        ConfigureCore(modelBuilder);
        ConfigureBrigades(modelBuilder);
        ConfigureClinical(modelBuilder);
        ConfigureFormsAndDocuments(modelBuilder);
        ConfigureSync(modelBuilder);
        ConfigureAuditAndOperations(modelBuilder);
    }

    private static void ConfigureCore(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("organizations", "core");
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.LegalName).HasMaxLength(250);
            entity.Property(x => x.Rfc).HasMaxLength(20);
            entity.Property(x => x.Email).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => x.Name);
        });

        modelBuilder.Entity<User>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("users", "core");
            entity.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(200);
            entity.Property(x => x.Username).HasMaxLength(100);
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.OrganizationId, x.Email });
            entity.HasIndex(x => new { x.OrganizationId, x.Username });

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("roles", "core");
            entity.Property(x => x.Code).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.OrganizationId, x.Code }).IsUnique();

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            ConfigureEntity(entity);
            entity.ToTable("permissions", "core");
            entity.Property(x => x.Code).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Module).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Action).HasMaxLength(100).IsRequired();
            entity.Property(x => x.SensitivityLevel).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => x.Code).IsUnique();
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            ConfigureEntity(entity);
            entity.ToTable("user_roles", "core");
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.OrganizationId, x.UserId, x.RoleId });

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Role>()
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            ConfigureEntity(entity);
            entity.ToTable("role_permissions", "core");
            entity.HasIndex(x => new { x.RoleId, x.PermissionId }).IsUnique();

            entity.HasOne<Role>()
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Permission>()
                .WithMany()
                .HasForeignKey(x => x.PermissionId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Device>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("devices", "core");
            entity.Property(x => x.DeviceName).HasMaxLength(150);
            entity.Property(x => x.DeviceType).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Platform).HasMaxLength(50).IsRequired();
            entity.Property(x => x.OwnerType).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.OrganizationId, x.AssignedToUserId });
        });

        modelBuilder.Entity<Service>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("services", "core");
            entity.Property(x => x.Code).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Category).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.OrganizationId, x.Code }).IsUnique();

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureBrigades(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Community>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("communities", "brigades");
            entity.Property(x => x.State).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Municipality).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Colony).HasMaxLength(150);
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.OrganizationId, x.Municipality, x.Colony });

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<MobileUnit>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("mobile_units", "brigades");
            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.OrganizationId, x.Name });

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Brigade>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("brigades", "brigades");
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.BrigadeType).HasMaxLength(100).IsRequired();
            entity.Property(x => x.ScheduledDate).HasColumnType("date");
            entity.Property(x => x.Municipality).HasMaxLength(150);
            entity.Property(x => x.Colony).HasMaxLength(150);
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.OrganizationId, x.ScheduledDate });

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Community>()
                .WithMany()
                .HasForeignKey(x => x.CommunityId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<MobileUnit>()
                .WithMany()
                .HasForeignKey(x => x.MobileUnitId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<BrigadeService>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("brigade_services", "brigades");
            entity.HasIndex(x => new { x.BrigadeId, x.ServiceId }).IsUnique();

            entity.HasOne<Brigade>()
                .WithMany()
                .HasForeignKey(x => x.BrigadeId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Service>()
                .WithMany()
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureClinical(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("patients", "clinical");
            entity.Property(x => x.PatientFolio).HasMaxLength(50).IsRequired();
            entity.Property(x => x.FirstName).HasMaxLength(150);
            entity.Property(x => x.PaternalLastName).HasMaxLength(150);
            entity.Property(x => x.MaternalLastName).HasMaxLength(150);
            entity.Property(x => x.FullNameNormalized).HasMaxLength(400);
            entity.Property(x => x.BirthDate).HasColumnType("date");
            entity.Property(x => x.Curp).HasMaxLength(30);
            entity.Property(x => x.Phone).HasMaxLength(50);
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.Property(x => x.LocalPatientId).HasMaxLength(100);
            entity.Property(x => x.ClientOperationId).HasMaxLength(100);
            entity.Property(x => x.IdempotencyKey).HasMaxLength(100);
            entity.Property(x => x.SyncStatus).HasMaxLength(50);
            entity.Property(x => x.DataCaptureSource).HasMaxLength(100);
            entity.HasIndex(x => new { x.OrganizationId, x.PatientFolio }).IsUnique();
            entity.HasIndex(x => new { x.OrganizationId, x.FullNameNormalized });
            entity.HasIndex(x => new { x.OrganizationId, x.SourceBrigadeId });
            entity.HasIndex(x => new { x.OrganizationId, x.LocalPatientId });
            entity.HasIndex(x => new { x.OrganizationId, x.ClientOperationId });
            entity.HasIndex(x => new { x.OrganizationId, x.IdempotencyKey });
            entity.HasIndex(x => new { x.OrganizationId, x.SyncStatus });

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<PatientGuardian>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("patient_guardians", "clinical");
            entity.Property(x => x.FullName).HasMaxLength(250);
            entity.Property(x => x.Relationship).HasMaxLength(100);
            entity.Property(x => x.Phone).HasMaxLength(50);
            entity.HasIndex(x => x.PatientId);

            entity.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<PatientVisit>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("patient_visits", "clinical");
            entity.Property(x => x.VisitFolio).HasMaxLength(50).IsRequired();
            entity.Property(x => x.VisitStatus).HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.Property(x => x.SyncStatus).HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.OrganizationId, x.VisitFolio }).IsUnique();
            entity.HasIndex(x => new { x.BrigadeId, x.PatientId });

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Brigade>()
                .WithMany()
                .HasForeignKey(x => x.BrigadeId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<ServiceEncounter>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("service_encounters", "clinical");
            entity.Property(x => x.EncounterFolio).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.Property(x => x.SyncStatus).HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.OrganizationId, x.EncounterFolio }).IsUnique();
            entity.HasIndex(x => new { x.VisitId, x.ServiceId });

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<PatientVisit>()
                .WithMany()
                .HasForeignKey(x => x.VisitId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Brigade>()
                .WithMany()
                .HasForeignKey(x => x.BrigadeId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Service>()
                .WithMany()
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<MedicalReferral>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("medical_referrals", "clinical");
            entity.Property(x => x.ReferralFolio).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.OrganizationId, x.ReferralFolio }).IsUnique();

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<ServiceEncounter>()
                .WithMany()
                .HasForeignKey(x => x.EncounterId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<MedicationDelivery>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("medication_deliveries", "clinical");
            entity.Property(x => x.MedicationName).HasMaxLength(250).IsRequired();
            entity.Property(x => x.ExpirationDate).HasColumnType("date");
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.OrganizationId, x.PatientId });

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<ServiceEncounter>()
                .WithMany()
                .HasForeignKey(x => x.EncounterId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<VitalSignsRecord>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("vital_signs", "clinical");
            entity.Property(x => x.SystolicBloodPressureMmHg);
            entity.Property(x => x.DiastolicBloodPressureMmHg);
            entity.Property(x => x.HeartRateBpm);
            entity.Property(x => x.RespiratoryRatePerMinute);
            entity.Property(x => x.TemperatureCelsius).HasColumnType("decimal(4,1)");
            entity.Property(x => x.OxygenSaturationPercent);
            entity.Property(x => x.WeightKg).HasColumnType("decimal(6,2)");
            entity.Property(x => x.HeightCm).HasColumnType("decimal(5,2)");
            entity.Property(x => x.GlucoseMgDl).HasColumnType("decimal(7,2)");
            entity.Property(x => x.Source).HasMaxLength(100);
            entity.Property(x => x.Notes).HasMaxLength(1000);
            entity.Property(x => x.SyncStatus).HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.OrganizationId, x.PatientId, x.MeasuredAt });
            entity.HasIndex(x => x.VisitId);
            entity.HasIndex(x => x.EncounterId);
            entity.HasIndex(x => new { x.OrganizationId, x.SyncStatus });

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<PatientVisit>()
                .WithMany()
                .HasForeignKey(x => x.VisitId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<ServiceEncounter>()
                .WithMany()
                .HasForeignKey(x => x.EncounterId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.MeasuredByUserId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureFormsAndDocuments(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FormTemplate>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("form_templates", "forms");
            entity.Property(x => x.FormCode).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Version).HasMaxLength(50).IsRequired();
            entity.Property(x => x.SchemaJson).HasColumnType("nvarchar(max)").IsRequired();
            entity.Property(x => x.UiSchemaJson).HasColumnType("nvarchar(max)");
            entity.Property(x => x.ValidationRulesJson).HasColumnType("nvarchar(max)");
            entity.HasIndex(x => new { x.OrganizationId, x.ServiceId, x.FormCode, x.Version }).IsUnique();

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Service>()
                .WithMany()
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<FormResponse>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("form_responses", "forms");
            entity.Property(x => x.ResponseJson).HasColumnType("nvarchar(max)").IsRequired();
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.Property(x => x.SyncStatus).HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.OrganizationId, x.EncounterId });

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<FormTemplate>()
                .WithMany()
                .HasForeignKey(x => x.FormTemplateId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<ServiceEncounter>()
                .WithMany()
                .HasForeignKey(x => x.EncounterId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<DocumentTemplate>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("document_templates", "documents");
            entity.Property(x => x.DocumentType).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Title).HasMaxLength(250).IsRequired();
            entity.Property(x => x.Version).HasMaxLength(50).IsRequired();
            entity.Property(x => x.ContentText).HasColumnType("nvarchar(max)");
            entity.HasIndex(x => new { x.OrganizationId, x.DocumentType, x.Version });

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Service>()
                .WithMany()
                .HasForeignKey(x => x.AppliesToServiceId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<DocumentSignature>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("document_signatures", "documents");
            entity.Property(x => x.SignedByRole).HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.Property(x => x.SyncStatus).HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.OrganizationId, x.DocumentTemplateId });
            entity.HasIndex(x => new { x.PatientId, x.VisitId, x.EncounterId });

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<DocumentTemplate>()
                .WithMany()
                .HasForeignKey(x => x.DocumentTemplateId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<PatientVisit>()
                .WithMany()
                .HasForeignKey(x => x.VisitId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<ServiceEncounter>()
                .WithMany()
                .HasForeignKey(x => x.EncounterId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<MediaRelease>(entity =>
        {
            ConfigureAuditable(entity);
            entity.ToTable("media_releases", "documents");
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.OrganizationId, x.PatientId });

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<PatientVisit>()
                .WithMany()
                .HasForeignKey(x => x.VisitId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureSync(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SyncBatch>(entity =>
        {
            ConfigureEntity(entity);
            entity.ToTable("sync_batches", "sync");
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.Property(x => x.ErrorSummary).HasMaxLength(4000);
            entity.HasIndex(x => new { x.OrganizationId, x.DeviceId, x.StartedAt });

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Brigade>()
                .WithMany()
                .HasForeignKey(x => x.BrigadeId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<SyncEvent>(entity =>
        {
            ConfigureEntity(entity);
            entity.ToTable("sync_events", "sync");
            entity.Property(x => x.LocalEventId).HasMaxLength(150).IsRequired();
            entity.Property(x => x.IdempotencyKey).HasMaxLength(250).IsRequired();
            entity.Property(x => x.EntityType).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Operation).HasMaxLength(50).IsRequired();
            entity.Property(x => x.PayloadJson).HasColumnType("nvarchar(max)").IsRequired();
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.SyncBatchId, x.LocalEventId }).IsUnique();
            entity.HasIndex(x => new { x.OrganizationId, x.IdempotencyKey }).IsUnique();

            entity.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<SyncBatch>()
                .WithMany()
                .HasForeignKey(x => x.SyncBatchId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureAuditAndOperations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditEvent>(entity =>
        {
            ConfigureEntity(entity);
            entity.ToTable("audit_events", "audit");
            entity.Property(x => x.EntityType).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Action).HasMaxLength(100).IsRequired();
            entity.Property(x => x.MetadataJson).HasColumnType("nvarchar(max)");
            entity.HasIndex(x => new { x.OrganizationId, x.CreatedAt });
            entity.HasIndex(x => new { x.EntityType, x.EntityId });
        });

        modelBuilder.Entity<ExportJob>(entity =>
        {
            ConfigureEntity(entity);
            entity.ToTable("export_jobs", "operations");
            entity.Property(x => x.ExportType).HasMaxLength(100).IsRequired();
            entity.Property(x => x.FiltersJson).HasColumnType("nvarchar(max)");
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.OrganizationId, x.RequestedAt });
        });

        modelBuilder.Entity<AiRequestLog>(entity =>
        {
            ConfigureEntity(entity);
            entity.ToTable("ai_request_logs", "operations");
            entity.Property(x => x.Module).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Purpose).HasMaxLength(250).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.OrganizationId, x.RequestedAt });
        });

        modelBuilder.Entity<CryptoIntegrityRecord>(entity =>
        {
            ConfigureEntity(entity);
            entity.ToTable("crypto_integrity_records", "audit");
            entity.Property(x => x.EntityType).HasMaxLength(100).IsRequired();
            entity.Property(x => x.HashAlgorithm).HasMaxLength(100).IsRequired();
            entity.Property(x => x.PayloadHash).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.OrganizationId, x.EntityType, x.EntityId });
        });
    }

    private static void ConfigureEntity<T>(EntityTypeBuilder<T> entity)
        where T : Entity
    {
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Id).ValueGeneratedNever();
    }

    private static void ConfigureAuditable<T>(EntityTypeBuilder<T> entity)
        where T : AuditableEntity
    {
        ConfigureEntity(entity);

        entity.Property(x => x.CreatedAt).IsRequired();
        entity.Property(x => x.IsDeleted).IsRequired();

        entity.HasIndex(x => x.IsDeleted);
    }
}


