# P5.9 Patient Create Idempotency Runbook

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

Validate that patient creation is safe for offline/mobile retries.

## Validation command

Run from repository root:

    powershell -ExecutionPolicy Bypass -File "scripts/verify-p5-09-patient-create-idempotency.ps1"

## Build command

Run from repository root:

    dotnet build "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Caritas.Brigadas.Infrastructure.csproj" --configuration Release /p:TreatWarningsAsErrors=true
    dotnet build "services/api-dotnet/src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj" --configuration Release /p:TreatWarningsAsErrors=true

## Test command

Run from repository root:

    dotnet test "services/api-dotnet/Caritas.Brigadas.sln" --configuration Release /p:TreatWarningsAsErrors=true --no-restore

## Expected behavior

- Repeated IdempotencyKey returns the existing patient.
- Repeated ClientOperationId returns the existing patient.
- Repeated SourceBrigadeId plus LocalPatientId returns the existing patient.
- Idempotency is scoped to organization.
- Deleted patients are ignored.
- Non-idempotent duplicate folios continue to use conflict behavior.

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