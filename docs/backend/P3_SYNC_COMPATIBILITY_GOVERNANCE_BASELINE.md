# P3 Sync Compatibility Governance Baseline

Status: active  
Scope: compatibility governance for sync processor handler extraction  
Target phase: P3-22G.1  
Depends on: P3-22G patient visit sync event handler extraction

---

## 1. Purpose

P3-22G.1 clarifies that old processor-centered gates are compatibility governance, not accepted technical debt.

The backend must not use the term legacy to describe active sync code, active sync handlers, or active governance checks.

---

## 2. Compatibility governance definition

Compatibility governance means:

- a previous verifier or contract test still protects behavior introduced before extraction;
- the behavior may now live across SyncBatchProcessor, SyncProcessingOrder, SyncPayloadReader, or extracted sync event handlers;
- the verifier must inspect the correct current source files;
- the verifier must not force behavior back into SyncBatchProcessor;
- the verifier must not block decomposition.

---

## 3. Zero technical debt interpretation

Rules:

- compatibility gates are allowed only when they protect behavior during active decomposition;
- compatibility gates must be updated as soon as behavior moves to a new file;
- compatibility gates must not be used as an excuse to keep wrappers forever;
- compatibility wrappers in SyncBatchProcessor must be removed after handler extraction is complete;
- compatibility terminology must not be confused with legacy product code;
- no active backend source should describe current P3 sync code as legacy.

---

## 4. Current extraction status

Extracted handlers:

- PatientSyncEventHandler;
- PatientVisitSyncEventHandler.

Still pending extraction:

- ServiceEncounterSyncEventHandler;
- VitalSignsSyncEventHandler;
- FormResponseSyncEventHandler;
- ConsentDocumentSyncEventHandler;
- MedicalReferralSyncEventHandler;
- MedicationDeliverySyncEventHandler.

---

## 5. Backend closure path

The backend is not finished until:

- all sync handlers are extracted;
- temporary wrappers are removed or reduced to a clean dispatcher interface;
- compatibility gates are renamed or consolidated;
- sync processor no longer owns domain behavior;
- integration tests cover representative multi-event clinical sync batches;
- authorization/tenant/privacy gates remain green;
- database deployment baseline remains idempotent;
- CI remains green with build, tests, governance, and database gates.
---

## 6. Terminology scan scope

The terminology scan is intentionally scoped to active P3 sync handler-extraction governance files.

The scan must not target unrelated backend documents where legacy is a valid domain concept, including:

- historical import/orphan detection documentation;
- backward-compatible authorization claims;
- external paper/form migration notes;
- tests explicitly validating LegacyRole behavior.
