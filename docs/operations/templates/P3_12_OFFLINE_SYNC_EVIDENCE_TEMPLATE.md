# P3.12 Offline Sync Evidence Template

This template must be filled only with real sanitized evidence.

## Evidence metadata

| Field | Value |
|---|---|
| Environment name | TBD |
| Deployed commit SHA | TBD |
| Responsible owner | TBD |
| Date | TBD |
| Mobile client type | TBD |
| Status | TBD |
| Blockers | TBD |

## Device and actor metadata

| Field | Value |
|---|---|
| device id | TBD |
| test actor | TBD |
| user role | TBD |
| organization id | TBD |
| correlation id | TBD |
| request id | TBD |

## Outbox evidence

| Field | Value |
|---|---|
| outbox item id | TBD |
| client operation id | TBD |
| idempotency key | TBD |
| operation type | TBD |
| entity | TBD |
| local validation result | TBD |
| client captured at | TBD |
| sync status | TBD |
| sync attempt count | TBD |

## Server sync evidence

| Field | Value |
|---|---|
| server validation result | TBD |
| server acknowledgment id | TBD |
| server entity id | TBD |
| server version | TBD |
| audit trail reference | TBD |
| accepted records | TBD |
| rejected records | TBD |
| quarantine count | TBD |

## Conflict evidence

| Field | Value |
|---|---|
| conflict id | TBD |
| conflict type | TBD |
| conflict reason | TBD |
| recommended resolution | TBD |
| resolution owner | TBD |
| resolution status | TBD |
| before snapshot reference | TBD |
| after snapshot reference | TBD |
| correction event | TBD |
| audit trail reference | TBD |

## Required scenario checklist

| Scenario | Evidence reference | Status | Blockers |
|---|---|---|---|
| offline capture creates local draft | TBD | TBD | TBD |
| local draft becomes pending sync | TBD | TBD | TBD |
| pending sync sends idempotency key | TBD | TBD | TBD |
| server accepts valid operation | TBD | TBD | TBD |
| server returns acknowledgment | TBD | TBD | TBD |
| duplicate idempotency key does not duplicate server records | TBD | TBD | TBD |
| missing organization id is rejected | TBD | TBD | TBD |
| missing device id is rejected | TBD | TBD | TBD |
| unauthorized sync is rejected | TBD | TBD | TBD |
| stale record creates conflict | TBD | TBD | TBD |
| clinical correction creates correction event | TBD | TBD | TBD |
| rejected records are tracked | TBD | TBD | TBD |
| quarantine is tracked | TBD | TBD | TBD |
| accepted operation creates audit trail | TBD | TBD | TBD |
| client reconciles server acknowledgment | TBD | TBD | TBD |

## Final status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE