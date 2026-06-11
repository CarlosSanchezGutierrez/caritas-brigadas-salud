# P3.17 Web iOS Android Implementation Workstream Split

## Purpose

P3.17 separates client implementation into governed workstreams for Web client, iOS client, Android client, shared API client, cross-client QA, and client security.

This phase does not implement the clients.

This phase does not claim backend production readiness.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Client implementation workstream status: BLOCKED_PENDING_REAL_EVIDENCE

## Core principle

SQL Server is the operational source of truth.

The API is the only approved integration boundary for Web iOS Android.

No direct mobile write to SQL Server.

No cloud dependency.

No secrets in repository.

## Workstreams

| Workstream | Primary scope | Current status |
|---|---|---|
| Web workstream | Admin workflows dashboards reports audit review | blocked pending evidence |
| iOS workstream | Field capture offline draft outbox sync conflict handling | blocked pending evidence |
| Android workstream | Field capture offline draft outbox sync conflict handling | blocked pending evidence |
| Shared API client workstream | API boundary models error envelope metadata preservation | blocked pending evidence |
| Cross-client QA workstream | Contract tests smoke tests scenario matrix regression evidence | blocked pending evidence |
| Client security workstream | Auth role organization scope audit privacy offline sync safety | blocked pending evidence |

## Workstream rules

Every workstream must preserve:

- API contract version.
- endpoint integration status.
- request schema.
- response schema.
- standard error envelope.
- authentication requirement.
- authorization role.
- organization id.
- request id.
- correlation id.
- audit trail reference when applicable.
- device id when mobile.
- idempotency key when offline sync is involved.
- offline sync behavior when applicable.
- contract test evidence requirement.

## Workstream dependency order

Required order:

1. Shared API client workstream.
2. Client security workstream.
3. Cross-client QA workstream.
4. Web shell workstream.
5. iOS shell workstream.
6. Android shell workstream.
7. Feature-specific implementation.
8. Evidence package completion.

## P3.17 conclusion

Implementation must be split into governed workstreams before client coding expands.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
