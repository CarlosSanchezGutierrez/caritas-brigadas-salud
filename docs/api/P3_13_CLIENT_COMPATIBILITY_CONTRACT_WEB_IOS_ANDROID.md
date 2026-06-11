# P3.13 Client Compatibility Contract for Web/iOS/Android

## Purpose

This document defines how Web, iOS, and Android clients must consume the API contract.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Client compatibility principle

Web, iOS, and Android must consume the same governed API contract.

Client-specific behavior is allowed only when explicitly documented.

## Web contract

Web client responsibilities:

- administrative workflows.
- reporting workflows.
- dashboard dataset reads.
- organization management.
- brigade setup.
- role-aware operations.
- governed exports.
- audit review when authorized.
- conflict review when authorized.

Web must preserve:

- request id.
- correlation id.
- organization id.
- user role.
- audit trail reference when returned.

## iOS contract

iOS client responsibilities:

- field capture.
- offline capture.
- local draft handling.
- local outbox.
- idempotency key generation.
- device id submission.
- sync status reconciliation.
- conflict handling.
- consent capture.
- encounter capture.
- clinical timeline capture.

iOS must not write directly to SQL Server.

## Android contract

Android client responsibilities:

- field capture.
- offline capture.
- local draft handling.
- local outbox.
- idempotency key generation.
- device id submission.
- sync status reconciliation.
- conflict handling.
- consent capture.
- encounter capture.
- clinical timeline capture.

Android must not write directly to SQL Server.

## Shared client responsibilities

All approved clients must preserve:

- API version.
- request id.
- correlation id.
- organization id.
- actor context.
- user role.
- standard error envelope handling.
- validation error handling.
- authorization error handling.
- conflict error handling.
- idempotency error handling.
- sync error handling when applicable.

## Compatibility expectations

The API contract must specify:

- supported client.
- supported endpoint.
- required headers.
- required metadata.
- offline compatibility.
- sync compatibility.
- pagination compatibility.
- export compatibility.
- dashboard compatibility.

## Breaking change policy

Breaking changes require:

- new API version.
- migration note.
- client impact statement.
- deprecation period.
- endpoint catalog update.
- request schema update.
- response schema update.
- evidence template update.
- audit impact review.

## P3.13 conclusion

Web/iOS/Android integration must be stable, versioned, auditable, and contract-driven.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE