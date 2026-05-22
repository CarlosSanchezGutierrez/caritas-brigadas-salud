# P3.8 SQL Server On-Prem Operational Evidence

## Purpose

P3.8 defines the operational evidence baseline for SQL Server on-premise or institutional data center execution.

This phase moves the project from architecture-only documentation toward verifiable operational evidence for database deployment, migration execution, backup and restore, least privilege, health endpoint validation, smoke test execution, and controlled data injection.

This document does not claim real environment validation. It defines what must be proven before backend promotion.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Backend freeze status: NOT_FROZEN_PENDING_ON_PREM_EVIDENCE_AND_CONTRACTS

## Core decision

SQL Server is the operational source of truth.

The runtime connection key is:

- ConnectionStrings__SqlServer

## Scope

P3.8 covers:

- SQL Server on-premise target definition.
- Institutional or data center database baseline.
- Application runtime connection expectations.
- Migration execution evidence.
- Backup and restore evidence.
- Restore validation.
- Least privilege role separation.
- Health endpoint evidence.
- Smoke test evidence.
- Controlled data injection baseline.
- Evidence template for future real execution.

## Non-goals

P3.8 does not:

- Declare backend closure.
- Claim real SQL Server validation without artifacts.
- Store secrets.
- Require Azure, AWS, Vercel, or any cloud service.
- Introduce AI as a product dependency.
- Introduce blockchain as a product dependency.
- Store patient information in external systems by default.
- Bypass validation, consent, authorization, tenant boundaries, or audit trail requirements.

## Evidence required before promotion

A real deployment evidence package must include:

| Evidence area | Required proof | Acceptable artifact |
|---|---|---|
| SQL Server connectivity | Application reaches configured database through ConnectionStrings__SqlServer | Sanitized command output, health endpoint response, deployment log |
| Migration execution | Database schema is migrated from a known commit | Sanitized migration log, migration history table reference |
| Backup and restore | Backup exists and restore was tested | Sanitized backup job output, restore validation note |
| Restore validation | Restored database passes integrity and application smoke test | Sanitized integrity output, smoke test output |
| least privilege | Users are separated by operational responsibility | Sanitized role mapping, permission review |
| Health endpoint | Backend reports database dependency status | Sanitized HTTP response |
| Smoke test | Core API paths pass with expected status codes | Sanitized test output |
| Controlled data injection | Batch-level intake is validated, auditable, and idempotent | Sanitized batch evidence |

## Minimum operational gates

The backend remains blocked until these gates have real evidence:

1. Database connection through ConnectionStrings__SqlServer.
2. Migration execution against the intended SQL Server on-premise target.
3. Backup and restore procedure executed at least once.
4. Restore validation completed.
5. least privilege proof for app runtime user, migration user, read-only reporting user, backup/operator user, and auditor user.
6. health endpoint returns expected dependency status.
7. smoke test confirms API and database behavior.
8. controlled data injection proves accepted records, rejected records, quarantine, idempotency key, and audit trail.

## Evidence integrity rules

- No secrets in repository.
- No patient personal data in evidence.
- No raw database dumps in repository.
- No screenshots containing credentials.
- No fabricated logs.
- No manual edits to evidence pretending to be execution output.
- Evidence must include responsible person, environment, date, commit SHA, and blocker status.

## P3.8 conclusion

P3.8 creates the evidence baseline only.

Backend promotion remains blocked until real SQL Server on-premise operational artifacts exist.