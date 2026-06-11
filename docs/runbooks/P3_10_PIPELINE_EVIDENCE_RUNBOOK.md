# P3.10 Pipeline Evidence Runbook

## Purpose

This runbook defines how future pipeline evidence must be collected for operational reporting, analytical snapshots, dashboard datasets, exports, and institutional evidence packages.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Evidence package metadata

Every pipeline evidence package must include:

- environment name.
- deployed commit SHA.
- responsible owner.
- date.
- pipeline id.
- pipeline name.
- pipeline type.
- source system.
- source time range.
- organization id.
- correlation id.
- request id.
- audit trail reference.
- status.
- blockers.

## Required scenarios

Required evidence scenarios:

1. operational reporting pipeline execution.
2. analytical snapshot pipeline execution.
3. dashboard dataset refresh.
4. CSV/XLSX export generation.
5. institutional evidence package generation.
6. quality monitoring report.
7. rejected records summary.
8. quarantine summary.
9. controlled data injection lineage.
10. metric lineage proof.

## Scenario validation

Each scenario must prove:

- source system.
- source time range.
- source filters.
- organization id.
- input record count.
- output record count.
- validation result.
- rejected records.
- quarantine count.
- audit trail reference.
- snapshot id or export id when applicable.

## Prohibited evidence content

Do not store:

- credentials.
- connection strings.
- real patient identifiers.
- raw clinical notes from real patients.
- raw SQL Server backups.
- unrestricted patient-level exports.
- secrets.

No secrets in repository.

## Sanitized evidence allowed

Allowed evidence:

- synthetic patient identifiers.
- aggregate counts.
- sanitized JSON.
- sanitized CSV structure.
- sanitized XLSX structure.
- sanitized dashboard dataset schema.
- source view names.
- test correlation id.
- test request id.
- test organization id.
- test snapshot id.
- test export id.
- data quality summary.

## Failure handling

If evidence is incomplete:

1. Stop.
2. Record blocker.
3. Record missing scenario.
4. Record responsible owner.
5. Do not claim backend closure.
6. Do not proceed as if KPI catalog is production-proven.

## P3.10 conclusion

Pipeline evidence must prove reporting lineage, privacy, security, and auditability.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE