# P3 Medication Delivery Sync Event Handler Extraction Baseline

Status: active  
Scope: medication_delivery/create handler extraction from SyncBatchProcessor into an internal sync handler class  
Target phase: P3-22M  
Depends on: P3-22L medical referral sync event handler extraction

---

## 1. Purpose

P3-22M extracts medication_delivery/create sync handling from SyncBatchProcessor into MedicationDeliverySyncEventHandler.

MedicationDelivery represents medication access and receipt traceability tied to a clinical encounter.

---

## 2. MedicationDeliverySyncEventHandler contract

Rules:

- MedicationDeliverySyncEventHandler must be an internal infrastructure sync component;
- MedicationDeliverySyncEventHandler must own medication_delivery/create payload parsing;
- MedicationDeliverySyncEventHandler must reject unsupported medication_delivery operations;
- MedicationDeliverySyncEventHandler must validate EncounterId;
- MedicationDeliverySyncEventHandler must validate MedicationName;
- MedicationDeliverySyncEventHandler must validate Quantity;
- MedicationDeliverySyncEventHandler must reject SignatureId until document_signature handling exists;
- MedicationDeliverySyncEventHandler must validate encounter existence;
- MedicationDeliverySyncEventHandler must enforce batch brigade matching;
- MedicationDeliverySyncEventHandler must derive PatientId from ServiceEncounter.PatientId, not from payload trust;
- MedicationDeliverySyncEventHandler must validate patient existence;
- MedicationDeliverySyncEventHandler must validate DeliveredByUserId when MarkAsDelivered is true;
- MedicationDeliverySyncEventHandler must own medication delivery id conflict checks;
- MedicationDeliverySyncEventHandler must include globally duplicated ids in duplicate checks because primary key uniqueness is not tenant-scoped;
- MedicationDeliverySyncEventHandler must preserve non-delivered receipt metadata through constructor fields instead of silently dropping DeliveredByUserId or ReceivedByName;
- MedicationDeliverySyncEventHandler must support optional delivered transition only when MarkAsDelivered is true and DeliveredByUserId is provided;
- MedicationDeliverySyncEventHandler must reserve medication delivery id only after successful MedicationDelivery construction and optional delivered transition;
- MedicationDeliverySyncEventHandler must construct MedicationDelivery;
- MedicationDeliverySyncEventHandler must accept the SyncEvent only after staging MedicationDelivery.

---

## 3. SyncBatchProcessor contract

Rules:

- SyncBatchProcessor may keep a temporary compatibility wrapper named HandleMedicationDeliveryEventAsync;
- the wrapper must remain async to preserve formatting and compatibility governance contracts;
- the wrapper must delegate to MedicationDeliverySyncEventHandler.HandleAsync;
- SyncBatchProcessor must not directly construct MedicationDelivery;
- SyncBatchProcessor must not directly parse CreateMedicationDeliveryRequest;
- SyncBatchProcessor must not contain medication_delivery validation details;
- ProcessPendingEventAsync behavior must remain unchanged.

---

## 4. Traceability requirement

MedicationDelivery must support:

- internal medication delivery during brigades;
- scarce medication access traceability;
- external medication-routing evidence when tied to medical referral flows;
- receipt metadata through DeliveredByUserId and ReceivedByName;
- later document_signature support without silently accepting unsupported signatures.

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
- no weakening of P3-22L medical referral handler extraction.

---

## 6. Acceptance criteria

P3-22M is complete when:

- MedicationDeliverySyncEventHandler exists;
- SyncBatchProcessor constructs MedicationDeliverySyncEventHandler;
- SyncBatchProcessor delegates medication_delivery/create handling to MedicationDeliverySyncEventHandler;
- SyncBatchProcessor no longer contains direct MedicationDelivery construction;
- MedicationDeliverySyncEventHandler contains the previous medication_delivery/create behavior;
- all previous P3 sync processor verifiers remain green;
- dotnet build and dotnet test remain green.
---

## 7. P3-22N post-extraction hygiene note

P3-22N removes SyncBatchProcessor post-extraction residue after all primary P3 sync event handlers have been extracted.
