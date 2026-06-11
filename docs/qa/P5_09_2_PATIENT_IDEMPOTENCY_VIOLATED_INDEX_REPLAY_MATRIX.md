# P5.9.2 Patient Idempotency Violated Index Replay Matrix

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

| Area | Required evidence | Required for P5.9.2 merge | Production-closing |
|---|---|---:|---:|
| IdempotencyKey replay | Violated IdempotencyKey index re-reads by IdempotencyKey | Yes | No |
| ClientOperationId replay | Violated ClientOperationId index re-reads by ClientOperationId | Yes | No |
| Local patient replay | Violated local patient index re-reads by SourceBrigadeId + LocalPatientId | Yes | No |
| Generic lookup avoidance | Catch path does not use generic prioritized idempotency lookup | Yes | No |
| SQL Server uniqueness | SQL errors 2601 and 2627 remain guarded | Yes | No |