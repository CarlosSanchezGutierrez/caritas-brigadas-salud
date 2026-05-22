# P3.15 Client Integration Readiness Runbook

## Purpose

This runbook defines future evidence required to validate Web iOS Android integration readiness.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Evidence package metadata

Every evidence package must include environment name, deployed commit SHA, responsible owner, date, API contract version, OpenAPI artifact reference, client target, endpoint id, readiness status, request id, correlation id, organization id, device id when applicable, idempotency key when applicable, audit trail reference when applicable, and blockers.

## Required evidence scenarios

Required scenarios:

1. Web client endpoint status is documented.
2. iOS client endpoint status is documented.
3. Android client endpoint status is documented.
4. endpoint integration status matrix exists.
5. API contract version is documented.
6. OpenAPI contract evidence is referenced.
7. client stub baseline is referenced.
8. standard error envelope handling is documented.
9. request id preservation is documented.
10. correlation id preservation is documented.
11. organization id preservation is documented.
12. device id handling is documented for mobile.
13. idempotency key handling is documented for offline sync.
14. audit trail reference handling is documented.
15. offline sync readiness is documented.
16. contract testing evidence is documented.
17. blocked endpoint list is documented.
18. acceptance criteria are documented.

## Failure handling

If readiness evidence is incomplete, stop, keep client readiness blocked, record endpoint id, record affected client, record missing evidence, record responsible owner, do not claim backend closure, and do not allow client teams to rely on undocumented behavior.

## P3.15 conclusion

Client integration readiness must be governed by matrix status and real evidence.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
