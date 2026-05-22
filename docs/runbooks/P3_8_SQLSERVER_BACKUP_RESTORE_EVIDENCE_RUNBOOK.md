# P3.8 SQL Server Backup Restore Evidence Runbook

## Purpose

This runbook defines how to capture backup and restore evidence for SQL Server on-premise operation without exposing secrets, patient data, or institutional infrastructure details.

## Status

Evidence status: pending real execution.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Required metadata

Each backup and restore evidence record must include:

- Environment name.
- SQL Server target reference.
- Database name.
- Responsible owner.
- Date and time.
- Deployed commit SHA if application-dependent.
- Backup job reference.
- Restore target reference.
- RPO.
- RTO.
- restore validation result.
- health endpoint result after restore when applicable.
- smoke test result after restore when applicable.
- Blockers.

## What must not be captured

Do not store:

- Credentials.
- Connection strings with secrets.
- Raw patient data.
- Raw database backups.
- Full database dumps.
- Private network diagrams.
- Screenshots exposing server internals beyond approved references.
- Unredacted logs.

No secrets in repository.

## Acceptable evidence

Acceptable sanitized artifacts:

- Backup job identifier.
- Backup timestamp.
- Database name.
- Environment name.
- Sanitized SQL Agent job output.
- Sanitized DBA confirmation.
- Restore target reference.
- Restore completion status.
- Integrity validation summary if approved.
- Smoke test output without patient data.
- health endpoint output without secrets.
- Blocker register.

## Backup evidence procedure

1. Confirm environment name.
2. Confirm SQL Server on-premise target reference.
3. Confirm database name.
4. Confirm responsible owner.
5. Execute backup through approved DBA or institutional process.
6. Capture sanitized backup result.
7. Record backup location reference without credentials.
8. Record RPO and RTO assumptions.
9. Store evidence reference in the operational evidence template.
10. Record unresolved blockers.

## Restore evidence procedure

1. Select a safe restore target.
2. Confirm restore target is not a live operational database.
3. Restore from approved backup reference.
4. Capture sanitized restore log.
5. Run restore validation.
6. Run health endpoint validation if the backend can connect to the restored target.
7. Run smoke test validation if approved.
8. Record RPO and RTO findings.
9. Record blockers and owner.

## Restore validation

restore validation must prove:

- Database is accessible.
- Schema exists.
- Migration history is consistent.
- Core tables or approved views exist.
- Application connectivity works through ConnectionStrings__SqlServer where applicable.
- health endpoint returns expected status.
- smoke test returns expected status.
- No patient data was exposed in evidence.

## RPO and RTO

RPO and RTO are pending institutional decision.

The evidence template must record:

- Proposed RPO.
- Proposed RTO.
- Actual observed restore duration when tested.
- Owner of final approval.
- Known constraints.

## Audit trail requirement

Every backup and restore evidence package must preserve audit trail metadata:

- Who requested the operation.
- Who executed the operation.
- When it was executed.
- What environment was involved.
- What database was involved.
- What validation was completed.
- What blockers remain.

## Failure handling

If backup or restore fails:

1. Do not hide the failure.
2. Record sanitized error details.
3. Record owner.
4. Record next action.
5. Do not mark the backend as cleared.
6. Do not invent evidence.