# P5.9.1 Patient Create Atomic Idempotency Backstop Runbook

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Validation command

Run from repository root:

    powershell -ExecutionPolicy Bypass -File "scripts/verify-p5-09-1-patient-create-atomic-idempotency-backstop.ps1"

## Build command

Run from repository root:

    dotnet build "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Caritas.Brigadas.Infrastructure.csproj" --configuration Release /p:TreatWarningsAsErrors=true
    dotnet build "services/api-dotnet/src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj" --configuration Release /p:TreatWarningsAsErrors=true

## Test command

Run from repository root:

    dotnet test "services/api-dotnet/Caritas.Brigadas.sln" --configuration Release /p:TreatWarningsAsErrors=true