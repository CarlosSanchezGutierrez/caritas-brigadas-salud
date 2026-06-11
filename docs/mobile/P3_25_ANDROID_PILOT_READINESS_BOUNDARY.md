# P3.25 Android Pilot Readiness Boundary

## Purpose

This document defines the Android pilot readiness boundary.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Android pilot readiness status: BLOCKED_PENDING_REAL_EVIDENCE

## Android pilot readiness evidence

Required evidence:

- approved release candidate reference.
- artifact reference.
- deployed commit SHA.
- environment name.
- build profile.
- release channel.
- API contract version.
- OpenAPI artifact reference.
- pilot site or brigade scope.
- pilot participant scope.
- pilot device inventory.
- UAT acceptance criteria.
- offline field workflow evidence.
- sync dry run evidence.
- training evidence.
- support diagnostic evidence.
- privacy-safe telemetry evidence.
- rollback plan.
- incident response plan.
- support escalation plan.

## Android metadata evidence

The Android pilot must preserve request id, correlation id, organization id, authorization role, standard error envelope, audit trail reference, device id, idempotency key, client operation id, sync status, server acknowledgment, conflict id, and support diagnostic evidence.

## Android blocked pilot behavior

The Android client must not write directly to SQL Server, bypass the API, bypass authorization, bypass audit trail creation, sync without device id, sync without idempotency key, drop client operation id, drop server acknowledgment, silently overwrite conflicts, expand pilot scope without approval, or treat pilot readiness as production approval.

## P3.25 conclusion

The Android pilot must remain blocked until controlled pilot readiness evidence is complete.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
