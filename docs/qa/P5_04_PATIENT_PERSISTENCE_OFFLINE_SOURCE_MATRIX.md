# P5.4 Patient Persistence Offline Source Matrix

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Acceptance criteria

| Area | Evidence | Required for P5.4 merge | Production-closing |
|---|---|---:|---:|
| Domain entity | Patient exposes offline/source fields | Yes | No |
| Domain behavior | Patient can update offline/source metadata | Yes | No |
| EF mapping | Patient mapping includes offline/source field lengths and indexes | Yes | No |
| Migration | Migration adds patient offline/source columns | Yes | No |
| Write repository | Patient creation persists offline/source metadata | Yes | No |
| Read repository | Patient summary projections expose offline/source metadata | Yes | No |
| Build | API project builds | Yes | No |
| Verifier | P5.4 verifier passes | Yes | No |

## Rejection criteria

Reject P5.4 if persistence is not represented in the entity, EF mapping, repositories, and migration surface.

Reject P5.4 if backend readiness authorization is granted, SQL Server blocker is hidden, client direct SQL access is allowed, API bypass is allowed, or cloud is made mandatory.