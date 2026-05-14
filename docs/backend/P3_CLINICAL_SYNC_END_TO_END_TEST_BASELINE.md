# P3 Clinical Sync End-to-End Test Baseline

Status: active  
Scope: processor-level clinical sync integration test  
Target phase: P3-22P  
Depends on: P3-22O direct handler dispatch

---

## 1. Purpose

P3-22P adds a processor-level integration test for the complete offline clinical sync path.

This validates that the extracted handlers still work together after SyncBatchProcessor direct dispatch.

---

## 2. Required clinical flow

The test must process a single pending SyncBatch containing exactly eight create events:

1. patient;
2. patient_visit;
3. service_encounter;
4. vital_signs;
5. form_response;
6. consent_document;
7. medical_referral;
8. medication_delivery.

The test must intentionally submit the events as one offline batch to validate same-batch entity linkage through stable GUIDs.

---

## 3. Required assertions

The test must assert:

- ProcessAsync completes the batch;
- PendingEventsProcessed equals 8;
- AcceptedCount equals 8;
- RejectedCount equals 0;
- ConflictCount equals 0;
- one Patient is persisted;
- one PatientVisit is persisted;
- one ServiceEncounter is persisted;
- one VitalSignsRecord is persisted;
- one FormResponse is persisted;
- one ConsentDocument is persisted;
- one MedicalReferral is persisted;
- one MedicationDelivery is persisted;
- all SyncEvents are accepted;
- SyncBatch status is completed.

---

## 4. Test infrastructure rules

Rules:

- the test must use CaritasDbContext;
- the test must instantiate SyncBatchProcessor directly;
- the test must use EF Core InMemory only inside the test project;
- the test must seed Organization, User, Brigade, Service, FormTemplate, SyncBatch, and SyncEvents;
- the test must not require SQL Server;
- the test must not require HTTP;
- the test must not require external services;
- the test must not weaken any existing governance gate.

---

## 5. Non-goals

P3-22P does not replace SQL Server migration validation.

P3-22P does not validate filtered indexes or SQL Server-specific constraints.

P3-22P does not add new sync entity types.

P3-22P does not change production code.

---

## 6. Acceptance criteria

P3-22P is complete when:

- P3ClinicalSyncEndToEndIntegrationTests exists;
- the test uses SyncBatchProcessor;
- the test uses CaritasDbContext;
- the test processes all eight primary clinical sync events;
- all eight domain rows are persisted;
- all eight SyncEvents are accepted;
- dotnet build and dotnet test pass.