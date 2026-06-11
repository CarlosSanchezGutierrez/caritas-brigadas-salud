# P5.10 Patient Module Closure Matrix

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Closure matrix

| Area | Evidence | Closed for patient backend milestone | Production-closing |
|---|---|---:|---:|
| Patient contracts | Offline/source metadata fields exist in contracts | Yes | No |
| Patient persistence | Offline/source metadata is persisted | Yes | No |
| Patient endpoint hardening | Patient route contract is explicit and guarded | Yes | No |
| Patient organization scoping | Patient reads and writes are organization-aware | Yes | No |
| Patient validation | Minimum identity, source brigade, partial record, and empty route id guards exist | Yes | No |
| Patient audit evidence | Patient creation maps to AuditActionCodes.PatientCreate | Yes | No |
| Patient longitudinal record | Clinical record exposes Timeline and typed collections | Yes | No |
| Patient create idempotency | Repeated offline identities return existing patient | Yes | No |
| Atomic idempotency | SQL Server unique filtered indexes protect concurrent duplicate creates | Yes | No |
| Violated-index replay | Concurrent replay re-reads using the identity tied to the violated unique index | Yes | No |
| Evidence package | Docs, matrices, runbooks, verifiers, and tests exist | Yes | No |
| Production deployment | Real environment proof and institutional approval | No | Required later |

## Remaining backlog after P5.10

| Backlog item | Status after P5.10 |
|---|---|
| Offline sync processor | Open |
| Conflict resolution queue | Open |
| Patient merge/deduplication workflow | Open |
| Mobile release readiness | Open |
| Dashboard and analytics | Open |
| Production monitoring | Open |
| Load testing | Open |
| Security testing | Open |
| Legal/privacy approval | Open |
| Real SQL Server migration execution | Open |

## Rejection criteria

Reject P5.10 if it claims backend production deployment approval, removes guardrails, weakens the API boundary, allows direct mobile SQL Server writes, adds secrets, adds real patient data, introduces a cloud dependency, or treats offline sync processor work as complete.