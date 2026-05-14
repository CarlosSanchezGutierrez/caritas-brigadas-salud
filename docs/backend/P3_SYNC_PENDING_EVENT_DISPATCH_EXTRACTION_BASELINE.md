# P3 Sync Pending Event Dispatch Extraction Baseline

Status: active  
Scope: extraction of pending sync event dispatch from SyncBatchProcessor.ProcessAsync  
Target phase: P3-22E  
Depends on: P3-22D sync processor formatting hygiene

---

## 1. Purpose

P3-22E extracts per-event dispatch from ProcessAsync into ProcessPendingEventAsync.

This package does not extract domain handlers into separate classes yet. It only removes dispatch/orchestration noise from ProcessAsync so the next handler extraction can be done safely.

---

## 2. ProcessAsync contract

Rules:

- ProcessAsync must load the batch;
- ProcessAsync must load pending events;
- ProcessAsync must sort pending events through SyncProcessingOrder.GetOrder;
- ProcessAsync must create one PendingBatchReservationState per batch processing run;
- ProcessAsync must call ProcessPendingEventAsync for each pending event;
- ProcessAsync must increment processed count once per pending event after dispatch returns;
- ProcessAsync must calculate final accepted, rejected, and conflict counts;
- ProcessAsync must complete the batch;
- ProcessAsync must not directly branch on SyncEntityType for handler dispatch.

---

## 3. ProcessPendingEventAsync contract

Rules:

- ProcessPendingEventAsync must mark the event as processing;
- ProcessPendingEventAsync must validate the event through TryValidateEvent;
- ProcessPendingEventAsync must reject invalid events with a safe rejection reason;
- ProcessPendingEventAsync must dispatch to the existing patient handler;
- ProcessPendingEventAsync must dispatch to the existing patient visit handler;
- ProcessPendingEventAsync must dispatch to the existing service encounter handler;
- ProcessPendingEventAsync must dispatch to the existing vital signs handler;
- ProcessPendingEventAsync must dispatch to the existing form response handler;
- ProcessPendingEventAsync must dispatch to the existing consent document handler;
- ProcessPendingEventAsync must dispatch to the existing medical referral handler;
- ProcessPendingEventAsync must dispatch to the existing medication delivery handler;
- ProcessPendingEventAsync must mark unsupported events as conflict with the existing skeleton conflict reason.

---

## 4. Non-negotiable constraints

Rules:

- no database migration;
- no endpoint contract change;
- no sync entity type expansion;
- no handler behavior change;
- no handler class extraction in this package;
- no weakening of P3-21 integration hardening;
- no weakening of P3-22A zero technical debt gate;
- no weakening of P3-22B component extraction;
- no weakening of P3-22C payload reader extraction;
- no weakening of P3-22D formatting hygiene.

---

## 5. Acceptance criteria

P3-22E is complete when:

- ProcessPendingEventAsync exists;
- ProcessAsync calls ProcessPendingEventAsync inside the pending event loop;
- ProcessAsync no longer branches directly on SyncEntityType for handler dispatch;
- ProcessPendingEventAsync dispatches to all existing handlers;
- all previous P3 sync processor verifiers remain green;
- dotnet build and dotnet test remain green.
---

## 6. P3-22F patient sync event handler extraction note

P3-22F extracts patient/create behavior into PatientSyncEventHandler while preserving ProcessPendingEventAsync dispatch behavior.
