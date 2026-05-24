# P4.5 API Runtime and OpenAPI Evidence Runbook

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

This runbook explains how to collect API runtime and OpenAPI evidence for the P2 gaps discovered during P4.3.

## Correct API project path

Use this API project path:

services/api-dotnet/src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj

## Collector command without API startup

Use this when no API runtime is available:

& "scripts/p4/collect-p4-05-api-runtime-openapi-evidence.ps1"

## Collector command with explicit ApiBaseUrl

Use this when the API is already running:

& "scripts/p4/collect-p4-05-api-runtime-openapi-evidence.ps1" -ApiBaseUrl "https://localhost:7044"

## Collector command with local startup attempt

Use this when a local API startup attempt is appropriate:

& "scripts/p4/collect-p4-05-api-runtime-openapi-evidence.ps1" -StartLocalApi -ApiBaseUrl "https://localhost:7044"

## Expected health endpoint

/api/v1/health

## OpenAPI endpoint candidates

/swagger/v1/swagger.json
/openapi/v1.json
/openapi.json
/swagger.json

## Expected successful closure

To close api-runtime P2, the evidence must show a successful health endpoint response.

To close api-contract P2, the evidence must show a captured OpenAPI or Swagger artifact.

## Expected blocked result when SQL Server is unavailable

If SQL Server access is unavailable, the API may fail to start or fail runtime checks.

That is valid evidence.

Do not fake runtime readiness.

Reference P4.4 Real Environment SQL Server Access Blocker when SQL Server access is the reason runtime evidence cannot be completed.

## Guardrails

- No secrets in repository.
- No fabricated evidence.
- No backend production readiness approval.
- No direct mobile write to SQL Server.
- No client may bypass the API.
- SQL Server remains the operational source of truth.
- Backend production readiness remains BLOCKED_PENDING_REAL_EVIDENCE.