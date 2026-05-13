# P3 External Referral Pass and Scarce Care Traceability Baseline

Status: active  
Scope: backend clinical referrals, external care passes, scarce procedures, specialty consultations, external medication support, evidence, and follow-up traceability  
Target phase: P3-07.1  
Depends on: P3 clinical business rules, P3 clinical record read model, P3 offline sync policy

---

## 1. Purpose

This document defines how the platform must model and trace external medical support when Caritas sends a patient to another public or private medical center.

This includes:

- surgeries;
- specialty consultations;
- scarce procedures;
- external medication support;
- laboratory studies;
- imaging studies;
- rehabilitation;
- external treatment follow-up;
- urgent referral support;
- paper or digital pass issued by Caritas.

The goal is to prevent losing traceability after a patient leaves the brigade or office workflow.

---

## 2. Current baseline

The backend already has MedicalReferral.

MedicalReferral currently covers:

- OrganizationId;
- EncounterId;
- PatientId;
- ReferralFolio;
- DestinationInstitution;
- ReferralReason;
- Priority;
- ReferredByUserId;
- ProviderSignatureId;
- Status.

This is enough for a basic referral record, but not enough for full external pass traceability.

The backend also has MedicationDelivery.

MedicationDelivery covers medication delivered inside the platform workflow, but it does not fully represent external medication authorization or a pass to another institution.

---

## 3. Domain distinction

The system must distinguish these concepts:

| Concept | Meaning |
|---|---|
| MedicalReferral | Clinical decision that the patient needs external care or follow-up. |
| ExternalReferralPass | Administrative/clinical authorization document issued by Caritas for external care. |
| ExternalCareProvider | Public/private institution, clinic, hospital, laboratory, pharmacy, or partner center. |
| ReferralFollowUp | Tracking events after the pass is issued. |
| ReferralOutcome | Final result of the external care process. |
| ReferralEvidence | Document, signature, scan, note, or proof related to the referral/pass. |

Rule: MedicalReferral is the clinical need. ExternalReferralPass is the issued access/justification document.

---

## 4. Supported external care types

The external care type must be explicit.

Candidate care types:

| Care type | Meaning |
|---|---|
| specialty_consultation | Patient needs a specialist consultation. |
| surgery | Patient needs a surgical procedure or operation. |
| procedure | Patient needs a medical procedure. |
| medication_support | Patient needs medication that Caritas does not directly deliver. |
| laboratory_study | Patient needs laboratory testing. |
| imaging_study | Patient needs imaging such as X-ray, ultrasound, CT, MRI. |
| rehabilitation | Patient needs therapy or rehabilitation. |
| emergency_referral | Patient needs urgent external care. |
| other | Approved fallback requiring notes. |

Rules:

- care type must not be free text only;
- other requires explanation;
- care type must be usable for reporting and analytics;
- care type must be printable on the pass when needed.

---

## 5. External provider / destination rules

External destination data must be structured.

Candidate fields:

- ExternalCareProviderId;
- provider name;
- provider type;
- public/private/partner classification;
- address;
- municipality;
- contact phone;
- contact person;
- service agreement reference;
- active/inactive status;
- notes.

Rules:

- destination institution must not remain only as free text long term;
- free text destination is acceptable only as migration/MVP fallback;
- provider catalog must be tenant-scoped or global with explicit sharing policy;
- inactive providers must not be used for new passes unless explicitly allowed;
- provider data must support reporting by destination.

---

## 6. External referral pass rules

An ExternalReferralPass should represent the actual Caritas pass, access letter, justification, or authorization.

Candidate fields:

- Id;
- OrganizationId;
- MedicalReferralId;
- PatientId;
- EncounterId;
- VisitId optional;
- ExternalCareProviderId optional;
- PassFolio;
- CareType;
- RequestedService;
- ClinicalJustification;
- AdministrativeJustification;
- Priority;
- ValidFrom;
- ValidUntil;
- IssuedAt;
- IssuedByUserId;
- ApprovedByUserId optional;
- DeliveredToPatientAt optional;
- DeliveredByUserId optional;
- PatientAcknowledgedAt optional;
- Status;
- PrintedCount;
- LastPrintedAt;
- DocumentTemplateVersion;
- DocumentSnapshotHash;
- ProviderInstructions;
- PatientInstructions;
- Notes;
- CreatedAt;
- UpdatedAt;
- IsDeleted if approved by policy.

Rules:

- pass must belong to one OrganizationId;
- pass must link to one MedicalReferral;
- pass must link to one Patient;
- pass must link to one Encounter;
- pass folio must be unique inside the organization;
- pass must not expose unnecessary patient data;
- pass must preserve document version/snapshot evidence;
- pass must be auditable;
- pass must be tenant-scoped.

---

## 7. Pass status lifecycle

Candidate statuses:

| Status | Meaning |
|---|---|
| draft | Pass is being prepared. |
| pending_approval | Pass requires approval. |
| approved | Pass is approved but not issued. |
| issued | Pass was issued by Caritas. |
| delivered_to_patient | Patient received the pass. |
| scheduled | External appointment/procedure was scheduled. |
| attended | Patient attended external care. |
| completed | External care process completed. |
| rejected_by_provider | External center rejected the pass or service. |
| cancelled | Pass was cancelled. |
| expired | Pass expired. |
| lost_replaced | Pass was lost and replaced with audit trail. |

Rules:

- completed pass must not be edited without correction workflow;
- cancelled pass must preserve reason;
- expired pass must not be reused;
- replacement must link to original pass;
- status transitions must be auditable.

---

## 8. Follow-up traceability rules

