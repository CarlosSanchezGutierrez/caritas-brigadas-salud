# P3 Consent Document Sync Event Handler Extraction Baseline

Status: active  
Scope: consent_document/create handler extraction from SyncBatchProcessor into an internal sync handler class  
Target phase: P3-22K  
Depends on: P3-22J form response sync event handler extraction

---

## 1. Purpose

P3-22K extracts consent_document/create sync handling from SyncBatchProcessor into ConsentDocumentSyncEventHandler.

This preserves behavior while continuing the clinical/legal sync handler decomposition path:

Patient -> PatientVisit -> ServiceEncounter -> ConsentDocument

---

## 2. ConsentDocumentSyncEventHandler contract

Rules:

- ConsentDocumentSyncEventHandler must be an internal infrastructure sync component;
- ConsentDocumentSyncEventHandler must own consent_document/create payload parsing;
- ConsentDocumentSyncEventHandler must reject unsupported consent_document operations;
- ConsentDocumentSyncEventHandler must validate PatientId;
- ConsentDocumentSyncEventHandler must validate ConsentType;
- ConsentDocumentSyncEventHandler must validate DocumentVersion;
- ConsentDocumentSyncEventHandler must require SignatureDataUrl;
- ConsentDocumentSyncEventHandler must validate patient existence;
- ConsentDocumentSyncEventHandler must validate optional VisitId existence for the same patient and batch brigade;
- ConsentDocumentSyncEventHandler must validate optional signed-by user existence;
- ConsentDocumentSyncEventHandler must own consent document id conflict checks;
- ConsentDocumentSyncEventHandler must own duplicate PatientId plus VisitId plus ConsentType plus DocumentVersion checks;
- ConsentDocumentSyncEventHandler must preserve DocumentTextSnapshot as the legal text snapshot;
- ConsentDocumentSyncEventHandler must preserve SignatureDataUrl as the captured signature evidence;
- ConsentDocumentSyncEventHandler must preserve GuardianFullName and GuardianRelationship;
- ConsentDocumentSyncEventHandler must create ConsentDocument through CreateConsentDocumentForSync;
- ConsentDocumentSyncEventHandler must use SetConsentPropertyIfExists only inside the handler class;
- ConsentDocumentSyncEventHandler must reserve consent document id and patient-visit-type-version keys only after successful ConsentDocument construction and atomically;
- ConsentDocumentSyncEventHandler must roll back consent document id reservation when patient-visit-type-version key reservation fails;
- ConsentDocumentSyncEventHandler must accept the SyncEvent only after staging ConsentDocument.

---

## 3. SyncBatchProcessor contract

Rules:

- SyncBatchProcessor may keep a temporary compatibility wrapper named HandleConsentDocumentEventAsync;
- the wrapper must remain async to preserve formatting and compatibility governance contracts;
- the wrapper must delegate to ConsentDocumentSyncEventHandler.HandleAsync;
- SyncBatchProcessor must not directly create ConsentDocument;
- SyncBatchProcessor must not directly parse CreateConsentDocumentRequest;
- SyncBatchProcessor must not contain consent_document validation details;
- SyncBatchProcessor must not contain CreateConsentDocumentForSync;
- SyncBatchProcessor must not contain SetConsentPropertyIfExists;
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
- no weakening of P3-22I vital signs handler extraction;
- no weakening of P3-22J form response handler extraction.

---

## 5. Acceptance criteria

P3-22K is complete when:

- ConsentDocumentSyncEventHandler exists;
- SyncBatchProcessor constructs ConsentDocumentSyncEventHandler;
- SyncBatchProcessor delegates consent_document/create handling to ConsentDocumentSyncEventHandler;
- SyncBatchProcessor no longer contains direct ConsentDocument creation;
- SyncBatchProcessor no longer contains CreateConsentDocumentForSync;
- SyncBatchProcessor no longer contains SetConsentPropertyIfExists;
- ConsentDocumentSyncEventHandler contains the previous consent_document/create behavior;
- all previous P3 sync processor verifiers remain green;
- dotnet build and dotnet test remain green.
---

## 6. P3-22L medical referral sync event handler extraction note

P3-22L extracts medical_referral/create behavior into MedicalReferralSyncEventHandler while preserving ProcessPendingEventAsync dispatch behavior.
