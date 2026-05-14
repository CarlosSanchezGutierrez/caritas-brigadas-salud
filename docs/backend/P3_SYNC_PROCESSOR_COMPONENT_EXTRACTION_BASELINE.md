# P3 Sync Processor Component Extraction Baseline

Status: active  
Scope: first zero-debt internal extraction for SyncBatchProcessor responsibilities  
Target phase: P3-22B  
Depends on: P3-22A zero technical debt sync processor gate

---

## 1. Purpose

P3-22B begins decomposition of SyncBatchProcessor without changing sync behavior.

This package extracts two responsibilities:

- SyncProcessingOrder;
- PendingBatchReservationState.

No domain handler is extracted in this package. No new sync entity type is enabled.

---

## 2. SyncProcessingOrder contract

Rules:

- SyncProcessingOrder must be an internal infrastructure sync component;
- SyncProcessingOrder.GetOrder must own the topological create order;
- SyncBatchProcessor must sort pending events using SyncProcessingOrder.GetOrder;
- Legacy P3 processor tests must read SyncProcessingOrder for topological return tokens;
- old behavior must remain unchanged;
- unsupported events must keep fallback order after known create handlers.

---

## 3. PendingBatchReservationState contract

Rules:

- PendingBatchReservationState must own the pending-batch reservation sets;
- SyncBatchProcessor must instantiate PendingBatchReservationState once per ProcessAsync call;
- SyncBatchProcessor must not directly instantiate per-handler HashSet reservation variables;
- existing atomic reservation behavior must remain unchanged;
- handler bodies must continue using their received ISet parameters until handlers are extracted;
- reservation state must not be static;
- reservation state must not be shared across batches.

---

## 4. Non-negotiable constraints

Rules:

- no database migration;
- no endpoint contract change;
- no sync entity type expansion;
- no weakening of P3-21 integration hardening;
- no weakening of P3-22A zero technical debt gate;
- no raw PayloadJson exposure;
- no handler behavior change.

---

## 5. Acceptance criteria

P3-22B is complete when:

- SyncProcessingOrder exists;
- PendingBatchReservationState exists;
- SyncBatchProcessor uses SyncProcessingOrder.GetOrder;
- SyncBatchProcessor uses PendingBatchReservationState;
- SyncBatchProcessor no longer declares direct HashSet reservation variables in ProcessAsync;
- all P3 sync processor verifiers remain green;
- dotnet build and dotnet test remain green.