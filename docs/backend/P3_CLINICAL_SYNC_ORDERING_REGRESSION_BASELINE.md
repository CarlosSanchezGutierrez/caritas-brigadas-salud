# P3 Clinical Sync Ordering Regression Baseline

Status: active  
Scope: processor-level ordering regression for complete offline clinical sync  
Target phase: P3-23A  
Depends on: P3-22P clinical sync end-to-end test

---

## 1. Purpose

P3-23A adds regression coverage proving that a complete offline clinical sync batch still succeeds when events are inserted out of dependency order.

This protects the topological ordering rule implemented by SyncProcessingOrder.GetOrder.

---

## 2. Required regression

The integration test must process the same eight primary clinical sync events in reverse insertion order:

1. medication_delivery;
2. medical_referral;
3. consent_document;
4. form_response;
5. vital_signs;
6. service_encounter;
7. patient_visit;
8. patient.

Despite reverse insertion order, the processor must complete the batch with 8 accepted events.

---

## 3. Required test structure

Rules:

- the E2E test file must contain SyncBatchProcessor_ProcessesCompleteClinicalOfflineBatchEndToEnd;
- the E2E test file must contain SyncBatchProcessor_ProcessesOutOfOrderClinicalOfflineBatchUsingSyncProcessingOrder;
- both tests must reuse SeedCompleteClinicalBatchAsync;
- both tests must reuse AssertCompletedClinicalBatchAsync;
- the out-of-order test must call SeedCompleteClinicalBatchAsync with reverseEventInsertionOrder: true;
- the seeding helper must call events.Reverse();
- the batch must still seed BrigadeService;
- the batch must still contain all eight primary clinical sync events.

---

## 4. Required assertions

The shared assertion helper must assert:

- ProcessAsync completes the batch;
- PendingEventsProcessed equals 8;
- AcceptedCount equals 8;
- RejectedCount equals 0;
- ConflictCount equals 0;
- one row exists for every primary clinical entity;
- all SyncEvents are accepted;
- SyncBatch status is completed.

---

## 5. Non-goals

P3-23A does not change production code.

P3-23A does not add SQL Server-specific integration testing.

P3-23A does not weaken the P3-22P happy-path test.

---

## 6. Acceptance criteria

P3-23A is complete when:

- the happy-path E2E test still exists;
- the out-of-order E2E test exists;
- the test fixture supports reverse event insertion order;
- all eight clinical events are still validated;
- dotnet build and dotnet test pass.