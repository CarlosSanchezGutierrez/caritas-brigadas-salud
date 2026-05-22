# P3.25 Controlled Pilot Readiness Runbook

## Purpose

This runbook defines evidence required to validate controlled pilot readiness.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Evidence package metadata

Every evidence package must include environment name, deployed commit SHA, responsible owner, date, client target, approved release candidate reference, artifact reference, build profile, release channel, API contract version, OpenAPI artifact reference, pilot site or brigade scope, pilot participant scope, pilot device inventory when applicable, UAT acceptance criteria, training evidence, privacy consent evidence, data protection evidence, request id, correlation id, organization id, device id when applicable, idempotency key when applicable, client operation id when applicable, sync status when applicable, server acknowledgment when applicable, conflict id when applicable, audit trail reference when applicable, contract test status, runtime configuration test status, observability test status, privacy-safe telemetry test status, offline field workflow status when applicable, sync dry run status when applicable, rollback plan, incident response plan, support escalation plan, pilot approval state, and blockers.

## Required evidence scenarios

Required scenarios:

1. Controlled Pilot Readiness Boundary is documented.
2. Web pilot readiness boundary is documented.
3. iOS pilot readiness boundary is documented.
4. Android pilot readiness boundary is documented.
5. Field operations support and training boundary is documented.
6. Pilot privacy consent data protection boundary is documented.
7. Pilot acceptance UAT matrix is documented.
8. approved release candidate reference is required.
9. artifact reference is required.
10. deployed commit SHA is required.
11. environment name is required.
12. build profile is required.
13. release channel is required.
14. API contract version is required.
15. OpenAPI artifact reference is required.
16. pilot site or brigade scope is required.
17. pilot participant scope is required.
18. pilot device inventory is required for mobile.
19. UAT acceptance criteria is required.
20. training evidence is required.
21. privacy consent evidence is required.
22. data protection evidence is required.
23. contract test evidence is required.
24. runtime configuration test evidence is required.
25. observability evidence is required.
26. privacy-safe telemetry evidence is required.
27. offline field workflow evidence is required for mobile.
28. sync dry run evidence is required for mobile.
29. rollback plan is required.
30. incident response plan is required.
31. support escalation plan is required.

## Failure handling

If controlled pilot readiness evidence is incomplete, stop, keep controlled pilot readiness status blocked, record missing evidence, record affected client, record responsible owner, and do not start controlled pilot execution.

## P3.25 conclusion

Controlled pilot readiness must be evidenced before Web iOS Android are used in limited field validation.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
