# P3.17 Web Workstream Backlog

## Purpose

This document defines the Web workstream backlog for implementation planning.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Web workstream status: BLOCKED_PENDING_REAL_EVIDENCE

## Web backlog lanes

| Lane | Scope | Status |
|---|---|---|
| Web shell | routing layout authenticated navigation | blocked pending evidence |
| API boundary | typed API client standard error envelope metadata preservation | blocked pending evidence |
| Auth and role UI | authenticated context role-sensitive screens | blocked pending evidence |
| Organization context | organization id preservation scoped data UI | blocked pending evidence |
| Brigade admin | brigade setup service availability closure flow | blocked pending evidence |
| Patient workflows | registration correction timeline views | blocked pending evidence |
| Consent workflows | privacy consent capture review evidence | blocked pending evidence |
| Encounter workflows | encounter capture correction timeline review | blocked pending evidence |
| Dashboards | KPI dashboard datasets metric lineage | blocked pending evidence |
| Reports | governed exports export evidence | blocked pending evidence |
| Audit review | audit events search audit trail reference | blocked pending evidence |
| Conflict review | conflict queue explicit resolution | blocked pending evidence |

## Web blocked behavior

The Web workstream must not bypass the API, write directly to SQL Server, ignore organization id, ignore authorization role, hide standard error envelope, invent undocumented endpoints, treat exports as unrestricted, or treat UI completion as production evidence.

## Web workstream evidence

Required evidence includes contract test evidence, smoke test evidence, role-based screen evidence, organization-scoped request evidence, standard error envelope evidence, dashboard lineage evidence, export governance evidence, and audit trail reference evidence.

## P3.17 conclusion

The Web workstream must start from shell, API boundary, auth, organization scope, and evidence gates before feature expansion.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
