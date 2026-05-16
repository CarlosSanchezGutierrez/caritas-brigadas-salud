# P3.5-05 Backup, Restore and Rollback Evidence Contract

## Current decision

Status: BLOCKED

Backup, restore and rollback readiness is not approved until real evidence exists for encrypted backups, restore testing, deployment rollback, migration recovery, SQL Server recovery, offline sync recovery, mobile recovery, and web admin recovery.

## Scope

This contract applies to:

- ASP.NET Core API.
- SQL Server.
- EF Core migrations.
- SQL Server backups.
- Deployment pipeline.
- Runtime configuration.
- Secrets and key rotation.
- Offline sync.
- iOS app.
- Android app.
- Web admin.
- Reports and exports.
- Audit logs.
- Incident response.

## Non-negotiable rule

A backup that has not been restored is only an assumption.

Production readiness requires restore evidence.

## Recovery ownership

| Area | Required owner | Current owner | Evidence |
|---|---:|---|---|
| SQL Server backup | Yes | PENDING | PENDING |
| SQL Server restore | Yes | PENDING | PENDING |
| API deployment rollback | Yes | PENDING | PENDING |
| Database migration rollback | Yes | PENDING | PENDING |
| Secret rotation recovery | Yes | PENDING | PENDING |
| Mobile lost-device recovery | Yes | PENDING | PENDING |
| Web admin export recovery | Yes | PENDING | PENDING |
| Incident communication | Yes | PENDING | PENDING |

## Backup evidence

| Evidence item | Required | Current status |
|---|---:|---|
| SQL Server backup policy | Yes | PENDING |
| Backup frequency | Yes | PENDING |
| Backup retention | Yes | PENDING |
| Backup encryption | Yes | PENDING |
| Backup storage location | Yes | PENDING |
| Backup access owner | Yes | PENDING |
| Backup failure alert owner | Yes | PENDING |
| Manual backup procedure | Yes | PENDING |
| Pre-migration backup procedure | Yes | PENDING |
| Pre-release backup procedure | Yes | PENDING |

## Restore evidence

| Evidence item | Required | Current status |
|---|---:|---|
| Restore runbook | Yes | PENDING |
| Restore test date | Yes | PENDING |
| Restore test owner | Yes | PENDING |
| Restore target environment | Yes | PENDING |
| Restored database integrity check | Yes | PENDING |
| API connectivity to restored database | Yes | PENDING |
| Data sampling validation | Yes | PENDING |
| Restore duration measurement | Yes | PENDING |
| Restore approval record | Yes | PENDING |

## RTO and RPO

| Metric | Required | Current value | Owner |
|---|---:|---|---|
| Recovery Time Objective | Yes | PENDING | PENDING |
| Recovery Point Objective | Yes | PENDING | PENDING |
| Maximum acceptable downtime | Yes | PENDING | PENDING |
| Maximum acceptable data loss | Yes | PENDING | PENDING |
| Pilot recovery threshold | Yes | PENDING | PENDING |
| Production recovery threshold | Yes | PENDING | PENDING |

## Deployment rollback

| Evidence item | Required | Current status |
|---|---:|---|
| API artifact rollback procedure | Yes | PENDING |
| Configuration rollback procedure | Yes | PENDING |
| Secret rollback or rotation procedure | Yes | PENDING |
| Health check after rollback | Yes | PENDING |
| Deployment smoke after rollback | Yes | PENDING |
| Rollback owner | Yes | PENDING |
| Rollback approval path | Yes | PENDING |
| Rollback evidence record | Yes | PENDING |

## Migration rollback

Production migrations must not run automatically at API startup.

| Evidence item | Required | Current status |
|---|---:|---|
| Migration script generated | Yes | PENDING |
| Migration script reviewed | Yes | PENDING |
| Pre-migration backup completed | Yes | PENDING |
| Migration execution owner | Yes | PENDING |
| Migration success validation | Yes | PENDING |
| Rollback script or explicit rollback decision | Yes | PENDING |
| Data-loss risk review | Yes | PENDING |
| Manual remediation path | Yes | PENDING |
| Migration history verification | Yes | PENDING |
| Post-migration smoke test | Yes | PENDING |

## SQL Server disaster recovery

| Scenario | Required response | Current status |
|---|---|---|
| SQL Server outage | Runbook and owner | PENDING |
| SQL Server credential failure | Rotation/recovery path | PENDING |
| Database corruption | Restore path | PENDING |
| Accidental data deletion | Restore/remediation decision | PENDING |
| Network path failure | Infrastructure escalation | PENDING |
| Storage failure | Infrastructure escalation | PENDING |
| Failed backup | Alert and remediation | PENDING |
| Failed restore | Escalation and fallback | PENDING |

## Offline sync recovery

| Scenario | Required response | Current status |
|---|---|---|
| Failed batch | Reprocess/reject policy | PENDING |
| Duplicate event retry | Idempotent response | PENDING |
| Cross-batch duplicate event | Conflict response | PENDING |
| Partially processed batch | Recovery rule | PENDING |
| Corrupted payload | Controlled rejection | PENDING |
| Conflict event | Manual or UI-assisted resolution | PENDING |
| Stuck pending event | Inspection/escalation policy | PENDING |
| Sync audit trace | Required | PENDING |

## Mobile recovery

| Scenario | Required response | Current status |
|---|---|---|
| Lost device | Remote revoke/wipe decision | PENDING |
| Stolen device | Remote revoke/wipe decision | PENDING |
| Local storage corruption | Reset/replay policy | PENDING |
| Offline queue replay after outage | Required | PENDING |
| App version rollback | Decision required | PENDING |
| Forced update | Decision required | PENDING |
| Minimum supported app version | Required | PENDING |
| Session revocation | Required | PENDING |

## Web admin recovery

| Scenario | Required response | Current status |
|---|---|---|
| Failed export | Retry/recovery policy | PENDING |
| Wrong export permissions | Permission rollback | PENDING |
| Report generation failure | Incident path | PENDING |
| Admin account compromise | Session revocation and rotation | PENDING |
| Audit log review | Required | PENDING |
| Configuration rollback | Required | PENDING |

## Evidence record template

Each recovery test must record:

| Field | Required |
|---|---:|
| Date | Yes |
| Environment | Yes |
| Owner | Yes |
| Procedure executed | Yes |
| Result | Yes |
| Duration | Yes |
| Failure notes | Yes |
| Corrective actions | Yes |
| Evidence link | Yes |
| Approval | Yes |

## Current readiness

| State | Value |
|---|---|
| Backup readiness | BLOCKED |
| Restore readiness | BLOCKED |
| Deployment rollback readiness | BLOCKED |
| Migration rollback readiness | BLOCKED |
| SQL Server disaster recovery readiness | BLOCKED |
| Offline sync recovery readiness | BLOCKED |
| Mobile recovery readiness | BLOCKED |
| Web admin recovery readiness | BLOCKED |
| Production recovery readiness | BLOCKED |

## Next required evidence

1. Define SQL Server backup owner.
2. Define backup frequency and retention.
3. Confirm backup encryption.
4. Execute restore test.
5. Measure restore duration.
6. Define RTO and RPO.
7. Define deployment rollback path.
8. Define migration rollback decision.
9. Validate API against restored database.
10. Validate offline sync recovery scenarios.