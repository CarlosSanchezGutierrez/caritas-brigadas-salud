# P4.1 Real Evidence Capture Runbook

## Purpose

This runbook explains how to run the P4.1 real evidence collector.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Collector

The P4.1 real evidence collector is scripts/p4/collect-p4-01-real-evidence-baseline.ps1.

The evidence output root is artifacts/p4/p4-01-real-evidence-baseline.

The collector writes manifest.json.

## Standard command

`powershell
powershell -ExecutionPolicy Bypass -File scripts/p4/collect-p4-01-real-evidence-baseline.ps1
`",
        ",
        

`powershell
powershell -ExecutionPolicy Bypass -File scripts/p4/collect-p4-01-real-evidence-baseline.ps1 -ApiBaseUrl "https://localhost:7044"
`",
        ",
        
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

## Security rule

The collector must never print ConnectionStrings__SqlServer value. It may only report whether the key is present or missing.

## Failure handling

If a required command fails, the collector must write the command exit code, keep the manifest.json, classify blockers, and fail the run.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
