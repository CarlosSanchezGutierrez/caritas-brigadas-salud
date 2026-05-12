$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$SqlBaselinePath = Join-Path $RepoRoot "database/migrations/sqlserver/0001_initial_create.sql"
$MigrationsPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Persistence/Migrations"
$SnapshotPath = Join-Path $MigrationsPath "CaritasDbContextModelSnapshot.cs"

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

function Get-SnapshotEntityBlocks {
    param(
        [string]$SnapshotContent,
        [string]$EntityName
    )

    $FullName = "Caritas.Brigadas.Domain.Entities.$EntityName"

    $EntityPattern = '(?s)modelBuilder\.Entity\("' +
        [regex]::Escape($FullName) +
        '", b =>\s*\{.*?\r?\n\s*\}\);'

    return [regex]::Matches($SnapshotContent, $EntityPattern)
}

function Assert-SnapshotRelationship {
    param(
        [string]$SnapshotContent,
        [string]$DependentEntity,
        [string]$PrincipalEntity,
        [string]$ForeignKeyProperty,
        [bool]$Required
    )

    $PrincipalFullName = "Caritas.Brigadas.Domain.Entities.$PrincipalEntity"
    $EntityBlocks = Get-SnapshotEntityBlocks `
        -SnapshotContent $SnapshotContent `
        -EntityName $DependentEntity

    if ($EntityBlocks.Count -eq 0) {
        throw "EF model snapshot is missing dependent entity block: $DependentEntity."
    }

    $RelationshipPattern = '(?s)\.HasOne\("' +
        [regex]::Escape($PrincipalFullName) +
        '"(?:,\s*null)?\).*?\.HasForeignKey\("' +
        [regex]::Escape($ForeignKeyProperty) +
        '"\).*?;'

    $FoundRelationship = $false
    $WrongRelationshipDetails = New-Object System.Collections.Generic.List[string]

    foreach ($EntityBlock in $EntityBlocks) {
        $RelationshipMatches = [regex]::Matches($EntityBlock.Value, $RelationshipPattern)

        foreach ($RelationshipMatch in $RelationshipMatches) {
            $FoundRelationship = $true
            $RelationshipText = $RelationshipMatch.Value

            if (-not $RelationshipText.Contains(".OnDelete(DeleteBehavior.NoAction)")) {
                $WrongRelationshipDetails.Add("missing DeleteBehavior.NoAction")
                continue
            }

            $HasIsRequired = $RelationshipText.Contains(".IsRequired()")

            if ($Required -and -not $HasIsRequired) {
                $WrongRelationshipDetails.Add("missing IsRequired()")
                continue
            }

            if (-not $Required -and $HasIsRequired) {
                $WrongRelationshipDetails.Add("optional relationship is marked IsRequired()")
                continue
            }

            return
        }
    }

    $RequiredText = if ($Required) { "required" } else { "optional" }

    if ($FoundRelationship) {
        $Details = ($WrongRelationshipDetails | Sort-Object -Unique) -join ", "
        throw "EF model snapshot has $DependentEntity.$ForeignKeyProperty -> $PrincipalEntity.Id, but it is not the expected $RequiredText NoAction relationship. Details: $Details"
    }

    throw "EF model snapshot is missing expected $RequiredText relationship inside $DependentEntity block: $DependentEntity.$ForeignKeyProperty -> $PrincipalEntity.Id with DeleteBehavior.NoAction."
}

Assert-FileExists $SqlBaselinePath
Assert-DirectoryExists $MigrationsPath
Assert-FileExists $SnapshotPath

$SqlContent = Get-Content $SqlBaselinePath -Raw -Encoding UTF8

$MigrationContent = Get-ChildItem $MigrationsPath -Recurse -File -Include "*.cs" |
    Where-Object { $_.Name -ne "CaritasDbContextModelSnapshot.cs" } |
    ForEach-Object { Get-Content $_.FullName -Raw -Encoding UTF8 } |
    Out-String

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

