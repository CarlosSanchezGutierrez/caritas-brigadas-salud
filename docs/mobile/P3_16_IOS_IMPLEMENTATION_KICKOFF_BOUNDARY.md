# P3.16 iOS Implementation Kickoff Boundary

## Purpose

This document defines the iOS implementation kickoff boundary.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

iOS implementation kickoff status: BLOCKED_PENDING_REAL_EVIDENCE

## iOS implementation allowed scope

The iOS client may begin shell implementation for authenticated context, organization context, patient capture, privacy consent capture, encounter capture, local draft handling, offline outbox boundary, sync status handling, conflict response handling, and retry boundary.

## iOS implementation blocked scope

The iOS client must not write directly to SQL Server, bypass the API, bypass authorization, bypass audit trail creation, silently overwrite conflicts, sync without device id, sync without idempotency key, drop request id, drop correlation id, or drop organization id.

## iOS technical boundary

The iOS client must isolate API access through a single Swift API client boundary.

The iOS client must preserve API contract version, request id, correlation id, organization id, device id, idempotency key, client operation id, sync status, standard error envelope, and audit trail reference.

## iOS Definition of Ready

An iOS feature is ready to implement only when endpoint integration status, API contract version, request schema, response schema, standard error envelope, device id requirement, idempotency key requirement, offline sync rule, conflict rule, and evidence requirement are documented.

## iOS Definition of Done

An iOS feature is done only when the Swift API boundary, typed models, local state boundary, offline queue boundary, error envelope handling, sync status handling, conflict handling, and contract test evidence exist.

## P3.16 conclusion

iOS implementation may begin only through offline-first, idempotent, auditable, API-only boundaries.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
