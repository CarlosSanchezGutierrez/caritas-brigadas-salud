# P3 Sync Processor Patient Visit Handler Baseline

Status: active  
Scope: sync processor patient visit create handler, tenant-scoped visit creation, patient-to-visit linkage, brigade validation, and visit idempotency  
Target phase: P3-14  
Depends on: P3 patient sync handler, P3 sync processor skeleton, P3 sync payload governance, P3 sync idempotency guardrails

---

## 1. Purpose

P3-14 enables the second real sync processor handler.

The supported real domain write in this package is:

- EntityType: patient_visit
- Operation: create

This package intentionally does not process service encounters, vital signs, forms, consents, referrals, or medication deliveries.

---

## 2. Patient visit create rules

The patient visit create handler must:

- process only SyncEntityType.PatientVisit;
- process only SyncOperation.Create;
- parse PayloadJson as CreatePatientVisitRequest;
- require JSON object payload;
- require PatientId;
- require BrigadeId;
- create PatientVisit with OrganizationId from the sync batch route/context, not payload trust;
- validate PatientId belongs to the same OrganizationId;
- validate PatientId can be found either in persisted Patients or in Patients staged in the same DbContext;
- validate BrigadeId belongs to the same OrganizationId;
- validate BrigadeId matches the parent SyncBatch.BrigadeId;
- validate RegisteredByUserId belongs to the same OrganizationId when provided;
- normalize or generate VisitFolio;
- conflict duplicate VisitFolio inside the organization;
- conflict duplicate VisitFolio values inside the same pending batch before SaveChangesAsync;
- accept the SyncEvent only after the PatientVisit entity is staged;
- set SyncEvent.EntityId to the created PatientVisit.Id through Accept;
- complete batch counters from stored SyncEvent statuses.

---

## 3. Offline patient-to-visit linkage

P3-14 allows patient create and patient visit create inside the same sync batch when stable GUID references are used.

Rules:

- patient create may use SyncEvent.EntityId as the Patient.Id;
- patient_visit create may reference that PatientId;
- the processor must process patient create events before patient_visit create events;
- the processor must check tracked Patients in the current DbContext before checking only the database;
- missing patient reference must become conflict, not database failure.

---

## 4. Unsupported visit operations

Unsupported visit operations must not silently mutate records.

Rules:

- patient_visit update is not implemented in P3-14;
- patient_visit void/cancel is not implemented in P3-14;
- unsupported patient_visit operations must be marked conflict;
- future packages must implement update/void with explicit conflict and audit policy.

---

## 5. Privacy and safety

Rules:

- processor response must not expose PayloadJson;
- processor must not log raw PayloadJson;
- processor must not create service encounters, vital signs, forms, documents, referrals, or medication deliveries in P3-14;
- duplicate visit folio must not overwrite existing visit data.

---

## 6. Acceptance criteria

P3-14 is complete when:

- SyncBatchProcessor handles patient_visit create events;
- SyncBatchProcessor processes patient create events before patient_visit create events even when uploaded out of order;
- SyncBatchProcessor creates PatientVisit records from CreatePatientVisitRequest;
- SyncBatchProcessor accepts successful patient_visit create SyncEvents;
- SyncBatchProcessor stores created PatientVisit.Id on SyncEvent.EntityId;
- SyncBatchProcessor marks duplicate visit folio as conflict;
- SyncBatchProcessor detects duplicate visit folios inside the same pending batch before database save;
- SyncBatchProcessor marks missing patient as conflict;
- SyncBatchProcessor marks invalid brigade as conflict;
- SyncBatchProcessor still marks unsupported entity types as conflict;
- contract tests protect the patient_visit-only scope;
- repository governance and database deployment gates remain green.
---

## 7. P3-15 vital signs handler note

P3-15 adds vital_signs create handling. This is valid after P3-14 because vital signs are linked to PatientVisit through VisitId. Service encounters, forms, documents, referrals, and medication deliveries remain out of scope.
---

## 8. P3-16 service encounter handler note

P3-16 adds service_encounter create handling. This is valid after P3-14 because service encounters are linked to PatientVisit through VisitId. Forms, documents, referrals, and medication deliveries remain out of scope.
---

## 9. P3-17 form response handler note

P3-17 adds form_response create handling. This is valid after service_encounter create because form responses are linked to ServiceEncounter through EncounterId. Consent documents, referrals, and medication deliveries remain out of scope.
---

## 10. P3-18 consent document handler note

P3-18 adds consent_document create handling. Consent documents may reference VisitId when available and remain linked to PatientId as the legal subject.
