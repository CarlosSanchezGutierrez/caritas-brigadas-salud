# P3.8 SQL Server Migration Execution Runbook

## Purpose

This runbook defines controlled migration execution for SQL Server on-premise operation.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Preconditions

Before migration execution:

- Confirm commit SHA.
- Confirm environment name.
- Confirm SQL Server target reference.
- Confirm database name.
- Confirm ConnectionStrings__SqlServer is configured outside repository.
- Confirm migration user is available through approved secret handling.
- Confirm No secrets in repository.
- Confirm backup and restore rollback path.
- Confirm responsible owner.
- Confirm audit trail expectations.

## Required roles

- app runtime user.
- migration user.
- read-only reporting user.
- backup/operator user.
- auditor/read-only security user.

The app runtime user must not execute migrations.

## Execution procedure

1. Confirm clean repository state.
2. Confirm target environment.
3. Confirm SQL Server target reference.
4. Confirm ConnectionStrings__SqlServer.
5. Execute pre-checks.
6. Confirm rollback path.
7. Run migration execution with migration user.
8. Capture sanitized migration output.
9. Validate migration history.
10. Run health endpoint.
11. Run smoke test.
12. Record evidence and blockers.

## Post-migration validation

Required validation:

- migration execution result.
- migration history check.
- health endpoint.
- smoke test.
- audit trail.
- least privilege remains intact.
- no sysadmin for runtime.
- no db_owner for runtime.