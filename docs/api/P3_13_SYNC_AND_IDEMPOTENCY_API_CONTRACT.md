# P3.13 Sync and Idempotency API Contract

## Purpose

This document defines sync and idempotency API behavior for offline-first clients.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Sync principle

Offline sync must pass through the API.

No direct mobile write to SQL Server is allowed.

SQL Server is the operational source of truth.

## Required sync request metadata

Every offline sync request must include:

- client operation id.
- idempotency key.
- device id.
- organization id.
- actor.
- user role.
- operation type.
- entity.
- client captured at.
- client last modified at.
- request id.
- correlation id.
- API version.

## Required sync response metadata

Every sync response must include:

- sync status.
- server acknowledgment id.
- server entity id when accepted.
- server version when accepted.
- rejected records when applicable.
- quarantine count when applicable.
- conflict id when applicable.
- validation result.
- retryable.
- request id.
- correlation id.
- audit trail reference when accepted.

## Idempotency behavior

Idempotency rules:

- same idempotency key with same payload may return prior acknowledgment.
- same idempotency key with different payload must return idempotency error or quarantine.
- duplicate server persistence is forbidden.
- replay detection must be auditable.
- idempotency key is required for offline-originated writes.

## Sync status behavior

Required sync statuses:

- draft.
- pending sync.
- syncing.
- accepted.
- rejected.
- conflict.
- quarantined.
- retry scheduled.
- permanently failed.
- deleted locally after acknowledgment.

## Conflict behavior

Conflict responses must include:

- conflict id.
- conflict type.
- conflict reason.
- recommended resolution.
- resolution owner.
- resolution status.
- request id.
- correlation id.
- audit trail reference when applicable.

## Rejected and quarantine behavior

Rejected records and quarantine must be visible in:

- sync response.
- data quality reporting.
- audit trail.
- evidence templates.
- operational review.

## P3.13 conclusion

Sync and idempotency API behavior must be explicit before mobile client implementation.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE