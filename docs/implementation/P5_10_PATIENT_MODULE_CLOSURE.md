# P5.10 Patient Module Closure

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

P5.10 closes the backend patient module as a controlled implementation milestone.

This closure consolidates patient work from P5.3 through P5.9.2. It does not approve production deployment.

## Closure statement

Patient module backend controlled milestone: CLOSED_PENDING_REAL_ENVIRONMENT_EVIDENCE

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Completed patient backend phases

### P5.3 Patient API contracts

P5.3 prepared patient API contracts for offline-first capture, source brigade traceability, and longitudinal readiness.

Evidence:

- CreatePatientRequest includes SourceBrigadeId.
- CreatePatientRequest includes LocalPatientId.
- CreatePatientRequest includes ClientOperationId.
- CreatePatientRequest includes IdempotencyKey.
- PatientSummaryDto includes SyncStatus.
- PatientSummaryDto includes DataCaptureSource.

### P5.4 Patient offline and source persistence

P5.4 persisted patient offline and source metadata.

Evidence:

- Patient stores source brigade metadata.
- Patient stores local patient id.
- Patient stores client operation id.
- Patient stores idempotency key.
- Patient stores sync status.
- Patient stores data capture source.
- Entity Framework maps those fields.

### P5.5 Patient API endpoint hardening

P5.5 hardened the patient API endpoint contract.

Evidence:

- Patient list route remains explicit.
- Patient get route remains explicit.
- Patient clinical-record route remains explicit.
- Patient create route remains explicit.
- PatientsRead and PatientsWrite authorization boundaries remain in place.
- Patient create response uses CreatedAtAction.

### P5.6 Patient validation and organization scoping

P5.6 enforced organization-scoped patient behavior.

Evidence:

- Patient reads are scoped to OrganizationId.
- Patient create validates organization context.
- Empty route ids do not create unhandled server errors.
- Minimum identity signal validation exists.
- Partial record reason validation exists.
- SourceBrigadeId organization validation exists.

### P5.7 Patient write audit evidence

P5.7 verified patient creation audit evidence.

Evidence:

- Patient create maps to AuditActionCodes.PatientCreate.
- Patient create audit uses the clinical write audit mapper.
- CreatedAtActionResult is covered by the clinical audit filter.
- Entity id extraction from ApiResponse data is preserved.
- Organization id extraction from route or action data is preserved.

### P5.8 Patient longitudinal timeline

P5.8 added a derived patient longitudinal timeline.

Evidence:

- PatientClinicalRecordDto exposes Timeline.
- PatientClinicalRecordTimelineEventDto exists.
- Timeline includes clinical record events.
- Timeline preserves visits and encounters with unknown timestamps.
- Timeline summary fields exist.
- Existing typed clinical-record collections are preserved.

### P5.9 Patient create idempotency

P5.9 added repository-level idempotency for patient creation.

Evidence:

- FindExistingIdempotentPatientAsync exists.
- IdempotencyKey replay returns existing patient.
- ClientOperationId replay returns existing patient.
- SourceBrigadeId plus LocalPatientId replay returns existing patient.
- Idempotency lookup is organization-scoped.
- Deleted patients are excluded from idempotency lookup.

### P5.9.1 Patient create atomic idempotency backstop

P5.9.1 closed the concurrency gap for patient create idempotency.

Evidence:

- PatientCreateIdempotencyUniqueIndexNames exists.
- IX_patients_OrganizationId_IdempotencyKey_UQ exists.
- IX_patients_OrganizationId_ClientOperationId_UQ exists.
- IX_patients_OrganizationId_SourceBrigadeId_LocalPatientId_UQ exists.
- SQL Server unique violations 2601 and 2627 are handled for idempotency races.
- Existing patient summary is returned for concurrent idempotent replay.

### P5.9.2 Patient idempotency violated-index replay

P5.9.2 fixed replay after SQL Server unique index violations.

Evidence:

- FindExistingIdempotentPatientForUniqueViolationAsync exists.
- GetPatientCreateIdempotencyUniqueIndexName exists.
- FindExistingPatientByIdempotencyKeyAsync exists.
- FindExistingPatientByClientOperationIdAsync exists.
- FindExistingPatientByLocalPatientIdAsync exists.
- Replay re-read uses the identity tied to the violated SQL Server unique index.
- The catch path does not use the generic prioritized idempotency lookup.

## What is closed

P5.10 closes the patient backend module controlled milestone for:

- Patient API contracts.
- Patient offline/source metadata persistence.
- Patient endpoint hardening.
- Patient organization scoping.
- Patient create validation.
- Patient creation audit evidence.
- Patient longitudinal read model.
- Patient create idempotency.
- Patient create atomic SQL Server idempotency backstop.
- Patient create violated-index replay behavior.
- Patient module implementation evidence package.

## What remains open

P5.10 does not close:

- Backend production deployment approval.
- Real environment SQL Server migration execution.
- Real Cáritas environment configuration.
- Privacy/legal acceptance.
- Security penetration testing.
- Load testing.
- Full offline sync processor.
- Conflict resolution queues.
- Patient merge or deduplication workflow.
- Mobile release readiness.
- Store release readiness.
- Monitoring and alerting.
- Backup and restore validation.
- Institutional operations approval.

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