# P3 Clinical Sync Conflict Regression Baseline

Status: active  
Scope: processor-level regression for controlled sync conflicts  
Target phase: P3-23B  
Depends on: P3-23A clinical sync ordering regression

---

## 1. Purpose

P3-23B adds regression coverage proving that a controlled conflict does not abort sync batch processing.

The first protected case is duplicate patient folio detection inside the same pending batch.

---

## 2. Required regression

The integration test must process one SyncBatch with exactly two patient/create events:

1. a valid patient with PatientFolio PAT-CONFLICT-001;
2. a second patient with the same PatientFolio PAT-CONFLICT-001.

The processor must complete the batch instead of throwing or leaving it unprocessed.

---

## 3. Required assertions

The test must assert:

- ProcessAsync completes the batch;
- PendingEventsProcessed equals 2;
- AcceptedCount equals 1;
- RejectedCount equals 0;
- ConflictCount equals 1;
- one Patient is persisted;
- one SyncEvent is accepted;
- one SyncEvent is conflict;
- the conflict reason contains patient_folio_duplicate_in_pending_batch;
- SyncBatch status is completed_with_errors because controlled conflicts are completed with errors.

---

## 4. Architecture rule

Controlled conflicts are expected domain outcomes.

They must be represented as SyncEventStatus.Conflict, not as database save failures, unhandled exceptions, rejected payloads, or failed batches.

---

## 5. Non-goals

P3-23B does not test every domain conflict reason.

P3-23B does not change production code.

P3-23B does not add SQL Server-specific integration testing.

---

## 6. Acceptance criteria

P3-23B is complete when:

- the duplicate patient folio conflict regression exists;
- the test uses SyncBatchProcessor;
- the test uses CaritasDbContext;
- the test persists one Patient;
- the test produces one accepted SyncEvent and one conflict SyncEvent;
- the batch completes_with_errors with 1 accepted, 0 rejected, and 1 conflict;
- dotnet build and dotnet test pass.