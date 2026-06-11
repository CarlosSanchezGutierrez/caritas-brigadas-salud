# P3.8 SQL Server Operational Evidence Template

This template must be filled only with real sanitized operational evidence.

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
| Migration history validation | TBD |
| health endpoint evidence | TBD |
| smoke test evidence | TBD |
| Blockers | TBD |

## Backup and restore evidence

| Field | Value |
|---|---|
| backup and restore owner | TBD |
| Backup evidence reference | TBD |
| Restore evidence reference | TBD |
| restore validation result | TBD |
| RPO | TBD |
| RTO | TBD |
| Blockers | TBD |

## Least privilege evidence

| Identity | Evidence reference | Status |
|---|---|---|
| app runtime user | TBD | TBD |
| migration user | TBD | TBD |
| read-only reporting user | TBD | TBD |
| backup/operator user | TBD | TBD |
| auditor/read-only security user | TBD | TBD |

Required checks:

- least privilege.
- no sysadmin for runtime.
- no db_owner for runtime.

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
| audit trail | TBD |

## Final status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE