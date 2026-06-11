# P3.7 On-Prem Backend Closure Decision Register

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Decisions

| ID | Decision | Status | Rationale |
|---|---|---|---|
| P3.7-ADR-001 | SQL Server is the operational source of truth | Accepted | Caritas operates with SQL Server and institutional data center constraints. |
| P3.7-ADR-002 | Cloud infrastructure is optional, not required | Accepted | The backend must operate on-premise without Azure, AWS, or other cloud dependencies. |
| P3.7-ADR-003 | Data injection must pass through validation and audit | Accepted | Imported data must preserve traceability and cannot bypass business rules. |
| P3.7-ADR-004 | Longitudinal patient history is a core backend capability | Accepted | Clinical continuity, reporting, and research require timeline-based history. |
| P3.7-ADR-005 | Offline-first sync uses event batches and idempotency | Accepted | Field operations may have unreliable connectivity. |
| P3.7-ADR-006 | Operational and analytical pipelines are separated | Accepted | Reporting and research must not destabilize transactional operations. |
| P3.7-ADR-007 | Every significant action must be auditable | Accepted | Compliance, incident response, clinical traceability, and trust require audit trails. |
| P3.7-ADR-008 | Dashboards depend on governed read models | Accepted | Direction-level reporting requires stable, explainable indicators. |
| P3.7-ADR-009 | AI API Gateway is deferred behind an adapter boundary | Accepted | AI must not become a production dependency or privacy risk. |
| P3.7-ADR-010 | Blockchain is deferred as crypto-audit lab readiness | Accepted | Audit integrity may be explored without creating production complexity. |
| P3.7-ADR-011 | Frontend clients must wait for API contract freeze | Accepted | Web, iOS, and Android must not build against unstable backend contracts. |
| P3.7-ADR-012 | Research datasets require governance and de-identification | Accepted | Clinical and social data require privacy-preserving analytical handling. |

## Non-goals

- No cloud migration requirement.
- No direct production LLM integration.
- No blockchain production dependency.
- No raw patient data on external systems by default.
- No dashboard queries directly against write-heavy transactional tables without approved read models.
- No frontend-specific backend shortcuts.
