# P3 Sync Processor Vital Signs Handler Baseline

Status: active  
Scope: sync processor vital signs create handler, tenant-scoped clinical measurements, patient/visit linkage, optional encounter linkage, and canonical clinical units  
Target phase: P3-15  
Depends on: P3 patient sync handler, P3 patient visit sync handler, P3 sync processor skeleton, P3 clinical business rules baseline

---

## 1. Purpose

P3-15 enables the third real sync processor handler.

The supported real domain write in this package is:

- EntityType: vital_signs
- Operation: create

This package intentionally does not process service encounters, forms, consents, referrals, medication deliveries, or external pass records.

---

## 2. Vital signs create rules

The vital signs create handler must:

- process only SyncEntityType.VitalSigns;
- process only SyncOperation.Create;
- parse PayloadJson as CreateVitalSignsRecordRequest;
- require JSON object payload;
- require PatientId;
- require VisitId;
- create VitalSignsRecord with OrganizationId from the sync batch route/context, not payload trust;
- validate PatientId belongs to the same OrganizationId;
- validate PatientId can be found either in persisted Patients or in Patients staged in the same DbContext;
- validate VisitId belongs to the same OrganizationId, PatientId, and parent SyncBatch.BrigadeId;
- validate VisitId can be found either in persisted PatientVisits or in PatientVisits staged in the same DbContext;
- validate EncounterId belongs to the same OrganizationId, PatientId, and VisitId when provided;
- validate MeasuredByUserId belongs to the same OrganizationId when provided;
- use canonical TemperatureCelsius;
- use canonical pressure units SystolicBloodPressureMmHg and DiastolicBloodPressureMmHg;
- rely on VitalSignsRecord domain rules for positive values and at least one measurement;
- accept the SyncEvent only after the VitalSignsRecord entity is staged;
- set SyncEvent.EntityId to the created VitalSignsRecord.Id through Accept;
- complete batch counters from stored SyncEvent statuses.

---

## 3. Offline patient-to-visit-to-vitals linkage

P3-15 allows patient create, patient_visit create, and vital_signs create inside the same sync batch when stable GUID references are used.

Rules:

- patient create may use SyncEvent.EntityId as the Patient.Id;
- patient_visit create may use SyncEvent.EntityId as the PatientVisit.Id;
- vital_signs create may reference that PatientId and VisitId;
- the processor must process patient create events before patient_visit create events;
- the processor must process patient_visit create events before vital_signs create events;
- the processor must check tracked Patients and PatientVisits in the current DbContext before checking only the database;
- missing patient or visit references must become conflicts, not database failures.

---

## 4. Unsupported vital signs operations

Unsupported vital signs operations must not silently mutate records.

Rules:

- vital_signs update is not implemented in P3-15;
- vital_signs void/cancel is not implemented in P3-15;
- unsupported vital_signs operations must be marked conflict;
- future packages must implement update/void with explicit conflict and audit policy.

---

## 5. Privacy and safety

Rules:

- processor response must not expose PayloadJson;
- processor must not log raw PayloadJson;
- processor must not create service encounters, forms, documents, referrals, or medication deliveries in P3-15;
- vital signs must remain historical records, not overwritten fields on Patient;
- duplicate VitalSignsRecord id must not overwrite existing clinical data.

---

## 6. Acceptance criteria

P3-15 is complete when:

- CreateVitalSignsRecordRequest exists;
- SyncBatchProcessor handles vital_signs create events;
- SyncBatchProcessor creates VitalSignsRecord records from CreateVitalSignsRecordRequest;
- SyncBatchProcessor accepts successful vital_signs create SyncEvents;
- SyncBatchProcessor stores created VitalSignsRecord.Id on SyncEvent.EntityId;
- SyncBatchProcessor marks missing patient as conflict;
- SyncBatchProcessor marks missing visit as conflict;
- SyncBatchProcessor marks invalid encounter as conflict;
- SyncBatchProcessor validates measured-by user when provided;
- SyncBatchProcessor still marks unsupported entity types as conflict;
- contract tests protect the vital_signs-only scope;
- repository governance and database deployment gates remain green.