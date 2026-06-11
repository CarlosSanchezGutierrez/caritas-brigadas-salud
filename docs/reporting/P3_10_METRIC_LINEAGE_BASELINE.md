# P3.10 Metric Lineage Baseline

## Purpose

This document defines minimum metric lineage requirements before detailed KPI catalog work.

P3.10 does not finalize the KPI catalog. That belongs to P3.11.

P3.10 defines how metrics must trace back to operational sources, snapshots, and audit trail evidence.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Metric lineage principle

Every metric must be traceable.

A metric must define:

- metric id
- metric name
- metric owner
- metric purpose
- source system
- source table or source view
- source time range
- filter logic
- aggregation logic
- organization id scope
- snapshot id when applicable
- pipeline id
- pipeline version
- data quality rules
- audit trail reference

## Baseline metric groups

| Group | Examples |
|---|---|
| operational volume | patients served, encounters completed, services provided |
| clinical operations | vital signs captured, referrals created, follow-up recommendations |
| consent and legal | consent captured, consent revoked, privacy notice version |
| data quality | missing fields, rejected records, quarantine, correction event counts |
| longitudinal history | repeat visits, encounter timeline depth, patient timeline updates |
| logistics and capacity | brigade activity, service availability, resource utilization |
| reporting and exports | exports generated, dashboard refreshes, evidence packages |
| security and governance | denied actions, role changes, suspicious requests |

## Metric quality rules

Each metric must define:

- numerator.
- denominator when applicable.
- inclusion criteria.
- exclusion criteria.
- date logic.
- organization scope.
- refresh cadence.
- data quality caveats.
- owner.

## Metric reproducibility

A metric value must be reproducible from:

- source snapshot id or source query reference.
- pipeline id.
- pipeline version.
- source time range.
- organization id.
- filter logic.
- aggregation logic.
- audit trail reference.

## Metric correction handling

When source data is corrected:

- old metric evidence must not be silently overwritten.
- refreshed metric output must reference the new snapshot.
- correction event must remain auditable.
- dashboard dataset refresh must preserve lineage.

No silent overwrite is allowed.

## P3.10 boundary with P3.11

P3.10 defines metric lineage.

P3.11 will define the formal KPI, dashboard, insight, and direction reporting catalog.

## Metric lineage conclusion

Metrics must be explainable, reproducible, auditable, and scoped.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE