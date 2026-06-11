# P5.9.1 Patient Create Atomic Idempotency Backstop

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

P5.9.1 fixes the concurrency gap left by repository-level patient create idempotency.

P5.9 added a pre-insert lookup. P5.9.1 adds SQL Server unique filtered indexes and unique-violation replay handling so concurrent retries with the same idempotency identity cannot insert duplicate patients.

## Scope

- Adds unique filtered indexes for OrganizationId + IdempotencyKey.
- Adds unique filtered indexes for OrganizationId + ClientOperationId.
- Adds unique filtered indexes for OrganizationId + SourceBrigadeId + LocalPatientId.
- Keeps deleted rows out of idempotency uniqueness.
- Handles SQL Server unique errors 2601 and 2627 for idempotency indexes.
- Re-reads the existing patient after idempotency unique violation.
- Returns existing PatientSummaryDto for concurrent replay.
- Preserves non-idempotent patient folio conflict behavior.

## Boundary

P5.9.1 does not add a full offline sync processor, conflict queue, patient merge workflow, dashboard, analytics, or production readiness.

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