# P3 Patient Visit Sync Event Handler Extraction Baseline

Status: active  
Scope: patient_visit/create handler extraction from SyncBatchProcessor into an internal sync handler class  
Target phase: P3-22G  
Depends on: P3-22F patient sync event handler extraction

---

## 1. Purpose

P3-22G extracts patient_visit/create sync handling from SyncBatchProcessor into PatientVisitSyncEventHandler.

This package preserves behavior and keeps SyncBatchProcessor as an orchestrator with a temporary compatibility wrapper.

---

## 2. PatientVisitSyncEventHandler contract

Rules:

- PatientVisitSyncEventHandler must be an internal infrastructure sync component;
- PatientVisitSyncEventHandler must own patient_visit/create payload parsing;
- PatientVisitSyncEventHandler must reject unsupported patient_visit operations;
- PatientVisitSyncEventHandler must validate PatientId;
- PatientVisitSyncEventHandler must validate BrigadeId;
- PatientVisitSyncEventHandler must enforce batch brigade matching;
- PatientVisitSyncEventHandler must validate patient existence;
- PatientVisitSyncEventHandler must validate brigade existence;
- PatientVisitSyncEventHandler must validate optional registered-by user existence;
- PatientVisitSyncEventHandler must own visit id conflict checks;
- PatientVisitSyncEventHandler must own visit folio generation and duplicate checks;
- PatientVisitSyncEventHandler must construct PatientVisit;
- PatientVisitSyncEventHandler must accept the SyncEvent only after staging PatientVisit.

---

## 3. SyncBatchProcessor contract

Rules:

- SyncBatchProcessor may keep a temporary compatibility wrapper named HandlePatientVisitEventAsync;
- the wrapper must remain async to preserve formatting and legacy governance contracts;
- the wrapper must delegate to PatientVisitSyncEventHandler.HandleAsync;
- SyncBatchProcessor must not directly construct PatientVisit;
- SyncBatchProcessor must not directly parse CreatePatientVisitRequest;
- SyncBatchProcessor must not contain patient_visit validation details;
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
- no weakening of P3-22E pending event dispatch extraction;
- no weakening of P3-22F patient handler extraction.

---

## 5. Acceptance criteria

P3-22G is complete when:

- PatientVisitSyncEventHandler exists;
- SyncBatchProcessor constructs PatientVisitSyncEventHandler;
- SyncBatchProcessor delegates patient_visit/create handling to PatientVisitSyncEventHandler;
- SyncBatchProcessor no longer contains direct PatientVisit construction;
- PatientVisitSyncEventHandler contains the previous patient_visit/create behavior;
- all previous P3 sync processor verifiers remain green;
- dotnet build and dotnet test remain green.