$RequiredSnapshotRelationships = @(
    @{ Dependent = "Role"; Principal = "Organization"; Property = "OrganizationId"; Required = $true },
    @{ Dependent = "User"; Principal = "Organization"; Property = "OrganizationId"; Required = $true },
    @{ Dependent = "UserRole"; Principal = "Organization"; Property = "OrganizationId"; Required = $true },
    @{ Dependent = "UserRole"; Principal = "User"; Property = "UserId"; Required = $true },
    @{ Dependent = "UserRole"; Principal = "Role"; Property = "RoleId"; Required = $true },
    @{ Dependent = "RolePermission"; Principal = "Role"; Property = "RoleId"; Required = $true },
    @{ Dependent = "RolePermission"; Principal = "Permission"; Property = "PermissionId"; Required = $true },
    @{ Dependent = "Service"; Principal = "Organization"; Property = "OrganizationId"; Required = $true },

    @{ Dependent = "Community"; Principal = "Organization"; Property = "OrganizationId"; Required = $true },
    @{ Dependent = "MobileUnit"; Principal = "Organization"; Property = "OrganizationId"; Required = $true },
    @{ Dependent = "Brigade"; Principal = "Organization"; Property = "OrganizationId"; Required = $true },
    @{ Dependent = "Brigade"; Principal = "Community"; Property = "CommunityId"; Required = $false },
    @{ Dependent = "Brigade"; Principal = "MobileUnit"; Property = "MobileUnitId"; Required = $false },
    @{ Dependent = "BrigadeService"; Principal = "Brigade"; Property = "BrigadeId"; Required = $true },
    @{ Dependent = "BrigadeService"; Principal = "Service"; Property = "ServiceId"; Required = $true },

    @{ Dependent = "Patient"; Principal = "Organization"; Property = "OrganizationId"; Required = $true },
    @{ Dependent = "PatientGuardian"; Principal = "Patient"; Property = "PatientId"; Required = $true },
    @{ Dependent = "PatientVisit"; Principal = "Organization"; Property = "OrganizationId"; Required = $true },
    @{ Dependent = "PatientVisit"; Principal = "Patient"; Property = "PatientId"; Required = $true },
    @{ Dependent = "PatientVisit"; Principal = "Brigade"; Property = "BrigadeId"; Required = $true },
    @{ Dependent = "ServiceEncounter"; Principal = "Organization"; Property = "OrganizationId"; Required = $true },
    @{ Dependent = "ServiceEncounter"; Principal = "Patient"; Property = "PatientId"; Required = $true },
    @{ Dependent = "ServiceEncounter"; Principal = "PatientVisit"; Property = "VisitId"; Required = $true },
    @{ Dependent = "ServiceEncounter"; Principal = "Brigade"; Property = "BrigadeId"; Required = $true },
    @{ Dependent = "ServiceEncounter"; Principal = "Service"; Property = "ServiceId"; Required = $true },
    @{ Dependent = "MedicalReferral"; Principal = "Organization"; Property = "OrganizationId"; Required = $true },
    @{ Dependent = "MedicalReferral"; Principal = "Patient"; Property = "PatientId"; Required = $true },
    @{ Dependent = "MedicalReferral"; Principal = "ServiceEncounter"; Property = "EncounterId"; Required = $true },
    @{ Dependent = "MedicationDelivery"; Principal = "Organization"; Property = "OrganizationId"; Required = $true },
    @{ Dependent = "MedicationDelivery"; Principal = "Patient"; Property = "PatientId"; Required = $true },
    @{ Dependent = "MedicationDelivery"; Principal = "ServiceEncounter"; Property = "EncounterId"; Required = $true },

    @{ Dependent = "FormTemplate"; Principal = "Organization"; Property = "OrganizationId"; Required = $true },
    @{ Dependent = "FormTemplate"; Principal = "Service"; Property = "ServiceId"; Required = $true },
    @{ Dependent = "FormResponse"; Principal = "Organization"; Property = "OrganizationId"; Required = $true },
    @{ Dependent = "FormResponse"; Principal = "FormTemplate"; Property = "FormTemplateId"; Required = $true },
    @{ Dependent = "FormResponse"; Principal = "ServiceEncounter"; Property = "EncounterId"; Required = $true },
    @{ Dependent = "DocumentTemplate"; Principal = "Organization"; Property = "OrganizationId"; Required = $true },
    @{ Dependent = "DocumentTemplate"; Principal = "Service"; Property = "AppliesToServiceId"; Required = $false },
    @{ Dependent = "DocumentSignature"; Principal = "Organization"; Property = "OrganizationId"; Required = $true },
    @{ Dependent = "DocumentSignature"; Principal = "DocumentTemplate"; Property = "DocumentTemplateId"; Required = $true },
    @{ Dependent = "DocumentSignature"; Principal = "Patient"; Property = "PatientId"; Required = $false },
    @{ Dependent = "DocumentSignature"; Principal = "PatientVisit"; Property = "VisitId"; Required = $false },
    @{ Dependent = "DocumentSignature"; Principal = "ServiceEncounter"; Property = "EncounterId"; Required = $false },
    @{ Dependent = "MediaRelease"; Principal = "Organization"; Property = "OrganizationId"; Required = $true },
    @{ Dependent = "MediaRelease"; Principal = "Patient"; Property = "PatientId"; Required = $true },
    @{ Dependent = "MediaRelease"; Principal = "PatientVisit"; Property = "VisitId"; Required = $false },
    @{ Dependent = "SyncBatch"; Principal = "Organization"; Property = "OrganizationId"; Required = $true },
    @{ Dependent = "SyncBatch"; Principal = "Brigade"; Property = "BrigadeId"; Required = $false },
    @{ Dependent = "SyncEvent"; Principal = "Organization"; Property = "OrganizationId"; Required = $true },
    @{ Dependent = "SyncEvent"; Principal = "SyncBatch"; Property = "SyncBatchId"; Required = $true }
)

foreach ($Relationship in $RequiredSnapshotRelationships) {
    Assert-SnapshotRelationship `
        -SnapshotContent $SnapshotContent `
        -DependentEntity $Relationship.Dependent `
        -PrincipalEntity $Relationship.Principal `
        -ForeignKeyProperty $Relationship.Property `
        -Required ([bool]$Relationship.Required)
}

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

$ForbiddenSnapshotTokens = @(
    "DeleteBehavior.Cascade",
    "DeleteBehavior.ClientCascade",
    "DeleteBehavior.SetNull",
    ".HasOne(""Caritas.Brigadas.Domain.Entities.Device"", null)"
)

foreach ($Token in $ForbiddenSnapshotTokens) {
    Assert-NotContains $SnapshotContent $Token "EF model snapshot"
}

Write-Host ""
Write-Host "============================================================" -ForegroundColor Green
Write-Host "DATABASE FOREIGN KEY BASELINE VALIDATION PASO CORRECTAMENTE" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green