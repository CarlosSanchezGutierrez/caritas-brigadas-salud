# P2 Data Integrity Release

Status: completed  
Scope: backend data integrity baseline  
Branch target: develop  
Database target: SQL Server  
Application stack: ASP.NET Core, Entity Framework Core, SQL Server

---

## 1. Purpose

This document closes the P2 data integrity baseline for Caritas Brigadas de Salud.

The goal of P2 was to move the backend from a mostly index/property-based EF model into a stronger relational integrity baseline, without allowing destructive cascade behavior and without breaking offline/sync scenarios.

P2 focused on:

- EF model integrity contracts.
- Foreign key inventory.
- Explicit foreign key packages.
- Safe delete behavior.
- SQL Server deployment baseline alignment.
- Migration and SQL drift detection.
- Snapshot drift detection.
- Deferred policy for device references.

---

## 2. Completed P2 sequence

| Area | Result |
|---|---|
| P2-01 EF model integrity contracts | Added executable contracts for primary keys, client-generated IDs, auditable fields, soft-delete fields, indexes, and critical property limits. |
| P2-02 data integrity findings | Documented relational integrity gaps, FK candidates, delete behavior policy, and deferred relationships. |
| P2-03 delete behavior contracts | Added guardrails against Cascade and ClientCascade delete behavior. |
| P2-04 FK inventory contracts | Added reviewed FK inventory, package sequencing, required/optional validation, and deferred DeviceId policy. |
| P2-05 core/security FKs | Added FK package for users, roles, role permissions, user roles, services, and organizations. |
| P2-06 brigades FKs | Added FK package for communities, mobile units, brigades, and brigade services. |
| P2-07 clinical FKs | Added FK package for patients, visits, encounters, referrals, and medication deliveries. |
| P2-08 forms/documents/sync FKs | Added FK package for forms, documents, media releases, sync batches, and sync events. |
| P2-09 FK baseline gate | Added CI script to validate reviewed FK constraints across EF migrations and SQL Server baseline. |
| P2-09 hotfix snapshot gate | Strengthened snapshot validation to check each expected relationship inside its own entity block. |

---

## 3. Current FK package map

### 3.1 Core/security package

Included relationships:

- Role.OrganizationId -> Organization.Id
- User.OrganizationId -> Organization.Id
- UserRole.OrganizationId -> Organization.Id
- UserRole.UserId -> User.Id
- UserRole.RoleId -> Role.Id
- RolePermission.RoleId -> Role.Id
- RolePermission.PermissionId -> Permission.Id
- Service.OrganizationId -> Organization.Id

All use `DeleteBehavior.NoAction`.

---

### 3.2 Brigades package

Included relationships:

- Community.OrganizationId -> Organization.Id
- MobileUnit.OrganizationId -> Organization.Id
- Brigade.OrganizationId -> Organization.Id
- Brigade.CommunityId -> Community.Id
- Brigade.MobileUnitId -> MobileUnit.Id
- BrigadeService.BrigadeId -> Brigade.Id
- BrigadeService.ServiceId -> Service.Id

All use `DeleteBehavior.NoAction`.

---

### 3.3 Clinical package

Included relationships:

- Patient.OrganizationId -> Organization.Id
- PatientGuardian.PatientId -> Patient.Id
- PatientVisit.OrganizationId -> Organization.Id
- PatientVisit.PatientId -> Patient.Id
- PatientVisit.BrigadeId -> Brigade.Id
- ServiceEncounter.OrganizationId -> Organization.Id
- ServiceEncounter.PatientId -> Patient.Id
- ServiceEncounter.VisitId -> PatientVisit.Id
- ServiceEncounter.BrigadeId -> Brigade.Id
- ServiceEncounter.ServiceId -> Service.Id
- MedicalReferral.OrganizationId -> Organization.Id
- MedicalReferral.PatientId -> Patient.Id
- MedicalReferral.EncounterId -> ServiceEncounter.Id
- MedicationDelivery.OrganizationId -> Organization.Id
- MedicationDelivery.PatientId -> Patient.Id
- MedicationDelivery.EncounterId -> ServiceEncounter.Id

All use `DeleteBehavior.NoAction`.

---

### 3.4 Forms, documents, and sync package

Included relationships:

- FormTemplate.OrganizationId -> Organization.Id
- FormTemplate.ServiceId -> Service.Id
- FormResponse.OrganizationId -> Organization.Id
- FormResponse.FormTemplateId -> FormTemplate.Id
- FormResponse.EncounterId -> ServiceEncounter.Id
- DocumentTemplate.OrganizationId -> Organization.Id
- DocumentTemplate.AppliesToServiceId -> Service.Id
- DocumentSignature.OrganizationId -> Organization.Id
- DocumentSignature.DocumentTemplateId -> DocumentTemplate.Id
- DocumentSignature.PatientId -> Patient.Id
- DocumentSignature.VisitId -> PatientVisit.Id
- DocumentSignature.EncounterId -> ServiceEncounter.Id
- MediaRelease.OrganizationId -> Organization.Id
- MediaRelease.PatientId -> Patient.Id
- MediaRelease.VisitId -> PatientVisit.Id
- SyncBatch.OrganizationId -> Organization.Id
- SyncBatch.BrigadeId -> Brigade.Id
- SyncEvent.OrganizationId -> Organization.Id
- SyncEvent.SyncBatchId -> SyncBatch.Id

