# P3 Operational Incident Response Runbook Baseline

Status: active
Scope: production incident response, severity classification, escalation, rollback, communication, and postmortem traceability
Target phase: P3-26I
Depends on: P3-26H deployment evidence template and release checklist

---

## 1. Purpose

P3-26I defines the operational incident response runbook required before production deployment.

The goal is to make every production incident classifiable, assignable, traceable, reversible, communicable, and reviewable.

---

## 2. Production incident response status

Production go-live remains blocked until the incident response runbook and incident record template exist and are governed by repository validation.

Every production incident must have an incident record.

Every production incident must have a severity level, owner, timeline, mitigation decision, communication status, and postmortem decision.

---

## 3. Severity levels

The runbook must define these severity levels:

- SEV-1 Critical;
- SEV-2 High;
- SEV-3 Medium;
- SEV-4 Low.

Severity classification must consider:

- patient data exposure;
- authentication bypass;
- authorization bypass;
- API outage;
- database outage;
- failed deployment;
- failed migration;
- sync processing failure;
- elevated error rate;
- degraded performance;
- health endpoint unhealthy status.

---

## 4. Required response ownership

Every incident must assign:

- incident commander;
- technical owner;
- communications owner;
- database owner when data or migrations are involved;
- security/privacy owner when patient data, authentication, authorization, or secrets are involved;
- business owner when service delivery or partner operations are affected.

---

## 5. Required response timeline

Every incident must capture:

- detection timestamp UTC;
- acknowledgement timestamp UTC;
- triage timestamp UTC;
- mitigation timestamp UTC;
- resolution timestamp UTC;
- communication timestamp UTC;
- postmortem timestamp UTC when required.

---

## 6. Required triage checklist

Triage must verify:

- affected environment;
- affected endpoint;
- affected organization;
- affected users;
- correlation ids;
- request ids;
- deployment commit SHA;
- recent deployment status;
- health endpoint status;
- database connectivity status;
- authentication failure rate;
- authorization failure rate;
- sync rejection rate;
- rate limiting status;
- error logs;
- rollback availability.

---

## 7. Required mitigation checklist

Mitigation must document:

- mitigation owner;
- mitigation decision;
- rollback decision;
- rollback trigger criteria;
- rollback command or procedure;
- database rollback decision;
- backup/restore decision;
- customer/partner communication decision;
- incident status after mitigation.

---

## 8. Required communication checklist

Communication must document:

- internal notification status;
- partner notification status;
- user notification status when applicable;
- privacy/legal escalation status when patient data may be affected;
- communication owner;
- communication timestamp UTC;
- approved communication summary.

---

## 9. Required postmortem checklist

Postmortem is required for:

- every SEV-1 incident;
- every SEV-2 incident;
- any patient data exposure;
- any authentication bypass;
- any authorization bypass;
- any production rollback;
- any failed database migration.

The postmortem must include:

- root cause;
- contributing factors;
- impact summary;
- detection gap;
- prevention action;
- owner of prevention action;
- due date;
- verification method;
- follow-up PR or issue reference.

---

## 10. Required incident evidence

Every incident record must include:

- incident id;
- severity;
- status;
- affected environment;
- affected endpoint;
- affected organization;
- affected users;
- correlation ids;
- request ids;
- first detection source;
- deployment commit SHA;
- deployment evidence record link;
- logs reviewed;
- health evidence;
- database evidence;
- mitigation evidence;
- rollback evidence;
- communication evidence;
- postmortem decision.

---

## 11. Non-goals

P3-26I does not implement an incident management platform.

P3-26I does not create pager alerts.

P3-26I does not configure cloud monitoring.

P3-26I does not approve production go-live.

P3-26I does not replace legal/privacy review.

---

## 12. Acceptance criteria

P3-26I is complete when:

- this operational incident response runbook baseline exists;
- the incident response record template exists;
- the incident response verifier exists;
- the incident response contract tests exist;
- production deployment readiness references P3-26I;
- production observability references incident response;
- deployment evidence template references incident response escalation;
- repository governance validation includes the incident response verifier;
- dotnet build and dotnet test pass.