# P5.3 Patient Contract Hardening

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

P5.3 hardens the existing patient API contract for offline-first patient creation, source brigade traceability, and longitudinal patient history readiness.

## Existing backend surface

The backend already contains:

- Patient domain entity.
- PatientsController.
- Patient read repository.
- Patient write repository.
- CreatePatientRequest.
- PatientSummaryDto.
- Patient DbSet and EF configuration.

## Scope

P5.3 adds explicit patient contract fields for:

- SourceBrigadeId.
- LocalPatientId.
- ClientOperationId.
- IdempotencyKey.
- SyncStatus.
- DataCaptureSource.

P5.3 also adds PatientContractReadiness as a contract-level marker for final system requirements.

## Boundary

P5.3 does not close persistence, migration, endpoint behavior, idempotency enforcement, SQL Server access, offline sync, dashboards, analytics, longitudinal history, or production readiness.

## Required next implementation

The next practical patient PRs must cover:

- persistence mapping for new offline/source fields.
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