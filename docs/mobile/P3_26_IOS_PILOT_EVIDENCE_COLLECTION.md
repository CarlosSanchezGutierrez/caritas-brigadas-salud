# P3.26 iOS Pilot Evidence Collection

## Purpose

This document defines iOS pilot evidence collection.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

iOS pilot evidence collection status: BLOCKED_PENDING_REAL_EVIDENCE

## Required iOS evidence

Required evidence:

- approved pilot readiness reference.
- approved release candidate reference.
- artifact reference.
- deployed commit SHA.
- environment name.
- API contract version.
- pilot site or brigade scope.
- pilot participant scope.
- pilot device inventory.
- UAT execution evidence.
- workflow completion evidence.
- offline field workflow evidence.
- sync dry run evidence.
- sync reconciliation evidence.
- field feedback evidence.
- support ticket evidence.
- incident evidence.
- defect triage evidence.
- observability evidence.
- privacy-safe telemetry evidence.
- audit trail reference evidence.
- rollback decision evidence.

## iOS metadata evidence

The iOS pilot evidence must preserve request id, correlation id, organization id, authorization role, standard error envelope, audit trail reference, device id, idempotency key, client operation id, sync status, server acknowledgment, conflict id, support diagnostic evidence, and evidence sanitization status.

## iOS blocked evidence behavior

The iOS evidence package must not write directly to SQL Server, bypass the API, bypass authorization, bypass audit trail creation, sync without device id, sync without idempotency key, drop client operation id, drop server acknowledgment, silently overwrite conflicts, expand pilot scope without approval, or treat pilot evidence review as production approval.

## P3.26 conclusion

iOS pilot evidence must be reviewed before iOS readiness can advance.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
