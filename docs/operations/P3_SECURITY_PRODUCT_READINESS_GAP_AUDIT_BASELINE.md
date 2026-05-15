# P3 Security and Product Readiness Gap Audit Baseline

Status: active
Scope: security, infrastructure, functional product, staging, production, and frontend readiness gap audit
Target phase: P3-26L
Depends on: P3-26K backend production readiness closure report

---

## 1. Purpose

P3-26L defines the gap audit required before moving from backend governance into staging, production, or frontend work.

The goal is to classify every remaining concern as:

- COMPLETE;
- REQUIRED_BEFORE_FRONTEND;
- REQUIRED_BEFORE_STAGING;
- REQUIRED_BEFORE_PRODUCTION;
- OPTIONAL_LATER;
- NOT_APPLICABLE;
- OWNED_BY_INFRASTRUCTURE;
- OWNED_BY_CARITAS_OR_TEC.

---

## 2. Required audit categories

The audit must cover:

- rate limiting;
- dependency scanning;
- static analysis;
- secret scanning;
- penetration testing;
- SQL Server VM connectivity;
- SQL Server least privilege;
- network ACL and firewall rules;
- deny-by-default traffic posture;
- TLS between backend and SQL Server;
- encryption at rest;
- backup and restore evidence;
- rollback evidence;
- production secrets source;
- Grafana or equivalent observability tooling;
- health endpoints;
- structured logging;
- request telemetry;
- correlation id;
- patient signature;
- privacy notice consent;
- social security / insurance fields;
- emergency contact fields;
- migrant or incomplete patient data handling;
- OpenAPI/frontend contract readiness;
- staging evidence;
- production evidence.

---

## 3. Security position

The backend must not be declared production-ready until:

- SQL Server connectivity is validated against the real target;
- SQL least privilege is configured;
- network access is restricted by ACL/firewall;
- deny-by-default posture is documented;
- TLS is enforced for database connections;
- production secrets are not stored in source control;
- dependency scanning is automated;
- static analysis is automated;
- secret scanning is automated;
- staging smoke tests pass;
- production blocker matrix is completed.

---

## 4. Product position

The frontend should not be built deeply until the backend functional contract is clear for:

- patient identity;
- patient optional fields;
- patient signature;
- privacy notice consent;
- social security / insurance;
- emergency contact;
- services available per brigade;
- visit workflow;
- form response workflow;
- sync/offline workflow;
- data validation rules;
- OpenAPI contract.

---

## 5. Infrastructure position

The SQL Server VM does not replace the backend.

The backend remains the API layer.

SQL Server remains the database layer.

The API must connect to SQL Server through a controlled connection string, restricted network path, TLS, least-privilege SQL identity, and environment-specific secrets.

---

## 6. Observability position

Grafana or equivalent dashboarding is optional until there is real staging telemetry.

The required order is:

1. health endpoints;
2. structured logs;
3. correlation id;
4. request telemetry;
5. staging deployment;
6. metrics export;
7. dashboarding;
8. alerting.

---

## 7. Required output

P3-26L must produce:

- security and product readiness gap audit document;
- phase plan from backend closure to frontend;
- explicit list of items that block frontend;
- explicit list of items that block staging;
- explicit list of items that block production;
- explicit list of items that are optional later;
- explicit list of items owned by Tec/Cáritas infrastructure;
- repository verifier;
- contract tests.

---

## 8. Acceptance criteria

P3-26L is complete when:

- this baseline exists;
- the gap audit document exists;
- the verifier exists;
- contract tests exist;
- production readiness closure report references P3-26L;
- repository governance validation includes the gap audit verifier;
- dotnet build and dotnet test pass.