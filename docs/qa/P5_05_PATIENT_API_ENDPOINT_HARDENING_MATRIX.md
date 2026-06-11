# P5.5 Patient API Endpoint Hardening Matrix

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Acceptance criteria

| Area | Required evidence | Required for P5.5 merge | Production-closing |
|---|---|---:|---:|
| Route surface | Patient controller exposes list, get, clinical-record, and create routes | Yes | No |
| Read authorization | Read endpoints require PatientsRead | Yes | No |
| Write authorization | Create endpoint requires PatientsWrite | Yes | No |
| Success responses | Read returns 200 and create returns 201 | Yes | No |
| Error responses | Controller documents 400, 404, 409, and 503 where applicable | Yes | No |
| Canonical creation response | Create uses CreatedAtAction to the GetById endpoint | Yes | No |
| Database boundary | Missing repository returns database_not_configured with 503 | Yes | No |
| API boundary | No direct mobile SQL Server write or API bypass is approved | Yes | No |
| Verifier | P5.5 verifier passes | Yes | No |
| Build | API project builds in Release | Yes | No |

## Rejection criteria

Reject P5.5 if any patient endpoint is removed, if authorization is weakened, if create no longer returns 201, if error handling is removed, if backend readiness authorization is granted, if SQL Server blocker is hidden, if clients are allowed to bypass the API, if direct mobile SQL Server writes are allowed, or if cloud is made mandatory.