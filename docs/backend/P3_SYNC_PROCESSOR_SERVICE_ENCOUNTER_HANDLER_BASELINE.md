# P3 Sync Processor Service Encounter Handler Baseline

Status: active  
Scope: sync processor service encounter create handler, tenant-scoped encounter creation, visit/service linkage, provider validation, and brigade service availability  
Target phase: P3-16  
Depends on: P3 patient sync handler, P3 patient visit sync handler, P3 vital signs sync handler, P3 sync processor skeleton

---

## 1. Purpose

P3-16 enables the fourth real sync processor handler.

The supported real domain write in this package is:

- EntityType: service_encounter
- Operation: create

This package intentionally does not process forms, consents, referrals, medication deliveries, or external pass records.

---

## 2. Service encounter create rules

The service encounter create handler must:

- process only SyncEntityType.ServiceEncounter;
- process only SyncOperation.Create;
- parse PayloadJson as CreateServiceEncounterRequest;
- require JSON object payload;
- require VisitId;
- require ServiceCode;
- create ServiceEncounter with OrganizationId from the sync batch route/context, not payload trust;
- validate VisitId belongs to the same OrganizationId;
- validate VisitId can be found either in persisted PatientVisits or in PatientVisits staged in the same DbContext;
- validate VisitId belongs to the parent SyncBatch.BrigadeId;
- reject closed visits as conflict;
- validate ServiceCode belongs to the same OrganizationId;
- validate service is active;
- validate service is available for the visit brigade through BrigadeServices;
- validate ProviderUserId belongs to the same OrganizationId when provided;
- normalize or generate EncounterFolio;
- conflict duplicate EncounterFolio inside the organization;
- conflict duplicate EncounterFolio values inside the same pending batch before SaveChangesAsync;
- conflict duplicate VisitId plus ServiceId inside the organization;
- conflict duplicate VisitId plus ServiceId values inside the same pending batch before SaveChangesAsync;
- pending-batch encounter folio and visit-service keys must be reserved only after successful ServiceEncounter construction;
- accept the SyncEvent only after the ServiceEncounter entity is staged;
- set SyncEvent.EntityId to the created ServiceEncounter.Id through Accept;
- complete batch counters from stored SyncEvent statuses.

---

## 3. Offline patient-to-visit-to-encounter linkage

P3-16 allows patient create, patient_visit create, service_encounter create, and vital_signs create inside the same sync batch when stable GUID references are used.

Rules:

- patient create may use SyncEvent.EntityId as the Patient.Id;
- patient_visit create may use SyncEvent.EntityId as the PatientVisit.Id;
- service_encounter create may use SyncEvent.EntityId as the ServiceEncounter.Id;
- vital_signs create may reference EncounterId when an encounter already exists or is staged earlier in the same DbContext;
- the processor must process patient create events before patient_visit create events;
- the processor must process patient_visit create events before service_encounter create events;
- the processor must process service_encounter create events before vital_signs create events;
- missing visit or service references must become conflicts, not database failures.

---

## 4. Unsupported service encounter operations

Unsupported service encounter operations must not silently mutate records.

Rules:

- service_encounter update is not implemented in P3-16;
- service_encounter complete/close is not implemented in P3-16;
- service_encounter cancel is not implemented in P3-16;
- unsupported service_encounter operations must be marked conflict;
- future packages must implement update/complete/cancel with explicit conflict and audit policy.

---

## 5. Privacy and safety

Rules:

- processor response must not expose PayloadJson;
- processor must not log raw PayloadJson;
- processor must not create forms, documents, referrals, or medication deliveries in P3-16;
- duplicate encounter folio must not overwrite existing encounter data;
- duplicate visit-service pair must not create duplicate clinical encounters.

---

## 6. Acceptance criteria

P3-16 is complete when:

- SyncBatchProcessor handles service_encounter create events;
- SyncBatchProcessor creates ServiceEncounter records from CreateServiceEncounterRequest;
- SyncBatchProcessor accepts successful service_encounter create SyncEvents;
- SyncBatchProcessor stores created ServiceEncounter.Id on SyncEvent.EntityId;
- SyncBatchProcessor marks missing visit as conflict;
- SyncBatchProcessor marks inactive/missing service as conflict;
- SyncBatchProcessor validates brigade service availability;
- SyncBatchProcessor marks duplicate encounter folio as conflict;
- SyncBatchProcessor marks duplicate visit-service pair as conflict;
- SyncBatchProcessor does not reserve pending-batch duplicate keys before ServiceEncounter construction succeeds;
- SyncBatchProcessor processes service_encounter before vital_signs;
- contract tests protect the service_encounter-only scope;
- repository governance and database deployment gates remain green.
---

## 7. P3-17 form response handler note

P3-17 adds form_response create handling. After P3-17, form_response create events can resolve encounters staged earlier in the same DbContext because service_encounter create is processed before form_response create.
---

## 8. P3-19 medical referral handler note

P3-19 adds medical_referral create handling. Medical referrals are valid after service_encounter create because they are linked to ServiceEncounter through EncounterId and derive PatientId from the encounter.
---

## 9. P3-20 medication delivery handler note

P3-20 adds medication_delivery create handling. Medication deliveries are valid after service_encounter create because they are linked to ServiceEncounter through EncounterId and derive PatientId from the encounter.
---

## 10. P3-21 integration hardening note

P3-21 requires ServiceEncounter pending-batch encounter folio and visit-service key reservations to be atomic with rollback when the second reservation fails.
