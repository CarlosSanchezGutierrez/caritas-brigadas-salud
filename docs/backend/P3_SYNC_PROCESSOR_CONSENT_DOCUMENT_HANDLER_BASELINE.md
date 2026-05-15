# P3 Sync Processor Consent Document Handler Baseline

Status: active  
Scope: sync processor consent document create handler, signed privacy/consent evidence, patient/visit linkage, legal snapshot storage, and duplicate consent prevention  
Target phase: P3-18  
Depends on: P3 patient sync handler, P3 patient visit sync handler, P3 form response sync handler, P3 clinical data governance baseline

---

## 1. Purpose

P3-18 enables legal consent evidence ingestion through sync.

The supported real domain write in this package is:

- EntityType: consent_document
- Operation: create

This package intentionally does not process standalone document_signature, referrals, medication deliveries, external pass records, or media releases.

---

## 2. Consent document create rules

The consent document create handler must:

- process only SyncEntityType.ConsentDocument;
- process only SyncOperation.Create;
- parse PayloadJson as CreateConsentDocumentRequest;
- require JSON object payload;
- require PatientId;
- require ConsentType;
- require DocumentVersion;
- require SignatureDataUrl;
- create ConsentDocument with OrganizationId from the sync batch route/context, not payload trust;
- validate PatientId belongs to the same OrganizationId;
- validate PatientId can be found either in persisted Patients or in Patients staged in the same DbContext;
- validate VisitId belongs to the same OrganizationId, PatientId, and parent SyncBatch.BrigadeId when provided;
- validate VisitId can be found either in persisted PatientVisits or in PatientVisits staged in the same DbContext when provided;
- validate SignedByUserId belongs to the same OrganizationId when provided;
- normalize ConsentType to uppercase;
- normalize DocumentVersion;
- preserve DocumentTextSnapshot as the legal text snapshot;
- preserve SignatureDataUrl as the captured signature evidence;
- preserve guardian fields when provided;
- conflict duplicate ConsentDocument id inside the organization;
- conflict duplicate PatientId plus VisitId plus ConsentType plus DocumentVersion inside the organization;
- conflict duplicate PatientId plus VisitId plus ConsentType plus DocumentVersion values inside the same pending batch before SaveChangesAsync;
- reserve pending-batch consent document id and patient-visit-type-version keys only after successful ConsentDocument construction;
- reserve pending-batch consent document id and patient-visit-type-version keys atomically;
- rollback the consent document id reservation when patient-visit-type-version key reservation fails;
- accept the SyncEvent only after the ConsentDocument entity is staged;
- set SyncEvent.EntityId to the created ConsentDocument.Id through Accept;
- complete batch counters from stored SyncEvent statuses.

---

## 3. Legal and privacy safety

Rules:

- processor response must not expose PayloadJson;
- processor response must not expose SignatureDataUrl;
- processor must not log raw PayloadJson, DocumentTextSnapshot, or SignatureDataUrl;
- consent evidence must remain tenant-scoped;
- consent evidence must not be overwritten by duplicate submissions;
- signature data must be treated as sensitive legal evidence;
- offline consent documents must be traceable through DeviceId and SignedAt.

---

## 4. Offline patient-to-consent linkage

P3-18 allows patient create, patient_visit create, and consent_document create inside the same sync batch when stable GUID references are used.

Rules:

- patient create may use SyncEvent.EntityId as the Patient.Id;
- patient_visit create may use SyncEvent.EntityId as the PatientVisit.Id;
- consent_document create may reference PatientId and optional VisitId;
- the processor must process patient create events before consent_document create events;
- the processor must process patient_visit create events before consent_document create events when VisitId is provided;
- missing patient or visit references must become conflicts, not database failures.

---

## 5. Unsupported consent operations

Unsupported consent operations must not silently mutate legal evidence.

Rules:

- consent_document update is not implemented in P3-18;
- consent_document void is not implemented in P3-18;
- document_signature standalone sync is not implemented in P3-18;
- unsupported consent/document signature operations must be marked conflict;
- future packages must implement correction/void workflows with explicit audit policy.

---

## 6. Acceptance criteria

P3-18 is complete when:

- SyncBatchProcessor handles consent_document create events;
- SyncBatchProcessor creates ConsentDocument records from CreateConsentDocumentRequest;
- SyncBatchProcessor accepts successful consent_document create SyncEvents;
- SyncBatchProcessor stores created ConsentDocument.Id on SyncEvent.EntityId;
- SyncBatchProcessor marks missing patient as conflict;
- SyncBatchProcessor marks invalid visit as conflict when VisitId is provided;
- SyncBatchProcessor validates signed-by user when provided;
- SyncBatchProcessor requires signature evidence;
- SyncBatchProcessor marks duplicate patient-visit-type-version consent as conflict;
- SyncBatchProcessor rolls back consent document id reservation when patient-visit-type-version key reservation fails;
- contract tests protect the consent_document-only scope;
- repository governance and database deployment gates remain green.
---

## 7. P3-19 medical referral handler note

P3-19 adds medical_referral create handling. Referral/pass records remain separate from consent documents because they represent clinical external handoff evidence, not privacy signature evidence.
