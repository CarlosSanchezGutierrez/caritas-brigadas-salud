# P3.19 Shared API Client Model Contracts

## Purpose

P3.19 defines the shared API client model contracts for Web client, iOS client, and Android client.

This phase does not implement production client code.

This phase does not claim backend production readiness.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Shared API client model contract status: BLOCKED_PENDING_REAL_EVIDENCE

## Core principle

SQL Server is the operational source of truth.

The API is the only approved integration boundary for Web iOS Android.

No direct mobile write to SQL Server.

No cloud dependency.

No secrets in repository.

## Required shared model groups

| Model group | Purpose |
|---|---|
| request metadata model | preserves request id correlation id organization id API contract version and endpoint id |
| response metadata model | preserves request id correlation id audit trail reference and server timestamp when applicable |
| standard error envelope model | preserves machine-readable error behavior across clients |
| authentication context model | preserves authenticated user context without credential leakage |
| authorization context model | preserves authorization role and role-sensitive behavior |
| organization scope model | preserves organization id and scoped data rules |
| pagination model | preserves pagination convention for list endpoints |
| filtering model | preserves filtering convention for list and report endpoints |
| sorting model | preserves sorting convention for list and report endpoints |
| audit reference model | preserves audit trail reference after accepted writes |
| mobile device model | preserves device id for mobile clients |
| offline operation model | preserves idempotency key client operation id and sync status for offline sync |
| conflict model | preserves explicit conflict handling and prevents silent overwrite |

## Required shared fields

Every client must preserve these fields when applicable:

- API contract version.
- endpoint id.
- request id.
- correlation id.
- organization id.
- authorization role.
- standard error envelope.
- audit trail reference.
- device id.
- idempotency key.
- client operation id.
- sync status.
- conflict id.
- server acknowledgment.

## P3.19 conclusion

Shared API client models must be explicit before Web iOS Android generate or hand-code client boundaries.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
