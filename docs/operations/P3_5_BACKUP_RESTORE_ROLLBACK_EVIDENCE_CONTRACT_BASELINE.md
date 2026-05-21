# P3.5-05 Backup, Restore and Rollback Evidence Contract Baseline

## Status

Required before staging, pilot, production, App Store, Play Store, or web admin production release.

This document is not a production approval.

## Purpose

Define the required evidence for backup, restore, deployment rollback, database rollback, migration rollback, disaster recovery, and operational recovery for Caritas Brigadas de Salud.

## Core rule

A system is not production-ready until recovery has been tested.

Documentation without a restore test is not recovery evidence.

Required production model:

- Backups exist.
- Backups are encrypted.
- Backups are restorable.
- Restore is tested.
- Restore owner is defined.
- Rollback path exists.
- Rollback is tested.
- Migration rollback is reviewed.
- Deployment rollback is documented.
- RTO is defined.
- RPO is defined.
- Evidence is recorded.

## Required recovery concepts

Production must define:

- Backup frequency.
- Backup retention.
- Backup encryption.
- Backup storage location.
- Backup access owner.
- Restore procedure.
- Restore test date.
- Restore test owner.
- Recovery Time Objective.
- Recovery Point Objective.
- Deployment rollback procedure.
- Migration rollback procedure.
- Failed migration response.
- Failed deployment response.
- SQL Server outage response.
- Accidental deletion response.
- Corrupted sync batch response.
- Secrets rotation recovery.
- Key loss recovery decision.

## Backup requirements

Required before production:

- SQL Server backup policy.
- Backup encryption evidence.
- Backup retention policy.
- Backup storage access policy.
- Backup monitoring or review process.
- Backup failure alert owner.
- Manual backup procedure.
- Pre-migration backup procedure.
- Pre-release backup procedure.
- Post-release verification procedure.

## Restore requirements

Required before production:

- Restore runbook.
- Restore test evidence.
- Restore test environment.
- Restore validation checklist.
- Restored database integrity check.
- Application connectivity check against restored database.
- Data sampling validation.
- Restore duration measurement.
- Restore owner.
- Restore approval record.

## Rollback requirements

Required before production:

- API deployment rollback procedure.
- Configuration rollback procedure.
- Database migration rollback decision.
- Feature flag rollback decision.
- Secrets rollback or rotation process.
- Mobile compatibility rollback decision.
- Web admin rollback decision.
- Incident communication path.
- Rollback owner.
- Rollback evidence record.

## Migration rollback requirements

Migrations must not run automatically at API startup in production.

Required migration recovery evidence:

- Generated migration script.
- Reviewed migration script.
- Pre-migration backup.
- Migration execution owner.
- Migration success validation.
- Rollback script or explicit rollback decision.
- Data-loss risk review.
- Manual remediation path.
- Migration history verification.
- Post-migration smoke test.

## SQL Server disaster recovery requirements

Production must define:

- SQL Server owner.
- SQL Server backup operator.
- SQL Server restore operator.
- SQL Server outage contact.
- Data center or lab contact.
- Infrastructure escalation path.
- Network recovery path.
- Storage failure path.
- Database corruption path.
- Credential failure path.

## Sync and offline recovery requirements

Offline sync introduces additional recovery risks.

Required evidence:

- Failed batch recovery.
- Duplicate event retry behavior.
- Idempotency replay behavior.
- Conflict recovery.
- Partially processed batch behavior.
- Corrupted payload response.
- Dead-letter or rejected-event policy.
- Manual event inspection policy.
- Audit traceability for recovery actions.

## Mobile recovery requirements

iOS and Android must define:

- Lost device procedure.
- Remote revoke procedure.
- Local data wipe procedure.
- Offline queue retention.
- Offline queue replay after outage.
- App version rollback decision.
- Minimum supported app version.
- Forced update decision.
- Local storage corruption response.

## Web admin recovery requirements

Web admin must define:

- Failed export recovery.
- Export deletion recovery decision.
- Report generation failure response.
- Admin session revocation.
- Permission rollback.
- Configuration rollback.
- Audit trail retention.

## Evidence package requirements

Recovery evidence must include:

- Date.
- Environment.
- Owner.
- Procedure executed.
- Result.
- Duration.
- Failure notes.
- Corrective actions.
- Evidence link.
- Approval.

## Production readiness states

- BLOCKED.
- READY FOR STAGING RECOVERY.
- READY FOR PILOT RECOVERY.
- READY FOR PRODUCTION RECOVERY.

Default state is BLOCKED.