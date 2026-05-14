# P3 Medical Referral Sync Event Handler Extraction Baseline

Status: active  
Scope: medical_referral/create handler extraction from SyncBatchProcessor into an internal sync handler class  
Target phase: P3-22L  
Depends on: P3-22K consent document sync event handler extraction

---

## 1. Purpose

P3-22L extracts medical_referral/create sync handling from SyncBatchProcessor into MedicalReferralSyncEventHandler.

Medical referrals are clinical/legal traceability records for external medical passes, specialty consultations, procedures, operations, medication access, or government/private center referrals.

---

## 2. MedicalReferralSyncEventHandler contract

Rules:

- MedicalReferralSyncEventHandler must be an internal infrastructure sync component;
- MedicalReferralSyncEventHandler must own medical_referral/create payload parsing;
- MedicalReferralSyncEventHandler must reject unsupported medical_referral operations;
- MedicalReferralSyncEventHandler must validate EncounterId;
- MedicalReferralSyncEventHandler must validate ReferralReason;
- MedicalReferralSyncEventHandler must reject ProviderSignatureId until document_signature handling exists;
- MedicalReferralSyncEventHandler must validate encounter existence;
- MedicalReferralSyncEventHandler must enforce batch brigade matching;
- MedicalReferralSyncEventHandler must derive PatientId from ServiceEncounter.PatientId, not from payload trust;
- MedicalReferralSyncEventHandler must validate patient existence;
- MedicalReferralSyncEventHandler must validate optional referred-by user existence;
- MedicalReferralSyncEventHandler must own medical referral id conflict checks;
- MedicalReferralSyncEventHandler must include soft-deleted rows in id duplicate checks because primary key uniqueness is not filtered by IsDeleted;
- MedicalReferralSyncEventHandler must own ReferralFolio generation through GenerateSyncMedicalReferralFolio;
- MedicalReferralSyncEventHandler must include soft-deleted rows in ReferralFolio duplicate checks because database unique index is not filtered by IsDeleted;
- MedicalReferralSyncEventHandler must reserve medical referral id and referral folio only after successful MedicalReferral construction and atomically;
- MedicalReferralSyncEventHandler must roll back medical referral id reservation when referral folio reservation fails;
- MedicalReferralSyncEventHandler must construct MedicalReferral;
- MedicalReferralSyncEventHandler must accept the SyncEvent only after staging MedicalReferral.

---

## 3. SyncBatchProcessor contract

Rules:

- SyncBatchProcessor may keep a temporary compatibility wrapper named HandleMedicalReferralEventAsync;
- the wrapper must remain async to preserve formatting and compatibility governance contracts;
- the wrapper must delegate to MedicalReferralSyncEventHandler.HandleAsync;
- SyncBatchProcessor must not directly construct MedicalReferral;
- SyncBatchProcessor must not directly parse CreateMedicalReferralRequest;
- SyncBatchProcessor must not contain medical_referral validation details;
- SyncBatchProcessor must not contain GenerateSyncMedicalReferralFolio;
- ProcessPendingEventAsync behavior must remain unchanged.

---

## 4. Traceability requirement

MedicalReferral is the traceability foundation for external passes and referral papers.

The stable operational key is ReferralFolio. It must support:

- printed or PDF referral passes;
- government center referrals;
- private center referrals;
- specialty consultation referrals;
- scarce medical procedure referrals;
- operation/surgery referrals;
- medication access referrals when routed externally;
- later signature/document attachment support.

---

## 5. Non-negotiable constraints

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
- no weakening of P3-22K consent document handler extraction.

---

## 6. Acceptance criteria

P3-22L is complete when:

- MedicalReferralSyncEventHandler exists;
- SyncBatchProcessor constructs MedicalReferralSyncEventHandler;
- SyncBatchProcessor delegates medical_referral/create handling to MedicalReferralSyncEventHandler;
- SyncBatchProcessor no longer contains direct MedicalReferral construction;
- SyncBatchProcessor no longer contains GenerateSyncMedicalReferralFolio;
- MedicalReferralSyncEventHandler contains the previous medical_referral/create behavior;
- all previous P3 sync processor verifiers remain green;
- dotnet build and dotnet test remain green.
---

## 7. P3-22M medication delivery sync event handler extraction note

P3-22M extracts medication_delivery/create behavior into MedicationDeliverySyncEventHandler while preserving ProcessPendingEventAsync dispatch behavior.
