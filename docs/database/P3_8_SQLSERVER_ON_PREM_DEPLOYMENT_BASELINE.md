# P3.8 SQL Server On-Prem Deployment Baseline

## Purpose

This document defines the deployment baseline for SQL Server on-premise or institutional data center operation.

SQL Server is the operational source of truth.

The application runtime must use:

- ConnectionStrings__SqlServer

## Target model

| Item | Baseline |
|---|---|
| Database engine | SQL Server |
| Hosting model | SQL Server on-premise or institutional data center |
| Runtime source of truth | SQL Server operational database |
| Mandatory cloud dependency | None |
| Connection key | ConnectionStrings__SqlServer |
| Secrets storage | Outside repository |
| Evidence state | Pending real execution |

## Environment references

Real environments must be referenced without exposing secrets.

Required environment metadata:

- Environment name.
- SQL Server target reference.
- Instance reference.
- Database name.
- Network path or institutional access path.
- Reverse proxy or gateway reference if applicable.
- Responsible owner.
- Deployment date.
- Deployed commit SHA.

## Database naming

Database names must be environment-specific and must not embed credentials or personal data.

Recommended pattern:

- CaritasBrigadas_Dev
- CaritasBrigadas_Test
- CaritasBrigadas_Staging
- CaritasBrigadas_Operations

Final names require Caritas/Tec infrastructure approval.

## Application runtime user

The app runtime user is the identity used by the backend API during normal execution.

Required controls:

- least privilege.
- no sysadmin for runtime.
- no db_owner for runtime.
- No secrets in repository.
- Access only to the application database.
- Permission scope limited to runtime operations.
- No schema ownership.
- No server-wide permissions.
- No backup administration permissions.
- No migration ownership.

## Migration user

The migration user is used only during controlled deployment windows.

Required controls:

- Dedicated migration user.
- Used for migration execution.
- Disabled or rotated according to institutional policy after deployment if required.
- Not used by the app runtime.
- Evidence must reference migration logs and deployed commit SHA.

## Read-only reporting user

The read-only reporting user supports dashboards, exports, internal reporting, and analytical views.

Required controls:

- Read-only access.
- No write access to operational tables.
- No schema modification.
- No privilege escalation.
- Access limited to approved views, read models, or reporting schemas where possible.

## Backup/operator user

The backup/operator user supports backup and restore activities.

Required controls:

- Backup and restore responsibilities separated from application runtime.
- No application runtime use.
- Access granted only according to institutional DBA policy.
- Evidence must include backup and restore references.
- restore validation must be recorded after test restore.

## Auditor/read-only security user

The auditor user supports audit reviews, security checks, and compliance review.

Required controls:

- Read-only access to audit trail and operational metadata required for review.
- No write access.
- No schema modification.
- No patient data export unless formally approved.
- No bypass of tenant or authorization boundaries.

## Migration execution baseline

Migration execution must include:

1. Confirm deployed commit SHA.
2. Confirm intended environment.
3. Confirm SQL Server target reference.
4. Confirm ConnectionStrings__SqlServer is configured outside the repository.
5. Run migration execution with migration user.
6. Capture sanitized migration log.
7. Validate migration history.
8. Run health endpoint.
9. Run smoke test.
10. Register blockers.

## Backup and restore baseline

Backup and restore evidence must include:

- Backup timestamp.
- Backup job reference.
- Responsible owner.
- Environment name.
- Database name.
- Backup location reference without credentials.
- Restore target reference.
- restore validation result.
- RPO.
- RTO.
- Known blockers.

## Collation and timezone baseline

Final values require institutional confirmation.

Recommended baseline fields to capture:

- SQL Server collation.
- Database collation.
- Timezone policy.
- UTC storage expectations.
- Local display timezone expectations.
- Date/time audit behavior.

## Security restrictions

- No secrets in repository.
- No connection strings with credentials committed.
- No application use of elevated database roles.
- no sysadmin for runtime.
- no db_owner for runtime.
- No patient data exported into evidence.
- No cloud-only operational path.
- No external provider receives patient data by default.

## Required evidence artifacts

| Artifact | Required content |
|---|---|
| Migration evidence | migration execution log, commit SHA, environment |
| Backup evidence | backup job reference, timestamp, owner |
| Restore evidence | restore validation, target, smoke test |
| Least privilege evidence | user matrix and permission review |
| Health evidence | health endpoint output |
| Smoke evidence | smoke test output |
| Blocker register | unresolved risks and owners |