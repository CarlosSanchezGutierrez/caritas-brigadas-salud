# P3 Sync Processor Formatting Hygiene Baseline

Status: active  
Scope: formatting hygiene and zero-debt style guardrails for SyncBatchProcessor after payload reader extraction  
Target phase: P3-22D  
Depends on: P3-22C sync payload reader extraction

---

## 1. Purpose

P3-22D removes formatting debt introduced during sync processor refactoring and makes it verifier-protected.

This package does not change behavior. It only protects source readability and maintainability.

---

## 2. Formatting rules

Rules:

- SyncBatchProcessor must not contain trailing whitespace;
- SyncBatchProcessor handler methods must not start at column 1;
- SyncBatchProcessor must not contain unindented local var declarations at column 1;
- SyncBatchProcessor must not contain unindented if statements at column 1;
- SyncBatchProcessor must not contain method declarations glued directly after a closing brace;
- SyncBatchProcessor must remain dotnet-format compatible;
- generated code, raw PayloadJson exposure, and handler behavior must remain unchanged.

---

## 3. Non-negotiable constraints

Rules:

- no database migration;
- no endpoint contract change;
- no sync entity type expansion;
- no handler behavior change;
- no weakening of P3-21 integration hardening;
- no weakening of P3-22A zero technical debt gate;
- no weakening of P3-22B component extraction;
- no weakening of P3-22C payload reader extraction.

---

## 4. Acceptance criteria

P3-22D is complete when:

- SyncBatchProcessor formatting debt is removed;
- SyncBatchProcessor formatting hygiene verifier exists;
- SyncBatchProcessor formatting hygiene contract test exists;
- repository governance baseline runs the formatting hygiene verifier;
- all previous P3 sync processor verifiers remain green;
- dotnet build and dotnet test remain green.
---

## 5. P3-22E pending event dispatch extraction note

P3-22E extracts per-event dispatch from ProcessAsync into ProcessPendingEventAsync while preserving existing handler behavior.
