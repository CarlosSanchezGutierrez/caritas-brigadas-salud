# P5.6 Patient Validation and Organization Authorization

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

P5.6 strengthens the patient API and repository boundary after P5.5 endpoint hardening.

This increment focuses on validation and organization scoping. It does not add new persistence fields.

## Scope

P5.6 adds or validates:

- Organization-scoped patient lookup for GetById.
- Removal of broad patient lookup followed by post-query organization mismatch filtering.
- Explicit organization id validation for patient writes.
- Explicit create request null guard.
- Minimum identity signal validation before creating a patient.
- Partial-record reason validation before persistence.
- SourceBrigadeId validation against the same organization.
- P5.6 implementation documentation.
- P5.6 acceptance matrix.
- P5.6 runbook.
- P5.6 verifier.

## Required behavior

Patient reads must not return a record outside the route organization.

Patient creation must reject requests that have no meaningful identity signal.

Patient creation must reject empty organization ids.

Patient creation must reject empty SourceBrigadeId values.

Patient creation must reject SourceBrigadeId values that do not exist in the same organization.

Missing organization and source brigade scope violations remain NotFound-facing through the existing controller mapping.

Validation failures remain BadRequest-facing through DomainException handling.

## Boundary

P5.6 does not close:

- Full idempotency enforcement.
- Offline sync processor behavior.
- Conflict resolution strategy.
- Longitudinal history linkage.
- Audit proof.
- Dashboards.
- Analytics.
- Production readiness.

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