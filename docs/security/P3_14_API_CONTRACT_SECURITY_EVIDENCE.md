# P3.14 API Contract Security Evidence

## Purpose

This document defines the security evidence expected for API contract, OpenAPI, and future client stubs.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Security evidence principle

API contract artifacts must preserve security boundaries.

OpenAPI and generated clients must not weaken authentication, authorization, privacy, auditability, organization scope, or sync safety.

## Required security evidence

Required security evidence:

- authentication requirement per protected endpoint.
- authorization role per protected endpoint.
- organization id requirement per scoped endpoint.
- request id preservation.
- correlation id preservation.
- device id requirement for mobile/offline sync.
- idempotency key requirement for replay-protected writes.
- audit trail reference for accepted writes.
- standard error envelope for security failures.
- rate limit expectation.
- sensitive data classification when applicable.
- patient-level export governance when applicable.

## Security failure evidence

Security failure evidence must cover:

- unauthenticated protected request.
- unauthorized role.
- cross-organization request.
- missing organization id.
- missing device id for mobile sync.
- missing idempotency key for offline write.
- replay with changed payload.
- rejected export request.
- standard error envelope response.

## OpenAPI security requirements

OpenAPI must document:

- security scheme.
- protected route requirements.
- standard error envelope.
- status codes.
- request metadata.
- sensitive data notes when applicable.

## Client stub security boundary

Client stubs must not:

- store secrets in repository.
- bypass API security.
- remove organization id.
- remove request id.
- remove correlation id.
- hide authorization failures.
- retry unsafe writes without idempotency key.
- accept conflict responses as success.

## P3.14 conclusion

API contract evidence must preserve security rules across OpenAPI and future client stubs.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE