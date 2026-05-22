# P3.21 Client Runtime Configuration and Environment Boundary

## Purpose

P3.21 defines the runtime configuration and environment boundary for Web client, iOS client, and Android client.

This phase does not implement production client code.

This phase does not claim backend production readiness.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Client runtime configuration status: BLOCKED_PENDING_REAL_EVIDENCE

## Core principle

SQL Server is the operational source of truth.

The API is the only approved integration boundary for Web iOS Android.

No direct mobile write to SQL Server.

No cloud dependency.

No secrets in repository.

## Runtime configuration scope

Runtime configuration governs:

- environment name.
- API base URL.
- API contract version.
- OpenAPI artifact reference.
- feature flag boundary.
- telemetry toggle boundary.
- offline mode toggle boundary.
- sync mode toggle boundary.
- request timeout policy.
- retry policy.
- secure storage boundary.
- secret injection boundary.
- build profile boundary.
- release channel boundary.
- evidence package reference.

## Approved environment classes

| Environment class | Purpose | Production evidence allowed |
|---|---|---|
| local development | developer-only validation | no |
| integration test | contract test and smoke test evidence | no |
| staging evidence | sanitized operational evidence package | limited |
| production | real operational use after approvals | yes only after real approval |

## Runtime configuration rules

Every client must resolve configuration from an approved runtime boundary rather than hardcoded feature code.

Every client must preserve API contract version, request id, correlation id, organization id, standard error envelope, audit trail reference, device id when mobile, idempotency key when offline sync is involved, and client operation id when offline sync is involved.

## Blocked runtime configuration behavior

Blocked behavior includes hardcoded production URLs in feature code, credential persistence in source code, environment-specific logic scattered across screens, missing API contract version, missing organization id, missing request id, missing correlation id, bypassing standard error envelope handling, mobile sync without device id, offline sync without idempotency key, and synthetic configuration treated as evidence.

## P3.21 conclusion

Runtime configuration must be governed before Web iOS Android implementation depends on environment-specific behavior.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
