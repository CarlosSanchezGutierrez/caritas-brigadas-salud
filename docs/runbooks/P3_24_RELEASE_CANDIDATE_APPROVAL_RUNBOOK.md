# P3.24 Release Candidate Approval Runbook

## Purpose

This runbook defines evidence required to validate client release candidate approval.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Evidence package metadata

Every evidence package must include environment name, deployed commit SHA, responsible owner, date, client target, artifact reference, build profile, release channel, API contract version, OpenAPI artifact reference, request id, correlation id, organization id, device id when applicable, idempotency key when applicable, client operation id when applicable, sync status when applicable, server acknowledgment when applicable, conflict id when applicable, audit trail reference when applicable, dependency review status, secret scan status, static analysis status, contract test status, runtime configuration test status, observability test status, privacy-safe telemetry test status, schema drift status, breaking change status, artifact retention status, release notes evidence, rollback plan, support diagnostic evidence, approval state, and blockers.

## Required evidence scenarios

Required scenarios:

1. Client Release Candidate Approval Boundary is documented.
2. Web release candidate boundary is documented.
3. iOS release candidate boundary is documented.
4. Android release candidate boundary is documented.
5. Release security privacy approval boundary is documented.
6. Release candidate acceptance matrix is documented.
7. artifact reference is required.
8. deployed commit SHA is required.
9. environment name is required.
10. build profile is required.
11. release channel is required.
12. API contract version is required.
13. OpenAPI artifact reference is required.
14. dependency review evidence is required.
15. secret scan evidence is required.
16. static analysis evidence is required.
17. build reproducibility evidence is required.
18. contract test evidence is required.
19. runtime configuration test evidence is required.
20. observability test evidence is required.
21. privacy-safe telemetry test evidence is required.
22. schema drift evidence is required.
23. breaking change evidence is required.
24. release notes evidence is required.
25. rollback plan is required.
26. support diagnostic evidence is required.

## Failure handling

If release candidate evidence is incomplete, stop, keep client release candidate approval status blocked, record missing evidence, record affected client, record responsible owner, and do not accept the artifact as a release candidate.

## P3.24 conclusion

Release candidate approval must be evidenced before Web iOS Android artifacts move to pilot or production readiness review.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
