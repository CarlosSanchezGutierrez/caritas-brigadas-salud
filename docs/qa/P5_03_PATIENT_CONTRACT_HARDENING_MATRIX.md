# P5.3 Patient Contract Hardening Matrix

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Acceptance criteria

| Area | Evidence | Required for P5.3 merge | Production-closing |
|---|---|---:|---:|
| Patient create contract | CreatePatientRequest includes offline and source fields | Yes | No |
| Patient summary contract | PatientSummaryDto exposes sync and source fields | Yes | No |
| Offline contract | LocalPatientId, ClientOperationId, IdempotencyKey, SyncStatus, DataCaptureSource exist | Yes | No |
| Longitudinal contract | SourceBrigadeId and PatientFolio remain contract-visible | Yes | No |
| Readiness marker | PatientContractReadiness exists | Yes | No |
| Build | contracts project builds | Yes | No |
| Validation chain | P4 and P5 verifiers pass | Yes | No |

## Rejection criteria

Reject P5.3 if:

- patient contract removes existing patient fields.
- offline-first fields are missing.
- longitudinal linkage fields are missing.
- backend readiness authorization is granted.
- SQL Server blocker is hidden.
- direct SQL access from clients is allowed.
- API bypass is allowed.
- cloud is made mandatory.