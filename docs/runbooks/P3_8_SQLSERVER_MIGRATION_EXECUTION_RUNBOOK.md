# P3.8 SQL Server Migration Execution Runbook

## Purpose

This runbook defines controlled migration execution for SQL Server on-premise operation.

Migration execution must be auditable, repeatable, tied to a commit SHA, and validated through health endpoint and smoke test evidence.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Preconditions

Before migration execution:

- Confirm target branch and commit SHA.
- Confirm environment name.
- Confirm SQL Server on-premise target reference.
- Confirm database name.
- Confirm ConnectionStrings__SqlServer is configured outside repository.
- Confirm migration user is available through approved secret handling.
- Confirm No secrets in repository.
- Confirm backup point or rollback strategy.
- Confirm maintenance window if required.
- Confirm responsible owner.
- Confirm audit trail expectations.

## Required roles

- app runtime user: used only by backend runtime.
- migration user: used only for migration execution.
- backup/operator user: used for backup and restore.
- auditor/read-only security user: used for review.

The app runtime user must not execute migrations.

## Execution procedure

1. Fetch latest repository state.
2. Confirm deployed commit SHA.
3. Confirm clean working tree.
4. Confirm target environment.
5. Confirm SQL Server target reference.
6. Confirm ConnectionStrings__SqlServer configuration outside repository.
7. Execute pre-checks.
8. Execute backup or confirm approved rollback point.
9. Run migration execution with migration user.
10. Capture sanitized migration output.
11. Validate migration history.
12. Run health endpoint.
13. Run smoke test.
14. Record evidence references.
15. Record blockers.

## Pre-checks

Required pre-checks:

- Database reachable from authorized runtime context.
- Migration user can connect.
- Existing schema state understood.
- Migration history table readable.
- No pending unknown manual schema drift.
- Backup or rollback point exists.
- Application configuration uses ConnectionStrings__SqlServer.
- No secrets are printed to terminal logs.

## Rollback

Rollback must be defined before execution.

Acceptable rollback references:

- Restore from approved backup.
- Revert deployment and restore database state.
- Apply approved down migration only if explicitly validated.
- DBA-managed recovery path.

Rollback evidence must include:

- Trigger condition.
- Owner.
- Procedure reference.
- Validation after rollback.
- Blockers.

## Post-migration validation

Post-migration validation must include:

- Migration history check.
- Basic schema validation.
- health endpoint execution.
- smoke test execution.
- Audit log verification when applicable.
- Permission verification remains intact.
- No runtime elevation occurred.

## Evidence record

Migration evidence must include:

- Environment name.
- SQL Server target reference.
- Database name.
- Deployed commit SHA.
- Migration execution reference.
- Sanitized execution result.
- health endpoint evidence.
- smoke test evidence.
- Responsible owner.
- Date.
- Status.
- Blockers.

## Failure handling

If migration execution fails:

1. Stop.
2. Preserve sanitized logs.
3. Record blocker.
4. Record owner.
5. Execute rollback only through approved process.
6. Do not proceed with dependent phases.
7. Do not claim backend clearance.