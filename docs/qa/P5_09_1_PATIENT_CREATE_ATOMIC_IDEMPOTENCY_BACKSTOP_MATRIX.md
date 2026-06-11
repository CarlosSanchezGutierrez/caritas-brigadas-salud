# P5.9.1 Patient Create Atomic Idempotency Backstop Matrix

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

| Area | Required evidence | Required for P5.9.1 merge | Production-closing |
|---|---|---:|---:|
| IdempotencyKey atomicity | Unique filtered index exists | Yes | No |
| ClientOperationId atomicity | Unique filtered index exists | Yes | No |
| Local patient atomicity | Unique filtered index exists | Yes | No |
| Concurrent replay | DbUpdateException unique violation re-reads existing patient | Yes | No |
| SQL Server error handling | SQL errors 2601 and 2627 are handled for idempotency indexes | Yes | No |
| Deleted rows | Filter excludes deleted rows | Yes | No |
| Guardrails | No production readiness claim, secrets, real patient data, direct SQL mobile writes, or cloud dependency | Yes | No |