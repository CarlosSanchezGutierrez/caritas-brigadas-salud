# P3.16 Client Implementation Kickoff Boundary

## Purpose

P3.16 defines the implementation kickoff boundary for Web client, iOS client, and Android client.

This phase allows implementation planning to begin only inside controlled boundaries.

This phase does not claim that Web, iOS, or Android implementation is complete.

This phase does not claim backend production readiness.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Client implementation kickoff status: BLOCKED_PENDING_REAL_EVIDENCE

## Core principle

SQL Server is the operational source of truth.

The API is the only approved integration boundary for Web iOS Android.

No direct mobile write to SQL Server.

No cloud dependency.

No secrets in repository.

## Implementation kickoff boundary

Client teams may begin implementation only when the target capability has:

- endpoint integration status.
- API contract version.
- request schema.
- response schema.
- standard error envelope.
- authentication requirement.
- authorization role.
- organization id requirement.
- request id.
- correlation id.
- audit trail reference when applicable.
- idempotency key when applicable.
- device id when applicable.
- offline sync rules when applicable.
- acceptance criteria.
- blocked scope.
- evidence requirement.

## Allowed implementation activities

Allowed activities:

- create client folder structure.
- create API client boundary files.
- create typed request and response models.
- create error envelope handlers.
- create local state boundaries.
- create offline queue boundaries for mobile.
- create UI shell routes.
- create mocked data only when clearly marked synthetic.
- create contract tests.
- create smoke tests.

## Blocked implementation activities

Blocked activities:

- bypass the API.
- bypass authorization.
- bypass organization scope.
- bypass audit trail creation.
- invent undocumented endpoints.
- silently overwrite conflicts.
- store secrets in repository.
- store real patient data in fixtures.
- treat mocked data as evidence.
- treat UI shell completion as integration completion.

## P3.16 conclusion

Client implementation may start only within a strict API-only, evidence-backed, contract-governed boundary.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
