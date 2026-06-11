# P3.16 Client Implementation Definition of Ready and Definition of Done

## Purpose

This document defines Definition of Ready and Definition of Done for Web iOS Android implementation kickoff.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Definition of Ready

A client feature is ready to implement only when:

- endpoint integration status exists.
- API contract version exists.
- OpenAPI contract evidence is referenced.
- client stub baseline is referenced.
- request schema is documented.
- response schema is documented.
- standard error envelope is documented.
- authentication requirement is documented.
- authorization role is documented.
- organization id requirement is documented.
- request id is required.
- correlation id is required.
- audit trail reference is required when applicable.
- device id is required when mobile.
- idempotency key is required when offline sync is involved.
- offline sync behavior is documented when applicable.
- acceptance criteria are documented.
- blocked scope is documented.

## Definition of Done

A client feature is done only when:

- API client boundary exists.
- typed request model exists.
- typed response model exists.
- standard error envelope handler exists.
- organization scope is preserved.
- authorization behavior is handled.
- request id is preserved.
- correlation id is preserved.
- audit trail reference is handled when applicable.
- device id is preserved when mobile.
- idempotency key is preserved when offline sync is involved.
- contract test evidence exists.
- no secrets are committed.
- no real patient data is committed.
- no undocumented endpoint is used.

## P3.16 conclusion

Implementation kickoff must be controlled by measurable readiness and completion boundaries.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
