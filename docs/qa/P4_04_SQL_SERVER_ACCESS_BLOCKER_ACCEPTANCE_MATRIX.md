# P4.4 SQL Server Access Blocker Acceptance Matrix

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Acceptance purpose

This matrix defines the evidence required to close the P1 database-config blocker discovered during P4.3.

## Current blocker

| Severity | Category | Evidence | Status | Owner | Remediation | Blocker |
|---|---|---|---|---|---|---|
| P1 | database-config | SQL Server configuration presence evidence | captured | data owner | configuration or SQL Server remediation | ConnectionStrings__SqlServer missing. |

## Required acceptance evidence

| Evidence item | Required | Owner group | Acceptance condition |
|---|---:|---|---|
| SQL Server host and instance | Yes | data owner | Host and instance are provided through a secure channel, never committed. |
| Database name | Yes | data owner | Database name is provided for the correct environment. |
| Authentication mode | Yes | security owner | Authentication path is approved. |
| Least privilege credential boundary | Yes | security owner | Credential has only required runtime permissions. |
| Migration permission boundary | Yes | technical owner | Migration access is approved separately from runtime access. |
| Network access method | Yes | operations owner | VPN, allowlist, firewall, or private access path is documented. |
| Backup and restore ownership | Yes | operations owner | Backup and restore owner is identified. |
| Data classification | Yes | privacy owner | Data privacy boundary is documented. |
| ConnectionStrings__SqlServer presence evidence | Yes | technical owner | P4.1 captures presence without printing the value. |
| Sanitized evidence only | Yes | security owner | Evidence contains no secrets. |
| P4.2 database-config P1 closure | Yes | compliance owner | Classifier reports zero P1 database-config blockers. |
| Backend readiness status preservation | Yes | compliance owner | Backend production readiness remains BLOCKED_PENDING_REAL_EVIDENCE. |

## Rejection criteria

Reject P4.4 closure if any of the following happens:

- Connection string value is committed.
- Secrets are printed in logs.
- A fake local-only connection string is used as institutional evidence.
- SQL Server ownership is not identified.
- Runtime and migration permission boundaries are mixed without approval.
- Backend readiness approval is granted prematurely.
- Mobile clients are allowed to write directly to SQL Server.
- Frontend clients are allowed to bypass the API.
- Cloud dependency is introduced as mandatory.
- Evidence is not tied to an accountable institutional owner.

## Closure rule

The P1 is closed only when a future P4.1 evidence package and P4.2 classification prove that the SQL Server access package exists, is sanitized, and removes the database-config P1 without approving backend production readiness.