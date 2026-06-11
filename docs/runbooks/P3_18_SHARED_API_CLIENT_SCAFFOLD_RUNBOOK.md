# P3.18 Shared API Client Scaffold Runbook

## Purpose

This runbook defines evidence required to validate the shared API client scaffold governance.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Evidence package metadata

Every evidence package must include environment name, deployed commit SHA, responsible owner, date, client target, API contract version, endpoint id, request id, correlation id, organization id, device id when applicable, idempotency key when applicable, client operation id when applicable, audit trail reference when applicable, schema drift status, contract test status, and blockers.

## Required evidence scenarios

Required scenarios:

1. shared API client scaffold governance is documented.
2. Web API client scaffold is documented.
3. iOS API client scaffold is documented.
4. Android API client scaffold is documented.
5. API client contract test scaffold is documented.
6. API client security scaffold is documented.
7. API contract version propagation is required.
8. endpoint id mapping is required.
9. typed request model is required.
10. typed response model is required.
11. standard error envelope handler is required.
12. request id propagation is required.
13. correlation id propagation is required.
14. organization id propagation is required.
15. authorization role preservation is required.
16. device id propagation is required for mobile.
17. idempotency key propagation is required for offline sync.
18. client operation id propagation is required for offline sync.
19. audit trail reference handling is required.
20. contract test evidence is required.
21. schema drift evidence is required.

## Failure handling

If scaffold evidence is incomplete, stop, keep API client scaffold status blocked, record missing evidence, record affected client, record endpoint id, record responsible owner, and do not allow client feature implementation to depend on the scaffold.

## P3.18 conclusion

The shared API client scaffold must be evidenced before Web iOS Android implementation depends on it.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
