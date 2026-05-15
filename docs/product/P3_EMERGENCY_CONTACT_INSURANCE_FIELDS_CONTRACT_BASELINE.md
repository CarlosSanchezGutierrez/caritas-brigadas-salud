# P3 Emergency Contact and Insurance Fields Contract Baseline

Status: active
Scope: emergency contact, social security, and insurance fields contract
Target phase: P3-30C
Depends on: P3-30B consent and signature evidence contract

---

## 1. Purpose

P3-30C defines the emergency contact, social security, and insurance fields contract required before full frontend implementation.

The goal is to prevent frontend, backend, database, and offline sync drift for contact and coverage information.

This baseline does not create a production migration.

This baseline freezes expected field semantics, optionality rules, validation rules, sensitive data handling, and frontend contract expectations.

---

## 2. Product position

Emergency contact and insurance information must support medical brigade reality:

- patients may not have emergency contact information;
- patients may not have social security or insurance;
- patients may not know their provider;
- patients may refuse to provide contact or coverage data;
- migrant or vulnerable patients may have incomplete data;
- the workflow must not block urgent intake only because these fields are missing.

Emergency contact and insurance data improve care continuity, but they must not become rigid blockers for vulnerable patient intake.

---

## 3. Required emergency contact fields

The emergency contact contract must define:

- hasEmergencyContact;
- emergencyContactFullName;
- emergencyContactPhoneNumber;
- emergencyContactRelationship;
- emergencyContactNotes;
- emergencyContactIsUnavailable;
- emergencyContactUnavailableReason.

---

## 4. Required insurance and social security fields

The insurance/social security contract must define:

- hasSocialSecurity;
- socialSecurityProvider;
- socialSecurityProviderOther;
- hasPrivateInsurance;
- privateInsuranceProvider;
- insuranceCoverageNotes;
- insuranceInformationUnavailable;
- insuranceInformationUnavailableReason.

The MVP should not require national social security numbers or policy numbers unless Caritas explicitly confirms they are operationally necessary.

If future work adds identifiers, those identifiers must be treated as highly sensitive data.

---

## 5. Emergency contact optionality rules

Emergency contact fields are optional by default.

If hasEmergencyContact is true, these fields are required:

- emergencyContactFullName;
- emergencyContactPhoneNumber;
- emergencyContactRelationship.

If emergencyContactIsUnavailable is true, emergencyContactUnavailableReason is required.

Emergency contact must not block patient intake when the patient cannot provide it.

---

## 6. Insurance optionality rules

Insurance and social security fields are optional by default.

If hasSocialSecurity is true, socialSecurityProvider is recommended.

If socialSecurityProvider is OTHER, socialSecurityProviderOther is required.

If hasPrivateInsurance is true, privateInsuranceProvider is recommended.

If insuranceInformationUnavailable is true, insuranceInformationUnavailableReason is required.

Insurance/social security must not block patient intake when the patient cannot provide it.

---

## 7. Provider values

Required socialSecurityProvider values:

- IMSS;
- ISSSTE;
- PEMEX;
- SEDENA;
- SEMAR;
- STATE_PUBLIC_SERVICE;
- PRIVATE;
- NONE;
- UNKNOWN;
- OTHER.

Required emergencyContactRelationship examples:

- SPOUSE;
- PARENT;
- CHILD;
- SIBLING;
- RELATIVE;
- FRIEND;
- GUARDIAN;
- OTHER;
- UNKNOWN.

---

## 8. Sensitive data classification

Emergency contact and insurance data must be treated as sensitive or potentially sensitive data.

Sensitive fields include:

- emergencyContactFullName;
- emergencyContactPhoneNumber;
- emergencyContactRelationship;
- emergencyContactNotes;
- socialSecurityProvider;
- socialSecurityProviderOther;
- privateInsuranceProvider;
- insuranceCoverageNotes;
- future insurance identifiers;
- future social security identifiers.

The API must not expose these values in logs.

The frontend must not store these values outside approved offline storage rules.

---

## 9. Validation requirements

Validation must include:

- emergencyContactFullName required when hasEmergencyContact is true;
- emergencyContactPhoneNumber required when hasEmergencyContact is true;
- emergencyContactRelationship required when hasEmergencyContact is true;
- emergencyContactUnavailableReason required when emergencyContactIsUnavailable is true;
- socialSecurityProviderOther required when socialSecurityProvider is OTHER;
- insuranceInformationUnavailableReason required when insuranceInformationUnavailable is true;
- phone format tolerance;
- maximum length checks;
- whitespace normalization.

Validation must favor successful intake with explicit unavailable-data flags over rejecting vulnerable patients.

---

## 10. Offline sync requirements

Patient intake sync payloads must preserve:

- emergency contact fields;
- insurance/social security fields;
- unavailable-data indicators;
- unavailable-data reasons;
- capturedAtUtc;
- capturedByUserId;
- deviceId;
- localPatientKey;
- payloadVersion.

Required rejection examples:

- emergency_contact_name_missing;
- emergency_contact_phone_missing;
- emergency_contact_relationship_missing;
- emergency_contact_unavailable_reason_missing;
- social_security_provider_other_missing;
- insurance_unavailable_reason_missing.

---

## 11. Frontend requirements

The frontend must support:

- emergency contact section;
- toggle for hasEmergencyContact;
- toggle for emergencyContactIsUnavailable;
- reason field when emergency contact is unavailable;
- insurance/social security section;
- toggle for hasSocialSecurity;
- provider selector;
- OTHER provider text input;
- toggle for hasPrivateInsurance;
- private insurance provider input;
- unavailable insurance reason field;
- clear Spanish labels;
- offline save behavior;
- pending sync behavior.

---

## 12. Non-goals

P3-30C does not create database migrations.

P3-30C does not implement emergency contact endpoints.

P3-30C does not implement insurance/social security endpoints.

P3-30C does not freeze the full OpenAPI contract.

P3-30C does not start frontend implementation.

P3-30C does not require national social security numbers or insurance policy numbers.

---

## 13. Acceptance criteria

P3-30C is complete when:

- this emergency contact and insurance baseline exists;
- the emergency contact and insurance fields contract document exists;
- the emergency contact and insurance verifier exists;
- emergency contact and insurance contract tests exist;
- the patient intake functional contract references P3-30C;
- the consent signature contract references P3-30C;
- the security/product gap audit references P3-30C;
- repository governance validation includes the emergency contact and insurance verifier;
- dotnet build and dotnet test pass.