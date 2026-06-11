# P3.20 Client API Contract Test Harness Runbook

## Purpose

This runbook defines evidence required to validate the client API contract test harness.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Evidence package metadata

Every evidence package must include environment name, deployed commit SHA, responsible owner, date, client target, API contract version, endpoint id, request id, correlation id, organization id, device id when applicable, idempotency key when applicable, client operation id when applicable, sync status when applicable, audit trail reference when applicable, schema drift status, breaking change status, contract test status, and blockers.

## Required evidence scenarios

Required scenarios:

1. client API contract test harness baseline is documented.
2. cross-client contract test scenarios are documented.
3. Web contract test harness is documented.
4. iOS contract test harness is documented.
5. Android contract test harness is documented.
6. schema drift gate is documented.
7. breaking change gate is documented.
8. API contract version is tested.
9. endpoint id is tested.
10. request schema is tested.
11. response schema is tested.
12. standard error envelope model is tested.
13. request id preservation is tested.
14. correlation id preservation is tested.
15. organization id preservation is tested.
16. authorization role preservation is tested.
17. audit trail reference handling is tested.
18. device id propagation is tested for mobile.
19. idempotency key propagation is tested for offline sync.
20. client operation id propagation is tested for offline sync.
21. sync status handling is tested for mobile.
22. server acknowledgment handling is tested for mobile sync.
23. conflict id handling is tested.
24. schema drift evidence is required.
25. breaking change evidence is required.

## Failure handling

If contract test harness evidence is incomplete, stop, keep client API contract test harness status blocked, record missing evidence, record affected client, record endpoint id, record responsible owner, and do not allow client implementation to rely on untested API behavior.

## P3.20 conclusion

The contract test harness must be evidenced before Web iOS Android implementation relies on API behavior.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
