# P3.24 Client Release Candidate Approval Boundary

## Purpose

P3.24 defines the Client Release Candidate Approval Boundary for Web client, iOS client, and Android client.

This phase does not implement production client code.

This phase does not approve production deployment.

This phase does not claim backend production readiness.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Client release candidate approval status: BLOCKED_PENDING_REAL_EVIDENCE

## Core principle

SQL Server is the operational source of truth.

The API is the only approved integration boundary for Web iOS Android.

No direct mobile write to SQL Server.

No cloud dependency.

No secrets in repository.

## Release candidate approval scope

A client artifact may be considered a release candidate only when it has:

- artifact reference.
- deployed commit SHA.
- environment name.
- build profile.
- release channel.
- API contract version.
- OpenAPI artifact reference.
- dependency review evidence.
- secret scan evidence.
- static analysis evidence.
- build reproducibility evidence.
- unit test evidence.
- contract test evidence.
- runtime configuration test evidence.
- observability test evidence.
- privacy-safe telemetry test evidence.
- schema drift evidence.
- breaking change evidence.
- artifact retention evidence.
- release notes evidence.
- rollback plan.
- support diagnostic evidence.

## Approval states

| State | Meaning |
|---|---|
| blocked | evidence is incomplete or failed |
| candidate | evidence is complete enough for controlled review |
| rejected | evidence failed review |
| approved for pilot | artifact may be used in a controlled non-production pilot |
| approved for production review | artifact may enter production readiness review only after real evidence |

## Blocked release candidate behavior

Blocked behavior includes accepting local builds as release candidates, releasing without contract test evidence, releasing without runtime configuration evidence, releasing without observability evidence, releasing with secrets in source code, releasing with raw patient payload telemetry, releasing without rollback plan, and treating release candidate approval as production approval.

## P3.24 conclusion

Client artifacts must not be treated as release candidates without explicit evidence-backed approval boundaries.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
