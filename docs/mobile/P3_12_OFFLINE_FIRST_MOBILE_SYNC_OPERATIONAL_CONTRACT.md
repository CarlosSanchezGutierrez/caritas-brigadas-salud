# P3.12 Offline-First Mobile Sync Operational Contract

## Purpose

P3.12 defines the offline-first mobile synchronization contract for field operation on iOS, Android, and future mobile clients.

This phase does not implement the final mobile clients.

It defines the operational contract that mobile capture, local persistence, retry behavior, conflict resolution, auditability, privacy, and SQL Server synchronization must respect.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Backend freeze status: NOT_FROZEN_PENDING_ON_PREM_EVIDENCE_AND_CONTRACTS

## Core principle

SQL Server is the operational source of truth.

Mobile devices may capture data offline, but offline mobile storage is not the operational source of truth.

Offline records must synchronize through controlled API contracts, idempotency keys, audit trail references, and conflict resolution rules.

## Required offline-first capabilities

The mobile sync contract must support:

- offline capture.
- local draft state.
- local outbox.
- retry queue.
- idempotency key.
- sync attempt.
- sync status.
- conflict detection.
- conflict resolution.
- server acknowledgment.
- audit trail reference.
- correlation id.
- request id.
- device id.
- organization id.
- user role.
- patient timeline compatibility.
- consent timeline compatibility.
- encounter timeline compatibility.
- clinical timeline compatibility.

## Sync lifecycle

| Step | Description |
|---|---|
| capture | User captures data on device |
| validate locally | Client validates known offline rules |
| store locally | Record is stored in local draft/outbox state |
| assign idempotency key | Client assigns stable key |
| attempt sync | Client sends pending item to API |
| server validate | Backend validates authorization, schema, consent, organization id |
| server persist | Backend writes accepted record to SQL Server |
| server audit | Backend writes audit trail |
| acknowledge | Backend returns server id, version, and sync status |
| reconcile | Client updates local state from server acknowledgment |

## Required sync metadata

Every syncable operation must include:

- client operation id
- idempotency key
- device id
- actor
- user role
- organization id
- entity
- entity id when known
- operation type
- client captured at
- client last modified at
- sync attempt count
- correlation id
- request id
- local validation result
- server validation result
- sync status
- audit trail reference

## Sync statuses

Required statuses:

- draft
- pending sync
- syncing
- accepted
- rejected
- conflict
- quarantined
- retry scheduled
- permanently failed
- deleted locally after acknowledgment

## Guardrails

- No secrets in repository.
- No cloud dependency.
- No unaudited sync.
- No silent overwrite.
- No direct mobile write to SQL Server.
- No patient records may be stored on blockchain.
- No external AI dependency for sync.
- No offline record accepted without server validation.
- No cross-organization sync.
- No sync without device id.
- No sync without idempotency key.
- No sync without audit trail after server persistence.

## Relationship with previous phases

P3.12 depends on:

- P3.8 SQL Server on-prem operational evidence.
- P3.9 total auditability and longitudinal history.
- P3.10 operational and analytical pipelines.
- P3.11 KPI, dashboard, insight, and direction reporting catalog.

## P3.12 conclusion

Offline-first sync is required before API contract freeze for Web, iOS, and Android.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE