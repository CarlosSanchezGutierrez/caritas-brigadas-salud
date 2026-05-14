# P3 Sync Processor Form Response Handler Baseline

Status: active  
Scope: sync processor form response create handler, tenant-scoped form response creation, encounter/template linkage, response JSON validation, and duplicate response prevention  
Target phase: P3-17  
Depends on: P3 service encounter sync handler, P3 sync processor skeleton, P3 sync payload governance, P3 clinical data governance baseline

---

## 1. Purpose

P3-17 enables the fifth real sync processor handler.

The supported real domain write in this package is:

- EntityType: form_response
- Operation: create

This package intentionally does not process consents, referrals, medication deliveries, external pass records, or document signatures.

---

## 2. Form response create rules

The form response create handler must:

- process only SyncEntityType.FormResponse;
- process only SyncOperation.Create;
- parse PayloadJson as CreateFormResponseRequest;
- require JSON object payload;
- require EncounterId;
- require FormTemplateId;
- require ResponseJson;
- validate ResponseJson is valid JSON;
- create FormResponse with OrganizationId from the sync batch route/context, not payload trust;
- validate EncounterId belongs to the same OrganizationId;
- validate EncounterId belongs to the parent SyncBatch.BrigadeId;
- validate EncounterId can be found either in persisted ServiceEncounters or in ServiceEncounters staged in the same DbContext;
- validate FormTemplateId belongs to the same OrganizationId and to the encounter ServiceId;
- validate form template is active;
- validate form template effective date window when present;
- validate SubmittedByUserId belongs to the same OrganizationId when provided;
- conflict duplicate FormResponse id inside the organization;
- conflict duplicate EncounterId plus FormTemplateId inside the organization;
- conflict duplicate EncounterId plus FormTemplateId values inside the same pending batch before SaveChangesAsync;
- reserve pending-batch form response id and encounter-template keys only after successful FormResponse construction;
- accept the SyncEvent only after the FormResponse entity is staged;
- set SyncEvent.EntityId to the created FormResponse.Id through Accept;
- complete batch counters from stored SyncEvent statuses.

---

## 3. Offline encounter-to-form linkage

P3-17 allows patient, patient_visit, service_encounter, vital_signs, and form_response create events inside the same sync batch when stable GUID references are used.

Rules:

- service_encounter create may use SyncEvent.EntityId as the ServiceEncounter.Id;
- form_response create may reference that EncounterId;
- the processor must process service_encounter create events before form_response create events;
- the processor must check tracked ServiceEncounters in the current DbContext before checking only the database;
- missing encounter or template references must become conflicts, not database failures.

---

## 4. Unsupported form response operations

Unsupported form response operations must not silently mutate records.

Rules:

- form_response update is not implemented in P3-17;
- form_response void is not implemented in P3-17;
- unsupported form_response operations must be marked conflict;
- future packages must implement update/void with explicit conflict and audit policy.

---

## 5. Privacy and safety

Rules:

- processor response must not expose PayloadJson;
- processor must not log raw PayloadJson or ResponseJson;
- processor must not create consent documents, referrals, medication deliveries, or external pass records in P3-17;
- duplicate encounter-template pair must not overwrite existing form response data;
- response JSON is clinical data and must remain tenant-scoped.

---

## 6. Acceptance criteria

P3-17 is complete when:

- SyncBatchProcessor handles form_response create events;
- SyncBatchProcessor creates FormResponse records from CreateFormResponseRequest;
- SyncBatchProcessor accepts successful form_response create SyncEvents;
- SyncBatchProcessor stores created FormResponse.Id on SyncEvent.EntityId;
- SyncBatchProcessor marks missing encounter as conflict;
- SyncBatchProcessor marks missing/inactive/expired template as conflict;
- SyncBatchProcessor validates submitted-by user when provided;
- SyncBatchProcessor marks duplicate encounter-template pair as conflict;
- SyncBatchProcessor processes service_encounter before form_response;
- contract tests protect the form_response-only scope;
- repository governance and database deployment gates remain green.
---

## 7. P3-21 integration hardening note

P3-21 requires FormResponse pending-batch id and encounter-template key reservations to be atomic with rollback when the second reservation fails.
