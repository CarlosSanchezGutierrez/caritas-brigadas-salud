# P3.15 Android Client Readiness Baseline

## Purpose

This document defines Android client readiness expectations for mobile field capture and offline sync.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Android client readiness status: BLOCKED_PENDING_REAL_EVIDENCE

## Android client allowed scope

The Android client may plan against health check, identity context, organization context, patient registration, privacy consent capture, encounter capture, clinical timeline capture, offline local draft, offline outbox, sync reconciliation, and conflict response handling.

## Android client blocked scope

The Android client must not write directly to SQL Server, bypass the API, bypass server validation, bypass audit trail creation, silently overwrite conflicts, sync without device id, sync without idempotency key, accept conflict response as success, drop request id, drop correlation id, or drop organization id.

## Android client required metadata

The Android client must preserve API contract version, request id, correlation id, organization id, device id, idempotency key, client operation id, sync status, standard error envelope, and audit trail reference.

## Android evidence needed

Required evidence includes offline draft creation evidence, local outbox evidence, idempotency key evidence, device id evidence, server acknowledgment evidence, conflict handling evidence, retry behavior evidence, and audit trail reference evidence.

## P3.15 conclusion

Android integration must remain offline-first, auditable, idempotent, and API-only.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
