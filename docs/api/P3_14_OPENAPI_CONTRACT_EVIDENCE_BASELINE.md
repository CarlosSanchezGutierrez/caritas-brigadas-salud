# P3.14 OpenAPI Contract Evidence Baseline

## Purpose

P3.14 defines how the P3.13 API contract freeze must be represented, validated, evidenced, and consumed through OpenAPI and client stub baselines.

This phase does not claim that the backend is production-ready.

This phase does not implement all generated clients.

It defines the baseline required before Web, iOS, and Android teams treat the API contract as stable.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Backend freeze status: NOT_FROZEN_PENDING_ON_PREM_EVIDENCE_AND_CONTRACTS

## Core principle

SQL Server is the operational source of truth.

OpenAPI is a contract artifact, not the operational source of truth.

The API remains the only approved path for Web, iOS, and Android clients to interact with operational data.

## Contract evidence principle

API contract evidence must prove:

- endpoint id.
- route.
- HTTP method.
- API version.
- request schema.
- response schema.
- standard error envelope.
- authentication requirement.
- authorization role.
- organization id requirement.
- request id.
- correlation id.
- idempotency key when applicable.
- device id when applicable.
- audit trail reference when applicable.
- OpenAPI path coverage.
- schema drift check.
- breaking change review.
- client compatibility matrix.
- contract testing baseline.
- generated client boundary.

## Required OpenAPI evidence

Every frozen endpoint must have:

- OpenAPI path.
- operation id.
- request body schema when applicable.
- response schema.
- error response schema.
- security requirement.
- tags.
- API version.
- examples using synthetic data only.
- client support statement.
- audit metadata requirement when applicable.
- idempotency key requirement when applicable.
- offline sync metadata when applicable.

## Evidence boundaries

OpenAPI evidence does not prove:

- production readiness.
- real SQL Server deployment.
- real authentication integration.
- real mobile app correctness.
- real data privacy compliance.
- real backup and restore capability.
- real observability in production.

Those remain blocked pending real evidence.

## Guardrails

- No secrets in repository.
- No cloud dependency.
- No fabricated evidence.
- No direct mobile write to SQL Server.
- No unauthenticated protected endpoint.
- No generated client treated as final app implementation.
- No OpenAPI artifact treated as backend production evidence.
- No schema drift without review.
- No breaking change without API version review.
- No client stub without contract version.

## P3.14 conclusion

P3.14 gives the project a stable contract evidence baseline for OpenAPI, contract validation, and future Web/iOS/Android client stubs.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE