# P3 Clinical Data Governance, Privacy, and Analytics Baseline

Status: active  
Scope: backend data governance, privacy, analytics, engineering, data science, web capture workflows, and clinical data protection  
Target phase: P3-04.1  
Depends on: P3 Clinical Business Rules Baseline

---

## 1. Purpose

This document defines how clinical, operational, and analytics data must be handled before adding more clinical entities such as VitalSignsRecord and patient clinical record read models.

The system must work for:

- web data capturers in offices;
- medical brigade users;
- administrators;
- developers;
- data analysts;
- data engineers;
- data scientists;
- future reporting and research workflows.

The goal is to avoid technical debt, unsafe data exports, direct production data usage in development, unclear anonymization, and analytics pipelines that expose identifiable patient data.

---

## 2. Non-negotiable principles

The backend must follow these principles:

- collect only the data needed for the approved clinical and operational purpose;
- protect patient identity and clinical data as sensitive data;
- separate identified operational workflows from analytics workflows;
- never use raw production patient data in local development;
- never expose patient-sensitive data in logs, telemetry, metrics, screenshots, or demo datasets;
- use tenant scope in all identified operational data access;
- use de-identified, pseudonymized, anonymized, aggregated, or synthetic data for analytics depending on use case;
- document every export;
- audit access to sensitive data;
- deny by default when data purpose is unclear.

---

## 3. Data classification

Every field and dataset must be classified.

| Classification | Meaning | Examples |
|---|---|---|
| Direct identifier | Identifies a person directly. | Name, CURP, phone, email, signature, document image. |
| Indirect identifier | Can help identify a person when combined. | Age, birth date, community, brigade date, rare condition. |
| Sensitive clinical data | Health-related information. | Diagnosis notes, vital signs, medication, referrals, form responses. |
| Operational data | Needed to run brigades. | Sync status, visit status, brigade assignment, service availability. |
| Audit data | Traceability and accountability metadata. | Actor, action, entity type, timestamp, organization. |
| Analytics-ready data | Data prepared for analysis with identity controls. | Aggregated counts, de-identified cohorts, pseudonymized patient keys. |
| Synthetic data | Artificial data generated for testing or demos. | Fake patients, fake visits, fake vital signs. |

Rules:

- direct identifiers must not be used in analytics unless explicitly approved;
- indirect identifiers must be evaluated for re-identification risk;
- clinical data must not be exported without a documented purpose;
- audit data must not contain unnecessary patient-sensitive payloads;
- synthetic data is preferred for development, demos, and automated tests.

---

## 4. Identified operational data

Identified operational data is allowed only for workflows that need it.

Examples:

- patient registration;
- patient lookup;
- visit creation;
- encounter creation;
- consent document capture;
- clinical consultation;
- medication delivery;
- referral follow-up;
- audit investigation.

Rules:

- access must be role-based and permission-based;
- access must be tenant-scoped;
- access must be logged for sensitive operations;
- users must see only the minimum data needed for their task;
- capturers should not receive unnecessary medical history unless the workflow requires it;
- programmers must not use identified production data locally.

---

## 5. Web capturer workflow rules

The web version for office capturers must optimize speed, accuracy, and privacy.

Rules:

- show only the fields needed for the current capture task;
- support patient search inside the actor organization only;
- support quick confirmation when patient administrative data is recent;
- require reconfirmation when configured recapture interval is exceeded;
- avoid repeated capture of stable patient data;
- store clinical measurements as historical records, not as overwritten patient fields;
- validate obvious data-entry mistakes before saving;
- avoid displaying full clinical history unless the capturer role requires it;
- record who captured or updated sensitive data when possible.

Recommended UX behavior:

- clear required fields;
- explicit "unknown" or "not available" options when the patient lacks documents;
- duplicate-patient warning inside the same organization;
- autosave only when privacy and audit behavior are defined;
- no accidental cross-tenant search.

---

## 6. Developer data rules

Developers must not depend on raw production data.

Rules:

- local development must use synthetic or masked seed data;
- production database dumps must not be copied to local machines;
- screenshots must not include real patient data;
- logs must not contain names, phone numbers, CURP, signatures, document payloads, or clinical free text;
- test fixtures must use fake patients and fake clinical values;
- debugging production issues must use minimal scoped evidence;
- any exceptional access to production data must be approved and logged.

Allowed for developers by default:

- schema;
- migrations;
- anonymized sample datasets;
- synthetic seeds;
- aggregate metrics without patient identity.

Not allowed by default:

- raw patient tables;
- raw document signatures;
- raw form response payloads with patient identity;
- production backups;
- production sync payloads.

---

## 7. Analytics, data engineering, and data science rules

Analytics must use purpose-specific datasets.

Dataset tiers:

| Tier | Description | Default users |
|---|---|---|
| Identified operational | Full patient identity and clinical data. | Authorized operational/clinical users only. |
| Pseudonymized analytics | Direct identifiers removed and replaced with controlled keys. | Approved analysts/engineers. |
| De-identified analytics | Direct identifiers removed and indirect identifiers reduced. | Approved analytics workflows. |
| Aggregated analytics | Counts and summaries only. | Dashboards and reporting. |
| Synthetic | Artificial data. | Developers, demos, tests, training. |

Rules:

- analytics should prefer aggregated or de-identified data;
- pseudonymized data is not automatically anonymous;
- patient re-identification keys must be separately protected;
- small group reporting must be controlled to reduce re-identification risk;
- exports must include purpose, owner, scope, date, and retention expectation;
- data science experiments must not write back to operational clinical records without approved workflow;
- model training datasets must be reviewed before use.

