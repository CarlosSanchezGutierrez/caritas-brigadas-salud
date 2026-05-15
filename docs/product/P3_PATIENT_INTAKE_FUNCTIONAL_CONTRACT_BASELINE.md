# P3 Patient Intake Functional Contract Baseline

Status: active
Scope: patient intake functional contract for backend, sync, and frontend readiness
Target phase: P3-30A
Depends on: P3-26L security and product readiness gap audit

---

## 1. Purpose

P3-30A defines the patient intake functional contract required before building the frontend patient intake workflow.

The goal is to prevent frontend, backend, database, and offline sync drift.

This baseline does not create a production migration.

This baseline freezes the expected patient intake behavior, field semantics, validation rules, optionality rules, and frontend contract expectations.

---

## 2. Patient intake position

The patient intake flow must support Cáritas medical brigade reality:

- fast intake during brigades;
- incomplete patient information;
- migrant or vulnerable patients without complete documents;
- optional phone, address, and formal identity data;
- legally traceable privacy notice and consent handled separately;
- offline-first capture through sync;
- later clinical visit/service documentation.

---

## 3. Required patient identity fields

The patient intake contract must define:

- patientId;
- organizationId;
- localPatientKey;
- firstName;
- paternalLastName;
- maternalLastName;
- displayName;
- dateOfBirth;
- approximateAgeYears;
- sex;
- genderIdentity;
- phoneNumber;
- addressLine;
- colony;
- municipality;
- state;
- country;
- postalCode;
- isIdentityIncomplete;
- identityIncompleteReason;
- notes;
- createdAtUtc;
- updatedAtUtc.

---

## 4. Minimum identity rule

Patient intake must allow incomplete information.

A patient record is valid when it has at least:

- organizationId;
- localPatientKey or patientId;
- capturedAtUtc;
- capturedByUserId;
- one usable identity label.

A usable identity label can be:

- firstName;
- displayName;
- temporary intake label;
- anonymous/local identifier for vulnerable patient intake.

The frontend must not block intake only because a patient has no CURP, no phone, no full name, or no address.

---

## 5. Optionality rules

The following fields must be optional:

- paternalLastName;
- maternalLastName;
- dateOfBirth;
- phoneNumber;
- addressLine;
- colony;
- postalCode;
- CURP or national identifier;
- formal government id;
- social security or insurance information;
- emergency contact.

Social security / insurance fields are finalized in P3-30C.

Emergency contact fields are finalized in P3-30C.

Consent and signature evidence are finalized in P3-30B.

---

## 6. Sensitive data classification

Patient intake must treat the following as sensitive or potentially sensitive data:

- name;
- date of birth;
- phone number;
- address;
- health-related notes;
- national identifiers;
- social security / insurance information;
- emergency contact information;
- consent/signature evidence.

The API must not expose sensitive data in logs.

The frontend must not store sensitive data outside approved local/offline storage rules.

---

## 7. Validation requirements

Patient intake validation must include:

- maximum length checks;
- whitespace normalization;
- phone format tolerance;
- date of birth cannot be in the future;
- approximate age cannot be negative;
- identityIncompleteReason required when isIdentityIncomplete is true;
- capturedAtUtc required for offline sync payloads;
- capturedByUserId required for auditability;
- organizationId required.

Validation must favor successful intake with explicit incomplete-data flags over rejecting vulnerable patients.

---

## 8. Offline sync requirements

Patient intake sync payloads must preserve:

- device id;
- local patient key;
- event id;
- event type;
- captured timestamp UTC;
- captured by user id;
- patient identity fields;
- incomplete data indicators;
- conflict behavior;
- rejection reason when invalid.

Patient intake events must be idempotent by localPatientKey, device id, organization id, and event id.

---

## 9. Frontend contract requirements

Before full frontend implementation, the frontend must know:

- required fields;
- optional fields;
- validation messages;
- field labels in Spanish;
- empty-state behavior;
- incomplete patient behavior;
- offline save behavior;
- sync conflict behavior;
- patient search display label;
- patient detail display label.

---


## 10. Migrant or incomplete patient data handling

Patient intake must support migrant, transient, vulnerable, undocumented, or incomplete-data scenarios.

The backend and frontend must allow intake when a patient cannot provide full legal identity, phone, address, CURP, government id, insurance/social security, or emergency contact.

Incomplete patient data must be represented explicitly through isIdentityIncomplete and identityIncompleteReason instead of being silently ignored.

The patient intake workflow must prioritize safe care continuity over rigid demographic completeness.

## 11. Non-goals

P3-30A does not create database migrations.

P3-30A does not implement patient signature capture.

P3-30A does not implement emergency contact fields.

P3-30A does not implement insurance/social security fields.

P3-30A does not freeze the full OpenAPI contract.

P3-30A does not start frontend implementation.

---

## 12. Acceptance criteria

P3-30A is complete when:

- this patient intake functional contract baseline exists;
- the patient intake functional contract document exists;
- the patient intake verifier exists;
- the patient intake contract tests exist;
- the security/product gap audit references P3-30A;
- repository governance validation includes the patient intake verifier;
- dotnet build and dotnet test pass.