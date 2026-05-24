# P5.1 Backend Surface Inventory Matrix

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Acceptance criteria

| Area | Evidence | Required for P5.1 merge | Production-closing |
|---|---|---:|---:|
| Repository revision | commit SHA captured | Yes | No |
| Project inventory | .sln and .csproj inventory captured | Yes | No |
| API project | API project path captured | Yes | No |
| Controllers | controller surface inventoried | Yes | No |
| Endpoints | MapGet, MapPost, MapPut, MapDelete, and route surface inventoried | Yes | No |
| Contracts | DTO, request, response, command, query files inventoried | Yes | No |
| Entities | domain and entity files inventoried | Yes | No |
| DbContext | DbContext surface inventoried | Yes | No |
| Migrations | migration surface inventoried | Yes | No |
| Services | service layer inventoried | Yes | No |
| Authorization | authorization surface inventoried | Yes | No |
| Audit | audit surface inventoried | Yes | No |
| Tests | test project and test file inventory captured | Yes | No |
| Domain coverage | patient, brigade, service, encounter, consent, longitudinal, offline, dashboards, analytics detected | Yes | No |
| Gap backlog | missing backend surfaces listed | Yes | No |

## Required future domains

| Domain | Required in final system | Can P5.1 close it? |
|---|---:|---:|
| Patient core | Yes | No |
| Brigade core | Yes | No |
| Clinical encounters | Yes | No |
| Consent and privacy | Yes | No |
| Longitudinal history | Yes | No |
| Offline-first synchronization | Yes | No |
| Dashboards | Yes | No |
| Analytics | Yes | No |
| Reports and exports | Yes | No |
| Institutional SQL Server readiness | Yes | No |
| Production observability | Yes | No |

## Rejection criteria

Reject P5.1 if:

- Evidence is fabricated.
- Secret values are printed.
- Real patient data is committed.
- Backend production readiness is approved.
- SQL Server blocker is hidden.
- Client direct SQL access is allowed.
- API bypass is allowed.
- Cloud is made mandatory.
- Offline-first is treated as optional.
- Longitudinal history is treated as optional.
- Dashboards or analytics are treated as optional.