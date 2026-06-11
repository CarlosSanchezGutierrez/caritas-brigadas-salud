# P3.15 Client Integration Acceptance Criteria

## Purpose

This document defines acceptance criteria for Web iOS Android integration readiness.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Acceptance criteria

Client integration can move from blocked to allowed only when API contract version is identified, OpenAPI contract evidence exists where applicable, client stub baseline exists where applicable, endpoint integration status is defined, request schema is documented, response schema is documented, standard error envelope is documented, authentication requirement is documented, authorization role is documented, organization id requirement is documented, request id is preserved, correlation id is preserved, audit trail reference is preserved when applicable, idempotency key is preserved when applicable, device id is preserved when applicable, offline sync behavior is documented when applicable, contract testing evidence exists, schema drift check is documented, and breaking change review is documented.

## Rejection criteria

Client integration must remain blocked if the endpoint bypasses the API, allows direct mobile write to SQL Server, lacks standard error envelope, lacks organization id where scoped data is involved, lacks audit trail reference for accepted writes, mobile sync lacks device id, offline sync lacks idempotency key, conflict handling allows silent overwrite, contract tests are missing, evidence is fabricated, or secrets are stored in repository.

## P3.15 conclusion

Client integration acceptance must be measurable and evidence-backed before client teams rely on backend behavior.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
