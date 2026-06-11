# P5.8 Patient Longitudinal History Timeline Runbook

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

Validate that the existing clinical-record endpoint exposes a derived longitudinal timeline without replacing existing typed collections.

## Validation command

Run from repository root:

    powershell -ExecutionPolicy Bypass -File "scripts/verify-p5-08-patient-longitudinal-history-timeline.ps1"

## Build command

Run from repository root:

    dotnet build "services/api-dotnet/src/Caritas.Brigadas.Contracts/Caritas.Brigadas.Contracts.csproj" --configuration Release /p:TreatWarningsAsErrors=true
    dotnet build "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Caritas.Brigadas.Infrastructure.csproj" --configuration Release /p:TreatWarningsAsErrors=true
    dotnet build "services/api-dotnet/src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj" --configuration Release /p:TreatWarningsAsErrors=true

## Test command

Run from repository root:

    dotnet test "services/api-dotnet/Caritas.Brigadas.sln" --configuration Release /p:TreatWarningsAsErrors=true --no-restore

## Expected behavior

- Clinical record response includes Timeline.
- Timeline entries are derived from existing clinical record read model collections.
- Timeline entries with known timestamps are ordered newest first.
- Timeline preserves unknown-time visits and encounters instead of dropping them.
- Existing clinical record collections remain intact.
- Summary includes timeline count and first/last known timeline timestamps.

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