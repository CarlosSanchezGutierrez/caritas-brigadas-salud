# P5.4 Patient Persistence Offline Source

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

P5.4 persists the offline-first and source-traceability patient fields introduced in P5.3.

## Scope

P5.4 adds persistence support for:

- SourceBrigadeId.
- LocalPatientId.
- ClientOperationId.
- IdempotencyKey.
- SyncStatus.
- DataCaptureSource.

## Implementation

P5.4 updates:

- Patient domain entity.
- Patient EF mapping.
- Patient write repository.
- Patient read repository projections.
- Patient clinical record projection.
- EF migration surface.

## Boundary

P5.4 does not close full idempotency enforcement, conflict detection, offline sync processor behavior, dashboarding, analytics, institutional SQL Server access, or production readiness.

## Required next implementation

The next practical patient PRs must cover:

- P5.5 patient API endpoint hardening.
- P5.6 patient validation and organization authorization.
- P5.7 patient write audit proof.
- P5.8 longitudinal patient history linkage.
- P6 offline-first synchronization behavior.

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