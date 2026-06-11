# P4.6 API Route Evidence Alignment Matrix

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Required alignment

| Evidence area | Correct implemented route | Acceptance condition |
|---|---|---|
| Health liveness | /health/live | Collector attempts /health/live when ApiBaseUrl is provided. |
| Health readiness | /health/ready | Collector attempts /health/ready when ApiBaseUrl is provided. |
| OpenAPI JSON | /openapi/v1/openapi.json | Collector attempts /openapi/v1/openapi.json when ApiBaseUrl is provided. |
| Swagger UI | /swagger | Collector attempts /swagger when ApiBaseUrl is provided. |

## Acceptance criteria

- P4.5 collector includes /health/live.
- P4.5 collector includes /health/ready.
- P4.5 collector includes /openapi/v1/openapi.json.
- P4.5 collector includes /swagger.
- P4.5 documentation aligns to implemented API evidence routes.
- P4.6 verifier passes from repo root.
- P4.6 verifier passes from scripts directory.
- P4.5 collector smoke run without API still succeeds.
- Backend production readiness remains BLOCKED_PENDING_REAL_EVIDENCE.

## Rejection criteria

Reject the alignment if any of the following occurs:

- Runtime evidence is fabricated.
- /health/live is omitted.
- /health/ready is omitted.
- /openapi/v1/openapi.json is omitted.
- /swagger is omitted.
- SQL Server dependency is hidden.
- Backend readiness approval is granted.
- A client is allowed to bypass the API.
- Cloud dependency is introduced as mandatory.