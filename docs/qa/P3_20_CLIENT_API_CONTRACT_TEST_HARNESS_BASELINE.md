# P3.20 Client API Contract Test Harness Baseline

## Purpose

P3.20 defines the contract test harness baseline for Web client, iOS client, and Android client.

This phase does not implement production client code.

This phase does not claim backend production readiness.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Client API contract test harness status: BLOCKED_PENDING_REAL_EVIDENCE

## Core principle

SQL Server is the operational source of truth.

The API is the only approved integration boundary for Web iOS Android.

No direct mobile write to SQL Server.

No cloud dependency.

No secrets in repository.

## Harness scope

The contract test harness must validate:

- API contract version.
- endpoint id.
- request schema.
- response schema.
- request metadata model.
- response metadata model.
- standard error envelope model.
- authentication requirement.
- authorization role.
- organization id.
- request id.
- correlation id.
- audit trail reference.
- device id for mobile.
- idempotency key for offline sync.
- client operation id for offline sync.
- sync status for mobile.
- server acknowledgment for mobile sync.
- conflict id for conflict responses.
- schema drift detection.
- breaking change detection.

## Harness gates

| Gate | Requirement |
|---|---|
| contract availability gate | OpenAPI contract evidence must be referenced |
| schema gate | request schema and response schema must match documented contract |
| metadata gate | required metadata must be preserved |
| error gate | standard error envelope must be parsed consistently |
| auth gate | authentication and authorization behavior must be represented |
| organization gate | organization id must be preserved for scoped data |
| offline gate | mobile offline sync metadata must be preserved |
| audit gate | audit trail reference must be preserved for accepted writes |
| drift gate | schema drift must be detected before merge |
| breaking change gate | breaking changes require explicit version review |

## P3.20 conclusion

Client implementation must not depend on API behavior without contract test harness coverage.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
