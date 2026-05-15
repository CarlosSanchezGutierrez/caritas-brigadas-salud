# P3 Production Observability Baseline

Status: active
Scope: production observability, diagnostics, and incident evidence
Target phase: P3-26D
Depends on: P3-26C SQL Server integration smoke test baseline

---

## 1. Purpose

P3-26D defines the minimum observability baseline required before production deployment.

This baseline does not implement a telemetry provider.

It defines the required operational evidence and guardrails for logs, health checks, traces, metrics, alerts, incident response, and deployment verification.

---

## 2. Production observability status

Production observability status: blocked.

Production go-live remains blocked until the backend has verifiable coverage for:

- health endpoint;
- structured application logs;
- request correlation identifier;
- error correlation identifier;
- deployment smoke evidence;
- database connectivity signal;
- authentication failure visibility;
- authorization failure visibility;
- sync processing failure visibility;
- critical exception visibility;
- rate limiting visibility;
- operational owner;
- incident response checklist;
- post-deployment monitoring checklist.

---

## 3. Required health signals

Production must expose health evidence for:

- API process availability;
- application startup completion;
- database connectivity;
- dependency configuration validity;
- deployment version visibility;
- environment name visibility;
- degraded status classification;
- unhealthy status classification.

The health endpoint must not leak secrets, connection strings, tokens, stack traces, or sensitive configuration values.

---

## 4. Required logging posture

Production logs must be structured.

Every production log event related to requests, sync processing, authentication, authorization, database errors, and unhandled exceptions must include enough context to investigate the incident without exposing sensitive payloads.

Required log fields:

- timestamp UTC;
- log level;
- event name or event id;
- correlation id;
- request id;
- organization id when safely available;
- user id when safely available;
- endpoint route;
- HTTP method;
- response status code;
- elapsed milliseconds;
- exception type when applicable;
- sanitized error message.

Logs must not include:

- passwords;
- bearer tokens;
- connection strings;
- raw PayloadJson;
- patient names;
- phone numbers;
- CURP or equivalent national identifiers;
- clinical free-text content;
- raw request bodies from sync payloads.

---

## 5. Required tracing posture

Production tracing must support:

- request-level correlation;
- propagation of correlation id;
- database operation timing;
- sync batch processing timing;
- failed event processing timing;
- endpoint latency investigation;
- error root cause investigation.

Tracing must not record raw sensitive payload data.

---

## 6. Required metrics posture

Production metrics must support:

- request count;
- request duration;
- error rate;
- health status;
- authentication failures;
- authorization failures;
- rate limited requests;
- sync batches received;
- sync batches processed;
- sync events accepted;
- sync events rejected;
- sync events conflicted;
- SQL Server migration smoke status.

---

## 7. Required alerting posture

Production alerting must cover:

- API unavailable;
- database unavailable;
- sustained 5xx responses;
- sustained authentication failures;
- sustained authorization failures;
- high sync rejection rate;
- failed sync batch processing;
- dependency review failure;
- database migration smoke failure;
- health endpoint unhealthy status.

---

## 8. Required incident response evidence

Every production incident must capture:

- incident id;
- timestamp UTC;
- severity;
- affected environment;
- affected endpoint;
- correlation ids;
- first detection source;
- user impact;
- suspected root cause;
- mitigation action;
- rollback decision;
- responsible owner;
- follow-up action.

---

## 9. Required deployment monitoring checklist

Every production deployment must include:

- pre-deployment health check;
- database smoke evidence;
- deployment commit SHA;
- deployment timestamp UTC;
- post-deployment health check;
- post-deployment smoke test;
- log review window;
- error rate review;
- latency review;
- rollback decision point;
- approval record.

---

## 10. Required follow-up workstreams

After P3-26D, implementation work should continue with:

- P3-26E health endpoint and deployment smoke implementation;
- P3-26F structured logging and correlation id implementation;
- P3-26G production CORS and rate limiting validation;
- P3-26H deployment evidence template;
- P3-26I operational incident response runbook.

---

## 11. Non-goals

P3-26D does not implement OpenTelemetry.

P3-26D does not implement Serilog or another logging provider.

P3-26D does not create cloud alerts.

P3-26D does not approve production go-live.

P3-26D does not replace staging validation.

P3-26D does not expose sensitive diagnostic data.

---

## 12. Acceptance criteria

P3-26D is complete when:

- this production observability baseline exists;
- the production observability verifier exists;
- the production observability contract test exists;
- production deployment readiness references P3-26D;
- repository governance validation includes the observability verifier;
- production go-live remains blocked until observability implementation is complete;
- dotnet build and dotnet test pass.
---

## 16. Deployment evidence record

Production observability evidence must be captured in the deployment evidence record for every release.

The record must include health endpoint evidence, structured logging evidence, correlation id evidence, request telemetry evidence, post-deployment log review, error rate review, latency review, incident owner, and escalation contact.
