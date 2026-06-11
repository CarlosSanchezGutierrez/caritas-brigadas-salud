# P5.9 Patient Create Idempotency

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

P5.9 prevents duplicate patient creation when an offline or unstable client retries the same create operation.

P5.4 persisted offline/source metadata. P5.9 now uses that metadata to make patient creation idempotent at the repository boundary.

## Scope

P5.9 adds or validates:

- Patient create checks existing patients before inserting a new one.
- IdempotencyKey is the primary replay key within the route organization.
- ClientOperationId is a secondary replay key within the route organization.
- LocalPatientId plus SourceBrigadeId is a fallback replay key within the route organization.
- Existing matching patients are returned as PatientSummaryDto instead of creating duplicates.
- Idempotency lookup is organization-scoped.
- Idempotency lookup ignores deleted patients.
- Existing folio conflict behavior remains for non-idempotent duplicate folios.
- P5.9 implementation documentation.
- P5.9 acceptance matrix.
- P5.9 runbook.
- P5.9 verifier.
- P5.9 contract tests.

## Required behavior

When a create request repeats the same IdempotencyKey for the same organization, the repository must return the existing patient.

When no IdempotencyKey is provided, the repository may use ClientOperationId.

When neither IdempotencyKey nor ClientOperationId is provided, the repository may use LocalPatientId plus SourceBrigadeId.

The repository must not use cross-organization idempotency.

The repository must not treat deleted patients as valid idempotent matches.

The repository must continue to reject genuine patient folio conflicts when the request is not an idempotent replay.

## Boundary

P5.9 does not close:

- Full offline sync processor behavior.
- Conflict resolution queues.
- Patient merge/deduplication workflows.
- Dashboards.
- Analytics.
- Production readiness.
- Database unique indexes.

P5.9 is repository-level idempotency for patient creation only.

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