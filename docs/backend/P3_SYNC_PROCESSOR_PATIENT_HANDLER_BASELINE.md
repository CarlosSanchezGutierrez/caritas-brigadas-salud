# P3 Sync Processor Patient Handler Baseline

Status: active  
Scope: sync processor patient create handler, tenant-scoped patient creation, idempotent event acceptance, and unsupported patient operation conflicts  
Target phase: P3-13  
Depends on: P3 sync processor skeleton, P3 sync payload governance, P3 sync idempotency guardrails

---

## 1. Purpose

P3-13 enables the first real sync processor handler.

The only supported real domain write in this package is:

- EntityType: patient
- Operation: create

All other entity types remain conflict-staged until their specific handlers are implemented.

---

## 2. Patient create rules

The patient create handler must:

- process only SyncEntityType.Patient;
- process only SyncOperation.Create;
- parse PayloadJson as CreatePatientRequest;
- require JSON object payload;
- create Patient with OrganizationId from the sync batch route/context, not from payload;
- reuse domain Patient validation;
- normalize or generate PatientFolio;
- reject invalid patient payloads;
- conflict duplicate PatientFolio inside the organization;
- conflict duplicate PatientFolio values inside the same pending batch before SaveChangesAsync;
- accept the SyncEvent only after the Patient entity is staged;
- set SyncEvent.EntityId to the created Patient.Id through Accept;
- complete batch counters from stored SyncEvent statuses.

---

## 3. Unsupported patient operations

Unsupported patient operations must not silently mutate records.

Rules:

- patient update is not implemented in P3-13;
- patient void is not implemented in P3-13;
- unsupported patient operations must be marked conflict;
- future packages must implement update/void with explicit conflict and audit policy.

---

## 4. Privacy and safety

Rules:

- processor response must not expose PayloadJson;
- processor must not log raw PayloadJson;
- processor must not create visits, encounters, vital signs, forms, documents, referrals, or medication deliveries in P3-13;
- patient payload validation must rely on domain rules;
- duplicate folio must not overwrite existing patient data.

---

## 5. Acceptance criteria

P3-13 is complete when:

- SyncBatchProcessor handles patient create events;
- SyncBatchProcessor creates Patient records from CreatePatientRequest;
- SyncBatchProcessor accepts successful patient create SyncEvents;
- SyncBatchProcessor stores created Patient.Id on SyncEvent.EntityId;
- SyncBatchProcessor marks duplicate folio as conflict;
- SyncBatchProcessor detects duplicate patient folios inside the same pending batch before database save;
- SyncBatchProcessor rejects invalid patient payloads;
- SyncBatchProcessor still marks non-patient entity types as conflict;
- contract tests protect the patient-only scope;
- repository governance and database deployment gates remain green.
---

## 6. P3-14 patient visit handler note

P3-14 adds patient_visit create handling. After P3-14, PatientVisit creation is no longer forbidden in the sync processor. Service encounters, vital signs, forms, documents, referrals, and medication deliveries remain out of scope.
---

## 7. P3-15 vital signs handler note

P3-15 adds vital_signs create handling. After P3-15, VitalSignsRecord creation is no longer forbidden in the sync processor. Service encounters, forms, documents, referrals, and medication deliveries remain out of scope.
