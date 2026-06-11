# P5.9 Patient Create Idempotency Matrix

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Acceptance criteria

| Area | Required evidence | Required for P5.9 merge | Production-closing |
|---|---|---:|---:|
| IdempotencyKey replay | Repeated IdempotencyKey returns existing patient | Yes | No |
| ClientOperationId replay | Repeated ClientOperationId returns existing patient | Yes | No |
| Local patient replay | Repeated SourceBrigadeId + LocalPatientId returns existing patient | Yes | No |
| Organization scope | Idempotency lookup filters by OrganizationId | Yes | No |
| Deleted patient exclusion | Idempotency lookup excludes deleted patients | Yes | No |
| Existing response | Existing replay returns PatientSummaryDto | Yes | No |
| Folio conflict preservation | Non-idempotent duplicate patient folio still conflicts | Yes | No |
| Verifier | P5.9 verifier passes | Yes | No |
| Build | Infrastructure and API build in Release | Yes | No |
| Tests | API test suite passes | Yes | No |

## Rejection criteria

Reject P5.9 if idempotency can match across organizations, if deleted patients can be returned as idempotent matches, if it bypasses existing validation, if it removes folio conflict behavior, if direct mobile SQL Server writes are allowed, if secrets are added, if real patient data is committed, if a cloud dependency is introduced, or if production readiness closure is claimed.