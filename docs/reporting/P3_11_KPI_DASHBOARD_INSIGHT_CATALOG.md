# P3.11 KPI, Dashboard, Insight and Direction Reporting Catalog

## Purpose

P3.11 defines the formal KPI catalog, dashboard catalog, insight catalog, and direction reporting baseline for Caritas Brigadas de Salud.

This phase does not implement final dashboards.

It defines the reporting language that leadership, operations, clinical teams, social support teams, technology teams, and institutional partners can use consistently.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Backend freeze status: NOT_FROZEN_PENDING_ON_PREM_EVIDENCE_AND_CONTRACTS

## Core principle

SQL Server is the operational source of truth.

Reports, dashboards, KPIs, and insights must derive from governed snapshots, metric lineage, audit trail references, and approved pipeline outputs.

## Catalog layers

| Layer | Purpose |
|---|---|
| KPI catalog | Defines stable metrics, formulas, owners, and interpretation |
| dashboard catalog | Defines visual reporting surfaces and target users |
| insight catalog | Defines how observations become evidence-backed action recommendations |
| direction reporting | Defines executive, board, donor, partner, and institutional report structure |
| evidence package | Defines sanitized outputs for institutional proof |

## Required KPI metadata

Every KPI must define:

- metric id
- metric owner
- metric definition
- metric purpose
- numerator
- denominator
- aggregation logic
- filter logic
- organization id scope
- source snapshot id
- pipeline id
- pipeline version
- refresh cadence
- data quality caveats
- audit trail reference
- decision owner
- intended action

## Required dashboard metadata

Every dashboard must define:

- dashboard id
- dashboard owner
- dashboard audience
- dashboard purpose
- metric ids
- source snapshot id
- source pipeline id
- refresh cadence
- organization id scope
- access role
- export permissions
- audit trail reference

## Required insight metadata

Every insight must define:

- insight id
- insight owner
- insight statement
- supporting metric ids
- source snapshot id
- evidence package reference
- decision owner
- action recommendation
- confidence level
- limitations
- audit trail reference

## Direction reporting levels

| Level | Audience | Purpose |
|---|---|---|
| operational report | Brigade coordinators and operational staff | Daily or field-level decisions |
| tactical report | Program leaders and managers | Weekly or monthly resource planning |
| strategic report | Direction, board, donors, partners | Institutional decisions and accountability |
| research report | Academic, ODS, impact, causal, Bayesian, or statistical work | Governed learning and evidence generation |

## Guardrails

- No secrets in repository.
- No cloud dependency.
- No silent overwrite.
- No unaudited report export.
- No dashboard without metric lineage.
- No KPI without owner.
- No insight without evidence.
- No patient-level external report without governance.
- No direct dashboard write into operational clinical tables.
- No metric that bypasses organization id scope.
- No report that bypasses consent or privacy boundaries.

## Relationship with P3.10

P3.10 defines pipeline lineage.

P3.11 defines the catalog of metrics, dashboards, insights, and direction reporting outputs that use those pipelines.

## P3.11 conclusion

The KPI, dashboard, insight, and direction reporting catalog is required before offline-first sync and API contract freeze.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE