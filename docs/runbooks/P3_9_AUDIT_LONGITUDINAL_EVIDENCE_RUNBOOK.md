# P3.9 Audit and Longitudinal History Evidence Runbook

## Purpose

This runbook defines how to collect future evidence for total auditability and longitudinal history.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Evidence package metadata

Each evidence package must include:

- environment name.
- deployed commit SHA.
- responsible owner.
- date.
- test actor.
- organization id.
- patient test reference.
- encounter test reference.
- correlation id.
- request id.
- audit trail reference.
- status.
- blockers.

## Audit evidence scenarios

Required scenarios:

1. Create patient.
2. Update patient.
3. Capture consent.
4. Create encounter.
5. Correct clinical record.
6. Export report.
7. Run controlled data injection.
8. Reject records.
9. Quarantine records.
10. Change role.
11. Deny unauthorized action.
12. Merge patient candidate after review.

## Longitudinal evidence scenarios

Required scenarios:

1. Patient has multiple encounters.
2. Patient has consent version history.
3. Patient has partial identity and later identity enrichment.
4. Patient has clinical correction event.
5. Patient has referral history.
6. Patient has document timeline.
7. Patient has merge and deduplication timeline.
8. Patient event has audit trail reference.

## Evidence rules

Do not store:

- real patient data.
- credentials.
- connection strings.
- raw clinical notes from real patients.
- unredacted screenshots.
- database dumps.
- secrets.

No secrets in repository.

## Sanitized evidence allowed

Allowed artifacts:

- synthetic patient identifiers.
- sanitized audit event JSON.
- sanitized HTTP status output.
- sanitized domain event output.
- sanitized database query result.
- test correlation id.
- test request id.
- test device id.
- test organization id.
- screenshot without sensitive data.

## Failure handling

If an expected audit event is missing:

1. Stop.
2. Record blocker.
3. Record missing event type.
4. Record owner.
5. Do not mark backend closure.
6. Do not proceed to API contract freeze as if auditability were proven.

## P3.9 conclusion

P3.9 evidence must prove auditability and longitudinal history before later backend closure.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE