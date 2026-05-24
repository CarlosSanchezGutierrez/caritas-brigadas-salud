# P4.5 API Runtime and OpenAPI Evidence Boundary

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

P4.5 addresses the two P2 optional evidence gaps discovered during the P4.3 classified evidence run.

The P4.3 real evidence package detected:

- P0 required blockers: 0
- P1 blocker candidates: 1
- P2 optional evidence gaps: 2
- PASS items: 12
- UNKNOWN items: 0

The P2 gaps are:

| Severity | Category | Evidence | Blocker |
|---|---|---|---|
| P2 | api-runtime | API health check evidence | ApiBaseUrl not provided. |
| P2 | api-contract | OpenAPI artifact evidence | No OpenAPI artifact found. |

## Decision

P4.5 does not claim that the API is production ready.

P4.5 does not require institutional SQL Server access.

P4.5 defines how to collect API runtime and OpenAPI evidence safely when a valid runtime is available.

If the API cannot start because SQL Server access is missing, that result is valid evidence and must remain classified under the existing P4.4 institutional SQL Server access blocker.

## Correct API project path

The API project path for this repository is:

services/api-dotnet/src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj

This replaces incorrect local assumptions such as:

src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj

## Evidence scope

P4.5 introduces a dedicated collector for:

1. API project path evidence.
2. API startup attempt evidence.
3. API health endpoint evidence.
4. OpenAPI endpoint attempt evidence.
5. Existing OpenAPI or Swagger artifact scan evidence.
6. Sanitized runtime logs.
7. Explicit blocker text when SQL Server access prevents runtime evidence.

## API health endpoint

The expected health endpoint is:

/api/v1/health

The collector accepts an explicit ApiBaseUrl, for example:

https://localhost:7044

## OpenAPI endpoint candidates

The collector may try the following contract endpoints when ApiBaseUrl is provided:

/swagger/v1/swagger.json
/openapi/v1.json
/openapi.json
/swagger.json

## Closure criteria for the P2 gaps

The api-runtime P2 can be closed only when a future evidence package captures a successful health endpoint response.

The api-contract P2 can be closed only when a future evidence package captures at least one OpenAPI or Swagger artifact or endpoint response.

If SQL Server access is still missing, P4.5 may document the dependency, but it must not fake API runtime or OpenAPI contract closure.

## Guardrails

- No secrets in repository.
- No fabricated evidence.
- No backend production readiness approval.
- No direct mobile write to SQL Server.
- No client may bypass the API.
- No cloud dependency.
- SQL Server remains the operational source of truth.
- Backend production readiness remains BLOCKED_PENDING_REAL_EVIDENCE.