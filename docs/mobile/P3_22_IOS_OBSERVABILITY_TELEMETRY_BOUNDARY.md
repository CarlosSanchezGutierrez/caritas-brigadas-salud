# P3.22 iOS Observability Telemetry Boundary

## Purpose

This document defines the iOS observability telemetry boundary.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

iOS observability telemetry status: BLOCKED_PENDING_REAL_EVIDENCE

## iOS telemetry scope

The iOS client must capture support-safe diagnostic context for:

- environment name.
- API base URL reference.
- API contract version.
- endpoint id.
- request id.
- correlation id.
- organization id.
- authorization role.
- standard error envelope.
- audit trail reference.
- device id.
- idempotency key.
- client operation id.
- sync status.
- server acknowledgment.
- conflict id.
- offline mode toggle boundary.
- sync mode toggle boundary.
- contract test status.
- configuration test status.

## iOS event categories

Required iOS event categories:

- mobile startup event.
- local draft event.
- offline queue event.
- sync attempt event.
- sync accepted event.
- sync rejected event.
- sync conflict event.
- sync quarantine event.
- retry event.
- standard error envelope event.
- support diagnostic event.

## iOS blocked telemetry behavior

The iOS client must not log secrets, log real patient payloads, write directly to SQL Server, bypass the API, bypass authorization, bypass audit trail creation, sync without device id, sync without idempotency key, drop client operation id, drop server acknowledgment, silently overwrite conflicts, or treat local logs as server evidence.

## iOS evidence requirement

Required evidence includes device id evidence, idempotency key evidence, client operation id evidence, sync status evidence, server acknowledgment evidence, conflict id evidence, standard error envelope evidence, privacy-safe telemetry evidence, contract test evidence, and configuration test evidence.

## P3.22 conclusion

iOS observability must be centralized and privacy-safe before iOS offline-first implementation expands.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
