# P3.25 Web Pilot Readiness Boundary

## Purpose

This document defines the Web pilot readiness boundary.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Web pilot readiness status: BLOCKED_PENDING_REAL_EVIDENCE

## Web pilot readiness evidence

Required evidence:

- approved release candidate reference.
- artifact reference.
- deployed commit SHA.
- environment name.
- build profile.
- release channel.
- API contract version.
- OpenAPI artifact reference.
- pilot site or brigade scope.
- pilot participant scope.
- UAT acceptance criteria.
- role-based access evidence.
- organization scope evidence.
- support diagnostic evidence.
- privacy-safe telemetry evidence.
- rollback plan.
- incident response plan.
- support escalation plan.

## Web metadata evidence

The Web pilot must preserve request id, correlation id, organization id, authorization role, standard error envelope, audit trail reference, pagination convention, filtering convention, sorting convention, and support diagnostic evidence.

## Web blocked pilot behavior

The Web client must not bypass the API, write directly to SQL Server, ignore organization id, ignore authorization role, drop request id, drop correlation id, hide standard error envelope, treat exports as unrestricted, expand pilot scope without approval, or treat pilot readiness as production approval.

## P3.25 conclusion

The Web pilot must remain blocked until controlled pilot readiness evidence is complete.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
