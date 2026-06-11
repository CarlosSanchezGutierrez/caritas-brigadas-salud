# P4.1 Real Evidence Acceptance Matrix

## Purpose

This document defines the acceptance criteria for real evidence collected during P4.1.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Acceptance matrix

| Evidence area | Required | Acceptance rule |
|---|---:|---|
| P3.43 final production governance evidence index reference | yes | Must reference merged P3.43 evidence index |
| repository clean state evidence | yes | Must show no unexpected working tree changes before evidence capture |
| git commit SHA evidence | yes | Must identify the exact commit under review |
| dotnet restore evidence | yes | Must capture command output and command exit code |
| dotnet build evidence | yes | Must capture command output and command exit code |
| dotnet test evidence | yes | Must capture command output and command exit code or documented absence of test projects |
| P3 governance verifier evidence | yes | Must show P3.43 verifier execution result |
| P4 verifier evidence | yes | Must show P4.1 verifier execution result |
| SQL Server configuration presence evidence | yes | Must show whether ConnectionStrings__SqlServer is present without printing the value |
| API health check evidence | conditional | Required when API base URL is provided |
| OpenAPI artifact evidence | conditional | Required when OpenAPI artifact exists |
| endpoint contract evidence | conditional | Required when API endpoints are available |
| audit trail evidence | conditional | Required when audited operations are exercised |
| support diagnostic evidence | yes | Must identify where diagnostic outputs are captured |
| monitoring evidence | yes | Must identify monitoring source or blocker |
| alerting evidence | yes | Must identify alerting source or blocker |
| evidence sanitization status | yes | Must be explicit |
| real environment blocker register | yes | Must classify missing runtime dependencies |
| mobile release channel evidence | conditional | Required when mobile client is exercised |
| device fleet evidence | conditional | Required when mobile client is exercised |
| offline sync evidence | conditional | Required when mobile sync is exercised |
| conflict resolution evidence | conditional | Required when conflict handling is exercised |

## Rejection criteria

Reject P4.1 evidence when evidence is synthetic, not reproducible, missing command exit code, missing manifest.json, missing blocker classification, unsanitized, or disconnected from a commit SHA.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
