# P3 Sync Processor Medical Referral Handler Baseline

Status: active  
Scope: sync processor medical referral create handler, external referral/pass traceability, encounter linkage, patient linkage, referral folio uniqueness, and provider handoff guardrails  
Target phase: P3-19  
Depends on: P3 service encounter sync handler, P3 consent document sync handler, P3 external referral pass traceability baseline

---

## 1. Purpose

P3-19 enables external medical referral/pass creation through sync.

The supported real domain write in this package is:

- EntityType: medical_referral
- Operation: create

This package intentionally does not process medication_delivery, standalone document_signature, referral completion, referral cancellation, or external appointment outcome updates.

---

## 2. Medical referral create rules

The medical referral create handler must:

- process only SyncEntityType.MedicalReferral;
- process only SyncOperation.Create;
- parse PayloadJson as CreateMedicalReferralRequest;
- require JSON object payload;
- require EncounterId;
- require ReferralReason;
- create MedicalReferral with OrganizationId from the sync batch route/context, not payload trust;
- derive PatientId from ServiceEncounter.PatientId, not from payload trust;
- validate EncounterId belongs to the same OrganizationId;
- validate EncounterId belongs to the parent SyncBatch.BrigadeId;
- validate EncounterId can be found either in persisted ServiceEncounters or in ServiceEncounters staged in the same DbContext;
- validate derived PatientId belongs to the same OrganizationId;
- validate ReferredByUserId belongs to the same OrganizationId when provided;
- reject ProviderSignatureId until the document_signature handler exists;
- normalize or generate ReferralFolio;
- preserve DestinationInstitution;
- preserve ReferralReason;
- preserve Priority;
- conflict duplicate MedicalReferral id inside the organization;
- duplicate MedicalReferral id checks must include soft-deleted rows because primary key uniqueness is not soft-delete filtered;
- conflict duplicate ReferralFolio inside the organization;
- duplicate ReferralFolio checks must include soft-deleted rows because the OrganizationId plus ReferralFolio unique index is not soft-delete filtered;
- conflict duplicate ReferralFolio values inside the same pending batch before SaveChangesAsync;
- reserve pending-batch medical referral id and referral folio only after successful MedicalReferral construction;
- reserve pending-batch medical referral id and referral folio atomically;
- rollback the medical referral id reservation when referral folio reservation fails;
- accept the SyncEvent only after the MedicalReferral entity is staged;
- set SyncEvent.EntityId to the created MedicalReferral.Id through Accept;
- complete batch counters from stored SyncEvent statuses.

---

## 3. External pass traceability

Rules:

- referral is the internal source of truth for external pass generation;
- ReferralFolio is the stable traceability key for printed/PDF passes;
- DestinationInstitution identifies the government/private/external center when available;
- ReferralReason stores clinical justification for the referral/pass;
- Priority must be preserved for operational triage;
- sync event/batch metadata provides offline device traceability until the referral entity has native offline fields.

---

## 4. Privacy and safety

Rules:

- processor response must not expose PayloadJson;
- processor response must not expose full clinical justification beyond the created entity id;
- processor must not log raw PayloadJson or ReferralReason;
- referral data is clinical and must remain tenant-scoped;
- duplicate referral folio must not overwrite existing referral data.

---

## 5. Unsupported referral operations

Unsupported referral operations must not silently mutate referral/pass evidence.

Rules:

- medical_referral update is not implemented in P3-19;
- medical_referral complete is not implemented in P3-19;
- medical_referral cancel is not implemented in P3-19;
- standalone document_signature sync is not implemented in P3-19;
- unsupported referral/signature operations must be marked conflict;
- future packages must implement referral lifecycle transitions with explicit audit policy.

---

## 6. Acceptance criteria

P3-19 is complete when:

- CreateMedicalReferralRequest exists;
- SyncBatchProcessor handles medical_referral create events;
- SyncBatchProcessor creates MedicalReferral records from CreateMedicalReferralRequest;
- SyncBatchProcessor accepts successful medical_referral create SyncEvents;
- SyncBatchProcessor stores created MedicalReferral.Id on SyncEvent.EntityId;
- SyncBatchProcessor marks missing encounter as conflict;
- SyncBatchProcessor marks invalid patient linkage as conflict;
- SyncBatchProcessor validates referred-by user when provided;
- SyncBatchProcessor rejects ProviderSignatureId until document_signature handling exists;
- SyncBatchProcessor marks duplicate referral folio as conflict;
- SyncBatchProcessor includes soft-deleted rows when checking referral folio uniqueness;
- SyncBatchProcessor rolls back medical referral id reservation when referral folio reservation fails;
- contract tests protect the medical_referral-only scope;
- repository governance and database deployment gates remain green.