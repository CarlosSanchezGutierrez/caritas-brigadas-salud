# P3 Patient Intake Functional Contract

Status: active
Phase: P3-30A
Frontend readiness impact: BLOCKS_FULL_FRONTEND
Production readiness impact: BLOCKS_PRODUCTION_UNTIL_IMPLEMENTED_AND_EVIDENCED

---

## 1. Executive summary

The patient intake contract defines how the backend and frontend must represent a patient before clinical service documentation begins.

The contract intentionally supports incomplete information because Cáritas brigades may serve vulnerable patients, migrant patients, or patients without formal documents.

The frontend must not force complete demographic data before a medical encounter can be created.

The backend must preserve enough identity, audit, and sync information to avoid duplicate records and support later reporting.

---

## 2. Core intake workflow

Expected patient intake flow:

1. User opens patient intake.
2. User captures minimum available identity data.
3. User marks incomplete identity when needed.
4. User captures optional demographic/contact fields when available.
5. User saves locally when offline.
6. User syncs patient intake event when connectivity exists.
7. Backend accepts, rejects, or flags conflict.
8. Frontend displays accepted patient identity label.
9. Visit/service workflow continues.

---

## 3. Required backend contract fields

| Field | Type | Required | Notes |
|---|---|---:|---|
| patientId | Guid | Server-generated | Canonical backend identifier |
| organizationId | Guid | Yes | Tenant/organization boundary |
| localPatientKey | string | Yes for offline | Device/local idempotency key |
| firstName | string | Conditional | Usable identity label when available |
| paternalLastName | string | No | Optional |
| maternalLastName | string | No | Optional |
| displayName | string | Conditional | Used when formal name is incomplete |
| dateOfBirth | date | No | Optional |
| approximateAgeYears | int | No | Optional alternative when dateOfBirth is unavailable |
| sex | enum | No | Biological/clinical sex when clinically relevant |
| genderIdentity | enum/string | No | Optional and non-blocking |
| phoneNumber | string | No | Optional |
| addressLine | string | No | Optional |
| colony | string | No | Optional |
| municipality | string | No | Optional |
| state | string | No | Optional |
| country | string | No | Optional |
| postalCode | string | No | Optional |
| isIdentityIncomplete | bool | Yes | Explicit incomplete data flag |
| identityIncompleteReason | string | Conditional | Required when isIdentityIncomplete is true |
| notes | string | No | Internal intake note, must not leak into logs |
| createdAtUtc | datetime | Server-generated | Audit |
| updatedAtUtc | datetime | Server-generated | Audit |
| capturedAtUtc | datetime | Yes for sync | Device/user capture timestamp |
| capturedByUserId | Guid | Yes | Auditability |

---

## 4. Minimum valid patient intake

A patient intake request is valid when it contains:

- organizationId;
- capturedAtUtc;
- capturedByUserId;
- localPatientKey for offline-created patients;
- at least one usable identity label.

A usable identity label is one of:

- firstName;
- displayName;
- temporary intake label;
- local anonymous identifier.

The request must not fail only because these are missing:

- paternalLastName;
- maternalLastName;
- dateOfBirth;
- phoneNumber;
- address;
- CURP;
- government id;
- insurance/social security;
- emergency contact.

---

## 5. Incomplete identity behavior

When patient information is incomplete:

| Condition | Required behavior |
|---|---|
| Patient has only first name | Accept with isIdentityIncomplete=true |
| Patient has no formal last name | Accept with isIdentityIncomplete=true |
| Patient has no date of birth | Accept if approximateAgeYears or identity label exists |
| Patient has no phone | Accept |
| Patient has no address | Accept |
| Patient has no CURP/government id | Accept |
| Patient is migrant/vulnerable and cannot provide full data | Accept with reason |
| Patient refuses to provide optional demographic data | Accept with reason |

Recommended identityIncompleteReason values:

- MIGRANT_OR_TRANSIENT;
- DOES_NOT_REMEMBER;
- REFUSED_TO_ANSWER;
- NO_DOCUMENTS_AVAILABLE;
- EMERGENCY_OR_FAST_INTAKE;
- OTHER.

---

## 6. Validation rules

