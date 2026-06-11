# P3.16 Client Implementation Security Boundary

## Purpose

This document defines the security boundary for Web iOS Android implementation kickoff.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Security boundary

Client implementation must preserve authentication, authorization, organization scope, auditability, privacy, and offline sync safety.

## Required security behavior

Required behavior:

- protected screens require authenticated context.
- role-sensitive actions require authorization role.
- scoped data requires organization id.
- accepted writes require audit trail reference.
- mobile sync requires device id.
- offline sync requires idempotency key.
- conflicts require explicit conflict handling.
- standard error envelope must be shown or handled.
- request id must be preserved.
- correlation id must be preserved.

## Blocked security behavior

Blocked behavior:

- bypassing the API.
- bypassing authorization.
- bypassing organization scope.
- bypassing audit trail creation.
- storing secrets in repository.
- storing real patient data in fixtures.
- silent conflict overwrite.
- unrestricted export behavior.
- unaudited patient-level access.

## P3.16 conclusion

Client implementation security must be designed before feature coding begins.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
