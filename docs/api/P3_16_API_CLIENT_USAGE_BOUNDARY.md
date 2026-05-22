# P3.16 API Client Usage Boundary

## Purpose

This document defines how Web iOS Android clients may use the API during implementation.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## API client usage rules

Every client must use a controlled API client boundary.

Every API call must preserve:

- API contract version.
- request id.
- correlation id.
- organization id.
- authentication requirement.
- authorization role.
- standard error envelope.
- audit trail reference when applicable.
- device id when mobile.
- idempotency key when offline sync is involved.
- client operation id when offline sync is involved.

## API usage allowed scope

Allowed API usage includes contract-backed reads, contract-backed writes, standard error envelope handling, organization-scoped calls, role-aware calls, idempotent mobile sync calls, and audit-aware accepted writes.

## API usage blocked scope

Blocked API usage includes undocumented endpoints, unscoped patient requests, unaudited accepted writes, direct database access, silent conflict overwrite, missing idempotency key for offline sync, missing device id for mobile sync, and mocked API behavior treated as evidence.

## P3.16 conclusion

API usage during client implementation must remain versioned, typed, scoped, audited, and evidence-backed.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
