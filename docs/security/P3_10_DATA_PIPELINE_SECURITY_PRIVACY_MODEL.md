# P3.10 Data Pipeline Security and Privacy Model

## Purpose

This document defines security and privacy boundaries for operational and analytical data pipelines.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Security principle

Pipelines must not weaken the security model established for operations, auditability, longitudinal history, and SQL Server on-premise execution.

## Required controls

Required controls:

- No secrets in repository.
- No cloud dependency.
- SQL Server is the operational source of truth.
- organization id is mandatory.
- audit trail is mandatory.
- correlation id is mandatory.
- request id is mandatory.
- least privilege is mandatory.
- read-only reporting user is required for reporting reads.
- app runtime user must not run analytical administration.
- export generation must be auditable.
- dashboard dataset refresh must be auditable.
- evidence package generation must be auditable.

## Privacy controls

Pipelines must enforce:

- minimum necessary data.
- organization-scoped output.
- aggregate-first reporting.
- patient-level export governance.
- sensitive field classification.
- redaction where applicable.
- consent boundary awareness.
- no raw secrets.
- no unnecessary raw clinical notes.
- no unrestricted identifiers in research outputs.

## Pipeline identities

| Identity | Purpose | Forbidden |
|---|---|---|
| app runtime user | Operational API execution | Analytical administration, unrestricted exports |
| read-only reporting user | Reporting and dashboards | Writes, deletes, schema changes |
| migration user | Controlled schema migration | Runtime API use, dashboard refresh |
| auditor/read-only security user | Audit review | Operational writes |
| export operator | Approved export execution | Cross-organization export without approval |

## Threat model

| Threat | Control |
|---|---|
| Unauthorized export | Role-based approval and audit trail |
| Cross-organization data leakage | organization id filtering |
| Sensitive data leakage | minimization and classification |
| Dashboard bypassing API governance | read-only reporting user and governed views |
| Pipeline modifying operational data | read-only analytical path |
| Snapshot tampering | snapshot id, pipeline version, audit trail |
| Silent metric overwrite | snapshot lineage and no silent overwrite |
| Secret leakage | No secrets in repository |

## Evidence required later

Future evidence must prove:

- reporting uses read-only reporting user.
- exports are auditable.
- dashboard dataset refresh is auditable.
- organization id filtering exists.
- data minimization is documented.
- rejected records and quarantine are tracked.
- controlled data injection lineage exists.
- snapshots have audit trail references.
- no secrets are committed.

## P3.10 conclusion

Pipeline security and privacy remain hard backend closure requirements.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE