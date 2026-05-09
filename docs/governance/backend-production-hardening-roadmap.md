# Backend Production Hardening Roadmap

## Purpose

This roadmap defines the backend hardening sequence before implementing full frontend clients.

The backend must be stable, secure, documented, and contract-driven before web, Android, and iOS clients depend on it.

## Phase 1 — Governance hardening

Status: In progress.

Deliverables:

- Quality gates.
- Technical debt register.
- Release policy.
- Branch protection baseline.
- CODEOWNERS.
- PR template.
- Required checks baseline.

## Phase 2 — API production baseline

Deliverables:

- Rate limiting.
- Strong production configuration validation.
- CORS restrictions for production.
- Security headers.
- Health check readiness review.
- Request correlation and traceability.
- Standard error contract review.

## Phase 3 — API contracts

Deliverables:

- OpenAPI reviewed as the integration contract.
- DTO naming consistency.
- Error response standardization.
- Versioning policy.
- Created Location URL consistency.
- Frontend integration guide.

## Phase 4 — Pagination and query safety

Deliverables:

- Standard pagination request model.
- Standard paginated response model.
- Pagination on high-volume list endpoints.
- Maximum page size enforcement.
- No unbounded production list endpoints unless justified.

## Phase 5 — Database performance baseline

Deliverables:

- Index review.
- Query pattern review.
- N+1 query review.
- AsNoTracking review.
- Transaction review.
- Lock/concurrency review.
- Migration sequence review.

## Phase 6 — Authorization test expansion

Deliverables:

- Tests for unauthenticated access.
- Tests for missing permission.
- Tests for wrong organization.
- Tests for SUPER_ADMIN-only behavior.
- Tests for role assignment boundaries.
- Tests for student/service role boundaries.

## Phase 7 — Observability baseline

Deliverables:

- Structured logging.
- Correlation IDs.
- Health checks.
- Readiness/liveness distinction where applicable.
- Metrics plan.
- Error monitoring plan.
- Audit log boundaries.

## Deferred intentionally

The following are intentionally deferred until the product needs them:

- Message queues.
- Dead-letter queues.
- Event-driven architecture.
- Real-time notifications.
- Server-sent events.
- WebSockets.
- Chat.
- Advanced distributed tracing.
- Horizontal scaling automation.

These are not rejected. They are deferred to avoid unnecessary complexity before the first institutional pilot.