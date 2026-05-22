# P3.18 Android API Client Scaffold

## Purpose

This document defines the Android API client scaffold governance.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Android API client scaffold status: BLOCKED_PENDING_REAL_EVIDENCE

## Android scaffold responsibilities

The Android API client scaffold must provide:

- Kotlin API client boundary.
- configuration boundary.
- API contract version propagation.
- endpoint id mapping.
- typed request model.
- typed response model.
- standard error envelope handler.
- authentication metadata boundary.
- authorization role metadata boundary.
- organization id propagation.
- request id propagation.
- correlation id propagation.
- device id propagation.
- idempotency key propagation.
- client operation id propagation.
- sync status handling.
- audit trail reference handling.
- retry boundary.
- timeout boundary.
- contract test boundary.

## Android scaffold blocked behavior

The Android API client scaffold must not write directly to SQL Server, bypass the API, call undocumented endpoints, bypass authorization, bypass audit trail creation, sync without device id, sync without idempotency key, silently overwrite conflicts, drop request id, drop correlation id, or drop organization id.

## Android scaffold evidence

Required evidence includes Kotlin typed model evidence, standard error envelope evidence, device id evidence, idempotency key evidence, client operation id evidence, sync status evidence, conflict handling evidence, retry evidence, audit trail reference evidence, and contract test evidence.

## P3.18 conclusion

The Android API client scaffold must centralize offline-first mobile API access before Android feature implementation expands.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
