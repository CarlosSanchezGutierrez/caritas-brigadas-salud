# P3.13 API Contract Freeze Baseline for Web/iOS/Android

## Purpose

P3.13 defines the API contract freeze baseline for Web, iOS, Android, and future approved clients.

This phase does not implement every endpoint.

It defines the contract rules that prevent ambiguous frontend, mobile, offline sync, reporting, and administrative client integration.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Backend freeze status: NOT_FROZEN_PENDING_ON_PREM_EVIDENCE_AND_CONTRACTS

## Core principle

SQL Server is the operational source of truth.

The API is the only approved path for Web, iOS, and Android clients to interact with operational data.

Mobile and web clients must not write directly to SQL Server.

## API contract freeze meaning

P3.13 freezes the expected API behavior at the contract level.

The freeze includes:

- endpoint catalog.
- request schema.
- response schema.
- standard error envelope.
- authentication and authorization metadata.
- pagination convention.
- filtering convention.
- sorting convention.
- idempotency key convention.
- offline sync metadata.
- audit metadata.
- API version.
- deprecation policy.
- client compatibility rules.
- Web/iOS/Android expectations.

## API contract mandatory metadata

Every endpoint must define:

- endpoint id.
- HTTP method.
- route.
- API version.
- client support.
- authentication requirement.
- authorization role.
- organization id requirement.
- request schema.
- response schema.
- standard error envelope.
- validation rules.
- audit trail requirement.
- idempotency key requirement when applicable.
- pagination when applicable.
- filtering when applicable.
- sorting when applicable.
- offline sync compatibility when applicable.
- rate limit expectation.
- correlation id.
- request id.

## Client support classes

| Client | Contract expectation |
|---|---|
| Web | Full administrative, reporting, and operational workflows as approved |
| iOS | Field capture, offline-first sync, mobile validation, consent, encounter workflows |
| Android | Field capture, offline-first sync, mobile validation, consent, encounter workflows |
| Reporting clients | Read-only reporting and export flows through approved endpoints or views |
| Future clients | Must comply with versioned API contract |

## Guardrails

- No secrets in repository.
- No cloud dependency.
- No direct mobile write to SQL Server.
- No unauthenticated API access.
- No unaudited write endpoint.
- No endpoint without organization id where scoped data is involved.
- No patient-level export without governance.
- No silent overwrite.
- No endpoint that bypasses server validation.
- No offline sync without idempotency key.
- No accepted write without audit trail reference.

## Relationship with previous phases

P3.13 depends on:

- P3.8 SQL Server on-prem operational evidence.
- P3.9 total auditability and longitudinal history.
- P3.10 operational and analytical pipelines.
- P3.11 KPI, dashboard, insight, and direction reporting catalog.
- P3.12 offline-first mobile sync operational contract.

## P3.13 conclusion

API contract freeze is required before Web, iOS, and Android teams build against stable endpoint expectations.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE