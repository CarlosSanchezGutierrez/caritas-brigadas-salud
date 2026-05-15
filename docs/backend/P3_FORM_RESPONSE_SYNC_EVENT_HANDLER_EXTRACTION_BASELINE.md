# P3 Form Response Sync Event Handler Extraction Baseline

Status: active  
Scope: form_response/create handler extraction from SyncBatchProcessor into an internal sync handler class  
Target phase: P3-22J  
Depends on: P3-22I vital signs sync event handler extraction

---

## 1. Purpose

P3-22J extracts form_response/create sync handling from SyncBatchProcessor into FormResponseSyncEventHandler.

This preserves behavior while continuing the clinical sync handler decomposition path:

Patient -> PatientVisit -> ServiceEncounter -> VitalSigns -> FormResponse

---

## 2. FormResponseSyncEventHandler contract

Rules:

- FormResponseSyncEventHandler must be an internal infrastructure sync component;
- FormResponseSyncEventHandler must own form_response/create payload parsing;
- FormResponseSyncEventHandler must reject unsupported form_response operations;
- FormResponseSyncEventHandler must validate EncounterId;
- FormResponseSyncEventHandler must validate FormTemplateId;
- FormResponseSyncEventHandler must validate ResponseJson presence;
- FormResponseSyncEventHandler must validate ResponseJson parses as non-empty JSON;
- FormResponseSyncEventHandler must validate encounter existence;
- FormResponseSyncEventHandler must enforce batch brigade matching;
- FormResponseSyncEventHandler must validate form template existence;
- FormResponseSyncEventHandler must reject inactive templates;
- FormResponseSyncEventHandler must enforce EffectiveFrom and EffectiveTo boundaries;
- FormResponseSyncEventHandler must validate optional submitted-by user existence;
- FormResponseSyncEventHandler must own form response id conflict checks;
- FormResponseSyncEventHandler must own duplicate EncounterId plus FormTemplateId checks;
- FormResponseSyncEventHandler must reserve form response id and encounter-template keys only after successful FormResponse construction and atomically;
- FormResponseSyncEventHandler must roll back form response id reservation when encounter-template key reservation fails;
- FormResponseSyncEventHandler must construct FormResponse;
- FormResponseSyncEventHandler must accept the SyncEvent only after staging FormResponse.

---

## 3. SyncBatchProcessor contract

Rules:

- SyncBatchProcessor may keep a temporary compatibility wrapper named HandleFormResponseEventAsync;
- the wrapper must remain async to preserve formatting and compatibility governance contracts;
- the wrapper must delegate to FormResponseSyncEventHandler.HandleAsync;
- SyncBatchProcessor must not directly construct FormResponse;
- SyncBatchProcessor must not directly parse CreateFormResponseRequest;
- SyncBatchProcessor must not contain form_response validation details;
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
- no weakening of P3-22H service encounter handler extraction;
- no weakening of P3-22I vital signs handler extraction.

---

## 5. Acceptance criteria

P3-22J is complete when:

- FormResponseSyncEventHandler exists;
- SyncBatchProcessor constructs FormResponseSyncEventHandler;
- SyncBatchProcessor delegates form_response/create handling to FormResponseSyncEventHandler;
- SyncBatchProcessor no longer contains direct FormResponse construction;
- FormResponseSyncEventHandler contains the previous form_response/create behavior;
- all previous P3 sync processor verifiers remain green;
- dotnet build and dotnet test remain green.
---

## 6. P3-22K consent document sync event handler extraction note

P3-22K extracts consent_document/create behavior into ConsentDocumentSyncEventHandler while preserving ProcessPendingEventAsync dispatch behavior.
