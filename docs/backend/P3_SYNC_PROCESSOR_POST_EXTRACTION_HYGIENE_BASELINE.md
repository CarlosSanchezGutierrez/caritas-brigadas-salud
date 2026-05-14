# P3 Sync Processor Post-Extraction Hygiene Baseline

Status: active  
Scope: SyncBatchProcessor post-handler-extraction hygiene  
Target phase: P3-22N  
Depends on: P3-22M medication delivery sync event handler extraction

---

## 1. Purpose

P3-22N cleans SyncBatchProcessor after all primary P3 sync event handlers have been extracted.

This PR removes post-extraction residue without changing sync behavior.

---

## 2. Cleanup rules

Rules:

- SyncBatchProcessor must not contain stale request contract usings for extracted handlers;
- SyncBatchProcessor must not contain GenerateSyncPatientFolio;
- SyncBatchProcessor must not contain ParseSex;
- SyncBatchProcessor must not contain multiple consecutive blank-line blocks;
- SyncBatchProcessor wrapper methods must remain properly separated by one blank line;
- SyncBatchProcessor must still instantiate all extracted handlers;
- SyncBatchProcessor must still delegate to all extracted handlers;
- SyncBatchProcessor must still validate event entity type, operation, and payload JSON;
- SyncBatchProcessor must still use SyncProcessingOrder.GetOrder;
- SyncBatchProcessor must still create one PendingBatchReservationState per batch processing call.

---

## 3. Explicit non-goals

P3-22N does not remove temporary compatibility wrappers.

Wrapper removal belongs to P3-22O after the formatting, compatibility, and handler extraction gates are aligned.

---

## 4. Non-negotiable constraints

Rules:

- no endpoint change;
- no database migration;
- no behavior change;
- no handler behavior change;
- no weakening of extracted handler gates;
- no weakening of payload governance;
- no weakening of zero technical debt governance.

---

## 5. Acceptance criteria

P3-22N is complete when:

- SyncBatchProcessor has no extracted-handler request contract usings;
- SyncBatchProcessor has no GenerateSyncPatientFolio method;
- SyncBatchProcessor has no ParseSex method;
- SyncBatchProcessor has no glued wrapper method declarations;
- SyncBatchProcessor has no excessive blank-line blocks;
- all primary handler extraction gates remain green;
- dotnet build and dotnet test remain green.