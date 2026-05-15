# P3 OpenAPI Frontend Contract Freeze

Status: active
Phase: P3-30D
Frontend readiness impact: UNBLOCKS_FRONTEND_MVP_SCAFFOLD
Production readiness impact: BLOCKS_PRODUCTION_UNTIL_STAGING_AND_EVIDENCE_EXIST

---

## 1. Executive summary

This document freezes the frontend integration contract for the first Web/PWA, iOS, and Android implementations.

The backend contract is now sufficiently defined for frontend MVP scaffolding after P3-30A, P3-30B, P3-30C, and this P3-30D phase.

This does not mean the backend is production-ready.

This means the frontend team can start building against a stable API contract, mock API mode, and documented DTO expectations.

---

## 2. OpenAPI entry points

Development OpenAPI document:

- /openapi/v1/openapi.json

Development Swagger UI:

- /swagger

Swagger/OpenAPI must remain controlled by environment and configuration.

Production environments must not expose API documentation unless explicitly approved.

---

## 3. Client architecture rule

Clients must never connect directly to SQL Server.

Allowed clients:

- Web/PWA frontend;
- iOS app;
- Android app;
- controlled admin tools.

All clients must communicate through the backend API.

The backend API is responsible for:

- authentication;
- authorization;
- validation;
- rate limiting;
- audit;
- offline sync processing;
- database access;
- sensitive data controls;
- response/error shaping.

---

## 4. API base URL strategy

Frontend environment variables:

| Variable | Required | Purpose |
|---|---:|---|
| NEXT_PUBLIC_API_BASE_URL | Yes | Backend API base URL |
| NEXT_PUBLIC_API_TIMEOUT_MS | Yes | HTTP client timeout |
| NEXT_PUBLIC_ENABLE_MOCK_API | Yes | Enables mock mode before real API wiring |
| NEXT_PUBLIC_ENABLE_OFFLINE_MODE | Yes | Enables offline queue behavior |
| NEXT_PUBLIC_APP_ENVIRONMENT | Yes | local, development, staging, production |

Native clients must use equivalent configuration values.

No client may hardcode production URLs.

No client may store backend secrets.

---

## 5. Required headers

| Header | Required | Notes |
|---|---:|---|
| Authorization | Protected operations | Bearer token or configured auth mode |
| X-Correlation-Id | Recommended | Client-generated or backend-generated trace id |
| Content-Type | JSON requests | application/json |
| Accept | Yes | application/json |

Correlation id behavior:

- frontend may generate X-Correlation-Id;
- backend may generate fallback correlation id;
- frontend should display or persist correlation id only for diagnostic flows;
- correlation id must not contain patient data.

---

## 6. Response envelope rules

Frontend must support standardized success envelopes.

Expected success envelope concepts:

- success indicator;
- data payload;
- correlation id or trace id;
- optional metadata.

Frontend must not assume every endpoint returns a raw DTO.

Frontend API client should centralize response parsing.

---

## 7. Error envelope rules

Frontend must support standardized error envelopes.

Expected error concepts:

- error code;
- human-readable message;
- correlation id or trace id;
- field-level validation details when applicable.

Expected HTTP behavior:

| Status | Meaning | Frontend behavior |
|---|---|---|
| 400 | Validation error | Show field errors or form-level error |
| 401 | Unauthenticated | Redirect/login/session recovery |
| 403 | Unauthorized | Show permission error |
| 404 | Not found | Show not-found state |
| 409 | Conflict | Show sync conflict or duplicate state |
| 429 | Rate limited | Show retry/backoff message |
| 500 | Unexpected server error | Show generic error and correlation id |

---

## 8. Contract areas frozen for frontend MVP

The following areas are frozen for frontend MVP design and scaffolding:

- patient intake field semantics;
- incomplete patient identity behavior;
- consent and signature evidence semantics;
- refusalReason;
- unableToSignReason;
- voidReason;
- emergency contact fields;
- insurance/social security fields;
- sensitive logging restrictions;
- offline sync event expectations;
- rejection code mapping;
- frontend validation categories;
- Spanish label source;
- mock API allowance.

---

## 9. Patient intake contract reference

Frontend must follow:

- P3_PATIENT_INTAKE_FUNCTIONAL_CONTRACT.md

Key frontend assumptions:

- incomplete patient identity is allowed;
- missing CURP must not block intake;
- missing phone must not block intake;
- missing full address must not block intake;
- migrant or vulnerable patient intake is supported;
- isIdentityIncomplete and identityIncompleteReason must be represented.

