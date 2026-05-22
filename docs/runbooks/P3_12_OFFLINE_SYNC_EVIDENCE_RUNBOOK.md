# P3.12 Offline Sync Evidence Runbook

## Purpose

This runbook defines how future offline-first sync evidence must be collected.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Evidence package metadata

Every evidence package must include:

- environment name.
- deployed commit SHA.
- responsible owner.
- date.
- mobile client type.
- device id.
- organization id.
- test actor.
- user role.
- client operation id.
- idempotency key.
- correlation id.
- request id.
- sync status.
- audit trail reference.
- status.
- blockers.

## Required evidence scenarios

Required scenarios:

1. offline capture creates local draft.
2. local draft becomes pending sync.
3. pending sync sends idempotency key.
4. server accepts valid operation.
5. server returns acknowledgment.
6. duplicate idempotency key does not duplicate server records.
7. missing organization id is rejected.
8. missing device id is rejected.
9. unauthorized sync is rejected.
10. stale record creates conflict.
11. clinical correction creates correction event.
12. rejected records are tracked.
13. quarantine is tracked.
14. accepted operation creates audit trail.
15. client reconciles server acknowledgment.

## Scenario validation

Each scenario must prove:

- device id.
- organization id.
- idempotency key.
- sync status.
- server validation result.
- rejected records when applicable.
- quarantine when applicable.
- conflict id when applicable.
- audit trail reference when accepted.
- correlation id.
- request id.

## Prohibited evidence content

Do not store:

- credentials.
- connection strings.
- secrets.
- real patient identifiers.
- raw clinical notes from real patients.
- unredacted screenshots.
- unrestricted local database dumps.
- mobile platform secrets.

No secrets in repository.

## Sanitized evidence allowed

Allowed evidence:

- synthetic patient identifiers.
- synthetic device id.
- test idempotency key.
- sanitized HTTP response.
- sanitized sync payload schema.
- sanitized outbox item.
- sanitized conflict event.
- sanitized audit event reference.
- aggregate counts.
- rejected records count.
- quarantine count.

## Failure handling

If evidence is incomplete:

1. Stop.
2. Record blocker.
3. Record missing scenario.
4. Record responsible owner.
5. Do not claim backend closure.
6. Do not proceed to API contract freeze as if offline sync were proven.

## P3.12 conclusion

Offline sync evidence must prove field resilience without bypassing backend governance.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE