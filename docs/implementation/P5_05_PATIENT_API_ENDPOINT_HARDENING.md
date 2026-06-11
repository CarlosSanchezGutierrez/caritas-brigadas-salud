# P5.5 Patient API Endpoint Hardening

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

P5.5 hardens the patient API endpoint contract after P5.4 patient persistence.

This PR focuses on the HTTP/API boundary, not new domain persistence.

## Endpoint surface

P5.5 validates the existing patient endpoint surface:

- GET /api/v1/organizations/{organizationId:guid}/patients
- GET /api/v1/organizations/{organizationId:guid}/patients/{patientId:guid}
- GET /api/v1/organizations/{organizationId:guid}/patients/{patientId:guid}/clinical-record
- POST /api/v1/organizations/{organizationId:guid}/patients

## Required behavior

The patient API boundary must preserve:

- PatientsRead authorization for read endpoints.
- PatientsWrite authorization for create endpoint.
- 200 OK for successful reads.
- 201 Created for successful creation.
- 400 Bad Request for domain validation errors.
- 404 Not Found for missing organization, missing patient, or organization mismatch.
- 409 Conflict for duplicate patient folio or equivalent write conflict.
- 503 Service Unavailable when database access is not configured.
- Correlation-aware ApiResponse and ApiErrorResponse payloads.
- API-only access: clients must not bypass the API to write directly to SQL Server.

## Hardening added

P5.5 replaces the literal create Location response with CreatedAtAction against GetByIdAsync. This keeps the 201 Created response linked to the actual canonical patient read endpoint instead of relying on a manually formatted URL string.

## Boundary

P5.5 does not close:

- Full idempotency enforcement.
- Offline sync processor behavior.
- Conflict resolution strategy.
- Organization-level authorization beyond the existing policy boundary.
- Longitudinal history linkage.
- Dashboarding.
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