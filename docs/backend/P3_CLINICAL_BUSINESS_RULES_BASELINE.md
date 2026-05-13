# P3 Clinical Business Rules Baseline

Status: active  
Scope: backend clinical workflow, patient expediente, visits, encounters, consent, vital signs, and tenant-safe clinical behavior  
Target phase: P3-04  
Depends on: P3 Architecture & Business Rules Decision Register, P3 Tenant Boundary & Authorization Inventory, P3 Endpoint Authorization Contracts, P3 Tenant Scope Contracts

---

## 1. Purpose

This document defines the clinical business rules that must guide the next backend changes.

P3-04 does not add new tables or runtime behavior. It freezes clinical rules before the backend adds or modifies domain models such as VitalSignsRecord and patient clinical record read models.

The goal is to prevent:

- flat patient records without history;
- overwriting clinical measurements;
- unclear expediente behavior;
- inconsistent patient recapture;
- cross-tenant clinical linkage;
- forms and documents attached to the wrong patient, visit, or encounter;
- offline sync payloads bypassing clinical rules.

---

## 2. Clinical domain model baseline

The clinical workflow must be modeled as a sequence of related records.

Canonical concepts:

| Concept | Meaning |
|---|---|
| Patient | Person receiving care. Owns stable or slowly changing identity data. |
| PatientGuardian | Guardian/contact data for a patient when needed. |
| PatientVisit | A patient's attendance to a brigade or care event. |
| ServiceEncounter | A specific service delivered during a visit. |
| VitalSignsRecord | Historical clinical measurements for a visit and optionally an encounter. |
| MedicalReferral | Referral generated from a clinical encounter or visit. |
| MedicationDelivery | Medication delivery record linked to patient and encounter when applicable. |
| FormResponse | Structured response for a service, visit, or encounter. |
| DocumentSignature | Signed consent or document evidence. |
| MediaRelease | Media consent attached to patient and optionally visit. |
| ClinicalRecord / Expediente | Read model that aggregates the patient's clinical history. |

Rule: Patient must not become a catch-all table for visit-specific clinical facts.

---

## 3. Patient identity rules

Patient identity data is stable or slowly changing.

Examples:

- name;
- date of birth if available;
- sex or gender fields if later approved;
- CURP or external identifier if available;
- phone or contact method;
- guardian/contact relation when applicable;
- organization ownership.

Rules:

- patient identity belongs to exactly one OrganizationId;
- patient identity can be reused across visits inside the same organization;
- patient identity updates must be auditable;
- patient identity changes must not delete or rewrite clinical history;
- patient identity must not be shared across organizations without an explicit approved policy;
- missing identifiers must be supported for vulnerable populations or patients without official documents.

---

## 4. Patient recapture and reconfirmation rules

The system must avoid unnecessary repeated capture while preserving clinical accuracy.

Initial rule:

- if patient administrative data was confirmed within the last 6 months, the workflow may allow quick confirmation instead of full recapture;
- if patient administrative data is older than 6 months, the workflow should prompt reconfirmation;
- reconfirmation must update a confirmation timestamp or equivalent audit evidence;
- reconfirmation must not overwrite visit-specific clinical data;
- the recapture interval must be configurable later.

Administrative data can be reconfirmed.

Clinical measurements must be recorded historically.

---

## 5. Visit rules

A PatientVisit represents the patient's attendance to a brigade or care event.

Rules:

- every visit must belong to one OrganizationId;
- every visit must belong to one Patient;
- every visit must belong to one Brigade when brigade workflow is used;
- patient, brigade, community, mobile unit, and services must belong to the same tenant boundary;
- visits must not be moved across organizations;
- visits may have many service encounters;
- visits may have documents, forms, media releases, and future vital signs;
- visit creation must validate patient ownership;
- visit reads and lists must be organization-scoped.

---

## 6. Encounter rules

A ServiceEncounter represents one service delivered during a visit.

Rules:

- every encounter must belong to one OrganizationId;
- every encounter must belong to one Patient;
- every encounter must belong to one PatientVisit;
- every encounter must reference a Service when service workflow is used;
- every encounter must reference a Brigade when brigade workflow is used;
- patient, visit, brigade, and service must belong to the same OrganizationId;
- encounters must not link records across organizations;
- encounter reads, lists, creates, and updates must be organization-scoped;
- forms, referrals, medication deliveries, and vital signs may attach to encounters depending on workflow.

---

## 7. Vital signs rules

Vital signs must be historical records.

Future entity candidate: VitalSignsRecord.

Expected fields:

- Id;
- OrganizationId;
- PatientId;
- VisitId;
- EncounterId optional;
- SystolicBloodPressureMmHg;
- DiastolicBloodPressureMmHg;
- HeartRateBpm;
- RespiratoryRatePerMinute;
- TemperatureCelsius;
- OxygenSaturationPercent;
- WeightKg;
- HeightCm;
- GlucoseMgDl optional;
- MeasuredAt;
- MeasuredByUserId;
- Source;
- Notes;
- CreatedAt;
- UpdatedAt;
- IsDeleted if soft delete is approved for this entity.

Rules:

