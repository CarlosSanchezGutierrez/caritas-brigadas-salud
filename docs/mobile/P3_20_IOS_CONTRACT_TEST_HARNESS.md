# P3.20 iOS Contract Test Harness

## Purpose

This document defines the iOS contract test harness expectations.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

iOS contract test harness status: BLOCKED_PENDING_REAL_EVIDENCE

## iOS harness scope

The iOS contract test harness must validate request schema, response schema, standard error envelope model, authentication requirement, authorization role, organization id, request id, correlation id, audit trail reference, device id, idempotency key, client operation id, sync status, server acknowledgment, conflict id, schema drift detection, and breaking change detection.

## iOS-specific required tests

Required tests:

- Swift API boundary preserves API contract version.
- Swift API boundary preserves request id.
- Swift API boundary preserves correlation id.
- Swift API boundary preserves organization id.
- mobile sync request preserves device id.
- offline write request preserves idempotency key.
- offline write request preserves client operation id.
- accepted sync response preserves server acknowledgment.
- sync response preserves sync status.
- conflict response preserves conflict id.
- accepted write preserves audit trail reference.

## iOS blocked behavior

The iOS harness must reject direct database access, undocumented endpoints, missing device id, missing idempotency key, missing client operation id, missing server acknowledgment, missing sync status, silent conflict overwrite, and local draft treated as server evidence.

## P3.20 conclusion

The iOS client must have contract test harness coverage before offline-first implementation relies on backend behavior.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
