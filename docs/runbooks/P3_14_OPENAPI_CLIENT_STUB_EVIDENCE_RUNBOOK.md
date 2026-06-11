# P3.14 OpenAPI and Client Stub Evidence Runbook

## Purpose

This runbook defines future evidence required to validate OpenAPI contract evidence and client stub baselines.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Evidence package metadata

Every evidence package must include:

- environment name.
- deployed commit SHA.
- responsible owner.
- date.
- contract version.
- OpenAPI artifact reference.
- endpoint id.
- client target.
- API version.
- request id.
- correlation id.
- organization id.
- audit trail reference when applicable.
- validation result.
- blockers.

## Required evidence scenarios

Required scenarios:

1. OpenAPI artifact exists.
2. OpenAPI artifact has contract version.
3. endpoint catalog maps to OpenAPI paths.
4. operation id exists for each frozen endpoint.
5. request schema exists where applicable.
6. response schema exists.
7. standard error envelope exists.
8. security requirement exists for protected endpoints.
9. organization id requirement is represented.
10. request id is represented.
11. correlation id is represented.
12. idempotency key is represented where applicable.
13. device id is represented where applicable.
14. audit trail reference is represented where applicable.
15. Web client stub boundary is documented.
16. iOS client stub boundary is documented.
17. Android client stub boundary is documented.
18. contract testing baseline exists.
19. schema drift check is documented.
20. breaking change check is documented.

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
- generated client credentials.

No secrets in repository.

## Sanitized evidence allowed

Allowed evidence:

- synthetic OpenAPI examples.
- synthetic request payload.
- synthetic response payload.
- standard error envelope example.
- test request id.
- test correlation id.
- test organization id.
- test idempotency key.
- test device id.
- test audit trail reference.
- generated client boundary summary.
- contract testing summary.
- schema drift summary.
- breaking change summary.

## Failure handling

If evidence is incomplete:

1. Stop.
2. Record blocker.
3. Record missing OpenAPI section or client stub boundary.
4. Record responsible owner.
5. Do not claim backend closure.
6. Do not treat generated clients as stable.
7. Do not allow client teams to rely on undocumented endpoint behavior.

## P3.14 conclusion

OpenAPI and client stub evidence must prove contract alignment, not production readiness.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE