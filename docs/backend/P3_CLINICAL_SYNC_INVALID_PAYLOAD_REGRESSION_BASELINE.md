# P3 Clinical Sync Invalid Payload Regression Baseline

Status: active
Scope: processor-level regression for rejected sync payloads
Target phase: P3-23C
Depends on: P3-23B clinical sync conflict regression

---

## 1. Purpose

P3-23C adds regression coverage proving that malformed sync payload JSON rejects only the invalid event and does not abort sync batch processing.

---

## 2. Required regression

The integration test must process one SyncBatch with exactly two patient/create events:

1. a valid patient event;
2. a patient event with malformed PayloadJson.

The processor must complete the batch with errors instead of throwing or leaving it unprocessed.

---

## 3. Required assertions

The test must assert:

- ProcessAsync completes the batch;
- PendingEventsProcessed equals 2;
- AcceptedCount equals 1;
- RejectedCount equals 1;
- ConflictCount equals 0;
- one Patient is persisted;
- one SyncEvent is accepted;
- one SyncEvent is rejected;
- the rejection reason contains Sync event payload JSON is invalid.;
- SyncBatch status is completed_with_errors.

---

## 4. Architecture rule

Malformed payload JSON is a controlled rejected input.

It must be represented as SyncEventStatus.Rejected, not as a conflict, database save failure, unhandled exception, or failed batch.

---

## 5. Non-goals

P3-23C does not test every rejection reason.

P3-23C does not change production code.

P3-23C does not add SQL Server-specific integration testing.

---

## 6. Acceptance criteria

P3-23C is complete when:

- the invalid payload rejection regression exists;
- the test uses SyncBatchProcessor;
- the test uses CaritasDbContext;
- the test persists one Patient;
- the test produces one accepted SyncEvent and one rejected SyncEvent;
- the batch completes_with_errors with 1 accepted, 1 rejected, and 0 conflicts;
- dotnet build and dotnet test pass.