| Rule | Severity | Frontend behavior | Backend behavior |
|---|---|---|---|
| organizationId missing | Blocking | Show error | Reject |
| capturedByUserId missing | Blocking | Show error | Reject |
| capturedAtUtc missing in sync | Blocking | Show error | Reject |
| no usable identity label | Blocking | Show error | Reject |
| dateOfBirth in future | Blocking | Show error | Reject |
| approximateAgeYears negative | Blocking | Show error | Reject |
| isIdentityIncomplete=true without reason | Blocking | Show error | Reject |
| phone format unusual | Warning | Allow save with warning | Accept normalized/original |
| optional fields missing | Non-blocking | Allow save | Accept |

---

## 7. Spanish frontend labels

Recommended Spanish labels:

| Field | Label |
|---|---|
| firstName | Nombre |
| paternalLastName | Apellido paterno |
| maternalLastName | Apellido materno |
| displayName | Nombre visible o referencia |
| dateOfBirth | Fecha de nacimiento |
| approximateAgeYears | Edad aproximada |
| sex | Sexo |
| genderIdentity | Identidad de género |
| phoneNumber | Teléfono |
| addressLine | Dirección |
| colony | Colonia |
| municipality | Municipio |
| state | Estado |
| country | País |
| postalCode | Código postal |
| isIdentityIncomplete | Datos incompletos |
| identityIncompleteReason | Motivo de datos incompletos |
| notes | Nota interna |

---

## 8. Offline sync contract

Patient intake sync events must include:

| Field | Required | Purpose |
|---|---:|---|
| eventId | Yes | Idempotency |
| eventType | Yes | patient_created or patient_updated |
| deviceId | Yes | Device traceability |
| organizationId | Yes | Tenant boundary |
| localPatientKey | Yes | Offline identity |
| capturedAtUtc | Yes | Audit |
| capturedByUserId | Yes | Audit |
| payloadVersion | Yes | Compatibility |
| patient | Yes | Patient intake payload |

Required event types:

- patient_created;
- patient_updated.

Required sync outcomes:

- accepted;
- rejected;
- conflict.

Required rejection examples:

- patient_identity_label_missing;
- patient_captured_by_user_missing;
- patient_captured_at_missing;
- patient_date_of_birth_in_future;
- patient_identity_incomplete_reason_missing.

---

## 9. Search/display behavior

Patient display label priority:

1. full name when available;
2. displayName;
3. firstName;
4. local anonymous identifier;
5. localPatientKey fallback.

Patient search should support:

- name;
- displayName;
- phone when available;
- localPatientKey;
- patientId;
- approximate age context when useful.

---

## 10. Privacy and logging requirements

Patient intake data must not be logged as raw request body.

Request telemetry must not include:

- full name;
- phone number;
- address;
- notes;
- national identifiers;
- social security / insurance data;
- emergency contact data;
- PayloadJson.

Logs may include:

- correlation id;
- request id;
- endpoint route;
- status code;
- elapsed time;
- sanitized route;
- non-sensitive event status.

---

## 11. Relationship to next phases

P3-30B will define:

- privacy notice consent;
- patient signature;
- consent version;
- signature evidence;
- signature storage strategy;
- signature hash/checksum.

P3-30C will define:

- emergency contact;
- social security / insurance;
- optional medical coverage fields.

P3-30D will freeze:

- OpenAPI contract;
- DTO names;
- endpoint request/response examples;
- frontend integration contract.

---

## 12. Frontend readiness result

Frontend readiness after this phase:

PARTIAL_FRONTEND_READY

Allowed after P3-30A:

- patient intake UI wireframe;
- form component layout;
- field labels;
- validation copy;
- mocked patient intake save.

Not allowed until P3-30B/P3-30C/P3-30D:

- final patient intake implementation;
- final consent/signature UI;
- final emergency contact/insurance UI;
- production API integration.
---

## 13. P3-30B consent and signature evidence contract

P3-30B defines privacy notice consent, patient or guardian signature evidence, consent status, signature method, signer type, offline consent sync, and logging restrictions.

Patient intake remains only partially frontend-ready until consent/signature, emergency contact, insurance/social security, and OpenAPI contracts are complete.
---

## 14. P3-30C emergency contact and insurance fields

P3-30C defines emergency contact, social security, and insurance field semantics, optionality, validation, privacy handling, and frontend expectations.

Patient intake remains only partially frontend-ready until OpenAPI/frontend contract freeze is complete.
---

## 15. P3-30D OpenAPI/frontend contract freeze

P3-30D freezes the frontend integration contract after patient intake, consent/signature, emergency contact, insurance/social security, and offline sync expectations have been documented.

After P3-30D, frontend MVP scaffolding is allowed.
