# P5.5 Patient API Endpoint Hardening Runbook

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

Validate that patient API endpoints preserve the expected HTTP boundary after patient persistence was added in P5.4.

## Validation command

Run from repository root:

    powershell -ExecutionPolicy Bypass -File "scripts/verify-p5-05-patient-api-endpoint-hardening.ps1"

## Build command

Run from repository root:

    dotnet build "services/api-dotnet/src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj" --configuration Release /p:TreatWarningsAsErrors=true

## Expected patient routes

- GET /api/v1/organizations/{organizationId:guid}/patients
- GET /api/v1/organizations/{organizationId:guid}/patients/{patientId:guid}
- GET /api/v1/organizations/{organizationId:guid}/patients/{patientId:guid}/clinical-record
- POST /api/v1/organizations/{organizationId:guid}/patients

## Expected response boundary

- 200 OK for read success.
- 201 Created for create success.
- 400 Bad Request for domain validation errors.
- 404 Not Found for missing organization or patient.
- 409 Conflict for duplicate patient folio or write conflict.
- 503 Service Unavailable when database access is not configured.

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