# P4.2 Real Evidence Blocker Classification Matrix

## Purpose

This matrix defines the acceptance criteria for classifying real P4.1 evidence into a P4.2 blocker backlog.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Classification acceptance matrix

| Evidence input | Category | Required classification |
|---|---|---|
| repository clean state evidence | repository | PASS, P0, or P1 |
| git commit SHA evidence | repository | PASS or P0 |
| git branch evidence | repository | PASS or P1 |
| dotnet info evidence | build-test | PASS or P0 |
| dotnet restore evidence | build-test | PASS or P0 |
| dotnet build evidence | build-test | PASS or P0 |
| dotnet test evidence | build-test | PASS, P0, or P1 |
| P3 governance verifier evidence | governance | PASS or P0 |
| P4 verifier evidence | governance | PASS or P0 |
| SQL Server configuration presence evidence | database-config | PASS, P0, or P1 |
| API health check evidence | api-runtime | PASS, P1, or P2 |
| OpenAPI artifact evidence | api-contract | PASS, P1, or P2 |
| endpoint contract evidence | api-contract | PASS, P1, or P2 |
| audit trail evidence | security-privacy | PASS, P1, or P2 |
| support diagnostic evidence | observability | PASS, P1, or P2 |
| monitoring evidence | observability | PASS, P1, or P2 |
| alerting evidence | observability | PASS, P1, or P2 |
| evidence sanitization status | security-privacy | PASS or P0 |
| real environment blocker register | governance | PASS, P0, or P1 |
| mobile release channel evidence | mobile-sync | PASS, P1, or P2 |
| device fleet evidence | mobile-sync | PASS, P1, or P2 |
| offline sync evidence | mobile-sync | PASS, P1, or P2 |
| conflict resolution evidence | mobile-sync | PASS, P1, or P2 |

## Required output fields

Each classified evidence item must include:

- evidence name.
- original status.
- command exit code.
- required blocker flag.
- blocker text.
- blocker severity.
- blocker category.
- owner group.
- remediation type.
- source log path.
- evidence source.
- classifier decision.
- sanitized evidence only.
- Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE.

## Rejection criteria

Reject P4.2 classification when:

- The manifest path is missing.
- The manifest is not valid JSON.
- The manifest phase is not P4.1 Real Evidence Execution Baseline.
- The manifest does not preserve Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE.
- Results are missing.
- Required blocker flag is missing.
- Severity is missing.
- Category is missing.
- Source log path is missing.
- The classifier writes secrets.
- The classifier approves backend production readiness.
- The classifier silently ignores failed required evidence.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE