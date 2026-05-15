# P3 Consent and Signature Evidence Contract

Status: active
Phase: P3-30B
Frontend readiness impact: BLOCKS_FULL_FRONTEND
Production readiness impact: BLOCKS_PRODUCTION_UNTIL_IMPLEMENTED_AND_EVIDENCED

---

## 1. Executive summary

The consent and signature evidence contract defines how the system captures and preserves privacy notice acceptance for patient care during medical brigades.

The frontend must be able to present the privacy notice, capture patient or guardian acceptance, capture signature evidence, store locally when offline, and sync later.

The backend must preserve auditable evidence without logging sensitive signature data.

---

## 2. Core consent workflow

Expected consent workflow:

1. User opens patient intake or visit intake.
2. System displays privacy notice version.
3. Patient, guardian, or witness accepts/refuses.
4. User captures drawn signature or documented fallback.
5. System stores consent evidence locally if offline.
6. System syncs consent event when connectivity exists.
7. Backend accepts, rejects, conflicts, replaces, or voids consent.
8. Frontend shows read-only consent status before clinical workflow continues.

---

## 3. Required fields

| Field | Type | Required | Notes |
|---|---|---:|---|
| consentDocumentId | Guid | Server-generated | Canonical backend identifier |
| organizationId | Guid | Yes | Tenant boundary |
| patientId | Guid | Conditional | Required when patient already exists |
| patientLocalKey | string | Conditional | Required for offline patient |
| brigadeId | Guid | Conditional | Required when consent is brigade-specific |
| visitId | Guid | No | Optional until visit exists |
| privacyNoticeVersion | string | Yes | Version shown to signer |
| privacyNoticeLanguage | string | Yes | Example: es-MX |
| consentStatus | enum | Yes | ACCEPTED, REFUSED, etc. |
| consentedAtUtc | datetime | Conditional | Required for accepted consent |
| capturedAtUtc | datetime | Yes | Device/user capture time |
| capturedByUserId | Guid | Yes | Audit |
| deviceId | Guid/string | Yes for offline | Device traceability |
| signatureMethod | enum | Yes | DRAWN_SIGNATURE or fallback |
| signerType | enum | Yes | PATIENT, GUARDIAN, etc. |
| signerFullName | string | Conditional | Required for witness/typed flows |
| signerRelationshipToPatient | string | Conditional | Required for guardian |
| signatureStorageMode | enum | Conditional | FILE_REFERENCE, LOCAL_PENDING, etc. |
| signatureObjectKey | string | Conditional | External object/file reference |
| signatureFileName | string | No | Optional display/debug metadata |
| signatureMimeType | string | Conditional | image/png recommended |
| signatureSizeBytes | long | Conditional | Required when binary exists |
| signatureSha256 | string | Conditional | Required when binary exists |
| signatureWidth | int | No | Optional drawn-signature metadata |
| signatureHeight | int | No | Optional drawn-signature metadata |
| signaturePointCount | int | No | Optional drawn-signature metadata |
| consentTextSnapshotHash | string | Yes | Integrity of shown consent text/version |
| isOfflineCapture | bool | Yes | Offline traceability |
| syncEventId | Guid | Conditional | Required when synced through offline event |
| notes | string | No | Sensitive internal note |
| createdAtUtc | datetime | Server-generated | Audit |
| updatedAtUtc | datetime | Server-generated | Audit |

---

## 4. Consent status behavior

| Status | Meaning | Medical workflow behavior |
|---|---|---|
| ACCEPTED | Patient accepted privacy notice | Continue |
| GUARDIAN_ACCEPTED | Guardian accepted for patient | Continue |
| WITNESS_ACCEPTED | Witness confirms acceptance | Continue with evidence |
| REFUSED | Patient refused | Do not silently proceed as accepted |
| UNABLE_TO_SIGN | Patient cannot sign | Continue only with documented reason/policy |
| REPLACED | Later consent replaced earlier evidence | Use latest valid consent |
| VOIDED | Consent evidence invalidated | Requires audit reason |

---

## 5. Signature method behavior

| Method | Meaning | Evidence required |
|---|---|---|
| DRAWN_SIGNATURE | Patient signs on device | Signature file/points/hash metadata |
| TYPED_NAME | Typed name acknowledgment | signerFullName and timestamp |
| CHECKBOX_ACKNOWLEDGEMENT | Explicit checkbox acceptance | consentTextSnapshotHash and timestamp |
| WITNESS_CONFIRMATION | Witness confirms consent | witness signerFullName |
| GUARDIAN_SIGNATURE | Guardian signs | relationship and signature evidence |
| UNABLE_TO_SIGN_REASON | Patient cannot sign | unable-to-sign reason |

