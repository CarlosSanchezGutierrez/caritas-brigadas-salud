# P3.22 Client Observability Telemetry and Support Boundary

## Purpose

P3.22 defines the observability telemetry and support boundary for Web client, iOS client, and Android client.

This phase does not implement production client code.

This phase does not claim backend production readiness.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Client observability telemetry support status: BLOCKED_PENDING_REAL_EVIDENCE

## Core principle

SQL Server is the operational source of truth.

The API is the only approved integration boundary for Web iOS Android.

No direct mobile write to SQL Server.

No cloud dependency.

No secrets in repository.

## Observability scope

Client observability must support:

- request id.
- correlation id.
- organization id.
- endpoint id.
- API contract version.
- environment name.
- client target.
- build profile.
- release channel.
- standard error envelope.
- audit trail reference when applicable.
- device id when mobile.
- idempotency key when offline sync is involved.
- client operation id when offline sync is involved.
- sync status when mobile.
- server acknowledgment when mobile sync is accepted.
- conflict id when conflict occurs.
- schema drift status.
- contract test status.
- configuration test status.

## Observability event categories

| Category | Purpose |
|---|---|
| client startup event | records environment and build profile without secrets |
| API request event | records endpoint id request id correlation id and contract version |
| API response event | records response metadata and standard error envelope when applicable |
| authorization event | records role-sensitive denial without sensitive payload |
| organization scope event | records organization scoped behavior without patient payload |
| offline queue event | records mobile local outbox state without sensitive payload |
| sync event | records device id idempotency key client operation id and sync status |
| conflict event | records conflict id and resolution requirement without silent overwrite |
| audit reference event | records audit trail reference after accepted writes |
| support event | records support-safe diagnostic context |

## Blocked observability behavior

Blocked behavior includes logging secrets, logging real patient payloads, logging unsupported sensitive fixtures, hiding standard error envelope, dropping request id, dropping correlation id, dropping organization id, dropping device id for mobile sync, dropping idempotency key for offline sync, treating telemetry as production approval, and treating local logs as backend evidence.

## P3.22 conclusion

Client observability must be privacy-safe, correlation-ready, contract-aware, and evidence-backed before Web iOS Android implementation expands.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
