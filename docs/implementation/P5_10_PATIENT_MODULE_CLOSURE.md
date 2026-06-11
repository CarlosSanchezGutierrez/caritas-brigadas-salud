# P5.10 Patient Module Closure

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

P5.10 closes the backend patient module workstream at a controlled functional milestone.

This is not a new feature phase. It is an evidence and boundary closure phase that consolidates what was implemented from P5.3 through P5.9 and explicitly records what remains outside this patient module closure.

## Closure statement

The backend patient module is considered functionally closed for the current controlled milestone when the following evidence exists:

- Patient API contracts expose offline-first and source-traceability fields.
- Patient persistence stores offline/source metadata.
- Patient read responses project offline/source metadata.
- Patient API routes are hardened for list, get, clinical record, and create.
- Patient create responses use framework route generation instead of literal Location construction.
- Patient reads are organization-scoped.
- Patient create validation enforces organization and source brigade boundaries.
- Patient create validation requires at least one identity signal.
- Empty patient route identifiers are handled without unhandled server errors.
- Patient creation is mapped to clinical audit evidence.
- Patient clinical record exposes a longitudinal timeline.
- Patient creation is idempotent for offline/mobile retries.
- Patient idempotency has an atomic SQL Server backstop for concurrent retries.
- Verification scripts, runbooks, QA matrices, and implementation notes exist for the patient phases.

## Completed patient backend phases

### P5.3 Patient API contracts

P5.3 prepared the patient API contract surface for offline-first creation, source brigade traceability, and longitudinal readiness.

Evidence expected:

- CreatePatientRequest includes offline/source metadata.
- PatientSummaryDto exposes offline/source metadata.
- Patient contract readiness marker exists.
- P5.3 docs, matrix, runbook, and verifier exist.

### P5.4 Patient offline/source persistence

P5.4 persisted the patient offline/source fields introduced by the contracts.

Evidence expected:

- Patient domain entity stores source brigade, local patient id, client operation id, idempotency key, sync status, and data capture source.
- Entity Framework maps offline/source columns.
- Patient creation persists offline/source metadata.
- Patient read responses project offline/source metadata.
- P5.4 docs, matrix, runbook, and verifier exist.

### P5.5 Patient API endpoint hardening

P5.5 hardened the patient API route contract.

Evidence expected:

- Patient list, get, clinical-record, and create surfaces are validated.
- PatientsRead and PatientsWrite authorization boundaries are preserved.
- Documented response behavior remains in place.
- Patient create response uses framework route generation through CreatedAtAction.
- P5.5 docs, matrix, runbook, and verifier exist.

### P5.6 Patient validation and organization authorization

P5.6 enforced organization-scoped patient behavior.

Evidence expected:

- Patient GetById repository lookup is organization-scoped.
- Patient controller passes organization id to read operations.
- Create request null guard exists.
- Empty organization id validation exists.
- Minimum identity signal validation exists.
- Partial record reason validation exists.
- SourceBrigadeId belongs to the same organization.
- Empty route ids do not surface unhandled server errors.
- P5.6 docs, matrix, runbook, and verifier exist.

### P5.7 Patient creation audit evidence

P5.7 verified patient creation audit evidence through the clinical write audit surface.

Evidence expected:

- POST /api/v1/organizations/{organizationId:guid}/patients maps to AuditActionCodes.PatientCreate.
- Patient is the audit entity name.
- CreatedAtActionResult success is covered by the clinical audit filter.
- Entity id extraction from ApiResponse Data.Id is preserved.
- Organization id extraction from route or action data is preserved.
- P5.7 docs, matrix, runbook, and verifier exist.

### P5.8 Patient longitudinal timeline

P5.8 added a derived longitudinal timeline to the existing patient clinical record read model.

Evidence expected:

- PatientClinicalRecordDto exposes Timeline.
- PatientClinicalRecordTimelineEventDto exists.
- Timeline includes visits, encounters, vital signs, form responses, consent documents, referrals, and medication deliveries.
- Timeline preserves visits and encounters even when timestamps are unknown.
- Timeline summary fields exist.
- Existing clinical-record typed collections remain available.
- P5.8 docs, matrix, runbook, and verifier exist.

### P5.9 Patient create idempotency

P5.9 added repository-level and SQL Server-backed idempotency for patient creation.

Evidence expected:

- Patient create checks existing patients before inserting a new one.
- IdempotencyKey is the primary replay key within the route organization.
- ClientOperationId is a secondary replay key within the route organization.
- SourceBrigadeId plus LocalPatientId is a fallback replay identity.
- Existing PatientSummaryDto is returned for idempotent replays.
- SQL Server unique filtered indexes protect concurrent duplicate replays.
- Unique violations on idempotency indexes are converted into existing patient summaries.
- P5.9 docs, matrix, runbook, verifier, and contract tests exist.

## What is closed

P5.10 closes the patient module at the backend milestone level for:

- Patient contract readiness.
- Patient metadata persistence.
- Patient endpoint hardening.
- Patient organization scoping.
- Patient create validation.
- Patient creation audit mapping.
- Patient clinical longitudinal read model.
- Patient create idempotency under retry and concurrent retry conditions.
- Patient implementation evidence and QA documentation.

## What is not closed

P5.10 does not close:

- Backend production readiness.
- Institutional SQL Server deployment proof.
- Real environment migration execution.
- Load testing.
- Security penetration testing.
- Privacy/legal acceptance by Cáritas.
- Offline sync processor.
- Conflict resolution queues.
- Patient merge or deduplication workflows.
- Analytics and dashboards.
- Mobile app release readiness.
- Store publication readiness.
- Operational runbooks for real brigades.
- Production monitoring and alerting.

## Required guardrails

No backend production readiness approval.

No fabricated evidence.

No secrets in repository.

No committed real patient data.

No direct mobile write to SQL Server.

No client may bypass the API.

No cloud dependency.

SQL Server remains the operational source of truth.

Backend production readiness remains BLOCKED_PENDING_REAL_EVIDENCE.

## Final milestone status

Patient module backend controlled milestone: CLOSED_PENDING_REAL_ENVIRONMENT_EVIDENCE

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE