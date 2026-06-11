# P3.9 Auditable Action Matrix

## Purpose

This matrix defines which actions must generate audit trail evidence.

## Required fields for every audit event

| Field | Required | Purpose |
|---|---:|---|
| actor | Yes | Human or service identity that initiated the action |
| action | Yes | Operation performed |
| entity | Yes | Domain object affected |
| entity id | Yes | Stable identifier of affected object |
| timestamp | Yes | Time of action |
| correlation id | Yes | Cross-service traceability |
| request id | Yes | HTTP/request-level traceability |
| source ip | Conditional | Network origin when available |
| device id | Conditional | Required for mobile/offline context |
| organization id | Yes | Institutional/tenant boundary |
| user role | Yes | Authorization context |
| result | Yes | success, denied, failed, partial |
| reason | Conditional | Required for correction, denial, deletion, rejection |
| before snapshot reference | Conditional | Required for corrections |
| after snapshot reference | Conditional | Required for corrections |
| audit trail | Yes | Persistent audit reference |

## Patient actions

| Action | Entity | Required audit evidence |
|---|---|---|
| create patient | Patient | actor, action, entity, organization id, timestamp, correlation id |
| update patient | Patient | before snapshot reference, after snapshot reference, reason |
| register partial identity | PatientIdentity | actor, reason, organization id |
| merge patient | Patient | source patient, target patient, reviewer, reason |
| mark duplicate candidate | Patient | matching signal, reviewer, status |
| deactivate patient record | Patient | reason, actor, approval if required |

## Consent actions

| Action | Entity | Required audit evidence |
|---|---|---|
| capture consent | Consent | consent version, patient reference, actor |
| revoke consent | Consent | reason, actor, timestamp |
| update privacy notice version | PrivacyNotice | version, approver, effective date |
| consent exception | Consent | reason, approval reference |

## Encounter actions

| Action | Entity | Required audit evidence |
|---|---|---|
| create encounter | Encounter | patient reference, brigade reference, service reference |
| update encounter | Encounter | before snapshot reference, after snapshot reference, reason |
| close encounter | Encounter | actor, timestamp, result |
| reopen encounter | Encounter | reason, actor, approval if required |

## Clinical actions

| Action | Entity | Required audit evidence |
|---|---|---|
| capture vital signs | VitalSigns | actor, patient reference, encounter reference |
| correct vital signs | VitalSigns | before snapshot reference, after snapshot reference, reason |
| create clinical note | ClinicalNote | actor, encounter reference |
| correct clinical note | ClinicalNote | correction event, reason |
| create referral | Referral | destination, reason, actor |
| record medication | MedicationRecord | actor, encounter reference |

## Controlled data injection actions

| Action | Entity | Required audit evidence |
|---|---|---|
| receive batch | DataInjectionBatch | batch id, source system, operator |
| accept records | DataInjectionBatch | accepted records, idempotency key |
| reject records | DataInjectionBatch | rejected records, reason |
| quarantine records | DataInjectionBatch | quarantine state, reviewer |
| replay batch | DataInjectionBatch | idempotency key, result |

## Reporting and export actions

| Action | Entity | Required audit evidence |
|---|---|---|
| export report | ReportExport | actor, filters, organization id, timestamp |
| refresh dashboard dataset | DashboardDataset | source snapshot, timestamp |
| generate analytical snapshot | AnalyticalSnapshot | source data range, responsible owner |

## Security actions

| Action | Entity | Required audit evidence |
|---|---|---|
| failed login | UserSession | actor or attempted identity, source ip |
| permission denied | AuthorizationDecision | actor, action, entity, user role |
| role changed | UserRole | actor, target user, before and after role |
| suspicious request | SecurityEvent | request id, correlation id, source ip |

## Matrix conclusion

Every listed action must preserve an audit trail.

No silent overwrite is allowed.