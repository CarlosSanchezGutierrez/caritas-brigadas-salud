# P3.12 Offline Queue and Outbox Model

## Purpose

This document defines the offline queue and outbox model for mobile clients.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Outbox principle

The local outbox represents pending client operations.

The local outbox is temporary client state.

SQL Server is the operational source of truth.

## Outbox item required fields

Every outbox item must preserve:

- outbox item id
- client operation id
- idempotency key
- device id
- actor
- user role
- organization id
- operation type
- entity
- entity id when known
- local payload reference
- local validation result
- client captured at
- client last modified at
- sync status
- sync attempt count
- last sync attempt at
- next retry at
- last error code
- last error message
- correlation id
- request id
- server acknowledgment id when available
- audit trail reference when accepted

## Outbox operation types

| Operation type | Examples |
|---|---|
| create patient | New patient captured offline |
| update patient | Patient identity enrichment or correction |
| capture consent | Privacy notice and consent capture |
| create encounter | Medical or social service encounter |
| update encounter | Encounter correction or completion |
| capture clinical record | Vital signs, note, referral, medication |
| upload document reference | Local document metadata pending upload |
| controlled data injection item | Batch-originated item pending sync |

## Retry behavior

Retry must be controlled.

Required retry fields:

- sync attempt count.
- last sync attempt at.
- next retry at.
- retry reason.
- final failure reason.
- conflict status when applicable.

Retry must not create duplicate server records.

The idempotency key prevents duplicate persistence.

## Local validation

Local validation may prevent obvious errors, but server validation remains authoritative.

Local validation can check:

- required local fields.
- expected field formats.
- local consent presence.
- local entity relationships.
- device id.
- organization id.
- user role.

Server validation must still check:

- authorization.
- organization id.
- consent boundaries.
- schema.
- domain rules.
- idempotency key.
- conflict state.
- audit trail requirements.

## Local storage boundaries

Local storage must respect:

- minimum necessary data.
- encryption at rest when platform permits.
- no secrets in repository.
- device lock expectations.
- controlled retention.
- deletion after server acknowledgment when applicable.
- offline privacy risk.

## Outbox conclusion

The outbox protects field continuity without replacing server governance.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE