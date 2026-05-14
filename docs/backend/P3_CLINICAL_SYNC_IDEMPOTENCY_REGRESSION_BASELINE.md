# P3 Clinical Sync Idempotency Regression Baseline

Status: active
Scope: processor-level regression for already completed sync batches
Target phase: P3-23D
Depends on: P3-23C clinical sync invalid payload regression

---

## 1. Purpose

P3-23D adds regression coverage proving that processing an already completed sync batch is idempotent.

A completed batch must not be processed twice and must not duplicate clinical rows.

---

## 2. Required regression

The integration test must:

1. seed a complete clinical offline sync batch;
2. process it once successfully;
3. process the same SyncBatch a second time;
4. assert that the second processing call returns already completed;
5. assert that no clinical rows are duplicated.

---

## 3. Required assertions

The second ProcessAsync call must assert:

- Completed is true;
- PendingEventsProcessed equals 0;
- AcceptedCount remains 8;
- RejectedCount remains 0;
- ConflictCount remains 0;
- Message equals Sync batch was already completed.;
- Patient count remains 1;
- PatientVisit count remains 1;
- ServiceEncounter count remains 1;
- VitalSignsRecord count remains 1;
- FormResponse count remains 1;
- ConsentDocument count remains 1;
- MedicalReferral count remains 1;
- MedicationDelivery count remains 1;
- SyncEvent count remains 8;
- all SyncEvents remain accepted;
- SyncBatch status remains completed.

---

## 4. Architecture rule

Already completed batches are immutable from the processor perspective.

Re-processing a completed batch must be a safe no-op.

---

## 5. Non-goals

P3-23D does not change production code.

P3-23D does not test SQL Server-specific idempotency constraints.

P3-23D does not replace idempotency-key intake validation.

---

## 6. Acceptance criteria

P3-23D is complete when:

- the already completed batch regression exists;
- the test uses SyncBatchProcessor;
- the test processes the same SyncBatch twice;
- the second call has PendingEventsProcessed equals 0;
- no clinical table receives duplicate rows;
- dotnet build and dotnet test pass.