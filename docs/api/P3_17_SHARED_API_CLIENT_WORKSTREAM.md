# P3.17 Shared API Client Workstream

## Purpose

This document defines the shared API client workstream used by Web iOS Android implementation planning.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Shared API client workstream status: BLOCKED_PENDING_REAL_EVIDENCE

## Shared API client scope

The shared API client workstream defines common behavior that every client must preserve even when implementation language differs.

## Required shared behavior

Every client API boundary must preserve:

- API contract version.
- endpoint id.
- request schema.
- response schema.
- standard error envelope.
- authentication requirement.
- authorization role.
- organization id.
- request id.
- correlation id.
- audit trail reference when applicable.
- device id when mobile.
- idempotency key when offline sync is involved.
- client operation id when offline sync is involved.
- sync status when offline sync is involved.

## Shared blocked behavior

The shared API client workstream must prevent undocumented endpoints, direct database access, missing organization scope, missing request id, missing correlation id, missing standard error envelope handling, missing idempotency key for offline sync, missing device id for mobile sync, and silent conflict overwrite.

## P3.17 conclusion

Shared API client behavior must be consistent across Web iOS Android before feature implementation expands.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
