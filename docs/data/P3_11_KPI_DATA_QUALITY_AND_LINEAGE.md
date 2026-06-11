# P3.11 KPI Data Quality and Lineage

## Purpose

This document defines data quality and lineage requirements for KPI, dashboard, insight, and direction reporting outputs.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Lineage principle

Every KPI value must be traceable from direction reporting back to source operational records.

Traceability path:

1. direction report.
2. insight.
3. dashboard.
4. KPI.
5. metric definition.
6. source snapshot id.
7. pipeline id.
8. pipeline version.
9. source system.
10. source tables or source views.
11. operational SQL Server record reference.
12. audit trail reference.

SQL Server is the operational source of truth.

## Required data quality fields

Every metric output must preserve:

- metric id.
- source snapshot id.
- pipeline id.
- pipeline version.
- organization id.
- source time range.
- input record count.
- output record count.
- rejected records.
- quarantine count.
- validation result.
- data quality caveats.
- audit trail reference.

## Quality dimensions

| Dimension | Meaning |
|---|---|
| completeness | Required fields are present |
| validity | Values pass validation rules |
| consistency | Values match domain expectations |
| timeliness | Data is fresh enough for intended use |
| traceability | Metric can be traced to source |
| auditability | Metric has audit trail reference |
| minimization | Output uses minimum necessary data |
| scope correctness | organization id filtering is correct |

## KPI caveats

Each KPI must define caveats when applicable:

- incomplete capture.
- partial identity.
- missing consent.
- rejected records.
- quarantine.
- delayed synchronization.
- correction event after snapshot.
- merge and deduplication after snapshot.
- source time range limitations.

## Correction handling

When source data changes after correction:

- metric output must not be silently overwritten.
- refreshed output must reference a new snapshot.
- old output must preserve its source snapshot id.
- correction event must remain auditable.
- insight must disclose caveat if relevant.

No silent overwrite is allowed.

## Data quality reporting

Data quality reporting must include:

- rejected records.
- quarantine count.
- missing field counts.
- correction event count.
- duplicate candidate count.
- controlled data injection errors.
- stale snapshot indicators.
- validation result.

## P3.11 conclusion

KPI outputs are only useful when data quality and lineage are explicit.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE