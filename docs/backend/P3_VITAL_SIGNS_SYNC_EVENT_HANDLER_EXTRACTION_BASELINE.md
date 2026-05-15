# P3 Vital Signs Sync Event Handler Extraction Baseline

Status: active  
Scope: vital_signs/create handler extraction from SyncBatchProcessor into an internal sync handler class  
Target phase: P3-22I  
Depends on: P3-22H service encounter sync event handler extraction

---

## 1. Purpose

P3-22I extracts vital_signs/create sync handling from SyncBatchProcessor into VitalSignsSyncEventHandler.

This preserves behavior while continuing the clinical sync handler decomposition path:

Patient -> PatientVisit -> ServiceEncounter -> VitalSigns

---

## 2. VitalSignsSyncEventHandler contract

Rules:

- VitalSignsSyncEventHandler must be an internal infrastructure sync component;
- VitalSignsSyncEventHandler must own vital_signs/create payload parsing;
- VitalSignsSyncEventHandler must reject unsupported vital_signs operations;
- VitalSignsSyncEventHandler must validate PatientId;
- VitalSignsSyncEventHandler must validate VisitId;
- VitalSignsSyncEventHandler must validate patient existence;
- VitalSignsSyncEventHandler must validate visit existence within the batch brigade;
- VitalSignsSyncEventHandler must validate optional encounter existence;
- VitalSignsSyncEventHandler must validate optional measured-by user existence;
- VitalSignsSyncEventHandler must own vital signs id conflict checks;
- VitalSignsSyncEventHandler must construct VitalSignsRecord;
- VitalSignsSyncEventHandler must accept the SyncEvent only after staging VitalSignsRecord.

---

## 3. SyncBatchProcessor contract

Rules:

- SyncBatchProcessor may keep a temporary compatibility wrapper named HandleVitalSignsEventAsync;
- the wrapper must remain async to preserve formatting and compatibility governance contracts;
- the wrapper must delegate to VitalSignsSyncEventHandler.HandleAsync;
- SyncBatchProcessor must not directly construct VitalSignsRecord;
- SyncBatchProcessor must not directly parse CreateVitalSignsRecordRequest;
- SyncBatchProcessor must not contain vital_signs validation details;
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
- no weakening of P3-22H service encounter handler extraction.

---

## 5. Acceptance criteria

P3-22I is complete when:

- VitalSignsSyncEventHandler exists;
- SyncBatchProcessor constructs VitalSignsSyncEventHandler;
- SyncBatchProcessor delegates vital_signs/create handling to VitalSignsSyncEventHandler;
- SyncBatchProcessor no longer contains direct VitalSignsRecord construction;
- VitalSignsSyncEventHandler contains the previous vital_signs/create behavior;
- all previous P3 sync processor verifiers remain green;
- dotnet build and dotnet test remain green.
---

## 6. P3-22J form response sync event handler extraction note

P3-22J extracts form_response/create behavior into FormResponseSyncEventHandler while preserving ProcessPendingEventAsync dispatch behavior.