- systolic and diastolic blood pressure must be separate fields with mmHg units;
- vital signs must not be stored only on Patient;
- vital signs must not overwrite previous vital signs;
- vital signs must belong to a PatientVisit;
- EncounterId can be optional because some brigades may measure signs before assigning a specific service;
- OrganizationId, PatientId, VisitId, and optional EncounterId must be tenant-consistent;
- values must have domain validation before production use; all persisted measurement fields must use canonical units in the field name or an explicit unit field;
- updates must be auditable;
- deletion must be soft delete if deletion is allowed.

---

## 8. Measurement validation rules

Initial validation rules must be conservative and configurable later.

Minimum rules:

| Field | Baseline rule |
|---|---|
| SystolicBloodPressureMmHg | Optional until workflow requires it. Must be numeric and positive when present. Unit: mmHg. |
| DiastolicBloodPressureMmHg | Optional until workflow requires it. Must be numeric and positive when present. Unit: mmHg. |
| HeartRateBpm | Optional. Must be numeric and positive when present. Unit: beats per minute. |
| RespiratoryRatePerMinute | Optional. Must be numeric and positive when present. Unit: breaths per minute. |
| TemperatureCelsius | Optional. Must be numeric and positive when present. Unit: Celsius. |
| OxygenSaturationPercent | Optional. Must be between 0 and 100 when present. Unit: percent. |
| WeightKg | Optional. Must be positive when present. |
| HeightCm | Optional. Must be positive when present. |
| GlucoseMgDl | Optional. Must be positive when present. |

No automatic medical diagnosis should be generated from these values in P3.

---

## 9. Expediente / clinical record rules

The expediente must be a read model, not a duplicated table that rewrites history.

The clinical record should aggregate:

- Patient;
- PatientGuardian;
- PatientVisit;
- ServiceEncounter;
- VitalSignsRecord;
- MedicalReferral;
- MedicationDelivery;
- FormResponse;
- DocumentSignature;
- MediaRelease.

Rules:

- expediente access must be tenant-scoped;
- patient must belong to actor OrganizationId;
- included records must belong to the same OrganizationId or derive from the same patient/visit/encounter boundary;
- clinical history must be chronological;
- clinical history must not hide historical values because newer values exist;
- SuperAdmin access must be explicit and audited;
- expediente response must not expose records from another organization.

---

## 10. Consent and document rules

Consent documents must be treated as evidence.

Rules:

- consent records must belong to one OrganizationId;
- consent must link to patient when applicable;
- consent may link to visit or encounter depending on document type;
- signed document evidence must not be overwritten silently;
- replacement or correction must be auditable;
- signed documents must not be deleted without explicit approval;
- media releases must not be assumed from general consent;
- consent document reads and writes must be tenant-scoped.

---

## 11. Forms rules

Forms are structured clinical or operational data.

Rules:

- form templates belong to one OrganizationId unless a future global template policy is approved;
- form responses belong to one OrganizationId;
- form responses must link to the expected form template;
- clinical form responses should link to encounter when applicable;
- form response payloads must not bypass tenant validation;
- form templates must not be used across organizations unless a future explicit sharing model is approved.

---

## 12. Referrals and medication delivery rules

MedicalReferral and MedicationDelivery are clinical history.

Rules:

- records must belong to one OrganizationId;
- records must belong to one Patient;
- records should link to ServiceEncounter when clinically meaningful;
- records must not cross tenant boundaries;
- records must not be hard-deleted without policy approval;
- reads and writes must be tenant-scoped;
- changes must be auditable when clinically significant.

---

## 13. Offline and sync clinical rules

Offline sync must not weaken clinical integrity.

Rules:

- offline payloads must include OrganizationId;
- offline payloads must be validated against actor OrganizationId before persistence;
- sync must not create patient, visit, encounter, document, form, referral, medication, or vital signs records outside the actor tenant;
- sync conflicts must not leak data across organizations;
- DeviceId remains deferred as a strong FK, but DeviceId must not become a tenant bypass;
- sync events must be auditable;
- retries must be bounded and must not duplicate clinical records.

---

## 14. Audit rules

Audit candidates:

- patient identity update;
- patient visit creation;
- service encounter creation;
- vital signs creation/update/delete;
- document signature creation;
- consent correction;
- medical referral creation/update;
- medication delivery creation/update;
- sync conflict resolution;
- cross-tenant rejection event if later supported.

Rules:

- audit must identify actor when possible;
- audit must include OrganizationId when tenant-related;
- audit must not store unnecessary patient-sensitive payloads;
- audit must capture enough metadata for traceability.

---

## 15. Tenant boundary rules

Clinical business rules must enforce tenant boundary below the controller layer.

Rules:

- controller authorization is not enough;
- application and repository paths must validate OrganizationId;
- related clinical entities must belong to the same OrganizationId;
- list endpoints must filter by OrganizationId;
- read-by-id endpoints must validate OrganizationId ownership;
- mutation endpoints must pass OrganizationId into the application layer;
- future background jobs and sync handlers must enforce the same rules.

---

## 16. Explicitly out of scope for P3-04

P3-04 does not implement:

- new database migrations;
- new VitalSignsRecord table;
- new endpoints;
- diagnosis engine;
- LLM medical assistant;
- blockchain;
- advanced analytics;
- production Azure deployment;
- frontend clinical workflow.

Those belong to later P3/P4 packages after the baseline is approved.

---

## 17. Acceptance criteria

P3-04 is complete when:

- this clinical business rules baseline exists;
- a verifier protects required sections;
- repository governance gate validates it;
- existing database and security gates remain green;
- P3-05 can implement VitalSignsRecord from explicit rules.