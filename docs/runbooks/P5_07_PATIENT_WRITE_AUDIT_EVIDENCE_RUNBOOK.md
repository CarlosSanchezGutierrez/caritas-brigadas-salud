# P5.7 Patient Write Audit Evidence Runbook

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

Validate that successful patient creation is wired into the existing operational write audit surface.

## Validation command

Run from repository root:

    powershell -ExecutionPolicy Bypass -File "scripts/verify-p5-07-patient-write-audit-evidence.ps1"

## Build command

Run from repository root:

    dotnet build "services/api-dotnet/src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj" --configuration Release /p:TreatWarningsAsErrors=true

## Expected behavior

- POST /api/v1/organizations/{organizationId:guid}/patients maps to patients.create.
- Audit entity name is Patient.
- CreatedAtActionResult is considered a successful write response.
- The audit filter extracts the created patient id from Data.Id when present.
- The audit filter resolves organization id from route/action/result data.
- HttpAuditLogger persists through IAuditLogWriteRepository.
- Audit logging failures are warning-only and do not leak sensitive details.

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