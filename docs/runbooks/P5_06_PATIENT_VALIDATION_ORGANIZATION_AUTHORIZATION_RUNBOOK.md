# P5.6 Patient Validation and Organization Authorization Runbook

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

Validate organization-scoped patient reads and create-time validation.

## Validation command

Run from repository root:

    powershell -ExecutionPolicy Bypass -File "scripts/verify-p5-06-patient-validation-organization-authorization.ps1"

## Build command

Run from repository root:

    dotnet build "services/api-dotnet/src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj" --configuration Release /p:TreatWarningsAsErrors=true

## Expected behavior

- Patient GetById lookup is organization-scoped at repository query level.
- Empty route IDs return NotFound through the controller path instead of throwing unhandled repository exceptions.
- Controller passes organizationId into GetByIdAsync.
- Create validates organization id.
- Create validates request is not null.
- Create requires at least one identity signal.
- Create validates partial record reason.
- Create validates SourceBrigadeId belongs to the same organization.

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