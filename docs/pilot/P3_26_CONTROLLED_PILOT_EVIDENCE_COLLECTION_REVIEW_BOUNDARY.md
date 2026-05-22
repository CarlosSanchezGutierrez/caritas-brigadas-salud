# P3.26 Controlled Pilot Evidence Collection Review Boundary

## Purpose

P3.26 defines the Controlled Pilot Evidence Collection Review Boundary for Web client, iOS client, and Android client.

This phase does not implement production client code.

This phase does not approve production deployment.

This phase does not claim backend production readiness.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Controlled pilot evidence review status: BLOCKED_PENDING_REAL_EVIDENCE

## Core principle

SQL Server is the operational source of truth.

The API is the only approved integration boundary for Web iOS Android.

No direct mobile write to SQL Server.

No cloud dependency.

No secrets in repository.

## Evidence collection scope

Controlled pilot evidence collection must capture:

- approved pilot readiness reference.
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
- pilot device inventory.
- UAT execution evidence.
- workflow completion evidence.
- field feedback evidence.
- support ticket evidence.
- incident evidence.
- defect triage evidence.
- privacy review evidence.
- consent workflow evidence.
- offline field workflow evidence.
- sync dry run evidence.
- sync reconciliation evidence.
- observability evidence.
- privacy-safe telemetry evidence.
- audit trail reference evidence.
- rollback decision evidence.

## Evidence review states

| State | Meaning |
|---|---|
| blocked | evidence is missing or invalid |
| collecting evidence | controlled pilot evidence is being captured |
| under review | evidence is complete enough for review |
| rejected | evidence does not support readiness progression |
| accepted with actions | evidence is accepted but requires remediation |
| accepted for production readiness review | evidence may feed production readiness review only after real approval |

## Blocked evidence behavior

Blocked behavior includes missing pilot readiness reference, missing release candidate reference, missing UAT execution evidence, missing consent workflow evidence, missing privacy review evidence, missing support ticket review, missing incident review, missing defect triage, missing rollback decision evidence, using unsanitized evidence, expanding pilot scope without approval, and treating pilot evidence review as production approval.

## P3.26 conclusion

Controlled pilot evidence must be collected and reviewed before any production readiness review is considered.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
