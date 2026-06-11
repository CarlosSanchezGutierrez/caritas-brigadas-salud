# P3.19 Web Model Mapping

## Purpose

This document maps shared API client models to the Web client.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Web model mapping status: BLOCKED_PENDING_REAL_EVIDENCE

## Web model mapping

| Shared model | Web usage |
|---|---|
| request metadata model | every Web API request |
| response metadata model | every Web API response |
| standard error envelope model | validation authorization conflict and server errors |
| authentication context model | protected routes and protected API calls |
| authorization context model | role-sensitive navigation and actions |
| organization scope model | scoped data and reports |
| pagination model | tables lists dashboards reports |
| filtering model | tables reports dashboards |
| sorting model | tables reports dashboards |
| audit reference model | accepted writes audit review |
| conflict model | conflict review and authorized resolution |

## Web blocked mapping

The Web client must not implement endpoint-specific error shapes, bypass organization scope, drop request id, drop correlation id, treat exports as unrestricted, or treat UI table data as evidence.

## P3.19 conclusion

Web model mapping must remain aligned with shared API client model contracts.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
