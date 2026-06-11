# P3.17 Cross Client QA Workstream

## Purpose

This document defines the cross-client QA workstream for Web iOS Android implementation planning.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Cross-client QA workstream status: BLOCKED_PENDING_REAL_EVIDENCE

## QA scope

Cross-client QA must validate that Web iOS Android preserve the same contract expectations while respecting each client-specific workflow.

## Required QA lanes

| QA lane | Scope |
|---|---|
| Contract tests | endpoint schema standard error envelope metadata preservation |
| Smoke tests | health identity organization context basic navigation |
| Role tests | authorization role and protected actions |
| Organization scope tests | organization id and scoped data isolation |
| Offline sync tests | mobile draft outbox idempotency server acknowledgment |
| Conflict tests | explicit conflict handling no silent overwrite |
| Audit tests | audit trail reference after accepted writes |
| Export tests | governed reports and patient-level export restrictions |

## QA evidence

Required QA evidence includes contract test evidence, smoke test evidence, cross-client scenario matrix, blocked scenario list, failure evidence, regression evidence, and evidence package reference.

## P3.17 conclusion

Cross-client QA must be planned before implementation produces disconnected Web iOS Android behavior.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
