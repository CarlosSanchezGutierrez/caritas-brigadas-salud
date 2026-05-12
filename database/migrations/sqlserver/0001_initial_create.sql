IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    IF SCHEMA_ID(N'operations') IS NULL EXEC(N'CREATE SCHEMA [operations];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    IF SCHEMA_ID(N'audit') IS NULL EXEC(N'CREATE SCHEMA [audit];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    IF SCHEMA_ID(N'brigades') IS NULL EXEC(N'CREATE SCHEMA [brigades];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    IF SCHEMA_ID(N'core') IS NULL EXEC(N'CREATE SCHEMA [core];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    IF SCHEMA_ID(N'documents') IS NULL EXEC(N'CREATE SCHEMA [documents];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    IF SCHEMA_ID(N'forms') IS NULL EXEC(N'CREATE SCHEMA [forms];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    IF SCHEMA_ID(N'clinical') IS NULL EXEC(N'CREATE SCHEMA [clinical];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    IF SCHEMA_ID(N'sync') IS NULL EXEC(N'CREATE SCHEMA [sync];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [operations].[ai_request_logs] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [RequestedByUserId] uniqueidentifier NOT NULL,
        [Module] nvarchar(100) NOT NULL,
        [Purpose] nvarchar(250) NOT NULL,
        [Provider] nvarchar(max) NULL,
        [Model] nvarchar(max) NULL,
        [PromptHash] nvarchar(max) NULL,
        [InputHash] nvarchar(max) NULL,
        [OutputHash] nvarchar(max) NULL,
        [ContainsSensitiveData] bit NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        [RequestedAt] datetimeoffset NOT NULL,
        [CompletedAt] datetimeoffset NULL,
        [ErrorMessage] nvarchar(max) NULL,
        CONSTRAINT [PK_ai_request_logs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [audit].[audit_events] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [ActorUserId] uniqueidentifier NULL,
        [DeviceId] uniqueidentifier NULL,
        [EntityType] nvarchar(100) NOT NULL,
        [EntityId] uniqueidentifier NULL,
        [Action] nvarchar(100) NOT NULL,
        [OldValueHash] nvarchar(max) NULL,
        [NewValueHash] nvarchar(max) NULL,
        [MetadataJson] nvarchar(max) NULL,
        [IpAddress] nvarchar(max) NULL,
        [UserAgent] nvarchar(max) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [EventHash] nvarchar(max) NULL,
        [PreviousEventHash] nvarchar(max) NULL,
        CONSTRAINT [PK_audit_events] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [brigades].[brigade_services] (
        [Id] uniqueidentifier NOT NULL,
        [BrigadeId] uniqueidentifier NOT NULL,
        [ServiceId] uniqueidentifier NOT NULL,
        [IsAvailable] bit NOT NULL,
        [CapacityEstimate] int NULL,
        [AssignedLeadUserId] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_brigade_services] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [brigades].[brigades] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [BrigadeType] nvarchar(100) NOT NULL,
        [ScheduledDate] date NOT NULL,
        [StartTime] datetimeoffset NULL,
        [EndTime] datetimeoffset NULL,
        [CommunityId] uniqueidentifier NULL,
        [Municipality] nvarchar(150) NULL,
        [Colony] nvarchar(150) NULL,
        [LocationText] nvarchar(max) NULL,
        [MobileUnitId] uniqueidentifier NULL,
        [CoordinatorUserId] uniqueidentifier NULL,
        [Status] nvarchar(50) NOT NULL,
        [OpenedAt] datetimeoffset NULL,
        [OpenedByUserId] uniqueidentifier NULL,
        [ClosedAt] datetimeoffset NULL,
        [ClosedByUserId] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_brigades] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [brigades].[communities] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [State] nvarchar(100) NOT NULL,
        [Municipality] nvarchar(150) NOT NULL,
        [Colony] nvarchar(150) NULL,
        [CommunityName] nvarchar(max) NULL,
        [AddressReference] nvarchar(max) NULL,
        [RiskLevel] nvarchar(max) NULL,
        [Status] nvarchar(50) NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_communities] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [audit].[crypto_integrity_records] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [EntityType] nvarchar(100) NOT NULL,
        [EntityId] uniqueidentifier NOT NULL,
        [HashAlgorithm] nvarchar(100) NOT NULL,
        [PayloadHash] nvarchar(256) NOT NULL,
        [PreviousHash] nvarchar(max) NULL,
        [ChainKey] nvarchar(max) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [Status] nvarchar(50) NOT NULL,
        [VerifiedAt] datetimeoffset NULL,
        [VerifiedByUserId] uniqueidentifier NULL,
        [VerificationError] nvarchar(max) NULL,
        CONSTRAINT [PK_crypto_integrity_records] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [core].[devices] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [DeviceName] nvarchar(150) NULL,
        [DeviceType] nvarchar(50) NOT NULL,
        [Platform] nvarchar(50) NOT NULL,
        [OsVersion] nvarchar(max) NULL,
        [AppVersion] nvarchar(max) NULL,
        [OwnerType] nvarchar(50) NOT NULL,
        [AssignedToUserId] uniqueidentifier NULL,
        [IsApproved] bit NOT NULL,
        [IsRevoked] bit NOT NULL,
        [LastSyncAt] datetimeoffset NULL,
        [RegisteredAt] datetimeoffset NOT NULL,
        [ApprovedAt] datetimeoffset NULL,
        [ApprovedByUserId] uniqueidentifier NULL,
        [RevokedAt] datetimeoffset NULL,
        [RevokedByUserId] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_devices] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [documents].[document_signatures] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [DocumentTemplateId] uniqueidentifier NOT NULL,
        [PatientId] uniqueidentifier NULL,
        [VisitId] uniqueidentifier NULL,
        [EncounterId] uniqueidentifier NULL,
        [GuardianId] uniqueidentifier NULL,
        [SignedByName] nvarchar(max) NULL,
        [SignedByRole] nvarchar(50) NOT NULL,
        [SignatureFileUrl] nvarchar(max) NULL,
        [SignatureHash] nvarchar(max) NULL,
        [SignedAt] datetimeoffset NOT NULL,
        [SignedByUserId] uniqueidentifier NULL,
        [CreatedOffline] bit NOT NULL,
        [DeviceId] uniqueidentifier NULL,
        [SyncStatus] nvarchar(50) NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_document_signatures] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [documents].[document_templates] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [DocumentType] nvarchar(100) NOT NULL,
        [Title] nvarchar(250) NOT NULL,
        [Version] nvarchar(50) NOT NULL,
        [ContentText] nvarchar(max) NULL,
        [FileUrl] nvarchar(max) NULL,
        [AppliesToServiceId] uniqueidentifier NULL,
        [RequiresPatientSignature] bit NOT NULL,
        [RequiresGuardianSignature] bit NOT NULL,
        [RequiresProviderSignature] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [EffectiveFrom] datetimeoffset NULL,
        [EffectiveTo] datetimeoffset NULL,
        [DocumentHash] nvarchar(max) NULL,
        [ApprovedByUserId] uniqueidentifier NULL,
        [ApprovedAt] datetimeoffset NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_document_templates] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [operations].[export_jobs] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [RequestedByUserId] uniqueidentifier NOT NULL,
        [ExportType] nvarchar(100) NOT NULL,
        [FiltersJson] nvarchar(max) NULL,
        [IncludesIdentifiableData] bit NOT NULL,
        [FileUrl] nvarchar(max) NULL,
        [Status] nvarchar(50) NOT NULL,
        [RequestedAt] datetimeoffset NOT NULL,
        [CompletedAt] datetimeoffset NULL,
        [ErrorMessage] nvarchar(max) NULL,
        CONSTRAINT [PK_export_jobs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [forms].[form_responses] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [EncounterId] uniqueidentifier NOT NULL,
        [FormTemplateId] uniqueidentifier NOT NULL,
        [ResponseJson] nvarchar(max) NOT NULL,
        [ResponseHash] nvarchar(max) NULL,
        [CompletedByUserId] uniqueidentifier NULL,
        [CompletedAt] datetimeoffset NULL,
        [Status] nvarchar(50) NOT NULL,
        [CreatedOffline] bit NOT NULL,
        [DeviceId] uniqueidentifier NULL,
        [SyncStatus] nvarchar(50) NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_form_responses] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [forms].[form_templates] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [ServiceId] uniqueidentifier NOT NULL,
        [FormCode] nvarchar(100) NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Version] nvarchar(50) NOT NULL,
        [SchemaJson] nvarchar(max) NOT NULL,
        [UiSchemaJson] nvarchar(max) NULL,
        [ValidationRulesJson] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [EffectiveFrom] datetimeoffset NULL,
        [EffectiveTo] datetimeoffset NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_form_templates] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [documents].[media_releases] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [PatientId] uniqueidentifier NOT NULL,
        [VisitId] uniqueidentifier NULL,
        [CampaignName] nvarchar(max) NULL,
        [Community] nvarchar(max) NULL,
        [AllowPhoto] bit NOT NULL,
        [AllowVideo] bit NOT NULL,
        [SignedByName] nvarchar(max) NULL,
        [SignatureId] uniqueidentifier NULL,
        [Status] nvarchar(50) NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_media_releases] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [clinical].[medical_referrals] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [EncounterId] uniqueidentifier NOT NULL,
        [PatientId] uniqueidentifier NOT NULL,
        [ReferralFolio] nvarchar(50) NOT NULL,
        [DestinationInstitution] nvarchar(max) NULL,
        [ReferralReason] nvarchar(max) NOT NULL,
        [Priority] nvarchar(max) NULL,
        [ReferredByUserId] uniqueidentifier NULL,
        [ProviderSignatureId] uniqueidentifier NULL,
        [Status] nvarchar(50) NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_medical_referrals] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [clinical].[medication_deliveries] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [EncounterId] uniqueidentifier NOT NULL,
        [PatientId] uniqueidentifier NOT NULL,
        [MedicationName] nvarchar(250) NOT NULL,
        [Presentation] nvarchar(max) NULL,
        [Quantity] nvarchar(max) NULL,
        [LotNumber] nvarchar(max) NULL,
        [ExpirationDate] date NULL,
        [Instructions] nvarchar(max) NULL,
        [DeliveredByUserId] uniqueidentifier NULL,
        [ReceivedByName] nvarchar(max) NULL,
        [SignatureId] uniqueidentifier NULL,
        [Status] nvarchar(50) NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_medication_deliveries] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [brigades].[mobile_units] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [UnitType] nvarchar(max) NULL,
        [PlateNumber] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [Status] nvarchar(50) NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_mobile_units] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [core].[organizations] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [LegalName] nvarchar(250) NULL,
        [Rfc] nvarchar(20) NULL,
        [Address] nvarchar(max) NULL,
        [Phone] nvarchar(max) NULL,
        [Email] nvarchar(200) NULL,
        [Website] nvarchar(max) NULL,
        [LogoUrl] nvarchar(max) NULL,
        [PrimaryColor] nvarchar(max) NULL,
        [SecondaryColor] nvarchar(max) NULL,
        [AccentColor] nvarchar(max) NULL,
        [FontFamily] nvarchar(max) NULL,
        [Status] nvarchar(50) NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_organizations] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [clinical].[patient_guardians] (
        [Id] uniqueidentifier NOT NULL,
        [PatientId] uniqueidentifier NOT NULL,
        [FullName] nvarchar(250) NULL,
        [Relationship] nvarchar(100) NULL,
        [Phone] nvarchar(50) NULL,
        [IdentificationType] nvarchar(max) NULL,
        [IdentificationValue] nvarchar(max) NULL,
        [IsPresent] bit NOT NULL,
        [AbsenceReason] nvarchar(max) NULL,
        [IsPrimary] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_patient_guardians] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [clinical].[patient_visits] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [VisitFolio] nvarchar(50) NOT NULL,
        [PatientId] uniqueidentifier NOT NULL,
        [BrigadeId] uniqueidentifier NOT NULL,
        [ArrivalTime] datetimeoffset NULL,
        [RegisteredByUserId] uniqueidentifier NULL,
        [VisitStatus] nvarchar(50) NOT NULL,
        [CreatedOffline] bit NOT NULL,
        [DeviceId] uniqueidentifier NULL,
        [SyncStatus] nvarchar(50) NOT NULL,
        [ClosedAt] datetimeoffset NULL,
        [ClosedByUserId] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_patient_visits] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [clinical].[patients] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [PatientFolio] nvarchar(50) NOT NULL,
        [FirstName] nvarchar(150) NULL,
        [PaternalLastName] nvarchar(150) NULL,
        [MaternalLastName] nvarchar(150) NULL,
        [FullNameNormalized] nvarchar(400) NULL,
        [BirthDate] date NULL,
        [ApproximateAge] int NULL,
        [Sex] int NOT NULL,
        [Curp] nvarchar(30) NULL,
        [Phone] nvarchar(50) NULL,
        [AddressLine] nvarchar(max) NULL,
        [Municipality] nvarchar(max) NULL,
        [Colony] nvarchar(max) NULL,
        [Community] nvarchar(max) NULL,
        [IsMinor] bit NOT NULL,
        [IsMigrant] bit NOT NULL,
        [IsPartialRecord] bit NOT NULL,
        [PartialRecordReason] nvarchar(max) NULL,
        [NotesAdmin] nvarchar(max) NULL,
        [Status] nvarchar(50) NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_patients] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [core].[permissions] (
        [Id] uniqueidentifier NOT NULL,
        [Code] nvarchar(150) NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(max) NULL,
        [Module] nvarchar(100) NOT NULL,
        [Action] nvarchar(100) NOT NULL,
        [SensitivityLevel] nvarchar(50) NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_permissions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [core].[role_permissions] (
        [Id] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        [PermissionId] uniqueidentifier NOT NULL,
        [GrantedAt] datetimeoffset NOT NULL,
        [GrantedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_role_permissions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [core].[roles] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [Code] nvarchar(100) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Description] nvarchar(max) NULL,
        [IsSystemRole] bit NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_roles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [clinical].[service_encounters] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [EncounterFolio] nvarchar(50) NOT NULL,
        [VisitId] uniqueidentifier NOT NULL,
        [PatientId] uniqueidentifier NOT NULL,
        [BrigadeId] uniqueidentifier NOT NULL,
        [ServiceId] uniqueidentifier NOT NULL,
        [ProviderUserId] uniqueidentifier NULL,
        [StartedAt] datetimeoffset NULL,
        [EndedAt] datetimeoffset NULL,
        [Status] nvarchar(50) NOT NULL,
        [NotesSummary] nvarchar(max) NULL,
        [Recommendations] nvarchar(max) NULL,
        [RequiresFollowUp] bit NOT NULL,
        [RequiresReferral] bit NOT NULL,
        [CreatedOffline] bit NOT NULL,
        [DeviceId] uniqueidentifier NULL,
        [SyncStatus] nvarchar(50) NOT NULL,
        [ClosedAt] datetimeoffset NULL,
        [ClosedByUserId] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_service_encounters] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [core].[services] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [Code] nvarchar(100) NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Category] nvarchar(100) NOT NULL,
        [Description] nvarchar(max) NULL,
        [RequiresConsent] bit NOT NULL,
        [RequiresClinicalNotes] bit NOT NULL,
        [RequiresFollowUpOption] bit NOT NULL,
        [RequiresReferralOption] bit NOT NULL,
        [IsSensitive] bit NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_services] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [sync].[sync_batches] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [DeviceId] uniqueidentifier NULL,
        [UserId] uniqueidentifier NOT NULL,
        [BrigadeId] uniqueidentifier NULL,
        [StartedAt] datetimeoffset NOT NULL,
        [CompletedAt] datetimeoffset NULL,
        [Status] nvarchar(50) NOT NULL,
        [EventsCount] int NOT NULL,
        [AcceptedCount] int NOT NULL,
        [RejectedCount] int NOT NULL,
        [ConflictCount] int NOT NULL,
        [ErrorSummary] nvarchar(4000) NULL,
        CONSTRAINT [PK_sync_batches] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [sync].[sync_events] (
        [Id] uniqueidentifier NOT NULL,
        [SyncBatchId] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [LocalEventId] nvarchar(150) NOT NULL,
        [EntityType] nvarchar(100) NOT NULL,
        [EntityId] uniqueidentifier NULL,
        [Operation] nvarchar(50) NOT NULL,
        [PayloadJson] nvarchar(max) NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        [ErrorMessage] nvarchar(max) NULL,
        [ConflictReason] nvarchar(max) NULL,
        [CreatedAtDevice] datetimeoffset NULL,
        [ReceivedAtServer] datetimeoffset NOT NULL,
        [ProcessedAt] datetimeoffset NULL,
        CONSTRAINT [PK_sync_events] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [core].[user_roles] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [AssignedAt] datetimeoffset NOT NULL,
        [AssignedByUserId] uniqueidentifier NULL,
        [ExpiresAt] datetimeoffset NULL,
        [Status] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_user_roles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE TABLE [core].[users] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [FullName] nvarchar(200) NOT NULL,
        [Email] nvarchar(200) NULL,
        [Phone] nvarchar(max) NULL,
        [Username] nvarchar(100) NULL,
        [Status] nvarchar(50) NOT NULL,
        [LastLoginAt] datetimeoffset NULL,
        [DeactivatedAt] datetimeoffset NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetimeoffset NULL,
        [DeletedByUserId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        CONSTRAINT [PK_users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ai_request_logs_OrganizationId_RequestedAt] ON [operations].[ai_request_logs] ([OrganizationId], [RequestedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_audit_events_EntityType_EntityId] ON [audit].[audit_events] ([EntityType], [EntityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_audit_events_OrganizationId_CreatedAt] ON [audit].[audit_events] ([OrganizationId], [CreatedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_brigade_services_BrigadeId_ServiceId] ON [brigades].[brigade_services] ([BrigadeId], [ServiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_brigade_services_IsDeleted] ON [brigades].[brigade_services] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_brigades_IsDeleted] ON [brigades].[brigades] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_brigades_OrganizationId_ScheduledDate] ON [brigades].[brigades] ([OrganizationId], [ScheduledDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_communities_IsDeleted] ON [brigades].[communities] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_communities_OrganizationId_Municipality_Colony] ON [brigades].[communities] ([OrganizationId], [Municipality], [Colony]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_crypto_integrity_records_OrganizationId_EntityType_EntityId] ON [audit].[crypto_integrity_records] ([OrganizationId], [EntityType], [EntityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_devices_IsDeleted] ON [core].[devices] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_devices_OrganizationId_AssignedToUserId] ON [core].[devices] ([OrganizationId], [AssignedToUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_document_signatures_IsDeleted] ON [documents].[document_signatures] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_document_signatures_OrganizationId_DocumentTemplateId] ON [documents].[document_signatures] ([OrganizationId], [DocumentTemplateId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_document_signatures_PatientId_VisitId_EncounterId] ON [documents].[document_signatures] ([PatientId], [VisitId], [EncounterId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_document_templates_IsDeleted] ON [documents].[document_templates] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_document_templates_OrganizationId_DocumentType_Version] ON [documents].[document_templates] ([OrganizationId], [DocumentType], [Version]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_export_jobs_OrganizationId_RequestedAt] ON [operations].[export_jobs] ([OrganizationId], [RequestedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_form_responses_IsDeleted] ON [forms].[form_responses] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_form_responses_OrganizationId_EncounterId] ON [forms].[form_responses] ([OrganizationId], [EncounterId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_form_templates_IsDeleted] ON [forms].[form_templates] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_form_templates_OrganizationId_ServiceId_FormCode_Version] ON [forms].[form_templates] ([OrganizationId], [ServiceId], [FormCode], [Version]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_media_releases_IsDeleted] ON [documents].[media_releases] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_media_releases_OrganizationId_PatientId] ON [documents].[media_releases] ([OrganizationId], [PatientId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_medical_referrals_IsDeleted] ON [clinical].[medical_referrals] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_medical_referrals_OrganizationId_ReferralFolio] ON [clinical].[medical_referrals] ([OrganizationId], [ReferralFolio]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_medication_deliveries_IsDeleted] ON [clinical].[medication_deliveries] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_medication_deliveries_OrganizationId_PatientId] ON [clinical].[medication_deliveries] ([OrganizationId], [PatientId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_mobile_units_IsDeleted] ON [brigades].[mobile_units] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_mobile_units_OrganizationId_Name] ON [brigades].[mobile_units] ([OrganizationId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_organizations_IsDeleted] ON [core].[organizations] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_organizations_Name] ON [core].[organizations] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_patient_guardians_IsDeleted] ON [clinical].[patient_guardians] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_patient_guardians_PatientId] ON [clinical].[patient_guardians] ([PatientId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_patient_visits_BrigadeId_PatientId] ON [clinical].[patient_visits] ([BrigadeId], [PatientId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_patient_visits_IsDeleted] ON [clinical].[patient_visits] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_patient_visits_OrganizationId_VisitFolio] ON [clinical].[patient_visits] ([OrganizationId], [VisitFolio]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_patients_IsDeleted] ON [clinical].[patients] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_patients_OrganizationId_FullNameNormalized] ON [clinical].[patients] ([OrganizationId], [FullNameNormalized]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_patients_OrganizationId_PatientFolio] ON [clinical].[patients] ([OrganizationId], [PatientFolio]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_permissions_Code] ON [core].[permissions] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_role_permissions_RoleId_PermissionId] ON [core].[role_permissions] ([RoleId], [PermissionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_roles_IsDeleted] ON [core].[roles] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_roles_OrganizationId_Code] ON [core].[roles] ([OrganizationId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_service_encounters_IsDeleted] ON [clinical].[service_encounters] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_service_encounters_OrganizationId_EncounterFolio] ON [clinical].[service_encounters] ([OrganizationId], [EncounterFolio]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_service_encounters_VisitId_ServiceId] ON [clinical].[service_encounters] ([VisitId], [ServiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_services_IsDeleted] ON [core].[services] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_services_OrganizationId_Code] ON [core].[services] ([OrganizationId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_sync_batches_OrganizationId_DeviceId_StartedAt] ON [sync].[sync_batches] ([OrganizationId], [DeviceId], [StartedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_sync_events_SyncBatchId_LocalEventId] ON [sync].[sync_events] ([SyncBatchId], [LocalEventId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_user_roles_OrganizationId_UserId_RoleId] ON [core].[user_roles] ([OrganizationId], [UserId], [RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_users_IsDeleted] ON [core].[users] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_users_OrganizationId_Email] ON [core].[users] ([OrganizationId], [Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_users_OrganizationId_Username] ON [core].[users] ([OrganizationId], [Username]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504184935_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260504184935_InitialCreate', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504220324_AddConsentDocuments'
)
BEGIN
    CREATE TABLE [ConsentDocuments] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [PatientId] uniqueidentifier NOT NULL,
        [VisitId] uniqueidentifier NULL,
        [ConsentType] nvarchar(max) NOT NULL,
        [DocumentVersion] nvarchar(max) NOT NULL,
        [DocumentTextSnapshot] nvarchar(max) NULL,
        [SignatureDataUrl] nvarchar(max) NULL,
        [GuardianFullName] nvarchar(max) NULL,
        [GuardianRelationship] nvarchar(max) NULL,
        [SignedByUserId] uniqueidentifier NULL,
        [SignedAt] datetimeoffset NOT NULL,
        [CreatedOffline] bit NOT NULL,
        [DeviceId] uniqueidentifier NULL,
        [SyncStatus] nvarchar(max) NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ConsentDocuments] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260504220324_AddConsentDocuments'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260504220324_AddConsentDocuments', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260505014437_AddAuditLogs'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NULL,
        [Action] nvarchar(max) NOT NULL,
        [EntityName] nvarchar(max) NOT NULL,
        [EntityId] uniqueidentifier NULL,
        [DetailsJson] nvarchar(max) NULL,
        [CorrelationId] nvarchar(max) NULL,
        [IpAddress] nvarchar(max) NULL,
        [UserAgent] nvarchar(max) NULL,
        [OccurredAtUtc] datetimeoffset NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260505014437_AddAuditLogs'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260505014437_AddAuditLogs', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508054350_MakeSyncBatchDeviceIdNullable'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[sync].[sync_batches]') AND [c].[name] = N'DeviceId');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [sync].[sync_batches] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [sync].[sync_batches] ALTER COLUMN [DeviceId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508054350_MakeSyncBatchDeviceIdNullable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260508054350_MakeSyncBatchDeviceIdNullable', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508062505_AddFormResponseSubmittedTimestamps'
)
BEGIN
    ALTER TABLE [forms].[form_responses] ADD [SubmittedAt] datetimeoffset NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508062505_AddFormResponseSubmittedTimestamps'
)
BEGIN
    ALTER TABLE [forms].[form_responses] ADD [CapturedAt] datetimeoffset NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260508062505_AddFormResponseSubmittedTimestamps'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260508062505_AddFormResponseSubmittedTimestamps', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512194857_AddCoreSecurityForeignKeys'
)
BEGIN
    CREATE INDEX [IX_user_roles_RoleId] ON [core].[user_roles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512194857_AddCoreSecurityForeignKeys'
)
BEGIN
    CREATE INDEX [IX_user_roles_UserId] ON [core].[user_roles] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512194857_AddCoreSecurityForeignKeys'
)
BEGIN
    CREATE INDEX [IX_role_permissions_PermissionId] ON [core].[role_permissions] ([PermissionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512194857_AddCoreSecurityForeignKeys'
)
BEGIN
    ALTER TABLE [core].[role_permissions] ADD CONSTRAINT [FK_role_permissions_permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [core].[permissions] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512194857_AddCoreSecurityForeignKeys'
)
BEGIN
    ALTER TABLE [core].[role_permissions] ADD CONSTRAINT [FK_role_permissions_roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [core].[roles] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512194857_AddCoreSecurityForeignKeys'
)
BEGIN
    ALTER TABLE [core].[roles] ADD CONSTRAINT [FK_roles_organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [core].[organizations] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512194857_AddCoreSecurityForeignKeys'
)
BEGIN
    ALTER TABLE [core].[services] ADD CONSTRAINT [FK_services_organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [core].[organizations] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512194857_AddCoreSecurityForeignKeys'
)
BEGIN
    ALTER TABLE [core].[user_roles] ADD CONSTRAINT [FK_user_roles_organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [core].[organizations] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512194857_AddCoreSecurityForeignKeys'
)
BEGIN
    ALTER TABLE [core].[user_roles] ADD CONSTRAINT [FK_user_roles_roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [core].[roles] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512194857_AddCoreSecurityForeignKeys'
)
BEGIN
    ALTER TABLE [core].[user_roles] ADD CONSTRAINT [FK_user_roles_users_UserId] FOREIGN KEY ([UserId]) REFERENCES [core].[users] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512194857_AddCoreSecurityForeignKeys'
)
BEGIN
    ALTER TABLE [core].[users] ADD CONSTRAINT [FK_users_organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [core].[organizations] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512194857_AddCoreSecurityForeignKeys'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260512194857_AddCoreSecurityForeignKeys', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512195629_AddBrigadesForeignKeys'
)
BEGIN
    CREATE INDEX [IX_brigades_CommunityId] ON [brigades].[brigades] ([CommunityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512195629_AddBrigadesForeignKeys'
)
BEGIN
    CREATE INDEX [IX_brigades_MobileUnitId] ON [brigades].[brigades] ([MobileUnitId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512195629_AddBrigadesForeignKeys'
)
BEGIN
    CREATE INDEX [IX_brigade_services_ServiceId] ON [brigades].[brigade_services] ([ServiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512195629_AddBrigadesForeignKeys'
)
BEGIN
    ALTER TABLE [brigades].[brigade_services] ADD CONSTRAINT [FK_brigade_services_brigades_BrigadeId] FOREIGN KEY ([BrigadeId]) REFERENCES [brigades].[brigades] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512195629_AddBrigadesForeignKeys'
)
BEGIN
    ALTER TABLE [brigades].[brigade_services] ADD CONSTRAINT [FK_brigade_services_services_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [core].[services] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512195629_AddBrigadesForeignKeys'
)
BEGIN
    ALTER TABLE [brigades].[brigades] ADD CONSTRAINT [FK_brigades_communities_CommunityId] FOREIGN KEY ([CommunityId]) REFERENCES [brigades].[communities] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512195629_AddBrigadesForeignKeys'
)
BEGIN
    ALTER TABLE [brigades].[brigades] ADD CONSTRAINT [FK_brigades_mobile_units_MobileUnitId] FOREIGN KEY ([MobileUnitId]) REFERENCES [brigades].[mobile_units] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512195629_AddBrigadesForeignKeys'
)
BEGIN
    ALTER TABLE [brigades].[brigades] ADD CONSTRAINT [FK_brigades_organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [core].[organizations] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512195629_AddBrigadesForeignKeys'
)
BEGIN
    ALTER TABLE [brigades].[communities] ADD CONSTRAINT [FK_communities_organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [core].[organizations] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512195629_AddBrigadesForeignKeys'
)
BEGIN
    ALTER TABLE [brigades].[mobile_units] ADD CONSTRAINT [FK_mobile_units_organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [core].[organizations] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512195629_AddBrigadesForeignKeys'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260512195629_AddBrigadesForeignKeys', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    CREATE INDEX [IX_service_encounters_BrigadeId] ON [clinical].[service_encounters] ([BrigadeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    CREATE INDEX [IX_service_encounters_PatientId] ON [clinical].[service_encounters] ([PatientId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    CREATE INDEX [IX_service_encounters_ServiceId] ON [clinical].[service_encounters] ([ServiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    CREATE INDEX [IX_patient_visits_PatientId] ON [clinical].[patient_visits] ([PatientId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    CREATE INDEX [IX_medication_deliveries_EncounterId] ON [clinical].[medication_deliveries] ([EncounterId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    CREATE INDEX [IX_medication_deliveries_PatientId] ON [clinical].[medication_deliveries] ([PatientId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    CREATE INDEX [IX_medical_referrals_EncounterId] ON [clinical].[medical_referrals] ([EncounterId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    CREATE INDEX [IX_medical_referrals_PatientId] ON [clinical].[medical_referrals] ([PatientId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    ALTER TABLE [clinical].[medical_referrals] ADD CONSTRAINT [FK_medical_referrals_organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [core].[organizations] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    ALTER TABLE [clinical].[medical_referrals] ADD CONSTRAINT [FK_medical_referrals_patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [clinical].[patients] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    ALTER TABLE [clinical].[medical_referrals] ADD CONSTRAINT [FK_medical_referrals_service_encounters_EncounterId] FOREIGN KEY ([EncounterId]) REFERENCES [clinical].[service_encounters] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    ALTER TABLE [clinical].[medication_deliveries] ADD CONSTRAINT [FK_medication_deliveries_organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [core].[organizations] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    ALTER TABLE [clinical].[medication_deliveries] ADD CONSTRAINT [FK_medication_deliveries_patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [clinical].[patients] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    ALTER TABLE [clinical].[medication_deliveries] ADD CONSTRAINT [FK_medication_deliveries_service_encounters_EncounterId] FOREIGN KEY ([EncounterId]) REFERENCES [clinical].[service_encounters] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    ALTER TABLE [clinical].[patient_guardians] ADD CONSTRAINT [FK_patient_guardians_patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [clinical].[patients] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    ALTER TABLE [clinical].[patient_visits] ADD CONSTRAINT [FK_patient_visits_brigades_BrigadeId] FOREIGN KEY ([BrigadeId]) REFERENCES [brigades].[brigades] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    ALTER TABLE [clinical].[patient_visits] ADD CONSTRAINT [FK_patient_visits_organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [core].[organizations] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    ALTER TABLE [clinical].[patient_visits] ADD CONSTRAINT [FK_patient_visits_patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [clinical].[patients] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    ALTER TABLE [clinical].[patients] ADD CONSTRAINT [FK_patients_organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [core].[organizations] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    ALTER TABLE [clinical].[service_encounters] ADD CONSTRAINT [FK_service_encounters_brigades_BrigadeId] FOREIGN KEY ([BrigadeId]) REFERENCES [brigades].[brigades] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    ALTER TABLE [clinical].[service_encounters] ADD CONSTRAINT [FK_service_encounters_organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [core].[organizations] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    ALTER TABLE [clinical].[service_encounters] ADD CONSTRAINT [FK_service_encounters_patient_visits_VisitId] FOREIGN KEY ([VisitId]) REFERENCES [clinical].[patient_visits] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    ALTER TABLE [clinical].[service_encounters] ADD CONSTRAINT [FK_service_encounters_patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [clinical].[patients] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    ALTER TABLE [clinical].[service_encounters] ADD CONSTRAINT [FK_service_encounters_services_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [core].[services] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260512202316_AddClinicalForeignKeys'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260512202316_AddClinicalForeignKeys', N'10.0.7');
END;

COMMIT;
GO

