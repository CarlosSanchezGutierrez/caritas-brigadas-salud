# P3.8 SQL Server On-Prem Deployment Baseline

## Purpose

This document defines the SQL Server on-premise deployment baseline.

SQL Server is the operational source of truth.

The application must use:

- ConnectionStrings__SqlServer

## Target model

| Area | Baseline |
|---|---|
| Database engine | SQL Server |
| Hosting model | SQL Server on-premise or institutional data center |
| Mandatory cloud dependency | None |
| Runtime connection key | ConnectionStrings__SqlServer |
| Evidence state | Pending real execution |

## Required environment metadata

- Environment name.
- SQL Server target reference.
- Instance reference.
- Database name.
- Responsible owner.
- Deployment date.
- Deployed commit SHA.

## Runtime identity

The app runtime user is used only by the backend API.

Required controls:

- least privilege.
- no sysadmin for runtime.
- no db_owner for runtime.
- No secrets in repository.
- No schema ownership.
- No migration execution.
- No backup and restore administration.

## Migration identity

The migration user is used only for migration execution during controlled deployment.

Required controls:

- Separate from app runtime user.
- Used only during deployment.
- Produces sanitized migration execution evidence.
- Tied to deployed commit SHA.

## Reporting identity

The read-only reporting user supports dashboards, reports, exports, and read models.

Required controls:

- Read-only access.
- No operational writes.
- No schema changes.
- No privilege grants.

## Backup/operator identity

The backup/operator user supports backup and restore.

Required controls:

- Not used by runtime.
- Not used by reporting.
- Produces backup and restore evidence.
- Produces restore validation evidence.

## Security restrictions

- No secrets in repository.
- No connection strings with credentials committed.
- no sysadmin for runtime.
- no db_owner for runtime.
- No patient data exported into evidence.
- No cloud-only operational path.

## Validation requirements

- migration execution.
- backup and restore.
- restore validation.
- health endpoint.
- smoke test.
- RPO.
- RTO.