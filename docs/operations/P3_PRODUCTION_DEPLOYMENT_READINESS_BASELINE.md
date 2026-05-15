# P3 Production Deployment Readiness Baseline

Status: active
Scope: production deployment readiness baseline
Target phase: P3-26A
Depends on: P3-25A sync backend readiness checklist

---

## 1. Purpose

P3-26A defines the minimum production deployment readiness baseline for the backend.

This baseline does not approve production go-live.

It creates the operational checklist and governance gate required before moving from backend feature readiness into deployment readiness.

---

## 2. Production go-live status

Production go-live status: blocked.

The backend sync workstream is ready as a technical package, but production deployment is blocked until the following workstreams are closed:

1. P3-26B authentication and authorization hardening;
2. P3-26C SQL Server integration smoke test;
3. production secrets and connection string management;
4. production migration execution evidence;
5. production rollback and restore evidence;
6. production observability and incident response evidence;
7. production CORS and public origin lock-down;
8. production rate limiting and abuse protection validation.

---

## 3. Required production deployment principles

The production deployment baseline requires:

- no automatic database migrations during API startup;
- SQL Server migration scripts generated as idempotent SQL;
- separate runtime and migration database users;
- minimum privilege for the runtime user;
- no local development headers in production authentication flows;
- no development authentication mode in production;
- no localhost CORS origins in production;
- secrets stored outside source control;
- connection strings injected through environment or secret manager;
- health endpoint available for deployment verification;
- structured logs enabled;
- deployment evidence captured for every production release;
- rollback and restore plan available before release;
- dependency review and repository governance gates passing before release.

---

## 4. Required deployment evidence

Every production release must produce deployment evidence containing:

- git commit SHA;
- build/run identifier;
- environment name;
- deployment date and time;
- deployed API version;
- database migration script name;
- migration execution result;
- runtime connection validation result;
- health endpoint validation result;
- smoke test result;
- rollback decision point;
- responsible operator;
- approval record.

---

## 5. Required environment configuration

Production deployment requires explicit configuration for:

- ASPNETCORE_ENVIRONMENT;
- ConnectionStrings:SqlServer;
- Authentication:Mode;
- allowed issuers;
- allowed audiences;
- token authority;
- CORS allowed origins;
- rate limiting settings;
- logging level;
- telemetry settings;
- SQL Server command timeout;
- backup and restore references.

---

## 6. Required security posture

Production deployment must guarantee:

- Authentication:Mode is not Development;
- X-Dev-* headers are not trusted in production;
- Sync endpoints require explicit read or write permissions;
- tenant boundary tests remain passing;
- PayloadJson is not exposed through listing endpoints;
- dependency review passes;
- no critical or high vulnerability is accepted without documented exception;
- secrets are not committed to the repository;
- runtime database user cannot run schema migrations.

---

## 7. Required database deployment posture

Database deployment must guarantee:

- migrations are reviewed before execution;
- generated SQL script is idempotent;
- migration dry-run evidence exists;
- rollback and restore plan exists;
- restore procedure has been tested;
- RPO is defined;
- RTO is defined;
- orphan detection and cleanup playbooks exist;
- foreign key baseline remains valid.

---

## 8. Required operational posture

Operations readiness requires:

- health check endpoint;
- structured logs;
- error response consistency;
- rate limiting configuration;
- incident response owner;
- deployment rollback owner;
- smoke test checklist;
- post-deployment monitoring checklist;
- known failure modes documented.

---

## 9. Explicit non-goals

P3-26A does not implement production authentication.

P3-26A does not execute SQL Server smoke tests.

P3-26A does not deploy infrastructure.

P3-26A does not approve go-live.

P3-26A does not replace staging validation.

P3-26A does not replace cloud-specific infrastructure-as-code.

---

## 10. Required follow-up workstreams

The required follow-up workstreams are:

- P3-26B authentication and authorization hardening;
- P3-26C SQL Server integration smoke test;
- P3-26D production observability baseline;
- P3-26E deployment evidence template hardening;
- P3-26F production CORS and rate limiting validation.

---

## 11. Acceptance criteria

P3-26A is complete when:

- this production deployment readiness baseline exists;
- the production readiness verifier exists;
- the production readiness contract test exists;
- repository governance validation includes the production readiness verifier;
- database deployment baseline validation remains passing;
- sync backend readiness checklist remains present;
- production go-live remains explicitly blocked until P3-26B and P3-26C are complete;
- dotnet build and dotnet test pass.
---

## 12. P3-26B production authentication hardening note

P3-26B formalizes that Development and Disabled authentication modes are prohibited outside Development.

Production authentication must use JWT Bearer configuration and must not rely on X-Dev-* headers.
---

## 13. P3-26C SQL Server integration smoke test note

P3-26C adds an opt-in SQL Server smoke test script.

Production go-live remains blocked until the SQL Server smoke script is executed successfully against a controlled smoke or staging database and the execution evidence is attached to the deployment record.
---

## 16. P3-26H deployment evidence template and release checklist

P3-26H defines the mandatory deployment evidence record for production releases.

Production go-live remains blocked until every production deployment captures release identity, pre-deployment gates, database evidence, smoke evidence, security evidence, observability evidence, rollback evidence, approval evidence, and an explicit go/no-go decision.
---

## 17. P3-26I operational incident response runbook

P3-26I defines the mandatory operational incident response runbook for production incidents.

Production go-live remains blocked until incidents can be classified by severity, assigned to owners, triaged, mitigated, rolled back when needed, communicated, and reviewed through postmortem evidence.