ReferralFollowUp records track what happened after the pass was issued.

Candidate fields:

- Id;
- OrganizationId;
- ExternalReferralPassId;
- MedicalReferralId;
- PatientId;
- FollowUpType;
- FollowUpAt;
- FollowUpByUserId;
- Outcome;
- Notes;
- NextActionAt;
- ExternalAppointmentAt;
- ExternalProviderResponse;
- CreatedAt.

Candidate follow-up types:

- phone_call;
- office_visit;
- patient_reported_update;
- provider_confirmation;
- appointment_scheduled;
- appointment_missed;
- care_completed;
- care_rejected;
- document_received;
- other.

Rules:

- follow-up must be append-only by default;
- follow-up must not overwrite the original referral;
- follow-up must be visible in the patient clinical record;
- follow-up must be tenant-scoped;
- sensitive notes must not leak into analytics datasets.

---

## 9. Evidence and document rules

External referral evidence may include:

- generated pass PDF;
- signed acknowledgement;
- scanned external document;
- external appointment proof;
- provider response;
- completion evidence;
- cancellation evidence;
- replacement evidence.

Rules:

- raw files must not be stored directly in relational tables;
- database should store metadata, hashes, storage keys, and ownership;
- evidence must link to OrganizationId and PatientId;
- evidence must link to MedicalReferral or ExternalReferralPass;
- evidence must be auditable;
- evidence must not be exposed in analytics by default;
- generated documents must record template version and snapshot hash.

---

## 10. Printable pass / format requirements

The printable pass must be generated from structured data.

Expected sections:

- Caritas organization header;
- pass folio;
- issue date;
- validity period;
- patient identification summary;
- patient age/sex when appropriate;
- external care type;
- requested service;
- destination provider;
- clinical justification;
- administrative justification when needed;
- priority;
- referring professional;
- approving user if applicable;
- instructions for patient;
- instructions for provider;
- signature/authorization block;
- QR or verification code in a future phase;
- privacy notice reference.

Rules:

- printed pass must not be the only source of truth;
- generated pass must be reproducible from stored structured data plus template version;
- QR verification must not expose sensitive patient data;
- if the exact legacy format exists, fields must be mapped into this structured model before implementation.

---

## 11. Analytics and reporting

The referral/pass module must support reporting.

Required metrics:

- referrals by care type;
- passes issued by period;
- passes by status;
- passes by destination provider;
- surgeries requested;
- specialty consultations requested;
- medication support requests;
- completed external care;
- rejected external care;
- expired passes;
- pending follow-up;
- average time from referral to issued pass;
- average time from issued pass to completed care;
- patients with unresolved external referrals.

Rules:

- patient identifiers must not be in default analytics datasets;
- small cohorts must be controlled;
- provider-level reports must be tenant-scoped;
- analytics should use status history and follow-up events, not overwritten fields only.

---

## 12. Clinical record integration

The patient clinical record should include external referral/pass information.

Clinical record should eventually show:

- medical referrals;
- external referral passes;
- pass status;
- destination provider;
- follow-up events;
- final outcome;
- evidence metadata;
- medication support records;
- related encounter/visit.

Rules:

- clinical record must not expose raw evidence files by default;
- clinical record must show enough traceability for care continuity;
- medical users may need more detail than office capturers;
- analytics users should receive aggregated/de-identified views.

---

## 13. Security and privacy rules

External referral passes contain sensitive health data.

Rules:

- access must require PermissionCodes-based authorization;
- reads and writes must be tenant-scoped;
- pass generation must be auditable;
- pass printing/download must be auditable;
- raw PDF/evidence access must be permission-gated;
- public verification must not leak patient-sensitive data;
- exports must follow data governance baseline;
- logs must not include clinical justification, patient identifiers, or raw document content.

---

## 14. Offline and sync rules

Offline referral/pass behavior must be conservative.

Rules:

- medical referral draft may be created offline if actor has permission;
- pass issuance should require online validation unless an approved emergency offline policy exists;
- pass folio generation must avoid duplicates;
- follow-up updates can be captured offline and synced later;
- duplicate pass issuance must be prevented through idempotency;
- conflict resolution must not create two active passes for the same referral unless replacement policy allows it.

---

## 15. Future implementation options

Option A: extend MedicalReferral.

Pros:

- smaller change;
- faster MVP.

Cons:

- risks mixing clinical decision, administrative pass, evidence, follow-up, provider response, and lifecycle into one table.

Option B: add ExternalReferralPass, ExternalCareProvider, ReferralFollowUp, ReferralEvidence.

Pros:

- cleaner traceability;
- better analytics;
- better document lifecycle;
- better audit;
- supports multiple passes/follow-ups from one referral.

Cons:

- more tables and implementation work.

Recommended direction:

- keep MedicalReferral as clinical need;
- add ExternalReferralPass for issued access/authorization;
- add ReferralFollowUp for tracking;
- add ExternalCareProvider catalog;
- add ReferralEvidence metadata when file storage policy is ready.

---

## 16. Explicitly out of scope for P3-07.1

P3-07.1 does not implement:

- new tables;
- PDF generation;
- QR verification;
- external provider portal;
- file storage integration;
- appointment scheduling integration;
- public verification endpoint;
- referral pass UI.

Those belong to later P3/P4 packages.

---

## 17. Acceptance criteria

P3-07.1 is complete when:

- this external referral pass and traceability baseline exists;
- a verifier protects required sections;
- repository governance gate validates it;
- database and security gates remain green;
- future implementation can model scarce procedures, specialty consultations, surgeries, external medication support, passes, follow-up, evidence, and outcomes without mixing all concerns into MedicalReferral only.