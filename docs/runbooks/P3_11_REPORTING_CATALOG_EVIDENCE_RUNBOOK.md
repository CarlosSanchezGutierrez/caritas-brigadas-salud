# P3.11 Reporting Catalog Evidence Runbook

## Purpose

This runbook defines how future evidence must be collected for KPI, dashboard, insight, and direction reporting catalog validation.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Evidence package metadata

Every evidence package must include:

- environment name.
- deployed commit SHA.
- responsible owner.
- date.
- report owner.
- organization id.
- source snapshot id.
- pipeline id.
- pipeline version.
- dashboard id.
- metric id.
- insight id when applicable.
- direction report id when applicable.
- evidence package reference.
- audit trail reference.
- status.
- blockers.

## Required scenarios

Required evidence scenarios:

1. KPI catalog entry has complete metric definition.
2. KPI value traces to source snapshot id.
3. Dashboard catalog entry references approved metric ids.
4. Dashboard dataset references pipeline id and pipeline version.
5. Insight references supporting metric ids.
6. Insight includes action recommendation.
7. Direction report includes executive summary.
8. Direction report includes data quality caveats.
9. CSV/XLSX export is governed and auditable.
10. Evidence package is sanitized.
11. Rejected records and quarantine appear in quality reporting.
12. No silent overwrite occurs after refreshed snapshot.

## Scenario validation

Each scenario must prove:

- metric id.
- metric owner.
- metric definition.
- source snapshot id.
- pipeline id.
- pipeline version.
- organization id.
- audit trail reference.
- data quality caveats.
- decision owner when applicable.
- action recommendation when applicable.

## Prohibited evidence content

Do not store:

- credentials.
- connection strings.
- secrets.
- unrestricted patient identifiers.
- raw clinical notes from real patients.
- ungoverned patient-level exports.
- raw database dumps.
- unredacted screenshots.

No secrets in repository.

## Sanitized evidence allowed

Allowed evidence:

- synthetic patient identifiers.
- aggregate metrics.
- sanitized dashboard dataset schema.
- sanitized CSV/XLSX structure.
- sanitized evidence package reference.
- metric definition excerpts.
- test snapshot id.
- test dashboard id.
- test metric id.
- test insight id.
- test direction report id.
- audit trail reference.

## Failure handling

If evidence is incomplete:

1. Stop.
2. Record blocker.
3. Record missing scenario.
4. Record responsible owner.
5. Do not claim backend closure.
6. Do not proceed as if reporting is institutionally proven.

## P3.11 conclusion

Reporting catalog evidence must prove lineage, governance, and decision usefulness.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE