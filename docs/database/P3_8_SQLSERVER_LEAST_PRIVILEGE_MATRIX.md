# P3.8 SQL Server Least Privilege Matrix

## Purpose

This matrix defines least privilege boundaries for SQL Server on-premise operation.

## Core rules

- least privilege is mandatory.
- No secrets in repository.
- no sysadmin for runtime.
- no db_owner for runtime.
- The app runtime user cannot run migrations.
- The app runtime user cannot manage backup and restore.
- The migration user cannot be reused as the app runtime user.
- The read-only reporting user cannot write operational data.
- Audit access must protect the audit trail.

## Role matrix

| Identity | Purpose | Allowed | Forbidden |
|---|---|---|---|
| app runtime user | Backend API runtime | Approved runtime reads/writes | Server administration, schema ownership, migration execution, backup and restore |
| migration user | Controlled deployment | migration execution | Runtime API use, reporting use, unrestricted exports |
| read-only reporting user | Reporting and dashboards | Approved reads | Writes, deletes, schema changes |
| backup/operator user | backup and restore | Backup, restore, restore validation | Runtime API use |
| auditor/read-only security user | Security review | Read audit trail and approved metadata | Modify operational data or audit trail |

## Required evidence

- app runtime user permission review.
- migration user permission review.
- read-only reporting user permission review.
- backup/operator user review.
- auditor/read-only security user review.
- no sysadmin for runtime proof.
- no db_owner for runtime proof.
- No secrets in repository verification.