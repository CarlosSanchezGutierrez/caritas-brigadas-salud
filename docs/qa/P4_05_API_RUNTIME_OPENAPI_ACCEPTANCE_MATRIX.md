# P4.5 API Runtime and OpenAPI Acceptance Matrix

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Current P2 evidence gaps

| Severity | Category | Evidence | Status | Owner | Remediation | Blocker |
|---|---|---|---|---|---|---|
| P2 | api-runtime | API health check evidence | skipped_or_blocker_candidate | operations owner | API runtime remediation | ApiBaseUrl not provided. |
| P2 | api-contract | OpenAPI artifact evidence | skipped_or_blocker_candidate | technical owner | API contract remediation | No OpenAPI artifact found. |

## Required evidence to close api-runtime P2

| Evidence item | Required | Owner group | Acceptance condition |
|---|---:|---|---|
| API project path evidence | Yes | technical owner | The project path resolves to services/api-dotnet/src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj. |
| ApiBaseUrl evidence | Yes | operations owner | ApiBaseUrl is explicitly provided or discovered from a controlled local run. |
| API startup attempt evidence | Yes | operations owner | Startup attempt is logged with sanitized output. |
| API health endpoint evidence | Yes | operations owner | /api/v1/health returns a successful HTTP response. |
| Sanitized logs | Yes | security owner | Logs do not expose secrets or connection string values. |
| SQL dependency boundary | Yes | data owner | If startup fails due to SQL Server access, evidence references the P4.4 blocker. |

## Required evidence to close api-contract P2

| Evidence item | Required | Owner group | Acceptance condition |
|---|---:|---|---|
| OpenAPI endpoint attempt evidence | Yes | technical owner | At least one OpenAPI or Swagger endpoint is attempted. |
| OpenAPI artifact scan evidence | Yes | technical owner | Repository scan for openapi or swagger artifacts is captured. |
| Contract artifact evidence | Yes | technical owner | At least one OpenAPI or Swagger artifact is captured. |
| Contract failure blocker evidence | Conditional | technical owner | If no artifact exists, the blocker remains explicit and documented. |
| Sanitized evidence only | Yes | security owner | Contract evidence contains no secrets. |

## Rejection criteria

Reject P4.5 closure if any of the following happens:

- API runtime evidence is claimed without an actual health endpoint response.
- OpenAPI evidence is claimed without an artifact or endpoint response.
- SQL Server access blocker is hidden or bypassed.
- Secrets are printed in logs.
- Connection string values are committed.
- Backend readiness approval is granted prematurely.
- Mobile clients are allowed to write directly to SQL Server.
- Frontend clients are allowed to bypass the API.
- Cloud dependency is introduced as mandatory.
- Evidence is not reproducible from documented commands.

## Closure rule

P4.5 may close the P2 gaps only through real runtime or contract evidence.

If SQL Server access is still unavailable, P4.5 must preserve the P2 blockers and reference P4.4 rather than fabricating successful runtime evidence.