# P3.12 Mobile Sync Security and Privacy Model

## Purpose

This document defines security and privacy boundaries for offline-first mobile synchronization.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Security principle

Offline-first operation must not weaken backend security, auditability, consent, organization boundaries, or privacy.

## Required security controls

Required controls:

- No secrets in repository.
- No cloud dependency.
- No direct mobile write to SQL Server.
- server validation is authoritative.
- organization id is mandatory.
- user role is mandatory.
- device id is mandatory.
- idempotency key is mandatory.
- audit trail reference is mandatory after acceptance.
- correlation id is mandatory.
- request id is mandatory.
- local storage minimization is mandatory.
- offline retention policy is required.
- rejected records and quarantine must be tracked.

## Authentication and authorization

Mobile sync must preserve:

- actor.
- user role.
- organization id.
- device id.
- request id.
- correlation id.
- authorization decision.
- sync status.
- audit trail reference.

Expired or invalid authorization must not silently sync.

## Privacy controls

Offline mobile storage must prefer:

- minimum necessary data.
- local draft minimization.
- deletion after server acknowledgment when appropriate.
- no unrestricted patient-level local cache.
- no unnecessary raw clinical note persistence.
- no credentials stored in repository.
- no real patient data in test fixtures.

## Threat model

| Threat | Control |
|---|---|
| Lost device | local minimization, platform protection, remote policy where available |
| Duplicate submission | idempotency key |
| Unauthorized sync | server authorization and organization id |
| Cross-organization leakage | organization id and user role validation |
| Silent overwrite | conflict detection and correction event |
| Offline stale consent | consent version validation |
| Replay with changed payload | idempotency replay detection |
| Sensitive local data exposure | local storage minimization |
| Audit bypass | server audit trail after persistence |

## Evidence required later

Future evidence must prove:

- mobile sync rejects missing device id.
- mobile sync rejects missing organization id.
- mobile sync rejects missing idempotency key.
- mobile sync rejects unauthorized operation.
- mobile sync handles duplicate idempotency key.
- mobile sync creates audit trail on accepted persistence.
- mobile sync tracks rejected records.
- mobile sync tracks quarantine.
- mobile sync handles conflict without silent overwrite.

## Security conclusion

Offline-first mobile sync must remain governed by server-side validation and auditability.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE