# P3.25 Controlled Pilot Readiness Boundary

## Purpose

P3.25 defines the Controlled Pilot Readiness Boundary for Web client, iOS client, and Android client.

This phase does not implement production client code.

This phase does not approve production deployment.

This phase does not claim backend production readiness.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Controlled pilot readiness status: BLOCKED_PENDING_REAL_EVIDENCE

## Core principle

SQL Server is the operational source of truth.

The API is the only approved integration boundary for Web iOS Android.

No direct mobile write to SQL Server.

No cloud dependency.

No secrets in repository.

## Controlled pilot readiness scope

A controlled pilot may begin only when the evidence package includes:

- approved release candidate reference.
- artifact reference.
- deployed commit SHA.
- environment name.
- build profile.
- release channel.
- API contract version.
- OpenAPI artifact reference.
- pilot site or brigade scope.
- pilot participant scope.
- pilot data boundary.
- UAT acceptance criteria.
- field operations support plan.
- training evidence.
- rollback plan.
- incident response plan.
- support escalation plan.
- privacy consent evidence.
- data protection evidence.
- observability evidence.
- contract test evidence.
- runtime configuration test evidence.
- privacy-safe telemetry test evidence.
- release candidate approval evidence.

## Pilot approval states

| State | Meaning |
|---|---|
| blocked | evidence is incomplete or failed |
| ready for dry run | artifact may be validated without operational use |
| ready for controlled pilot | artifact may be used with limited approved users and scope |
| pilot rejected | pilot cannot proceed or must stop |
| pilot completed pending review | pilot evidence exists but has not been accepted |
| approved for production readiness review | pilot evidence may enter production readiness review only after real evidence |

## Blocked pilot behavior

Blocked behavior includes launching without release candidate approval evidence, launching without consent evidence, launching without training evidence, launching without support escalation plan, launching without rollback plan, launching without incident response plan, using unrestricted patient-level exports, using raw patient payload telemetry, treating pilot approval as production approval, and expanding scope without approval.

## P3.25 conclusion

Controlled pilot readiness must be evidence-backed before Web iOS Android are used in limited field validation.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
