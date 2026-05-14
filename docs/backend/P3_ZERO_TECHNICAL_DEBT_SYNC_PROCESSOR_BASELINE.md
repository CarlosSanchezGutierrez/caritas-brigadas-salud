# P3 Zero Technical Debt Sync Processor Baseline

Status: active  
Scope: zero-technical-debt guardrails for SyncBatchProcessor after P3-21 integration hardening  
Target phase: P3-22A  
Depends on: P3-21 sync processor integration hardening

---

## 1. Purpose

P3-22A stops functional expansion of SyncBatchProcessor and converts detected complexity risk into enforceable governance.

This baseline does not permit technical debt. It exists to prevent new debt while the processor is decomposed in the following packages.

---

## 2. Current policy

Rules:

- no new sync entity handlers may be added directly to SyncBatchProcessor before decomposition;
- SyncBatchProcessor must keep no more than the current eight domain event handlers;
- processor formatting must not contain trailing whitespace;
- processor formatting must not contain over-indented pending-batch comments;
- processor code must not contain TODO, HACK, quick fix, temporary workaround, or technical debt accepted language;
- P3-21 topological ordering must remain protected;
- P3-21 atomic pending-batch reservation rules must remain protected;
- P3-20 medication delivery global id duplicate behavior must remain protected.

---

## 3. Required decomposition path

The only acceptable next refactor path is:

1. extract sync processing order into a dedicated internal component;
2. extract pending-batch reservation state into a dedicated internal component;
3. extract payload parsing/validation into a dedicated internal component;
4. extract each domain handler into a dedicated internal handler class;
5. keep ISyncBatchProcessor as an orchestration boundary only.

---

## 4. Non-negotiable constraints

Rules:

- no behavior regression;
- no endpoint contract regression;
- no database migration in this package;
- no new sync entity type in this package;
- no reduction in validation coverage;
- no weakening of privacy guarantees;
- no raw PayloadJson exposure;
- no raw clinical JSON echo in process results;
- no bypass of tenant checks.

---

## 5. Acceptance criteria

P3-22A is complete when:

- SyncBatchProcessor formatting debt is cleaned;
- SyncBatchProcessor is guarded against new direct handlers;
- P3 zero technical debt verifier exists;
- P3 zero technical debt contract tests exist;
- repository governance baseline runs the zero technical debt verifier;
- all previous P3 sync processor verifiers remain green;
- dotnet build and dotnet test remain green.
---

## 6. P3-22B component extraction note

P3-22B extracts SyncProcessingOrder and PendingBatchReservationState as internal components. New direct handlers remain forbidden until domain handlers are extracted into dedicated internal handler classes.
---

## 7. P3-22C payload reader extraction note

P3-22C reduces SyncBatchProcessor responsibility by moving repeated PayloadJson parsing into SyncPayloadReader. Direct handler expansion remains forbidden.
