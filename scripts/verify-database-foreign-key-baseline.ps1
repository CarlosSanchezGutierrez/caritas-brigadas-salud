$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$SqlBaselinePath = Join-Path $RepoRoot "database/migrations/sqlserver/0001_initial_create.sql"
$MigrationsPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Persistence/Migrations"

function Assert-FileExists {
    param([string]$Path)

    if (-not (Test-Path $Path)) {
        throw "Required file not found: $Path"
    }
}

function Assert-DirectoryExists {
    param([string]$Path)

    if (-not (Test-Path $Path)) {
        throw "Required directory not found: $Path"
    }
}

function Assert-Contains {
    param(
        [string]$Content,
        [string]$Token,
        [string]$Label
    )

    if (-not $Content.Contains($Token)) {
        throw "$Label does not contain required token: $Token"
    }
}

function Assert-NotContains {
    param(
        [string]$Content,
        [string]$Token,
        [string]$Label
    )

    if ($Content.Contains($Token)) {
        throw "$Label contains forbidden token: $Token"
    }
}

Assert-FileExists $SqlBaselinePath
Assert-DirectoryExists $MigrationsPath

$SqlContent = Get-Content $SqlBaselinePath -Raw -Encoding UTF8

$MigrationContent = Get-ChildItem $MigrationsPath -Recurse -File -Include "*.cs" |
    Where-Object { $_.Name -ne "CaritasDbContextModelSnapshot.cs" } |
    ForEach-Object { Get-Content $_.FullName -Raw -Encoding UTF8 } |
    Out-String

$SnapshotPath = Join-Path $MigrationsPath "CaritasDbContextModelSnapshot.cs"
Assert-FileExists $SnapshotPath
$SnapshotContent = Get-Content $SnapshotPath -Raw -Encoding UTF8

$RequiredForeignKeys = @(
    "FK_roles_organizations_OrganizationId",
    "FK_users_organizations_OrganizationId",
    "FK_user_roles_organizations_OrganizationId",
    "FK_user_roles_users_UserId",
    "FK_user_roles_roles_RoleId",
    "FK_role_permissions_roles_RoleId",
    "FK_role_permissions_permissions_PermissionId",
    "FK_services_organizations_OrganizationId",

    "FK_communities_organizations_OrganizationId",
    "FK_mobile_units_organizations_OrganizationId",
    "FK_brigades_organizations_OrganizationId",
    "FK_brigades_communities_CommunityId",
    "FK_brigades_mobile_units_MobileUnitId",
    "FK_brigade_services_brigades_BrigadeId",
    "FK_brigade_services_services_ServiceId",

    "FK_patients_organizations_OrganizationId",
    "FK_patient_guardians_patients_PatientId",
    "FK_patient_visits_organizations_OrganizationId",
    "FK_patient_visits_patients_PatientId",
    "FK_patient_visits_brigades_BrigadeId",
    "FK_service_encounters_organizations_OrganizationId",
    "FK_service_encounters_patients_PatientId",
    "FK_service_encounters_patient_visits_VisitId",
    "FK_service_encounters_brigades_BrigadeId",
    "FK_service_encounters_services_ServiceId",
    "FK_medical_referrals_organizations_OrganizationId",
    "FK_medical_referrals_patients_PatientId",
    "FK_medical_referrals_service_encounters_EncounterId",
    "FK_medication_deliveries_organizations_OrganizationId",
    "FK_medication_deliveries_patients_PatientId",
    "FK_medication_deliveries_service_encounters_EncounterId",

    "FK_form_templates_organizations_OrganizationId",
    "FK_form_templates_services_ServiceId",
    "FK_form_responses_organizations_OrganizationId",
    "FK_form_responses_form_templates_FormTemplateId",
    "FK_form_responses_service_encounters_EncounterId",
    "FK_document_templates_organizations_OrganizationId",
    "FK_document_templates_services_AppliesToServiceId",
    "FK_document_signatures_organizations_OrganizationId",
    "FK_document_signatures_document_templates_DocumentTemplateId",
    "FK_document_signatures_patients_PatientId",
    "FK_document_signatures_patient_visits_VisitId",
    "FK_document_signatures_service_encounters_EncounterId",
    "FK_media_releases_organizations_OrganizationId",
    "FK_media_releases_patients_PatientId",
    "FK_media_releases_patient_visits_VisitId",
    "FK_sync_batches_organizations_OrganizationId",
    "FK_sync_batches_brigades_BrigadeId",
    "FK_sync_events_organizations_OrganizationId",
    "FK_sync_events_sync_batches_SyncBatchId"
)

foreach ($ForeignKey in $RequiredForeignKeys) {
    Assert-Contains $SqlContent $ForeignKey "SQL Server deployment baseline"
    Assert-Contains $MigrationContent $ForeignKey "EF migration files"
}

Assert-Contains $MigrationContent "AddForeignKey" "EF migration files"

$ForbiddenSqlTokens = @(
    "ON DELETE CASCADE",
    "ON DELETE SET NULL",
    "FK_sync_batches_devices_DeviceId",
    "FK_form_responses_devices_DeviceId",
    "FK_document_signatures_devices_DeviceId",
    "REFERENCES [core].[devices]"
)

foreach ($Token in $ForbiddenSqlTokens) {
    Assert-NotContains $SqlContent $Token "SQL Server deployment baseline"
}

$ForbiddenMigrationTokens = @(
    "ReferentialAction.Cascade",
    "ReferentialAction.SetNull",
    "principalTable: ""devices""",
    "FK_sync_batches_devices_DeviceId",
    "FK_form_responses_devices_DeviceId",
    "FK_document_signatures_devices_DeviceId"
)

foreach ($Token in $ForbiddenMigrationTokens) {
    Assert-NotContains $MigrationContent $Token "EF migration files"
}

$RequiredSnapshotTokens = @(
    ".HasForeignKey(""OrganizationId"")",
    ".HasForeignKey(""PatientId"")",
    ".HasForeignKey(""EncounterId"")",
    ".HasForeignKey(""SyncBatchId"")",
    ".OnDelete(DeleteBehavior.NoAction)"
)

foreach ($Token in $RequiredSnapshotTokens) {
    Assert-Contains $SnapshotContent $Token "EF model snapshot"
}

Write-Host ""
Write-Host "============================================================" -ForegroundColor Green
Write-Host "DATABASE FOREIGN KEY BASELINE VALIDATION PASO CORRECTAMENTE" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green