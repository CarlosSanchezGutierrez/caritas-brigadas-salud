# P3.8 Controlled Data Injection Baseline

## Purpose

This document defines the controlled data injection baseline for Caritas Brigadas de Salud.

All operational data intake must be validated, auditable, idempotent, traceable, and compatible with SQL Server on-premise operation.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Core principles

- SQL Server is the operational source of truth.
- controlled data injection is mandatory for batch or external intake.
- No bypass of validation.
- No bypass of authentication.
- No bypass of consent requirements.
- No bypass of tenant or organization boundaries.
- No bypass of audit trail.
- No secrets in repository.
- No patient data in evidence artifacts.

## Required injection metadata

Every injection batch must include:

| Field | Purpose |
|---|---|
| batch id | Unique identifier for the injection attempt |
| source system | Origin of the data |
| source file/process reference | Sanitized reference to the file, process, or integration |
| operator | Person, service, or approved identity initiating the batch |
| validation status | Pending, accepted, partially accepted, rejected, quarantined |
| accepted records | Number of records accepted |
| rejected records | Number of records rejected |
| idempotency key | Stable key preventing duplicate processing |
| quarantine | Location or logical state for records requiring review |
| error details | Sanitized validation errors |
| traceability to domain records | Mapping to created or updated domain records |
| audit trail | Actor, timestamp, source, correlation id, operation, result |

## Required tokens

This phase requires explicit support for:

- controlled data injection.
- idempotency key.
- accepted records.
- rejected records.
- quarantine.
- audit trail.

## Batch lifecycle

1. Receive batch.
2. Assign batch id.
3. Capture source system.
4. Capture source file/process reference.
5. Capture operator.
6. Compute idempotency key.
7. Validate schema.
8. Validate authorization.
9. Validate consent and privacy boundaries where applicable.
10. Validate organization and tenant boundaries.
11. Separate accepted records and rejected records.
12. Move invalid records to quarantine.
13. Persist valid records.
14. Write audit trail.
15. Emit sanitized evidence summary.

## Rejection model

Rejected records must preserve enough sanitized context to support correction without exposing unnecessary patient data.

Rejection categories:

- Missing required field.
- Invalid format.
- Duplicate idempotency key.
- Unauthorized source system.
- Consent boundary violation.
- Tenant boundary violation.
- Unknown service reference.
- Invalid organization reference.
- Invalid date or time value.
- Unsafe payload.

## Quarantine model

quarantine is required when data cannot be accepted automatically but may be reviewed.

Quarantine records must include:

- batch id.
- source system.
- sanitized source reference.
- validation status.
- rejection reason.
- operator.
- timestamp.
- reviewer if later processed.
- final disposition.
- audit trail reference.

## Idempotency

idempotency key rules:

- Same source system and same logical payload must not create duplicate domain records.
- Replayed batches must return deterministic results.
- Batch retry must be safe.
- Partial failure must not silently duplicate accepted records.
- Idempotency evidence must be captured in smoke test or integration evidence when implemented.

## Traceability

Each accepted row must be traceable to:

- batch id.
- source system.
- operator.
- created domain record.
- validation result.
- audit trail entry.

Each rejected row must be traceable to:

- batch id.
- source system.
- rejection reason.
- quarantine state if applicable.
- reviewer decision if later corrected.

## Security restrictions

- No direct table import bypassing API/domain validation unless explicitly approved through an administrative ingestion path.
- No silent overwrite.
- No raw patient evidence in repository.
- No secrets in repository.
- No unapproved external AI processing.
- No external blockchain dependency.
- No cloud-only ingestion dependency.

## Evidence requirements

Controlled data injection evidence must include:

- Batch summary.
- accepted records count.
- rejected records count.
- quarantine count.
- idempotency key behavior.
- audit trail reference.
- health endpoint status if applicable.
- smoke test result if applicable.
- Responsible owner.
- Date.
- Blockers.