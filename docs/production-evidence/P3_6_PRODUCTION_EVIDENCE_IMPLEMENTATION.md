# P3.6 Production Evidence Implementation

## Status

`Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE`

P3.5 and P3.5.1 are closed as technical checkpoints in `main`.

P3.6 exists to collect real production evidence. This phase must not add business features. It must prove that the existing backend can be deployed, configured, observed, backed up, restored, tested, and rolled back with auditable discipline.

## Objective

Convert technical production closure into real operational evidence.

Production readiness must not be declared from architecture, local checks, or intention. It must be declared only from verifiable evidence.

## Required evidence categories

### 1. Deployment evidence

Required:

- environment name;
- provider or infrastructure target;
- deployed commit SHA;
- deployment date;
- deployment responsible;
- API URL or internal endpoint;
- deployment logs or CI reference;
- rollback reference.

### 2. Configuration evidence

Required:

- `ASPNETCORE_ENVIRONMENT`;
- configured CORS origins;
- configured forwarded header known proxies;
- configured forwarded header known networks;
- rate limiting status;
- max request body size;
- Swagger exposure status;
- authentication mode;
- secrets provider.

No secret values must be committed to the repository.

### 3. Database evidence

Required:

- SQL Server target;
- database name;
- migration status;
- application user privilege model;
- backup evidence;
- restore evidence;
- recovery time notes;
- data retention notes.

### 4. Security evidence

Required:

- CodeQL status;
- dependency review status;
- secret scanning status;
- authentication smoke test;
- authorization smoke test;
- security headers verification;
- rate limiting verification;
- sensitive logs verification.

### 5. Observability evidence

Required:

- health/live result;
- health/ready result;
- structured logging evidence;
- propagated correlation id evidence;
- 4xx/5xx traceability evidence;
- latency evidence;
- startup log evidence.

### 6. Smoke test evidence

Required:

- root endpoint;
- health/live endpoint;
- health/ready endpoint;
- anonymous request to protected endpoint fails;
- authenticated request to protected endpoint succeeds;
- representative organization endpoint succeeds;
- representative report/export endpoint succeeds when applicable.

### 7. Rollback evidence

Required:

- rollback criteria;
- rollback command/procedure;
- database rollback policy;
- restore procedure;
- decision owner;
- incident record template.

## Out of scope

P3.6 does not include:

- AI Gateway;
- blockchain;
- new clinical modules;
- app mobile implementation;
- dashboard redesign;
- advanced analytics;
- new SaaS architecture.

## Closing rule

P3.6 cannot be closed until the evidence register has complete references for deployment, configuration, database, security, observability, smoke tests, and rollback.

Until then, the only valid status is:

`Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE`