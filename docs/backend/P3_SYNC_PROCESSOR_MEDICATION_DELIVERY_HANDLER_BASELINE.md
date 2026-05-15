# P3 Sync Processor Medication Delivery Handler Baseline

Status: active  
Scope: sync processor medication delivery create handler, encounter linkage, patient linkage, optional delivered transition, medication traceability, and signature guardrails  
Target phase: P3-20  
Depends on: P3 service encounter sync handler, P3 medical referral sync handler, P3 clinical data governance baseline

---

## 1. Purpose

P3-20 enables medication delivery creation through sync.

The supported real domain write in this package is:

- EntityType: medication_delivery
- Operation: create

This package intentionally does not process inventory stock decrement, standalone document_signature, signed medication receipts, medication delivery cancellation, or medication delivery update.

---

## 2. Medication delivery create rules

The medication delivery create handler must:

- process only SyncEntityType.MedicationDelivery;
- process only SyncOperation.Create;
- parse PayloadJson as CreateMedicationDeliveryRequest;
- require JSON object payload;
- require EncounterId;
- require MedicationName;
- create MedicationDelivery with OrganizationId from the sync batch route/context, not payload trust;
- derive PatientId from ServiceEncounter.PatientId, not from payload trust;
- validate EncounterId belongs to the same OrganizationId;
- validate EncounterId belongs to the parent SyncBatch.BrigadeId;
- validate EncounterId can be found either in persisted ServiceEncounters or in ServiceEncounters staged in the same DbContext;
- validate derived PatientId belongs to the same OrganizationId;
- validate DeliveredByUserId belongs to the same OrganizationId when provided;
- reject SignatureId until the document_signature handler exists;
- preserve MedicationName;
- preserve Presentation;
- preserve Quantity;
- preserve LotNumber;
- preserve ExpirationDate;
- preserve Instructions;
- preserve ReceivedByName when provided;
- preserve DeliveredByUserId and ReceivedByName when provided even if MarkAsDelivered is false;
- support optional delivered transition only when MarkAsDelivered is true and DeliveredByUserId is provided;
- conflict duplicate MedicationDelivery id inside the organization;
- duplicate MedicationDelivery id checks must include globally duplicated ids because primary key uniqueness is not tenant-scoped;
- conflict duplicate MedicationDelivery id values inside the same pending batch before SaveChangesAsync;
- reserve pending-batch medication delivery id only after successful MedicationDelivery construction and optional delivered transition;
- accept the SyncEvent only after the MedicationDelivery entity is staged;
- set SyncEvent.EntityId to the created MedicationDelivery.Id through Accept;
- complete batch counters from stored SyncEvent statuses.

---

## 3. Medication traceability

Rules:

- MedicationDelivery is the clinical record of medication delivered or prepared for delivery;
- ExpirationDate must be validated by the domain entity;
- expired medication must be rejected by domain rules;
- delivered status must use MedicationDelivery.MarkDelivered;
- SignatureId must remain unsupported until document_signature sync exists;
- sync event/batch metadata provides offline device traceability until medication delivery has native offline fields.

---

## 4. Privacy and safety

Rules:

- processor response must not expose PayloadJson;
- processor response must not expose full medication instructions beyond created entity id;
- processor must not log raw PayloadJson or Instructions;
- medication delivery data is clinical and must remain tenant-scoped;
- duplicate MedicationDelivery id must not overwrite existing medication delivery data.

---

## 5. Unsupported medication delivery operations

Unsupported medication delivery operations must not silently mutate medication records.

Rules:

- medication_delivery update is not implemented in P3-20;
- medication_delivery cancel is not implemented in P3-20;
- standalone document_signature sync is not implemented in P3-20;
- inventory decrement/reservation is not implemented in P3-20;
- unsupported medication delivery/signature/inventory operations must be marked conflict;
- future packages must implement medication lifecycle transitions with explicit audit policy.

---

## 6. Acceptance criteria

P3-20 is complete when:

- CreateMedicationDeliveryRequest exists;
- SyncBatchProcessor handles medication_delivery create events;
- SyncBatchProcessor creates MedicationDelivery records from CreateMedicationDeliveryRequest;
- SyncBatchProcessor optionally calls MarkDelivered when MarkAsDelivered is true;
- SyncBatchProcessor accepts successful medication_delivery create SyncEvents;
- SyncBatchProcessor stores created MedicationDelivery.Id on SyncEvent.EntityId;
- SyncBatchProcessor marks missing encounter as conflict;
- SyncBatchProcessor marks invalid patient linkage as conflict;
- SyncBatchProcessor validates delivered-by user when provided;
- SyncBatchProcessor rejects SignatureId until document_signature handling exists;
- SyncBatchProcessor marks duplicate medication delivery id as conflict;
- SyncBatchProcessor includes globally duplicated medication delivery ids when checking id uniqueness;
- SyncBatchProcessor preserves non-delivered medication receipt metadata instead of silently dropping it;
- contract tests protect the medication_delivery-only scope;
- repository governance and database deployment gates remain green.