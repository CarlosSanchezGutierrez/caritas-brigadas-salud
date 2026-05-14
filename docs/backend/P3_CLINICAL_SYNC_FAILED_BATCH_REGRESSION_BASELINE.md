# P3 Clinical Sync Failed Batch Regression Baseline

Status: active
Scope: processor-level regression for failed sync batches
Target phase: P3-23E
Depends on: P3-23D clinical sync idempotency regression

---

## 1. Purpose

P3-23E adds regression coverage proving that failed sync batches cannot be processed.

A failed batch represents an unrecoverable processing state and must not be silently retried by SyncBatchProcessor.

---

## 2. Required regression

The integration test must:

1. seed Organization, User, Brigade, and SyncBatch;
2. mark the SyncBatch as failed using SyncBatch.Fail;
3. call SyncBatchProcessor.ProcessAsync;
4. assert that InvalidOperationException is thrown;
5. assert that no clinical rows or sync events are created.

---

## 3. Required assertions

The test must assert:

- ProcessAsync throws InvalidOperationException;
- exception message equals Failed sync batch cannot be processed.;
- Patient count remains 0;
- PatientVisit count remains 0;
- ServiceEncounter count remains 0;
- VitalSignsRecord count remains 0;
- FormResponse count remains 0;
- ConsentDocument count remains 0;
- MedicalReferral count remains 0;
- MedicationDelivery count remains 0;
- SyncEvent count remains 0;
- SyncBatch status remains failed;
- AcceptedCount remains 0;
- RejectedCount remains 0;
- ConflictCount remains 0.

---

## 4. Architecture rule

Failed batches are terminal for SyncBatchProcessor.

A failed batch must not be treated like received, processing, completed, or completed_with_errors.

---

## 5. Non-goals

P3-23E does not define retry recovery workflows.

P3-23E does not change production code.

P3-23E does not add SQL Server-specific integration testing.

---

## 6. Acceptance criteria

P3-23E is complete when:

- the failed batch regression exists;
- the test uses SyncBatch.Fail;
- the test uses SyncBatchProcessor;
- the test asserts InvalidOperationException;
- the test asserts no clinical rows or sync events are created;
- dotnet build and dotnet test pass.