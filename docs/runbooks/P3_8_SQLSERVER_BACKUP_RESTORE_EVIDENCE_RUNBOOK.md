# P3.8 SQL Server Backup Restore Evidence Runbook

## Purpose

This runbook defines how to capture backup and restore evidence for SQL Server on-premise operation.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Required metadata

Each evidence record must include:

- Environment name.
- SQL Server target reference.
- Database name.
- Responsible owner.
- Date.
- Deployed commit SHA when applicable.
- Backup job reference.
- Restore target reference.
- RPO.
- RTO.
- restore validation result.
- health endpoint result after restore when applicable.
- smoke test result after restore when applicable.
- Blockers.

## Forbidden evidence content

Do not store:

- Credentials.
- Connection strings with secrets.
- Raw patient data.
- Raw database backups.
- Full database dumps.
- Unredacted logs.

No secrets in repository.

## Backup evidence procedure

1. Confirm environment name.
2. Confirm SQL Server on-premise target reference.
3. Confirm database name.
4. Confirm responsible owner.
5. Execute backup through approved institutional process.
6. Capture sanitized backup result.
7. Record RPO and RTO assumptions.
8. Record blockers.

## Restore evidence procedure

1. Select safe restore target.
2. Confirm target is not live operational database.
3. Restore from approved backup reference.
4. Capture sanitized restore result.
5. Run restore validation.
6. Run health endpoint validation if applicable.
7. Run smoke test validation if applicable.
8. Record RPO and RTO findings.
9. Record blockers.

## Audit trail

backup and restore evidence must preserve audit trail metadata:

- Who requested the operation.
- Who executed it.
- When it happened.
- What environment was involved.
- What validation was completed.
- What blockers remain.