All use `DeleteBehavior.NoAction`.

---

## 4. Delete behavior policy

P2 standardizes reviewed foreign keys on `DeleteBehavior.NoAction`.

The backend must not introduce:

- `DeleteBehavior.Cascade`
- `DeleteBehavior.ClientCascade`
- `DeleteBehavior.SetNull`
- `ReferentialAction.Cascade`
- `ReferentialAction.SetNull`
- `ON DELETE CASCADE`
- `ON DELETE SET NULL`

Reason:

- The system handles clinical, operational, document, consent, audit, and sync data.
- The model uses soft-delete patterns.
- Patient and brigade history must not disappear through database cascades.
- Audit and compliance workflows require explicit lifecycle control.

Hard deletes must remain exceptional, manual, authorized, and documented.

---

## 5. Deferred DeviceId policy

`DeviceId` relationships are intentionally not strong FKs in P2.

Deferred relationships include:

- SyncBatch.DeviceId -> Device.Id
- FormResponse.DeviceId -> Device.Id
- DocumentSignature.DeviceId -> Device.Id

Reason:

- Offline sync can produce records from not-yet-synced devices.
- Historical sync batches can reference revoked devices.
- Device lifecycle policy is not finalized.
- Enforcing DeviceId FKs too early could reject valid offline or historical records.

P2 explicitly blocks accidental strong DeviceId FKs through the FK baseline gate.

---

## 6. CI and local validation

The P2 baseline is protected by:

- EF model integrity tests.
- Delete behavior contract tests.
- FK package contract tests.
- FK inventory contract tests.
- SQL Server deployment baseline script.
- Database FK baseline gate.
- Snapshot-specific FK drift validation.

Relevant scripts:

- `scripts/verify-no-mojibake.ps1`
- `scripts/verify-database-foreign-key-baseline.ps1`
- `scripts/validate-database-deployment-baseline.ps1`

The Verify workflow runs the database deployment baseline metadata gate, which calls the FK baseline gate.

---

## 7. What the FK baseline gate validates

The FK baseline gate verifies:

- Every reviewed P2 FK exists in EF migration files.
- Every reviewed P2 FK exists in `database/migrations/sqlserver/0001_initial_create.sql`.
- Every reviewed P2 FK relationship exists in `CaritasDbContextModelSnapshot.cs`.
- Required relationships remain required in the snapshot.
- Optional relationships remain optional in the snapshot.
- Reviewed relationships use `DeleteBehavior.NoAction`.
- The snapshot validation is bounded to each dependent entity block.
- DeviceId relationships are not accidentally converted into strong FKs.
- Cascade and set-null behavior are blocked in SQL, migrations, and snapshot.

---

## 8. Required validation before future database PRs

Any PR that changes EF persistence, migrations, FK behavior, SQL deployment scripts, or data integrity rules must pass:

```powershell
powershell -ExecutionPolicy Bypass -File "scripts/verify-no-mojibake.ps1"
powershell -ExecutionPolicy Bypass -File "scripts/verify-database-foreign-key-baseline.ps1"
powershell -ExecutionPolicy Bypass -File "scripts/validate-database-deployment-baseline.ps1"

Set-Location "services/api-dotnet"
dotnet build "Caritas.Brigadas.sln" -warnaserror
dotnet test "Caritas.Brigadas.sln" -warnaserror --no-build

For migration-generating changes, the SQL Server baseline must be regenerated with the approved EF migration script workflow.

9. Operational warnings

Before applying these migrations to any real database with existing data:

Run orphan detection queries.
Identify rows that would violate new FKs.
Decide whether to repair, migrate, archive, or delete inconsistent records.
Back up the database.
Confirm rollback plan.
Apply migrations with a migration user, not the runtime application user.
Validate post-deployment FK constraints.
Validate application flows that create affected records.

Do not run automatic migrations at API startup.

10. Remaining post-P2 items

P2 does not close all data governance work. Remaining areas include:

Device lifecycle and offline sync policy.
Orphan detection SQL scripts.
Data cleanup playbook.
Retention policy for audit, AI logs, exports, and sync payloads.
Tenant boundary tests at repository/service layer.
SQL performance review after FK additions.
Index review after real query patterns exist.
Backup and restore drill evidence.
Seed idempotency and production seeding controls.
Migration dry-run process for staging.
11. Backend status after P2

After P2, the backend has:

Explicit relational integrity for core/security.
Explicit relational integrity for brigades.
Explicit relational integrity for clinical records.
Explicit relational integrity for forms/documents/sync, except deferred DeviceId relationships.
No cascade delete policy.
Contract tests around EF model and FK packages.
SQL Server baseline aligned with EF migrations.
CI gate to prevent FK and snapshot baseline drift.

P2 is complete when this document is merged and the Verify workflow remains green.