Preferred MVP method:

DRAWN_SIGNATURE

Allowed fallback:

UNABLE_TO_SIGN_REASON with explicit reason.

---

## 6. Storage contract

Recommended MVP storage contract:

- signature binary is not logged;
- signature metadata is stored in SQL Server;
- signature file/object reference is stored as signatureObjectKey;
- signatureSha256 is stored for integrity;
- raw base64 signature must not appear in telemetry logs;
- local offline file may exist temporarily before sync;
- object/file storage can be local filesystem, controlled server storage, or future cloud object storage depending on infrastructure.

The API should treat signature evidence as sensitive.

---

## 7. Offline sync contract

Consent sync event payload must include:

| Field | Required | Purpose |
|---|---:|---|
| eventId | Yes | Idempotency |
| eventType | Yes | consent_document_created, replaced, or voided |
| deviceId | Yes | Device traceability |
| organizationId | Yes | Tenant boundary |
| patientId or patientLocalKey | Yes | Patient link |
| brigadeId | Conditional | Brigade context |
| capturedAtUtc | Yes | Audit |
| capturedByUserId | Yes | Audit |
| payloadVersion | Yes | Compatibility |
| consent | Yes | Consent payload |
| signatureMetadata | Conditional | Required for signature evidence |

Required event types:

- consent_document_created;
- consent_document_replaced;
- consent_document_voided.

Required rejection examples:

- consent_patient_missing;
- consent_privacy_notice_version_missing;
- consent_status_missing;
- consent_signature_missing;
- consent_guardian_relationship_missing;
- consent_witness_name_missing;
- consent_refusal_reason_missing;
- consent_unable_to_sign_reason_missing;
- consent_signature_hash_missing.

---

## 8. Validation rules

| Rule | Severity | Frontend behavior | Backend behavior |
|---|---|---|---|
| organizationId missing | Blocking | Show error | Reject |
| patientId and patientLocalKey missing | Blocking | Show error | Reject |
| privacyNoticeVersion missing | Blocking | Show error | Reject |
| consentStatus missing | Blocking | Show error | Reject |
| capturedAtUtc missing | Blocking | Show error | Reject |
| capturedByUserId missing | Blocking | Show error | Reject |
| ACCEPTED without signature evidence when DRAWN_SIGNATURE | Blocking | Re-capture signature | Reject |
| GUARDIAN signer without relationship | Blocking | Show error | Reject |
| WITNESS signer without full name | Blocking | Show error | Reject |
| REFUSED without reason | Blocking | Show error | Reject |
| UNABLE_TO_SIGN without reason | Blocking | Show error | Reject |
| signature binary exists without signatureSha256 | Blocking | Recompute hash | Reject |
| optional file metadata missing | Non-blocking | Allow save | Accept if hash/evidence exists |

---

## 9. Spanish frontend labels

| Field/Action | Label |
|---|---|
| privacyNoticeVersion | Versión del aviso de privacidad |
| consentStatus | Estado del consentimiento |
| accepted | Aceptado |
| refused | Rechazado |
| unableToSign | No puede firmar |
| signatureMethod | Método de firma |
| drawnSignature | Firma dibujada |
| typedName | Nombre escrito |
| signerFullName | Nombre de quien firma |
| signerRelationshipToPatient | Relación con el paciente |
| guardian | Tutor o responsable |
| witness | Testigo |
| refusalReason | Motivo de rechazo |
| unableToSignReason | Motivo por el que no puede firmar |
| pendingSync | Pendiente de sincronizacion |
| signatureSaved | Firma guardada |

---

## 10. Frontend readiness result

Frontend readiness after this phase:

PARTIAL_CONSENT_FRONTEND_READY

Allowed after P3-30B:

- privacy notice display mock;
- signature UI wireframe;
- signature canvas prototype;
- fallback reason UI;
- pending sync UI mock.

Not allowed until P3-30C/P3-30D:

- final patient intake implementation;
- final emergency contact/insurance UI;
- production API integration;
- final OpenAPI client generation.

---

## 11. Privacy and logging

Never log:

- raw signature image;
- base64 signature;
- signerFullName;
- patient full name;
- notes;
- PayloadJson;
- object storage signed URLs;
- secrets or access tokens.

Allowed telemetry:

- correlation id;
- request id;
- sanitized endpoint route;
- status code;
- elapsed time;
- consent event status;
- non-sensitive rejection reason.