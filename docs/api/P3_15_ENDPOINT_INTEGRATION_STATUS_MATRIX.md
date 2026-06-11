# P3.15 Endpoint Integration Status Matrix

## Purpose

This document maps endpoint integration status for Web client, iOS client, and Android client.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Endpoint integration status matrix

| Endpoint id | Web client | iOS client | Android client | Status | Evidence needed |
|---|---|---|---|---|---|
| health.read | allowed | allowed | allowed | contract ready | smoke evidence |
| identity.me | allowed | allowed | allowed | requires evidence | auth evidence |
| organizations.list | allowed | allowed | allowed | requires evidence | organization id evidence |
| brigades.create | allowed | blocked | blocked | requires evidence | role and audit evidence |
| patients.create | allowed | allowed | allowed | requires evidence | validation and audit evidence |
| consent.capture | allowed | allowed | allowed | requires evidence | privacy consent evidence |
| encounters.create | allowed | allowed | allowed | requires evidence | encounter audit evidence |
| sync.outbox.submit | blocked | allowed | allowed | requires evidence | idempotency key evidence |
| sync.status.read | blocked | allowed | allowed | requires evidence | server acknowledgment evidence |
| sync.conflicts.read | allowed | allowed | allowed | requires evidence | conflict evidence |
| reports.export | allowed | blocked | blocked | requires evidence | governed export evidence |
| dashboards.dataset.read | allowed | blocked | blocked | requires evidence | metric lineage evidence |
| audit.events.search | allowed | blocked | blocked | requires evidence | audit authorization evidence |

## Endpoint integration rules

Every endpoint integration must define endpoint id, API contract version, client support, readiness status, request schema, response schema, standard error envelope, authentication requirement, authorization role, organization id requirement, request id, correlation id, audit trail reference when applicable, idempotency key when applicable, and device id when applicable.

## P3.15 conclusion

Endpoint integration must be explicit, status-driven, evidence-backed, and client-specific.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
