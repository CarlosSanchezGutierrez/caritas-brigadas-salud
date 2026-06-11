# P5.3 Patient Contract Hardening

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

P5.3 hardens the patient API contract for the real patient workflow.

This PR moves beyond readiness inventory by adding explicit patient contract fields required for offline-first creation, source brigade traceability, flexible identity capture, and later longitudinal patient history linkage.

## Scope

P5.3 updates patient contracts with:

- SourceBrigadeId.
- LocalPatientId.
- ClientOperationId.
- IdempotencyKey.
- SyncStatus.
- DataCaptureSource.

P5.3 also adds a contract readiness marker for:

- patient core required for final system.
- offline-first required for final system.
- longitudinal history required for final system.
- dashboards required for final system.
- analytics required for final system.

## Why this matters

A field brigades system cannot depend on perfect online connectivity.

Patient creation must support retry-safe and sync-safe client operations before the mobile and web clients are implemented.

## Boundary

P5.3 does not close persistence, migration, endpoint behavior, SQL Server access, or production readiness.

P5.3 prepares the contract layer so P5.4 and later PRs can implement persistence, endpoint behavior, idempotency enforcement, audit, and sync behavior without breaking clients.

## Required future implementation

The next practical patient PRs must cover:

- patient persistence mapping for the new contract fields when required.
- idempotency enforcement.
- source brigade linkage.
- patient write audit.
- organization access enforcement.
- offline-first conflict handling.
- longitudinal patient history linkage.
- patient endpoint tests.

## Guardrails

No backend production readiness approval.

No fabricated evidence.

No secrets in repository.

No committed real patient data.

No direct mobile write to SQL Server.

No client may bypass the API.

No cloud dependency.

SQL Server remains the operational source of truth.

Backend production readiness remains BLOCKED_PENDING_REAL_EVIDENCE.