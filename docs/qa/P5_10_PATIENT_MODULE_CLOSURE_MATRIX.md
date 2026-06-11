# P5.10 Patient Module Closure Matrix

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Closure matrix

| Area | Evidence | Closed for patient backend milestone | Production-closing |
|---|---|---:|---:|
| Patient contracts | Offline/source metadata appears in create and read contracts | Yes | No |
| Patient persistence | Offline/source metadata is stored and mapped | Yes | No |
| Patient endpoint hardening | List, get, clinical-record, and create route surfaces are hardened | Yes | No |
| Patient create response | CreatedAtAction route generation is used | Yes | No |
| Organization scoping | Patient reads and create validation are organization-aware | Yes | No |
| Create validation | Minimum identity, partial record reason, source brigade, and empty ids are guarded | Yes | No |
| Audit evidence | Patient creation maps to clinical write audit evidence | Yes | No |
| Longitudinal record | Clinical record includes typed collections plus derived Timeline | Yes | No |
| Retry idempotency | Repeated offline/mobile creates return the existing patient | Yes | No |
| Concurrent idempotency | SQL Server unique filtered indexes backstop concurrent duplicate retries | Yes | No |
| Documentation | Implementation docs, QA matrices, and runbooks exist for patient phases | Yes | No |
| Verification | Patient phase verifiers exist and P5.10 verifier passes | Yes | No |
| Production readiness | Real environment, operational, legal, monitoring, and security evidence | No | Required later |

## Out-of-scope backlog

| Item | Reason |
|---|---|
| Offline sync processor | Requires separate sync ingestion, conflict, replay, and monitoring design |
| Conflict resolution queues | Requires operational workflow design |
| Patient merge/deduplication | Requires clinical and legal decision rules |
| Dashboards and analytics | Requires reporting requirements and aggregation design |
| Real SQL Server migration execution | Requires institutional environment credentials and deployment approval |
| Mobile release readiness | Requires end-to-end app testing and store pipeline evidence |
| Production readiness | Requires security, privacy, operational, and deployment evidence |

## Rejection criteria

Reject P5.10 if it claims backend production readiness, removes any previous guardrail, weakens the API boundary, claims real environment evidence without proof, commits secrets, commits real patient data, allows direct mobile SQL Server writes, or treats pending offline sync processor work as already complete.