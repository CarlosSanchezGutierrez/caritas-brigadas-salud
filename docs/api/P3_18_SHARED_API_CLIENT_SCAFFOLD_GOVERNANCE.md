# P3.18 Shared API Client Scaffold Governance

## Purpose

P3.18 defines governance for the shared API client scaffold used by Web client, iOS client, and Android client.

This phase does not implement production client code.

This phase does not claim backend production readiness.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Shared API client scaffold status: BLOCKED_PENDING_REAL_EVIDENCE

## Core principle

SQL Server is the operational source of truth.

The API is the only approved integration boundary for Web iOS Android.

No direct mobile write to SQL Server.

No cloud dependency.

No secrets in repository.

## Scaffold governance scope

The shared API client scaffold governs:

- base URL configuration boundary.
- API contract version propagation.
- endpoint id mapping.
- typed request model.
- typed response model.
- standard error envelope model.
- authentication metadata boundary.
- authorization role metadata boundary.
- organization id propagation.
- request id propagation.
- correlation id propagation.
- audit trail reference handling.
- device id propagation for mobile.
- idempotency key propagation for offline sync.
- client operation id propagation for offline sync.
- sync status handling for mobile.
- retry boundary.
- timeout boundary.
- contract test boundary.

## Scaffold layers

| Layer | Responsibility |
|---|---|
| configuration boundary | environment-specific base URL and contract version without secrets |
| transport boundary | HTTP request execution and timeout behavior |
| metadata boundary | request id correlation id organization id and role metadata |
| auth boundary | authenticated request preparation without credential leakage |
| schema boundary | typed request and response models |
| error boundary | standard error envelope parsing and propagation |
| offline boundary | device id idempotency key client operation id and sync status for mobile |
| audit boundary | audit trail reference propagation after accepted writes |
| test boundary | contract test evidence and schema drift detection |

## Blocked scaffold behavior

Blocked behavior includes direct database access, undocumented endpoint usage, missing organization scope, missing request id, missing correlation id, missing standard error envelope handling, missing device id for mobile sync, missing idempotency key for offline sync, silent conflict overwrite, credential persistence in source code, and synthetic data treated as evidence.

## P3.18 conclusion

The shared API client scaffold must be governed before Web iOS Android implement client-specific API access.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
