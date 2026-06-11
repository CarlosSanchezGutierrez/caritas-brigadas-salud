# P5.2 Patient Core Readiness Matrix

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Acceptance criteria

| Area | Evidence | Required for P5.2 merge | Production-closing |
|---|---|---:|---:|
| Patient domain | patient domain files inventoried | Yes | No |
| Patient contracts | request and response contracts inventoried | Yes | No |
| Patient endpoints | patient controller or endpoint surface inventoried | Yes | No |
| Patient persistence | DbContext, DbSet, configuration, or migration surface inventoried | Yes | No |
| Patient validation | validation surface inventoried | Yes | No |
| Patient authorization | authorization and organization scoping surface inventoried | Yes | No |
| Patient audit | patient write audit surface inventoried | Yes | No |
| Patient tests | patient test surface inventoried | Yes | No |
| Offline readiness | idempotency and client operation fields checked | Yes | No |
| Longitudinal readiness | history and timeline linkage checked | Yes | No |
| Gap backlog | missing patient core implementation items listed | Yes | No |

## Patient core closure criteria for later PRs

Patient core is not complete until the backend has create patient endpoint, get patient endpoint, search patient endpoint, update patient endpoint, SQL Server persistence, organization access enforcement, audit trail for patient writes, validation, idempotency-safe create behavior, offline-first client operation support, OpenAPI contract, unit tests, and integration tests.

## Rejection criteria

Reject P5.2 if evidence is fabricated, patient core is declared complete without implementation evidence, backend readiness authorization is granted, SQL Server blocker is hidden, offline-first is treated as optional, longitudinal history is treated as optional, dashboards or analytics are treated as optional, client direct SQL access is allowed, API bypass is allowed, or cloud is made mandatory.