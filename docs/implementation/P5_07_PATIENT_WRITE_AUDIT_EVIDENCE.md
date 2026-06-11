# P5.7 Patient Write Audit Evidence

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

P5.7 connects patient creation to the existing operational write audit surface.

The repository already has a general operational write audit filter, audit logger contract, HTTP audit logger implementation, audit action codes, and audit log write repository boundary. P5.7 makes patient creation explicitly auditable through that existing path.

## Scope

P5.7 adds or validates:

- POST /api/v1/organizations/{organizationId:guid}/patients is mapped to patients.create.
- Patient creation audit uses AuditActionCodes.PatientCreate.
- Patient creation audit uses entity name Patient.
- Successful CreatedAtAction results remain auditable as successful 201 responses.
- Patient entity id is extracted from ApiResponse Data.Id by the operational write audit filter.
- Organization id is extracted from route/action/result data by the operational write audit filter.
- Existing audit logger path remains responsible for correlation id, user id, ip address, user agent, and occurred timestamp.
- P5.7 implementation documentation.
- P5.7 acceptance matrix.
- P5.7 runbook.
- P5.7 verifier.

## Required behavior

Successful patient creation must produce an operational write audit event through the existing audit pipeline.

The audit action must be patients.create.

The audited entity name must be Patient.

The audited entity id must be the created patient id when available from the response.

The audited organization id must remain scoped to the route organization.

Audit logging failures must not expose sensitive metadata in application logs.

## Boundary

P5.7 does not close:

- Full idempotency enforcement.
- Offline sync processor behavior.
- Conflict resolution strategy.
- Longitudinal history linkage.
- Patient merge or deduplication.
- Dashboards.
- Analytics.
- Production readiness.

P5.7 does not add a new audit table or alternate audit runtime. It uses the existing operational write audit surface.

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