# P3 Sync Processor Direct Handler Dispatch Baseline

Status: active  
Scope: remove temporary SyncBatchProcessor compatibility wrappers after handler extraction  
Target phase: P3-22O  
Depends on: P3-22N SyncBatchProcessor post-extraction hygiene

---

## 1. Purpose

P3-22O removes temporary wrapper methods from SyncBatchProcessor and leaves direct dispatch to extracted sync event handlers.

This is the final structural cleanup after all primary P3 sync event handlers were extracted.

---

## 2. Direct dispatch rules

Rules:

- SyncBatchProcessor must dispatch patient events directly to PatientSyncEventHandler.HandleAsync;
- SyncBatchProcessor must dispatch patient_visit events directly to PatientVisitSyncEventHandler.HandleAsync;
- SyncBatchProcessor must dispatch service_encounter events directly to ServiceEncounterSyncEventHandler.HandleAsync;
- SyncBatchProcessor must dispatch vital_signs events directly to VitalSignsSyncEventHandler.HandleAsync;
- SyncBatchProcessor must dispatch form_response events directly to FormResponseSyncEventHandler.HandleAsync;
- SyncBatchProcessor must dispatch consent_document events directly to ConsentDocumentSyncEventHandler.HandleAsync;
- SyncBatchProcessor must dispatch medical_referral events directly to MedicalReferralSyncEventHandler.HandleAsync;
- SyncBatchProcessor must dispatch medication_delivery events directly to MedicationDeliverySyncEventHandler.HandleAsync;
- SyncBatchProcessor must not contain temporary Handle*EventAsync wrappers;
- SyncBatchProcessor must not contain GetSyncProcessingOrder;
- SyncBatchProcessor must still sort pending events using SyncProcessingOrder.GetOrder;
- SyncBatchProcessor must still create PendingBatchReservationState once per ProcessAsync call;
- SyncBatchProcessor must still call ProcessPendingEventAsync for each pending event.

---

## 3. Non-goals

P3-22O does not change handler behavior.

P3-22O does not add new sync entity types.

P3-22O does not add end-to-end clinical sync tests. That belongs to P3-22P.

---

## 4. Non-negotiable constraints

Rules:

- no endpoint change;
- no database migration;
- no behavior change;
- no payload contract change;
- no handler behavior change;
- no weakening of extracted handler governance;
- no weakening of payload governance;
- no weakening of zero technical debt governance.

---

## 5. Acceptance criteria

P3-22O is complete when:

- SyncBatchProcessor has no temporary Handle*EventAsync wrappers;
- SyncBatchProcessor has no GetSyncProcessingOrder wrapper;
- SyncBatchProcessor dispatches directly to all eight extracted handlers;
- all compatibility gates are aligned to direct handler dispatch;
- all handler extraction gates remain green;
- dotnet build and dotnet test remain green.