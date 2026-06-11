# P3.15 Client Integration Readiness Matrix for Web iOS Android

## Purpose

P3.15 defines the Client integration readiness matrix for Web client, iOS client, and Android client.

This phase does not claim client implementation is complete.

This phase does not claim backend production readiness.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Client integration readiness status: BLOCKED_PENDING_REAL_EVIDENCE

## Core principle

SQL Server is the operational source of truth.

The API is the only approved integration boundary for Web iOS Android.

No direct mobile write to SQL Server.

No cloud dependency.

No secrets in repository.

## Readiness status values

| Status | Meaning |
|---|---|
| allowed | Client may plan against the documented contract only |
| blocked | Client must not implement against the endpoint yet |
| requires evidence | Client may proceed only after evidence is attached |
| contract ready | API contract exists and verifier passes |
| implementation pending | API contract exists but implementation evidence is missing |
| evidence pending | Implementation may exist but evidence is incomplete |

## Client integration readiness matrix

| Capability | Web client | iOS client | Android client | Current status | Evidence needed |
|---|---|---|---|---|---|
| Health check | allowed | allowed | allowed | contract ready | smoke test evidence |
| Identity context | allowed | allowed | allowed | requires evidence | authenticated identity evidence |
| Organization context | allowed | allowed | allowed | requires evidence | organization id validation evidence |
| Brigade setup | allowed | blocked | blocked | implementation pending | role and audit evidence |
| Patient registration | allowed | allowed | allowed | requires evidence | validation and audit evidence |
| Privacy consent capture | allowed | allowed | allowed | requires evidence | consent evidence package |
| Encounter capture | allowed | allowed | allowed | requires evidence | encounter audit evidence |
| Clinical timeline | allowed | allowed | allowed | requires evidence | longitudinal history evidence |
| Offline local draft | blocked | allowed | allowed | contract ready | mobile local storage evidence |
| Offline outbox | blocked | allowed | allowed | contract ready | idempotency key evidence |
| Sync reconciliation | blocked | allowed | allowed | requires evidence | server acknowledgment evidence |
| Conflict resolution | allowed | allowed | allowed | requires evidence | conflict scenario evidence |
| Dashboards | allowed | blocked | blocked | contract ready | metric lineage evidence |
| Reports export | allowed | blocked | blocked | requires evidence | governed export evidence |
| Audit review | allowed | blocked | blocked | requires evidence | audit trail reference evidence |

## Required integration metadata

Every client integration must preserve:

- API contract version.
- request id.
- correlation id.
- organization id.
- user role.
- standard error envelope.
- audit trail reference when applicable.
- device id when mobile.
- idempotency key when offline sync is involved.
- client operation id when offline sync is involved.

## P3.15 conclusion

Client integration is status-driven, evidence-backed, and API-only.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
