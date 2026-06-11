# P3.17 Android Workstream Backlog

## Purpose

This document defines the Android workstream backlog for field capture and offline-first mobile implementation planning.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Android workstream status: BLOCKED_PENDING_REAL_EVIDENCE

## Android backlog lanes

| Lane | Scope | Status |
|---|---|---|
| Android shell | Kotlin project shell navigation field workflow shell | blocked pending evidence |
| Kotlin API boundary | typed client request response models standard error envelope | blocked pending evidence |
| Auth context | authenticated context role-aware flow | blocked pending evidence |
| Organization context | organization id preservation scoped data | blocked pending evidence |
| Device identity | device id lifecycle and sync metadata | blocked pending evidence |
| Patient capture | local draft patient registration correction | blocked pending evidence |
| Consent capture | privacy consent local capture sync evidence | blocked pending evidence |
| Encounter capture | encounter draft validation timeline link | blocked pending evidence |
| Offline draft | local draft state boundary | blocked pending evidence |
| Offline outbox | idempotency key client operation id retry boundary | blocked pending evidence |
| Sync reconciliation | server acknowledgment sync status handling | blocked pending evidence |
| Conflict handling | explicit conflict response handling no silent overwrite | blocked pending evidence |

## Android blocked behavior

The Android workstream must not write directly to SQL Server, bypass the API, bypass authorization, bypass audit trail creation, sync without device id, sync without idempotency key, silently overwrite conflicts, drop request id, drop correlation id, or drop organization id.

## Android workstream evidence

Required evidence includes offline draft evidence, outbox evidence, idempotency key evidence, device id evidence, server acknowledgment evidence, conflict handling evidence, retry behavior evidence, and audit trail reference evidence.

## P3.17 conclusion

The Android workstream must start from Kotlin API boundary, local state boundary, offline outbox, sync, conflict handling, and evidence gates.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
