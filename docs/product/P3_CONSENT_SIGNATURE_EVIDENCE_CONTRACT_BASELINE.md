# P3 Consent and Signature Evidence Contract Baseline

Status: active
Scope: privacy notice consent and patient signature evidence contract
Target phase: P3-30B
Depends on: P3-30A patient intake functional contract

---

## 1. Purpose

P3-30B defines the consent and signature evidence contract required before building the frontend consent workflow.

The goal is to make patient consent legally traceable, auditable, sync-compatible, and safe for offline brigade usage.

This baseline does not create a production migration.

This baseline freezes expected consent behavior, signature evidence semantics, validation rules, offline sync rules, and frontend contract expectations.

---

## 2. Consent position

The consent flow must support:

- privacy notice presentation;
- explicit patient acceptance;
- patient or guardian signature;
- timestamped consent evidence;
- versioned privacy notice;
- offline-first capture;
- later sync to backend;
- auditability by user, device, organization, brigade, and patient;
- incomplete or exceptional consent scenarios.

---

## 3. Required consent fields

The consent and signature contract must define:

- consentDocumentId;
- organizationId;
- patientId;
- patientLocalKey;
- brigadeId;
- visitId;
- privacyNoticeVersion;
- privacyNoticeLanguage;
- consentStatus;
- consentedAtUtc;
- capturedAtUtc;
- capturedByUserId;
- deviceId;
- signatureMethod;
- signerType;
- signerFullName;
- signerRelationshipToPatient;
- refusalReason;
- unableToSignReason;
- voidReason;
- signatureStorageMode;
- signatureObjectKey;
- signatureFileName;
- signatureMimeType;
- signatureSizeBytes;
- signatureSha256;
- signatureWidth;
- signatureHeight;
- signaturePointCount;
- consentTextSnapshotHash;
- isOfflineCapture;
- syncEventId;
- notes;
- createdAtUtc;
- updatedAtUtc.

---

## 4. Consent status values

Required consentStatus values:

- ACCEPTED;
- REFUSED;
- UNABLE_TO_SIGN;
- GUARDIAN_ACCEPTED;
- WITNESS_ACCEPTED;
- REPLACED;
- VOIDED.

ACCEPTED and GUARDIAN_ACCEPTED allow the medical workflow to continue.

REFUSED must not be silently converted into acceptance.

UNABLE_TO_SIGN requires reason and alternate evidence.

VOIDED requires audit reason.

---

## 5. Signature method values

Required signatureMethod values:

- DRAWN_SIGNATURE;
- TYPED_NAME;
- CHECKBOX_ACKNOWLEDGEMENT;
- WITNESS_CONFIRMATION;
- GUARDIAN_SIGNATURE;
- UNABLE_TO_SIGN_REASON.

The preferred MVP method is DRAWN_SIGNATURE.

Fallback methods are allowed only when explicitly documented.

---

## 6. Signer type values

Required signerType values:

- PATIENT;
- GUARDIAN;
- WITNESS;
- STAFF_ASSISTED;
- UNKNOWN.

When signerType is GUARDIAN, signerRelationshipToPatient is required.

When signerType is WITNESS, signerFullName is required.

---

## 7. Signature evidence requirements

Signature evidence must include:

- signatureStorageMode;
- signatureObjectKey or equivalent file reference;
- signatureMimeType;
- signatureSizeBytes;
- signatureSha256;
- capturedAtUtc;
- capturedByUserId;
- deviceId;
- privacyNoticeVersion;
- consentTextSnapshotHash.

Raw signature images must not be logged.

Signature evidence must be treated as sensitive data.

---

## 8. Storage strategy

The contract must support at least these storage strategies:

- database metadata with external object storage reference;
- local offline temporary storage before sync;
- future encrypted file/object storage.

Recommended MVP approach:

- store metadata in SQL Server;
- store binary signature file outside logs;
- store file/object reference in the database;
- store signatureSha256 for integrity validation;
- never include raw signature data in request telemetry logs.

---

## 9. Offline sync requirements

Consent signature sync payloads must preserve:

- eventId;
- eventType;
- deviceId;
- organizationId;
- patientId or patientLocalKey;
- brigadeId;
- visitId when available;
- capturedAtUtc;
- capturedByUserId;
- privacyNoticeVersion;
- consentStatus;
- signature metadata;
- signatureSha256;
- payloadVersion.

Required event types:

- consent_document_created;
- consent_document_replaced;
- consent_document_voided.

Required sync outcomes:

- accepted;
- rejected;
- conflict.

---

## 10. Validation requirements

Consent validation must include:

- organizationId required;
- patientId or patientLocalKey required;
- privacyNoticeVersion required;
- consentStatus required;
- capturedAtUtc required;
- capturedByUserId required;
- deviceId required for offline capture;
- signature evidence required for ACCEPTED or GUARDIAN_ACCEPTED when signatureMethod is DRAWN_SIGNATURE;
- signerRelationshipToPatient required for GUARDIAN;
- signerFullName required for WITNESS;
- refusalReason required for REFUSED;
- unableToSignReason required for UNABLE_TO_SIGN;
- voidReason required for VOIDED;
- signatureSha256 required when binary signature evidence exists.

---

## 11. Frontend requirements

The frontend must support:

- showing privacy notice version;
- capturing drawn signature;
- fallback typed name or unable-to-sign reason;
- guardian or witness signer flow;
- offline save;
- pending sync status;
- re-capture before sync when signature is invalid;
- read-only consent evidence after acceptance;
- clear Spanish labels.

---

## 12. Spanish frontend labels

Recommended Spanish labels:

- Aviso de privacidad;
- Versión del aviso de privacidad;
- Acepto el aviso de privacidad;
- Firma del paciente;
- Nombre de quien firma;
- Relación con el paciente;
- Tutor o responsable;
- Testigo;
- No puede firmar;
- Motivo por el que no puede firmar;
- Rechazó firmar;
- Motivo de rechazo;
- Firma guardada;
- Pendiente de sincronizacion.

---

## 13. Privacy and logging requirements

Consent and signature data must not be logged as raw request body.

Request telemetry must not include:

- raw signature image;
- base64 signature;
- signer full name;
- patient full name;
- notes;
- PayloadJson;
- object storage signed URLs;
- secrets or access tokens.

Logs may include:

- correlation id;
- request id;
- endpoint route;
- status code;
- elapsed time;
- sanitized route;
- consent event status;
- non-sensitive rejection reason.

---

## 14. Non-goals

P3-30B does not create database migrations.

P3-30B does not implement frontend signature canvas.

P3-30B does not implement file/object storage.

P3-30B does not finalize emergency contact fields.

P3-30B does not finalize insurance/social security fields.

P3-30B does not freeze the full OpenAPI contract.

---

## 15. Acceptance criteria

P3-30B is complete when:

- this consent and signature evidence baseline exists;
- the consent and signature evidence contract document exists;
- the consent signature verifier exists;
- consent signature contract tests exist;
- the patient intake functional contract references P3-30B;
- the security/product gap audit references P3-30B;
- repository governance validation includes the consent signature verifier;
- dotnet build and dotnet test pass.