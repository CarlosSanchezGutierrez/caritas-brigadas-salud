# P5.10 Patient Module Closure Runbook

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

Validate the controlled patient backend milestone closure after P5.9.2.

## Validation command

Run from repository root:

    powershell -ExecutionPolicy Bypass -File "scripts/verify-p5-10-patient-module-closure.ps1"

## Build command

Run from repository root:

    dotnet build "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Caritas.Brigadas.Infrastructure.csproj" --configuration Release /p:TreatWarningsAsErrors=true
    dotnet build "services/api-dotnet/src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj" --configuration Release /p:TreatWarningsAsErrors=true

## Test command

Run from repository root:

    dotnet test "services/api-dotnet/Caritas.Brigadas.sln" --configuration Release /p:TreatWarningsAsErrors=true

## Required interpretation

P5.10 means the patient backend module is closed as a controlled implementation milestone.

P5.10 does not approve deployment to a real institutional environment.

P5.10 does not close the offline sync processor.

P5.10 does not close conflict resolution queues.

P5.10 does not close patient merge or deduplication.

P5.10 does not close mobile release readiness.

P5.10 does not close legal, privacy, security, monitoring, backup, restore, or operational approval.

## Expected result

- P5.10 verifier passes.
- Infrastructure build passes.
- API build passes.
- Full test suite passes.
- git diff --check passes.
- Patient module backend controlled milestone is documented as CLOSED_PENDING_REAL_ENVIRONMENT_EVIDENCE.
- Backend production readiness remains BLOCKED_PENDING_REAL_EVIDENCE.

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