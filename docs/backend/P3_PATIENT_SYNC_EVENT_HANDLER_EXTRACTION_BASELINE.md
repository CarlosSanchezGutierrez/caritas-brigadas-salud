# P3 Patient Sync Event Handler Extraction Baseline

Status: active  
Scope: first domain handler extraction from SyncBatchProcessor into an internal sync handler class  
Target phase: P3-22F  
Depends on: P3-22E pending event dispatch extraction

---

## 1. Purpose

P3-22F extracts the patient/create sync handler from SyncBatchProcessor into PatientSyncEventHandler.

This is the first real domain handler extraction. It must not change behavior.

---

## 2. PatientSyncEventHandler contract

Rules:

- PatientSyncEventHandler must be an internal infrastructure sync component;
- PatientSyncEventHandler must own patient/create payload parsing;
- PatientSyncEventHandler must own patient id conflict checks;
- PatientSyncEventHandler must own patient folio generation and duplicate checks;
- PatientSyncEventHandler must own Patient construction;
- PatientSyncEventHandler must preserve sensitive identifier handling;
- PatientSyncEventHandler must preserve location handling;
- PatientSyncEventHandler must preserve migrant handling;
- PatientSyncEventHandler must preserve partial-record validation;
- PatientSyncEventHandler must preserve admin notes handling;
- PatientSyncEventHandler must accept the SyncEvent only after staging Patient.

---

## 3. SyncBatchProcessor contract

Rules:

- SyncBatchProcessor may keep a temporary compatibility wrapper named HandlePatientEventAsync;
- the wrapper must delegate to PatientSyncEventHandler.HandleAsync;
- the temporary compatibility wrapper may remain async to preserve formatting and compatibility governance contracts;
- SyncBatchProcessor must not directly construct Patient;
- SyncBatchProcessor must not directly parse CreatePatientRequest;
- SyncBatchProcessor must not contain patient mutation details;
- ProcessPendingEventAsync behavior must remain unchanged.

---

## 4. Non-negotiable constraints

Rules:

- no database migration;
- no endpoint contract change;
- no sync entity type expansion;
- no behavior change;
- no weakening of P3-21 integration hardening;
- no weakening of P3-22A zero technical debt gate;
- no weakening of P3-22B component extraction;
- no weakening of P3-22C payload reader extraction;
- no weakening of P3-22D formatting hygiene;
- no weakening of P3-22E pending event dispatch extraction.

---

## 5. Acceptance criteria

P3-22F is complete when:

- PatientSyncEventHandler exists;
- SyncBatchProcessor constructs PatientSyncEventHandler;
- SyncBatchProcessor delegates patient/create handling to PatientSyncEventHandler;
- SyncBatchProcessor no longer contains direct Patient construction;
- PatientSyncEventHandler contains the previous patient/create behavior;
- all previous P3 sync processor verifiers remain green;
- dotnet build and dotnet test remain green.
---

## 6. P3-22G patient visit sync event handler extraction note

P3-22G extracts patient_visit/create behavior into PatientVisitSyncEventHandler while preserving ProcessPendingEventAsync dispatch behavior.
