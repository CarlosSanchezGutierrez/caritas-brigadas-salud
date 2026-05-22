# P3.8 SQL Server Least Privilege Matrix

## Purpose

This matrix defines minimum database privilege boundaries for SQL Server on-premise operation.

The goal is strict separation of runtime, deployment, reporting, backup, and audit responsibilities.

## Core rules

- least privilege is mandatory.
- No secrets in repository.
- no sysadmin for runtime.
- no db_owner for runtime.
- The app runtime user cannot run migrations.
- The app runtime user cannot manage backup and restore.
- The app runtime user cannot change schema ownership.
- The migration user cannot be reused as the app runtime user.
- Reporting access must be read-only.
- Audit access must be read-only unless an approved security workflow requires otherwise.

## Role matrix

| Identity | Purpose | Allowed | Forbidden | Evidence required |
|---|---|---|---|---|
| app runtime user | Normal backend API database access | Execute approved runtime queries, stored procedures if used, read/write only required application data | Server administration, schema ownership, migration execution, backup administration, unrestricted export | Sanitized permission review |
| migration user | Controlled schema deployment | Apply approved migration execution during deployment window | Normal app runtime, unrestricted reporting, patient export, long-running unattended use | Migration log and commit SHA |
| read-only reporting user | Dashboards, exports, read models, analytical review | Read approved views, reporting schemas, aggregate outputs | Writes, deletes, schema changes, privilege grants | Reporting permission review |
| backup/operator user | Database backup and restore operations | Perform backup and restore according to institutional DBA policy | App runtime, product API access, patient data browsing outside approved operation | Backup job and restore validation evidence |
| auditor/read-only security user | Security and audit review | Read audit trail, security metadata, selected operational metadata | Writes, deletes, schema changes, operational updates | Audit access review |

## app runtime user

Minimum expectations:

- Uses ConnectionStrings__SqlServer configured outside the repository.
- Has only permissions required for backend runtime.
- Cannot perform administrative SQL Server actions.
- Cannot own schema.
- Cannot grant permissions.
- Cannot bypass authorization or tenant boundaries.
- Cannot disable audit trail.
- Cannot directly manipulate migration history.

Required token: app runtime user.

## migration user

Minimum expectations:

- Used only for migration execution.
- Tied to deployment procedure.
- Produces sanitized migration evidence.
- Requires approval for use in institutional environments.
- Must not be embedded in source code.
- Must not be shared with app runtime.

Required token: migration user.

## read-only reporting user

Minimum expectations:

- Read-only reporting user must not write operational data.
- Access should prefer approved views or read models.
- Direct table access requires explicit approval.
- Exports must follow privacy and consent boundaries.

Required token: read-only reporting user.

## backup/operator user

Minimum expectations:

- Supports backup and restore.
- Executes restore validation in a safe target.
- Records RPO and RTO.
- Does not act as application runtime.
- Does not bypass privacy restrictions.

## auditor/read-only security user

Minimum expectations:

- Reads audit trail.
- Reads security evidence.
- Reviews operational metadata.
- Does not modify application records.
- Does not modify audit records.

## Prohibited operational patterns

- Runtime identity with server-wide administrative rights.
- Runtime identity with schema ownership.
- Runtime identity reused for migration execution.
- Migration credentials committed to source control.
- Reporting identity with write access.
- Backup identity used by the API.
- Audit identity able to modify audit trail.
- Patient data copied into local files for debugging.
- Secrets committed into scripts, documentation, tests, screenshots, or logs.

## Required evidence checklist

| Check | Required? | Evidence |
|---|---:|---|
| app runtime user permission review | Yes | Sanitized role query output |
| migration user permission review | Yes | Sanitized role query output |
| read-only reporting user permission review | Yes | Sanitized role query output |
| backup/operator user review | Yes | Sanitized DBA confirmation |
| auditor/read-only security user review | Yes | Sanitized role query output |
| no sysadmin for runtime | Yes | Sanitized permission proof |
| no db_owner for runtime | Yes | Sanitized permission proof |
| No secrets in repository | Yes | Repository scan and review |