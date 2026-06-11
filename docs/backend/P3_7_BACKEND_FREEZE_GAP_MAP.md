# P3.7 Backend Freeze Gap Map

## Status

Backend freeze status: NOT_FROZEN_PENDING_ON_PREM_EVIDENCE_AND_CONTRACTS

## Freeze gates

| Gate | Area | Requirement | Status |
|---|---|---|---|
| P3.7-GATE-001 | SQL Server | On-prem SQL Server target documented | Pending |
| P3.7-GATE-002 | SQL Server | Migration execution evidence exists | Pending |
| P3.7-GATE-003 | SQL Server | Backup and restore evidence exists | Pending |
| P3.7-GATE-004 | Security | Auth and authorization verified | Pending |
| P3.7-GATE-005 | Security | Secrets outside repository verified | Pending |
| P3.7-GATE-006 | Security | Threat model updated for on-prem | Pending |
| P3.7-GATE-007 | Audit | Audit model covers all critical actions | Pending |
| P3.7-GATE-008 | History | Longitudinal patient timeline model verified | Pending |
| P3.7-GATE-009 | Offline | Sync conflict policy verified | Pending |
| P3.7-GATE-010 | Data injection | Batch import validation and audit contract defined | Pending |
| P3.7-GATE-011 | Reporting | Operational read models defined | Pending |
| P3.7-GATE-012 | Analytics | Analytical pipeline boundary defined | Pending |
| P3.7-GATE-013 | Dashboards | KPI catalog defined | Pending |
| P3.7-GATE-014 | Frontends | OpenAPI contract freeze completed | Pending |
| P3.7-GATE-015 | Testing | Automated tests cover backend closure areas | Pending |
| P3.7-GATE-016 | Operations | Incident, rollback, and recovery evidence exists | Pending |

## Backend freeze rule

Backend v1 cannot be frozen until every gate is either completed with evidence or explicitly deferred with owner, rationale, and impact.
