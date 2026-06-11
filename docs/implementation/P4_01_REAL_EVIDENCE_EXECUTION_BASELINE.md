# P4.1 Real Evidence Execution Baseline

## Purpose

P4.1 starts real execution after P3 governance closure.

This phase creates the baseline for capturing real sanitized evidence from the repository, backend, API, SQL Server configuration, tests, verifiers, and runtime checks.

This phase does not claim backend production readiness.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

P4.1 real evidence execution baseline status: BLOCKED_PENDING_REAL_EVIDENCE

## Mandatory boundary

SQL Server is the operational source of truth.

ConnectionStrings__SqlServer is the required configuration key for real SQL Server connectivity evidence.

No direct mobile write to SQL Server.

No cloud dependency.

No secrets in repository.

No fabricated evidence.

Only real evidence from controlled commands, real build output, real tests, real logs, real API checks, and sanitized runtime observations may be accepted.

## Required baseline evidence
- P4.1 Real Evidence Execution Baseline.
- Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE.
- SQL Server is the operational source of truth.
- ConnectionStrings__SqlServer.
- P3.43 final production governance evidence index reference.
- P4 implementation readiness handoff evidence.
- P4 real evidence backlog evidence.
- real evidence only.
- sanitized evidence only.
- evidence output root.
- artifacts/p4/p4-01-real-evidence-baseline.
- manifest.json.
- command exit code.
- git commit SHA evidence.
- repository clean state evidence.
- dotnet restore evidence.
- dotnet build evidence.
- dotnet test evidence.
- P3 governance verifier evidence.
- P4 verifier evidence.
- SQL Server configuration presence evidence.
- API health check evidence.
- OpenAPI artifact evidence.
- endpoint contract evidence.
- audit trail evidence.
- support diagnostic evidence.
- monitoring evidence.
- alerting evidence.
- evidence sanitization status.
- evidence rejection criteria.
- real environment blocker register.
- P4.1 real evidence collector.
- technical owner assignment.
- operations owner assignment.
- support owner assignment.
- security owner assignment.
- privacy owner assignment.
- data owner assignment.
- risk owner assignment.
- compliance owner assignment.
- mobile release channel evidence.
- device fleet evidence.
- offline sync evidence.
- conflict resolution evidence.
- device id.
- idempotency key.
- client operation id.
- sync status.
- server acknowledgment.
- conflict id.
- request id.
- correlation id.
- organization id.
- authorization role.
- endpoint id.
- standard error envelope.
- audit trail reference.

## Evidence collection rule

The collector script must write evidence under artifacts/p4/p4-01-real-evidence-baseline and must produce manifest.json with command exit code, evidence file path, status, and blocker details.

## Rejection criteria

Reject the evidence package when outputs are synthetic, stale, unsanitized, missing command exit code, missing git commit SHA evidence, missing repository clean state evidence, missing P3 governance verifier evidence, missing P4 verifier evidence, missing SQL Server configuration presence evidence, missing test evidence, missing build evidence, or missing blocker classification.

## P4.1 conclusion

P4.1 is accepted only when the real evidence collector exists, the P4.1 verifier passes, and the next execution step can capture real evidence without storing secrets.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
