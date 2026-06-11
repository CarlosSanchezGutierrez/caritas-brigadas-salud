# P5.8 Patient Longitudinal History Timeline Matrix

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Acceptance criteria

| Area | Required evidence | Required for P5.8 merge | Production-closing |
|---|---|---:|---:|
| Timeline contract | PatientClinicalRecordDto exposes Timeline | Yes | No |
| Timeline event contract | PatientClinicalRecordTimelineEventDto exists | Yes | No |
| Visit events | Timeline includes visit events | Yes | No |
| Encounter events | Timeline includes service-encounter events | Yes | No |
| Vital sign events | Timeline includes vital-signs events | Yes | No |
| Form response events | Timeline includes form-response events | Yes | No |
| Consent events | Timeline includes consent-document events | Yes | No |
| Referral events | Timeline includes medical-referral events | Yes | No |
| Medication events | Timeline includes medication-delivery events | Yes | No |
| Ordering | Timeline is ordered descending by OccurredAt | Yes | No |
| Summary | Summary includes TimelineEventCount and first/last timeline timestamps | Yes | No |
| Existing collections | Existing clinical record collections remain available | Yes | No |
| Verifier | P5.8 verifier passes | Yes | No |
| Build | Contracts, Infrastructure, and API build in Release | Yes | No |
| Tests | API test suite passes | Yes | No |

## Rejection criteria

Reject P5.8 if it removes existing clinical record collections, creates a new persistence table unnecessarily, weakens tenant scoping, allows direct mobile SQL Server writes, commits real patient data, adds secrets, introduces a cloud dependency, or claims production readiness closure.