---

## 8. Anonymization, pseudonymization, and de-identification

Definitions for this project:

| Term | Project meaning |
|---|---|
| Pseudonymization | Replaces direct identifiers with stable or random keys while a re-identification path may still exist. |
| De-identification | Removes direct identifiers and reduces indirect identifiers for a specific analytics purpose. |
| Anonymization / disassociation | Data can no longer be associated with a person by structure, content, or degree of detail. |
| Aggregation | Summarizes data into groups, counts, rates, or trends. |
| Synthetic data | Generated fake data that does not come from real patients. |

Rules:

- hashing PatientId alone is not anonymization;
- pseudonymized data must still be treated as protected;
- direct identifiers must be removed from analytics extracts unless approved;
- free-text clinical notes must be treated as high re-identification risk;
- location, dates, rare conditions, and small cohorts must be generalized or aggregated when needed;
- re-identification keys must be stored separately with stricter access;
- anonymized datasets must document the method used.

---

## 9. Minimum analytics export controls

Every analytics export must document:

- purpose;
- requester;
- approver;
- data owner;
- organization scope;
- fields included;
- fields excluded;
- de-identification method;
- date generated;
- retention expectation;
- storage location;
- access list;
- whether re-identification is possible.

Export rules:

- no export without purpose;
- no export without owner;
- no direct identifiers unless explicitly approved;
- no unrestricted raw database export for analytics;
- no patient-sensitive data in email attachments unless policy explicitly allows secure transfer;
- no public sharing of patient-level datasets.

---

## 10. Recommended data views

Future analytics should use controlled read models or views.

Candidate views:

- OperationalBrigadeSummary
- PatientVisitAnalyticsView
- ServiceEncounterAnalyticsView
- VitalSignsAnalyticsView
- ConsentCompletionAnalyticsView
- SyncPerformanceAnalyticsView
- DataQualityIssueView

Rules:

- operational views may be identified and tenant-scoped;
- analytics views should avoid direct identifiers by default;
- data science views should use pseudonymized or de-identified patient keys;
- dashboards should prefer aggregation;
- raw tables should not be the default analytics interface.

---

## 11. Data quality rules

The backend must support data quality without corrupting history.

Rules:

- validation should prevent impossible values;
- warnings should identify suspicious but possible values;
- correction must preserve audit trail;
- clinical history must not be overwritten silently;
- duplicate patient detection must be tenant-scoped;
- missing patient documents must be supported when socially necessary;
- unknown values should be explicit, not fake defaults.

Examples:

- unknown birth date must not become 1900-01-01 unless policy explicitly defines that sentinel value;
- missing phone must remain null or explicit not available;
- temperature must use TemperatureCelsius;
- blood pressure must use SystolicBloodPressureMmHg and DiastolicBloodPressureMmHg;
- heart rate must use HeartRateBpm.

---

## 12. Retention and deletion baseline

Retention must be policy-driven.

Rules:

- clinical records must not be hard-deleted without policy approval;
- consent evidence must not be silently deleted;
- audit records must remain available for accountability;
- soft delete may be used when legal/operational policy allows;
- archival, blocking, cancellation, and deletion require legal and operational review;
- analytics datasets must have a retention expectation.

P3 does not finalize retention periods. It creates the baseline for later legal review.

---

## 13. Security and access baseline

Sensitive data access must use:

- authentication;
- permission-based authorization;
- tenant scope;
- audit for sensitive operations;
- least privilege;
- deny by default;
- minimal response payloads;
- safe logs;
- controlled exports;
- protected secrets;
- separate runtime and migration users.

Production-grade environments should avoid public database exposure and should use private connectivity where possible.

---

## 14. Offline and sync privacy rules

Offline sync must not weaken privacy.

Rules:

- local/offline storage must be minimized;
- payloads must be scoped to the actor organization;
- sync retries must not duplicate sensitive records;
- conflicts must not expose records from another organization;
- sync payloads must not store unnecessary direct identifiers;
- DeviceId must not become a tenant bypass;
- future mobile storage must define encryption-at-rest expectations.

---

## 15. AI and advanced analytics baseline

AI or advanced analytics must not be added casually.

Rules:

- no automated medical diagnosis in P3;
- no LLM processing of patient-identifiable data without explicit approved architecture;
- no training on sensitive clinical data without documented governance;
- AI request logs must not store patient-sensitive prompts or outputs unless explicitly approved;
- any future LLM gateway must include redaction, purpose limitation, audit, and access controls.

---

## 16. Evidence and audit requirements

Sensitive data workflows should produce evidence.

Evidence candidates:

- consent acceptance;
- data export approval;
- de-identification method;
- production data access approval;
- sync conflict resolution;
- patient identity correction;
- clinical record correction;
- anonymized dataset generation;
- analytics dataset approval.

---

## 17. Out of scope for P3-04.1

This baseline does not implement:

- anonymization engine;
- analytics warehouse;
- Power BI dashboards;
- machine learning pipeline;
- LLM API gateway;
- legal retention schedule;
- production Azure deployment;
- full patient portal;
- mobile offline encryption implementation.

Those require later packages.

---

## 18. Acceptance criteria

P3-04.1 is complete when:

- this data governance, privacy, and analytics baseline exists;
- a verifier protects required sections;
- repository governance gate validates it;
- database and security gates remain green;
- P3-05 can implement VitalSignsRecord with explicit privacy and analytics rules.