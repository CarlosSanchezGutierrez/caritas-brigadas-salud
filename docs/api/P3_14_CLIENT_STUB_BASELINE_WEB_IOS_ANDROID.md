# P3.14 Client Stub Baseline for Web/iOS/Android

## Purpose

This document defines the baseline for future generated or manually maintained API client stubs for Web, iOS, and Android.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Client stub principle

Client stubs are integration helpers.

Client stubs are not final application implementation.

Client stubs must remain aligned with the API contract, OpenAPI specification, and contract testing baseline.

## Required client stub classes

| Client | Stub expectation |
|---|---|
| Web client stub | TypeScript API client boundary for Next.js/Web |
| iOS client stub | Swift API client boundary for iOS |
| Android client stub | Kotlin API client boundary for Android |
| Reporting stub | Read-only report/export API boundary when needed |
| Test client stub | Contract testing and smoke testing boundary |

## Web client stub baseline

The Web client stub must preserve:

- API version.
- request id.
- correlation id.
- organization id.
- user role.
- standard error envelope handling.
- pagination convention.
- filtering convention.
- sorting convention.
- audit trail reference handling.
- export governance handling.

## iOS client stub baseline

The iOS client stub must preserve:

- API version.
- request id.
- correlation id.
- organization id.
- device id.
- idempotency key.
- client operation id.
- offline sync metadata.
- sync status.
- standard error envelope handling.
- conflict response handling.
- audit trail reference handling.

## Android client stub baseline

The Android client stub must preserve:

- API version.
- request id.
- correlation id.
- organization id.
- device id.
- idempotency key.
- client operation id.
- offline sync metadata.
- sync status.
- standard error envelope handling.
- conflict response handling.
- audit trail reference handling.

## Generated client boundary

Generated clients must not:

- bypass authorization.
- bypass server validation.
- write directly to SQL Server.
- silently overwrite conflicts.
- hide standard error envelope fields.
- drop request id.
- drop correlation id.
- drop organization id.
- drop audit trail reference.
- treat generated code as business logic.

## Stub validation

Every client stub baseline must validate:

- endpoint coverage.
- schema compatibility.
- standard error envelope compatibility.
- metadata preservation.
- idempotency metadata when applicable.
- offline sync metadata when applicable.
- contract version.

## P3.14 conclusion

Client stubs must make Web/iOS/Android integration faster without weakening contract governance.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE