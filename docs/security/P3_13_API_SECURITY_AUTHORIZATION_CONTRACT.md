# P3.13 API Security and Authorization Contract

## Purpose

This document defines API security and authorization rules for Web, iOS, Android, reporting, administration, and offline sync.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Security principle

The API must enforce server-side security, organization scope, role-based authorization, auditability, and privacy boundaries.

## Required security metadata

Every protected endpoint must define:

- authentication requirement.
- authorization role.
- organization id requirement.
- user role.
- actor.
- request id.
- correlation id.
- device id when applicable.
- API version.
- audit trail requirement.
- rate limit expectation.
- sensitive data classification when applicable.

## Authorization rules

Required rules:

- No unauthenticated API access to protected resources.
- No cross-organization access.
- No write endpoint without authorization.
- No privileged action without audit trail.
- No role change without audit trail.
- No export without governance.
- No sync acceptance without server validation.
- No accepted write without organization id.
- No accepted mobile sync without device id.
- No accepted offline write without idempotency key.

## Privacy rules

API responses must follow:

- minimum necessary data.
- organization-scoped data.
- aggregate-first external reporting.
- patient-level export governance.
- sensitive field classification.
- no raw secrets.
- no unnecessary raw clinical note exposure.
- consent boundary awareness.

## Rate limit and abuse controls

Endpoints must define:

- rate limit expectation.
- retry behavior.
- error category.
- request id.
- correlation id.
- audit event when applicable.

## Evidence required later

Future evidence must prove:

- unauthorized protected access fails.
- cross-organization access fails.
- write endpoint requires authorization.
- mobile sync requires device id.
- offline write requires idempotency key.
- protected endpoint returns standard error envelope.
- accepted write returns audit trail reference.
- export endpoint is governed and auditable.

## P3.13 conclusion

API security rules must be part of the contract, not added after client implementation.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE