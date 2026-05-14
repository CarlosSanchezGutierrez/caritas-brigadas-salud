# P3 Service Encounter Sync Event Handler Extraction Baseline

Status: active  
Scope: service_encounter/create handler extraction from SyncBatchProcessor into an internal sync handler class  
Target phase: P3-22H  
Depends on: P3-22G.1 sync compatibility governance cleanup

---

## 1. Purpose

P3-22H extracts service_encounter/create sync handling from SyncBatchProcessor into ServiceEncounterSyncEventHandler.

This preserves behavior while continuing the clinical sync handler decomposition path:

Patient -> PatientVisit -> ServiceEncounter

---

## 2. ServiceEncounterSyncEventHandler contract

Rules:

- ServiceEncounterSyncEventHandler must be an internal infrastructure sync component;
- ServiceEncounterSyncEventHandler must own service_encounter/create payload parsing;
- ServiceEncounterSyncEventHandler must reject unsupported service_encounter operations;
- ServiceEncounterSyncEventHandler must validate VisitId;
- ServiceEncounterSyncEventHandler must validate ServiceCode;
- ServiceEncounterSyncEventHandler must validate visit existence;
- ServiceEncounterSyncEventHandler must enforce batch brigade matching;
- ServiceEncounterSyncEventHandler must reject closed visits;
- ServiceEncounterSyncEventHandler must validate service existence;
- ServiceEncounterSyncEventHandler must reject inactive services;
- ServiceEncounterSyncEventHandler must validate service availability for the visit brigade through BrigadeServices;
- ServiceEncounterSyncEventHandler must validate optional provider user existence;
- ServiceEncounterSyncEventHandler must own encounter id conflict checks;
- ServiceEncounterSyncEventHandler must own encounter folio generation and duplicate checks;
- ServiceEncounterSyncEventHandler must own visit-service duplicate checks;
- ServiceEncounterSyncEventHandler must reserve encounter folio and visit-service keys only after successful ServiceEncounter construction and atomically;
- ServiceEncounterSyncEventHandler must roll back encounter folio reservation when visit-service key reservation fails;
- ServiceEncounterSyncEventHandler must construct ServiceEncounter;
- ServiceEncounterSyncEventHandler must accept the SyncEvent only after staging ServiceEncounter.

---

## 3. SyncBatchProcessor contract

Rules:

- SyncBatchProcessor may keep a temporary compatibility wrapper named HandleServiceEncounterEventAsync;
- the wrapper must remain async to preserve formatting and compatibility governance contracts;
- the wrapper must delegate to ServiceEncounterSyncEventHandler.HandleAsync;
- SyncBatchProcessor must not directly construct ServiceEncounter;
- SyncBatchProcessor must not directly parse CreateServiceEncounterRequest;
- SyncBatchProcessor must not contain service_encounter validation details;
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
- no weakening of P3-22F patient handler extraction;
- no weakening of P3-22G patient visit handler extraction;
- no weakening of P3-22G.1 compatibility governance.

---

## 5. Acceptance criteria

P3-22H is complete when:

- ServiceEncounterSyncEventHandler exists;
- SyncBatchProcessor constructs ServiceEncounterSyncEventHandler;
- SyncBatchProcessor delegates service_encounter/create handling to ServiceEncounterSyncEventHandler;
- SyncBatchProcessor no longer contains direct ServiceEncounter construction;
- ServiceEncounterSyncEventHandler contains the previous service_encounter/create behavior;
- all previous P3 sync processor verifiers remain green;
- dotnet build and dotnet test remain green.