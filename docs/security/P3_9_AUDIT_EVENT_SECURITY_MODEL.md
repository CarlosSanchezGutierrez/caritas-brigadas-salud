# P3.9 Audit Event Security Model

## Purpose

This document defines the security model for audit events and longitudinal history.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Security boundaries

Audit events must preserve security-relevant context without exposing unnecessary sensitive patient data.

Required boundaries:

- organization id boundary.
- user role boundary.
- patient data minimization.
- source system validation.
- device id tracking when applicable.
- request id and correlation id tracking.
- audit trail immutability policy.
- no secrets in repository.

## Authorization evidence

Every denied or privileged action must preserve:

- actor.
- action.
- entity.
- organization id.
- user role.
- result.
- reason.
- timestamp.
- correlation id.
- request id.
- source ip when available.
- audit trail reference.

## Audit integrity risks

| Risk | Control |
|---|---|
| Silent overwrite | correction event and before snapshot reference |
| Privileged unaudited change | mandatory audit trail for role and permission changes |
| Export without traceability | export audit event with filters and actor |
| Patient merge without governance | merge and deduplication audit event |
| Offline batch replay | idempotency key and controlled data injection audit |
| Sensitive data leakage in logs | metadata and references instead of raw patient data |
| Cross-organization access | organization id required in audit event |
| Device-origin ambiguity | device id required for mobile/offline source |

## Retention and access

Audit retention policy is pending institutional decision.

Minimum evidence fields to define later:

- retention period.
- owner.
- access role.
- export restrictions.
- review cadence.
- incident review procedure.
- legal hold procedure if applicable.

## Operational security evidence

Future evidence must prove:

- audit events cannot be written anonymously.
- audit events preserve actor and role.
- audit events preserve organization id.
- privileged changes are auditable.
- denied actions are auditable.
- correction events are auditable.
- audit trail cannot be bypassed by normal runtime.
- secrets are not committed.

## P3.9 conclusion

Audit security evidence remains required before backend closure.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE