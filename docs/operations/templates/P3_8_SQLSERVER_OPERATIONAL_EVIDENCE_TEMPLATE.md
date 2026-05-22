# P3.8 SQL Server Operational Evidence Template

> This template must be filled only with real, sanitized operational evidence.

## Environment

| Field | Value |
|---|---|
| Environment name | TBD |
| SQL Server target reference | TBD |
| Database name | TBD |
| SQL Server on-premise confirmation | TBD |
| Deployed commit SHA | TBD |
| Responsible | TBD |
| Date | TBD |
| Status | TBD |

## Runtime configuration

| Field | Value |
|---|---|
| Runtime connection key | ConnectionStrings__SqlServer |
| Secrets location | External to repository |
| No secrets in repository | TBD |
| App identity | app runtime user |
| Migration identity | migration user |
| Reporting identity | read-only reporting user |

## Migration evidence

| Field | Value |
|---|---|
| migration execution reference | TBD |
| Migration log reference | TBD |
| Migration history validation | TBD |
| Executed by | TBD |
| Date | TBD |
| Blockers | TBD |

## Backup evidence

| Field | Value |
|---|---|
| backup and restore owner | TBD |
| Backup evidence reference | TBD |
| Backup timestamp | TBD |
| Backup target reference | TBD |
| RPO | TBD |
| RTO | TBD |
| Blockers | TBD |

## Restore evidence

| Field | Value |
|---|---|
| Restore evidence reference | TBD |
| Restore target reference | TBD |
| restore validation result | TBD |
| Post-restore health endpoint | TBD |
| Post-restore smoke test | TBD |
| Blockers | TBD |

## Least privilege evidence

| Identity | Evidence reference | Status | Blockers |
|---|---|---|---|
| app runtime user | TBD | TBD | TBD |
| migration user | TBD | TBD | TBD |
| read-only reporting user | TBD | TBD | TBD |
| backup/operator user | TBD | TBD | TBD |
| auditor/read-only security user | TBD | TBD | TBD |

Required checks:

- least privilege.
- no sysadmin for runtime.
- no db_owner for runtime.

## Health evidence

| Field | Value |
|---|---|
| health endpoint evidence | TBD |
| Database dependency status | TBD |
| Sanitized output reference | TBD |
| Blockers | TBD |

## Smoke evidence

| Field | Value |
|---|---|
| smoke test evidence | TBD |
| API route set | TBD |
| Expected status codes | TBD |
| Actual status codes | TBD |
| Blockers | TBD |

## Controlled data injection evidence

| Field | Value |
|---|---|
| controlled data injection batch id | TBD |
| source system | TBD |
| source file/process reference | TBD |
| operator | TBD |
| validation status | TBD |
| accepted records | TBD |
| rejected records | TBD |
| idempotency key | TBD |
| quarantine | TBD |
| error details | TBD |
| traceability to domain records | TBD |
| audit trail | TBD |

## Final blocker summary

| Blocker | Owner | Severity | Next action |
|---|---|---|---|
| TBD | TBD | TBD | TBD |

## Final status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE