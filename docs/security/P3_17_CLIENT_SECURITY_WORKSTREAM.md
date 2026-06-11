# P3.17 Client Security Workstream

## Purpose

This document defines the client security workstream for Web iOS Android implementation planning.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Client security workstream status: BLOCKED_PENDING_REAL_EVIDENCE

## Security scope

The client security workstream governs authentication, authorization, organization scope, auditability, privacy, offline sync safety, and sensitive data handling.

## Required security lanes

| Security lane | Scope |
|---|---|
| Authentication | protected screens and protected API calls |
| Authorization | role-sensitive actions and role-specific visibility |
| Organization scope | organization id preservation and scoped data isolation |
| Auditability | audit trail reference for accepted writes |
| Privacy | no real patient data in fixtures and safe evidence packages |
| Offline sync safety | device id idempotency key client operation id conflict handling |
| Error handling | standard error envelope preservation |

## Blocked security behavior

Blocked behavior includes bypassing the API, bypassing authorization, bypassing organization scope, bypassing audit trail creation, storing credentials in repository, storing real patient data in fixtures, silent conflict overwrite, unrestricted export behavior, and unaudited patient-level access.

## P3.17 conclusion

Client security must be a dedicated workstream before implementation expands.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
