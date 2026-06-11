# P3.11 KPI Definition Catalog

## Purpose

This document defines baseline KPI groups and required metric definitions.

The final values are not asserted in this phase.

P3.11 defines the metric contract.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## KPI definition rule

Every KPI must have:

- metric id
- metric owner
- metric definition
- numerator
- denominator
- aggregation logic
- filter logic
- source snapshot id
- pipeline id
- pipeline version
- organization id
- refresh cadence
- data quality caveats
- audit trail reference
- decision owner
- action recommendation

## KPI group 1: Operational volume

| Metric | Purpose |
|---|---|
| patients served | Count patients attended in selected period |
| encounters completed | Count completed encounters |
| services provided | Count services delivered |
| brigades executed | Count brigades executed |
| service availability | Count active services by brigade |

Required lineage:

- source snapshot id.
- pipeline id.
- pipeline version.
- organization id.
- audit trail reference.

## KPI group 2: Clinical operations

| Metric | Purpose |
|---|---|
| vital signs captured | Monitor clinical capture completeness |
| referrals created | Track patients referred to external or follow-up care |
| medication records created | Track medication documentation |
| follow-up recommendations | Track continuity of care |
| clinical correction events | Track clinical data corrections |

Required controls:

- correction event awareness.
- patient timeline compatibility.
- encounter timeline compatibility.
- clinical timeline compatibility.
- audit trail reference.

## KPI group 3: Consent and legal

| Metric | Purpose |
|---|---|
| consent captured | Track privacy notice and consent capture |
| consent version distribution | Track version used |
| consent revoked | Track revocation events |
| consent exceptions | Track governed exceptions |
| privacy notice updates | Track legal/document changes |

Required controls:

- consent timeline.
- privacy notice version.
- audit trail reference.
- organization id.

## KPI group 4: Data quality

| Metric | Purpose |
|---|---|
| missing required fields | Track incomplete records |
| rejected records | Track failed validation |
| quarantine count | Track records pending review |
| duplicate candidates | Track deduplication workload |
| correction event count | Track corrected records |
| controlled data injection errors | Track injection quality |

Required controls:

- rejected records.
- quarantine count.
- controlled data injection lineage.
- audit trail reference.

## KPI group 5: Longitudinal history

| Metric | Purpose |
|---|---|
| repeat visits | Track patient continuity |
| encounters per patient | Track timeline depth |
| identity enrichments | Track partial identity improvements |
| referral follow-up events | Track continuity support |
| merge and deduplication events | Track identity governance |

Required controls:

- patient timeline.
- consent timeline.
- encounter timeline.
- clinical timeline.
- merge and deduplication.
- audit trail reference.

## KPI group 6: Logistics and capacity

| Metric | Purpose |
|---|---|
| brigade utilization | Track activity by brigade |
| service utilization | Track service demand |
| resource availability | Track available operational resources |
| location coverage | Track geographic or institutional coverage |
| operational bottlenecks | Track capacity issues |

Required controls:

- organization id.
- source snapshot id.
- pipeline version.
- audit trail reference.

## KPI group 7: Reporting and governance

| Metric | Purpose |
|---|---|
| dashboard refreshes | Track dataset freshness |
| CSV/XLSX export count | Track report output |
| evidence package count | Track institutional evidence |
| denied actions | Track authorization behavior |
| role changes | Track privileged governance changes |

Required controls:

- dashboard id.
- export id.
- evidence package.
- audit trail reference.

## KPI conclusion

Every KPI must be explainable, reproducible, scoped, and actionable.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE