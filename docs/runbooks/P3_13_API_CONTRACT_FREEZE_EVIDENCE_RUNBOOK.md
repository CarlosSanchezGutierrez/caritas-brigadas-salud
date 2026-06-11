# P3.13 API Contract Freeze Evidence Runbook

## Purpose

This runbook defines future evidence required to validate the API contract freeze for Web, iOS, Android, reporting, administration, and offline sync.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Evidence package metadata

Every evidence package must include:

- environment name.
- deployed commit SHA.
- responsible owner.
- date.
- API version.
- endpoint id.
- route.
- HTTP method.
- client support.
- organization id.
- request id.
- correlation id.
- audit trail reference when applicable.
- status.
- blockers.

## Required evidence scenarios

Required scenarios:

1. endpoint catalog entry exists.
2. request schema is documented.
3. response schema is documented.
4. standard error envelope is documented.
5. authentication requirement is documented.
6. authorization role is documented.
7. organization id requirement is documented.
8. pagination convention is documented when applicable.
9. filtering convention is documented when applicable.
10. sorting convention is documented when applicable.
11. idempotency key behavior is documented when applicable.
12. offline sync metadata is documented when applicable.
13. Web compatibility is documented.
14. iOS compatibility is documented.
15. Android compatibility is documented.
16. audit trail requirement is documented.
17. API version is documented.
18. breaking change policy is documented.

## Prohibited evidence content

Do not store:

- credentials.
- connection strings.
- secrets.
- unrestricted patient identifiers.
- raw clinical notes from real patients.
- unredacted screenshots.
- raw database dumps.
- mobile platform secrets.

No secrets in repository.

## Sanitized evidence allowed

Allowed evidence:

- synthetic request payload.
- synthetic response payload.
- endpoint catalog excerpt.
- standard error envelope example.
- test request id.
- test correlation id.
- test organization id.
- test idempotency key.
- test device id.
- audit trail reference.
- sanitized OpenAPI excerpt if available.

## Failure handling

If evidence is incomplete:

1. Stop.
2. Record blocker.
3. Record missing endpoint or convention.
4. Record responsible owner.
5. Do not claim backend closure.
6. Do not allow Web/iOS/Android teams to treat the endpoint as frozen.

## P3.13 conclusion

API contract freeze evidence must prove stable, auditable, versioned, and client-compatible API expectations.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE