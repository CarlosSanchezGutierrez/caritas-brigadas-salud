# Technical Debt Register

## Purpose

This register tracks intentional, controlled, and prioritized technical debt.

Technical debt is acceptable only when it is:

- Explicit.
- Documented.
- Owned.
- Prioritized.
- Reviewed.
- Scheduled for removal or mitigation.

Undocumented debt is not accepted.

## Debt classification

### D1 — Critical

Must be fixed before production.

Examples:

- Authorization bypass.
- Tenant isolation weakness.
- Secrets in code.
- Broken migrations.
- Patient data leakage.
- Production configuration unsafe by default.

### D2 — High

Must be fixed before institutional pilot.

Examples:

- Missing pagination on high-volume endpoints.
- Missing indexes for expected production queries.
- Weak observability.
- Insufficient authorization tests.
- Incomplete production validation.

### D3 — Medium

Can be accepted temporarily with owner and target date.

Examples:

- Manual operational steps.
- Limited API documentation.
- Missing non-critical tests.
- Repetitive repository code.
- Incomplete admin tooling.

### D4 — Low

Can be kept if documented.

Examples:

- Minor naming inconsistency.
- Non-critical documentation gaps.
- Developer-experience improvements.

## Current register

| ID | Severity | Area | Description | Owner | Target | Status |
|---|---|---|---|---|---|---|
| TD-001 | D2 | API | Pagination must be standardized across list endpoints before high-volume production use. | Maintainer | Before frontend scale-up | Open |
| TD-002 | D2 | Database | Index strategy must be reviewed for OrganizationId, PatientId, BrigadeId, VisitId, EncounterId, and common reporting filters. | Maintainer | Before production DB pilot | Open |
| TD-003 | D2 | Security | Authorization and tenant-isolation tests must be expanded for all sensitive endpoints. | Maintainer | Before institutional pilot | Open |
| TD-004 | D2 | Operations | Observability baseline must include structured logs, health checks, correlation IDs, and production monitoring plan. | Maintainer | Before deployment | Open |
| TD-005 | D2 | API Contracts | OpenAPI must become the source of truth for web, Android, and iOS integration. | Maintainer | Before frontend implementation | Open |
| TD-006 | D3 | Architecture | Message queues, dead-letter queues, event-driven architecture, and real-time notifications are intentionally deferred. | Maintainer | Future phase | Accepted |
| TD-007 | D3 | Frontend | Web, Android, and iOS clients are not implemented yet; backend contracts must stabilize first. | Maintainer | Frontend phase | Open |

## Rules

1. A PR may not introduce D1 debt.
2. D2 debt requires explicit justification.
3. D3 debt requires owner and target phase.
4. D4 debt may be grouped into cleanup work.
5. Accepted debt must not hide security, privacy, or clinical integrity risks.

## Review cadence

The register should be reviewed:

- Before every release to `main`.
- Before starting frontend implementation.
- Before institutional demo.
- Before connecting real data.
- Before production deployment.