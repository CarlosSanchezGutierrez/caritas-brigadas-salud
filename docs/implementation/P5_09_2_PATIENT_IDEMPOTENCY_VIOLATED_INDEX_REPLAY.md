# P5.9.2 Patient Idempotency Violated Index Replay

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

P5.9.2 fixes the replay path used after a SQL Server idempotency unique index violation.

P5.9.1 added the atomic SQL Server backstop. P5.9.2 ensures that when a unique idempotency index is violated, the repository re-reads by the exact idempotency identity tied to the violated index instead of reusing the generic prioritized lookup.

## Required behavior

If IX_patients_OrganizationId_IdempotencyKey_UQ is violated, replay must re-read by IdempotencyKey.

If IX_patients_OrganizationId_ClientOperationId_UQ is violated, replay must re-read by ClientOperationId.

If IX_patients_OrganizationId_SourceBrigadeId_LocalPatientId_UQ is violated, replay must re-read by SourceBrigadeId and LocalPatientId.

The catch path must not re-read through the generic prioritized FindExistingIdempotentPatientAsync method.

## Boundary

P5.9.2 does not add a full offline sync processor, conflict queue, patient merge workflow, dashboards, analytics, or production readiness.

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