---

## 10. Consent and signature contract reference

Frontend must follow:

- P3_CONSENT_SIGNATURE_EVIDENCE_CONTRACT.md

Key frontend assumptions:

- DRAWN_SIGNATURE is the preferred MVP signature method;
- privacyNoticeVersion is required;
- signatureSha256 is required when binary signature evidence exists;
- refusalReason is required for REFUSED;
- unableToSignReason is required for UNABLE_TO_SIGN;
- voidReason is required for VOIDED;
- raw signature data must not be logged.

---

## 11. Emergency contact and insurance contract reference

Frontend must follow:

- P3_EMERGENCY_CONTACT_INSURANCE_FIELDS_CONTRACT.md

Key frontend assumptions:

- emergency contact is optional by default;
- insurance/social security is optional by default;
- socialSecurityProvider is sensitive;
- emergencyContactRelationship is sensitive;
- national social security numbers and policy numbers are not part of MVP unless Caritas explicitly confirms operational need.

---

## 12. Offline sync frontend behavior

Frontend must support these states:

- saved locally;
- pending sync;
- syncing;
- accepted;
- rejected;
- conflict;
- retry required.

Frontend must preserve:

- eventId;
- eventType;
- deviceId;
- organizationId;
- localPatientKey;
- capturedAtUtc;
- capturedByUserId;
- payloadVersion.

Frontend must not silently drop rejected events.

Frontend must show actionable rejection or conflict states.

---

## 13. Rejection code mapping

Frontend must prepare handling for:

- patient_identity_label_missing;
- patient_captured_by_user_missing;
- patient_captured_at_missing;
- patient_date_of_birth_in_future;
- patient_identity_incomplete_reason_missing;
- consent_patient_missing;
- consent_privacy_notice_version_missing;
- consent_status_missing;
- consent_signature_missing;
- consent_guardian_relationship_missing;
- consent_witness_name_missing;
- consent_refusal_reason_missing;
- consent_unable_to_sign_reason_missing;
- consent_signature_hash_missing;
- emergency_contact_name_missing;
- emergency_contact_phone_missing;
- emergency_contact_relationship_missing;
- emergency_contact_unavailable_reason_missing;
- social_security_provider_other_missing;
- insurance_unavailable_reason_missing.

---

## 14. Sensitive data frontend rules

Frontend must not log sensitive values in browser console, native logs, telemetry, analytics, or crash reports.

Never log:

- patient full name;
- phone number;
- address;
- notes;
- raw signature image;
- base64 signature;
- signerFullName;
- emergencyContactFullName;
- emergencyContactPhoneNumber;
- emergencyContactRelationship;
- socialSecurityProvider;
- socialSecurityProviderOther;
- privateInsuranceProvider;
- insuranceCoverageNotes;
- PayloadJson.

---

## 15. Mock API allowance

Frontend may use mock API mode before staging exists.

Mock API mode must be clearly marked by:

- NEXT_PUBLIC_ENABLE_MOCK_API=true;
- visible development/staging banner;
- no production claim;
- no production evidence claim.

Mock API mode is allowed for:

- UI layout;
- form validation;
- interaction design;
- offline queue prototype;
- sync status prototype;
- demo walkthrough.

Mock API mode is not allowed as:

- production validation;
- staging evidence;
- security evidence;
- SQL Server connectivity evidence.

---

## 16. Frontend readiness result

Frontend readiness after P3-30D:

FRONTEND_MVP_SCAFFOLD_READY

Allowed after P3-30D:

- create frontend repository/app scaffold;
- build layout and navigation;
- build patient intake UI;
- build consent/signature UI;
- build emergency contact UI;
- build insurance/social security UI;
- build offline queue UI;
- build mock API client;
- prepare generated API client strategy.

Still blocked before production:

- real staging API integration evidence;
- SQL Server VM smoke evidence;
- auth configuration evidence;
- CORS/AllowedHosts evidence;
- backup/restore evidence;
- rollback evidence;
- observability evidence;
- final blocker matrix approval.

---

## 17. Contract change control

After P3-30D, frontend-facing contract changes require:

- product contract update;
- OpenAPI/frontend contract update;
- verifier/test update;
- frontend impact note;
- compatibility or migration note;
- review before merge.

Breaking changes must not be merged silently once frontend implementation depends on the contract.