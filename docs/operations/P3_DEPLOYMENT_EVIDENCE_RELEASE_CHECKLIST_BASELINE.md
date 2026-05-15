# P3 Deployment Evidence and Release Checklist Baseline

Status: active
Scope: production deployment evidence, release checklist, approval, and rollback traceability
Target phase: P3-26H
Depends on: P3-26G production CORS and rate limiting validation

---

## 1. Purpose

P3-26H defines the mandatory evidence package for every production deployment.

The goal is to make every release auditable, repeatable, reversible, and attributable to a responsible operator.

---

## 2. Production deployment status

Production go-live remains blocked until every deployment has a completed deployment evidence record.

A deployment evidence record must be created before release and completed after post-deployment verification.

---

## 3. Required release identity evidence

Every release record must include:

- release id;
- environment name;
- deployment timestamp UTC;
- git commit SHA;
- source branch;
- pull request number;
- release operator;
- technical approver;
- business approver;
- deployment status;
- rollback status.

---

## 4. Required pre-deployment evidence

Before production deployment, the release record must include:

- repository governance baseline result;
- backend build result;
- backend test result;
- dependency review result;
- database deployment baseline result;
- SQL Server smoke readiness result;
- production auth hardening verification;
- production CORS and rate limiting verification;
- production observability verification;
- deployment health smoke readiness.

---

## 5. Required database deployment evidence

Every production deployment must document:

- migration script name;
- migration script checksum;
- target database name;
- database operator;
- backup completed;
- restore point captured;
- rollback script available;
- migration applied status;
- post-migration validation status.

---

## 6. Required smoke evidence

Every production deployment must document:

- SQL Server smoke command;
- SQL Server smoke result;
- deployment health smoke command;
- deployment health smoke result;
- /health/live status;
- /health/ready status;
- root endpoint status;
- smoke timestamp UTC;
- smoke operator.

---

## 7. Required security evidence

Every production deployment must document:

- Authentication:Mode;
- JWT Authority configured;
- JWT Audience configured;
- no X-Dev-* authentication in production;
- explicit HTTPS CORS origins;
- no localhost CORS origins;
- no wildcard CORS origins;
- Security:RateLimiting:Enabled;
- Security:RequireHttps;
- explicit AllowedHosts.

---

## 8. Required observability evidence

Every production deployment must document:

- health endpoint evidence;
- structured logging evidence;
- correlation id evidence;
- request telemetry evidence;
- error rate review;
- latency review;
- post-deployment log review window;
- incident owner;
- escalation contact.

---

## 9. Required rollback evidence

Every production deployment must document:

- rollback decision point;
- rollback owner;
- rollback trigger criteria;
- rollback command or procedure;
- database rollback procedure;
- backup/restore procedure;
- expected rollback duration;
- rollback verification steps.

---

## 10. Required approval evidence

Every production deployment must document:

- technical approval;
- business approval;
- data/privacy approval when patient data handling is affected;
- deployment operator signature;
- approval timestamp UTC;
- known risks;
- explicit go/no-go decision.

---

## 11. Non-goals

P3-26H does not deploy production.

P3-26H does not approve production go-live.

P3-26H does not replace change management.

P3-26H does not execute migrations.

P3-26H does not create cloud infrastructure.

---

## 12. Acceptance criteria

P3-26H is complete when:

- this deployment evidence baseline exists;
- the deployment evidence template exists;
- the release checklist verifier exists;
- the release checklist contract tests exist;
- production deployment readiness references P3-26H;
- repository governance validation includes the release checklist verifier;
- dotnet build and dotnet test pass.