# P4.6 API Route Evidence Alignment

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

P4.6 aligns the P4 API runtime and OpenAPI evidence collector with the real API routes implemented in the backend.

## Finding

The repository implementation exposes these actual runtime routes:

| Evidence type | Real route |
|---|---|
| API liveness | /health/live |
| API readiness | /health/ready |
| OpenAPI JSON | /openapi/v1/openapi.json |
| Swagger UI | /swagger |

## Decision

P4.6 updates the evidence collector and documentation to target the implemented routes.

P4.6 does not approve backend production readiness.

P4.6 does not require institutional SQL Server access.

P4.6 preserves the P4.4 institutional SQL Server access blocker because /health/ready may depend on database connectivity.

## Runtime interpretation

- /health/live can validate that the API process is running.
- /health/ready can validate readiness dependencies and may fail when SQL Server access is unavailable.
- /openapi/v1/openapi.json is the primary OpenAPI contract endpoint.
- /swagger is the development Swagger UI route.

## Guardrails

- No secrets in repository.
- No fabricated evidence.
- No backend production readiness approval.
- No direct mobile write to SQL Server.
- No client may bypass the API.
- No cloud dependency.
- SQL Server remains the operational source of truth.
- Backend production readiness remains BLOCKED_PENDING_REAL_EVIDENCE.