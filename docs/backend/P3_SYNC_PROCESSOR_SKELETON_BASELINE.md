# P3 Sync Processor Skeleton Baseline

Status: active  
Scope: sync processor skeleton, safe processing transition, pending event validation, conflict staging, and no clinical domain writes  
Target phase: P3-12  
Depends on: P3 sync batch event intake, P3 sync event read model, P3 sync payload governance, P3 sync idempotency guardrails

---

## 1. Purpose

P3-12 introduces the first sync processor skeleton.

The processor exists to move received batches and pending events through safe status transitions without applying clinical domain writes yet.

---

## 2. Processor endpoint

Required endpoint:

POST /api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}/process

Rules:

- endpoint must be tenant-scoped;
- endpoint must require SyncBatchesWrite permission;
- endpoint must call ISyncBatchProcessor;
- endpoint must not accept raw payload in the process request;
- endpoint must not expose PayloadJson in the response.

---

## 3. Skeleton behavior

The skeleton processor may:

- load SyncBatch by OrganizationId and SyncBatchId;
- reject missing batch;
- reject failed batch processing;
- return safely when the batch is already completed;
- mark received batch as processing;
- load pending SyncEvent rows;
- mark pending events as processing;
- validate EntityType allowlist;
- validate Operation allowlist;
- validate PayloadJson syntax;
- reject invalid pending events;
- mark valid pending events as conflict because domain handlers are not implemented yet;
- complete the batch with counters.
- processor must not complete against client-supplied event totals; intake must persist SyncBatch.EventsCount from server-parsed event count.

The skeleton processor must not:

- create Patient records;
- create PatientVisit records;
- create ServiceEncounter records;
- create VitalSignsRecord records;
- create FormResponse records;
- create ConsentDocument records;
- create MedicalReferral records;
- create MedicationDelivery records;
- accept events as applied clinical writes;
- log raw PayloadJson;
- send raw PayloadJson to analytics or AI.

---

## 4. Future processor handoff

Future packages must replace the skeleton conflict behavior with entity-specific handlers.

Required future handlers:

- patient create/update/void;
- patient_visit create/update/void;
- service_encounter create/update/void;
- vital_signs create/update/void;
- form_response create/update/void;
- consent_document create/sign/void;
- medical_referral create/update/void;
- medication_delivery create/update/void.

---

## 5. Acceptance criteria

P3-12 is complete when:

- ISyncBatchProcessor exists;
- SyncBatchProcessor exists;
- SyncBatchProcessor processes only tenant-scoped batches;
- SyncBatchProcessor marks received batches as processing;
- SyncBatchProcessor validates pending events;
- SyncBatchProcessor rejects invalid events;
- SyncBatchProcessor marks valid events as conflict while domain handlers are not implemented;
- SyncBatchProcessor completes the batch with counters;
- SyncBatchesController exposes process endpoint;
- processor response does not expose PayloadJson;
- repository governance and database deployment gates remain green.
---

## 6. P3-13 patient handler note

P3-13 supersedes the skeleton for patient create only. After P3-13, SyncBatchProcessor may accept SyncEntityType.Patient with SyncOperation.Create and may create Patient records. All other entity types remain conflict-staged until their handlers are implemented.