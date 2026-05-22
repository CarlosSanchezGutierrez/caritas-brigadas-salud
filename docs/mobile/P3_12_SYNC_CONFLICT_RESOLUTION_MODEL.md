# P3.12 Sync Conflict Resolution Model

## Purpose

This document defines conflict detection and conflict resolution rules for offline-first synchronization.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Conflict principle

A conflict must never be silently overwritten.

No silent overwrite is allowed.

A conflict must become an explicit sync state with audit trail evidence.

## Conflict causes

Possible conflict causes:

- server record changed after client captured data.
- same patient updated by multiple devices.
- consent version changed before sync.
- encounter was closed before offline update arrived.
- organization id mismatch.
- duplicate patient candidate detected.
- idempotency key replay with different payload.
- clinical correction requires explicit reason.
- patient merge occurred before sync.
- deleted or deactivated record referenced by offline item.

## Conflict detection fields

Every conflict must preserve:

- conflict id
- outbox item id
- client operation id
- idempotency key
- device id
- actor
- organization id
- entity
- entity id
- operation type
- client captured at
- server version
- client version
- conflict type
- conflict reason
- recommended resolution
- resolution owner
- resolution status
- audit trail reference

## Conflict resolution statuses

Required statuses:

- conflict detected
- pending review
- client retry required
- server wins
- client correction required
- merge required
- rejected
- accepted with correction
- resolved

## Resolution strategies

| Strategy | Use case |
|---|---|
| server wins | Server has newer authoritative state |
| client correction required | User must correct offline payload |
| merge required | Patient identity or duplicate candidate review |
| accepted with correction | Server accepts after explicit correction event |
| rejected | Payload violates validation, consent, authorization, or organization boundary |
| quarantine | Payload requires manual review |

## Clinical conflict rules

Clinical conflicts require extra caution.

Required rules:

- clinical correction must preserve reason.
- before snapshot reference is required.
- after snapshot reference is required.
- correction event is required.
- audit trail reference is required.
- actor and user role are required.
- organization id is required.

## Consent conflict rules

Consent conflicts require:

- consent version.
- privacy notice version.
- captured at timestamp.
- actor.
- patient reference.
- organization id.
- audit trail reference.
- rejection or correction when stale or invalid.

## Idempotency conflict rules

If the same idempotency key is replayed:

- same payload may return prior acknowledgment.
- different payload must be rejected or quarantined.
- audit trail must show replay detection.
- duplicate server persistence is forbidden.

## Conflict conclusion

Conflicts are first-class sync events and must be auditable.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE