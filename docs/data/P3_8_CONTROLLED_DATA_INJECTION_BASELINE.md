# P3.8 Controlled Data Injection Baseline

## Purpose

This document defines the controlled data injection baseline for SQL Server on-premise operation.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Core principles

- SQL Server is the operational source of truth.
- controlled data injection is mandatory for batch or external intake.
- No bypass of validation.
- No bypass of authentication.
- No bypass of consent requirements.
- No bypass of tenant boundaries.
- No bypass of audit trail.
- No secrets in repository.

## Required metadata

Every injection batch must include:

| Field | Purpose |
|---|---|
| batch id | Unique identifier for injection attempt |
| source system | Origin of data |
| source file/process reference | Sanitized source reference |
| operator | Approved identity initiating batch |
| validation status | Pending, accepted, partially accepted, rejected, quarantined |
| accepted records | Records accepted |
| rejected records | Records rejected |
| idempotency key | Stable duplicate-prevention key |
| quarantine | Review state for invalid records |
| error details | Sanitized validation errors |
| traceability to domain records | Mapping to created or updated records |
| audit trail | Actor, timestamp, correlation id, operation, result |

## Batch lifecycle

1. Receive batch.
2. Assign batch id.
3. Capture source system.
4. Capture operator.
5. Compute idempotency key.
6. Validate schema.
7. Validate authorization.
8. Validate consent boundaries.
9. Separate accepted records and rejected records.
10. Move invalid records to quarantine.
11. Persist valid records.
12. Write audit trail.
13. Emit sanitized evidence summary.

## Required evidence

- controlled data injection batch summary.
- accepted records count.
- rejected records count.
- quarantine count.
- idempotency key behavior.
- audit trail reference.
- health endpoint status if applicable.
- smoke test result if applicable.