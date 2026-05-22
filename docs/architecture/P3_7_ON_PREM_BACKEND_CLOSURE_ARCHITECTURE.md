# P3.7 On-Prem Backend Closure Architecture

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Backend freeze status: NOT_FROZEN_PENDING_ON_PREM_EVIDENCE_AND_CONTRACTS

## Purpose

P3.7 defines the backend closure architecture required before frontend clients can move fast without creating backend debt.

The backend must support Caritas operations in an on-premise context where SQL Server is the operational source of truth, the institution may operate from its own data center, and data ingestion may happen through controlled data injection processes.

## Core decisions

SQL Server is the operational source of truth.

The production backend must not require cloud infrastructure to operate.

Cloud services may exist only as optional future adapters. They must not be required for MVP operation, clinical capture, reporting, auditability, backups, restore, or frontend access.

## Required backend closure domains

### On-prem runtime

The backend must support internal network deployment, institutional reverse proxy or gateway, SQL Server connectivity, environment-based configuration, health endpoints, structured logs, correlation IDs, and operational runbooks.

### Data injection

Data injection must include batch identity, source reference, operator identity, validation status, rejected records, accepted records, idempotency key, traceability, and audit trail.

Data injection must not bypass validation, tenant boundaries, consent rules, authorization, or audit logging.

### Total auditability

Every significant action must record actor, role, organization scope, timestamp, action, entity type, entity identifier, correlation ID, request ID, client identity, workflow context, and before or after state reference when applicable.

### Longitudinal history

Longitudinal history must support patient identity events, consent events, brigades attended, encounters, services received, vital signs, form responses, referrals, medication delivery, follow-up notes, document events, correction events, and merge or deduplication events when applicable.

### Offline-first sync

Offline-first sync must support device identity, local event queue, idempotency keys, sync batches, event sequence, server acceptance or rejection, deterministic conflict policy, retry policy, failed batch diagnostics, and audit trail.

### Operational pipeline

The operational pipeline must support capture, validation, SQL Server persistence, audit logs, operational read models, exports, direction-level reports, data quality monitoring, incomplete workflow monitoring, service coverage monitoring, and brigade throughput monitoring.

### Analytical pipeline

The analytical pipeline must be separated from operational transactions. It must support governed snapshots, de-identification or aggregation, indicators, time-series analysis, territorial analysis, clinical statistics, vulnerability and needs mapping, data quality metrics, research-ready longitudinal datasets, reproducible extraction windows, and export logs.

### KPIs, insights, dashboards, and monitoring

The backend must support KPIs, insights, dashboards, indicators, direction reporting, operational reports, monitoring, clinical operations, social vulnerability, service demand, service coverage, patient follow-up, brigade performance, data quality, security, audit, and system health.

### Vulnerability and social needs map

The backend must prepare aggregated territorial data, service demand, clinical indicators, location hierarchy, risk categories, priority scoring, temporal trends, confidence scoring, ethical aggregation rules, and small population suppression.

### Advanced clinical statistics

The backend must prepare governed data structures for longitudinal follow-up, cohort-like analysis, service recurrence, risk factor tracking, referral completion, support continuity, data completeness scoring, clinical monitoring indicators, and research lab exports.

### AI API Gateway readiness

AI API Gateway is deferred. The backend must prepare only an adapter boundary, audit policy, approved use-case registry, human review rule, and privacy-safe logging policy.

No raw patient data may be sent to external AI providers by default.

### Blockchain or crypto-audit readiness

Blockchain is deferred. The backend may prepare future crypto-audit concepts such as hash chains, Merkle batch roots, external anchoring adapter, and verification procedure.

No patient data may be stored on-chain. No cryptocurrency dependency is allowed.

## Frontend readiness rule

Frontend clients may move fast only after API endpoints are versioned, OpenAPI is stable, the error model is stable, pagination and filtering are stable, auth behavior is stable, offline sync contracts are stable, audit expectations are stable, mock data exists, and backend freeze status is explicitly approved.
