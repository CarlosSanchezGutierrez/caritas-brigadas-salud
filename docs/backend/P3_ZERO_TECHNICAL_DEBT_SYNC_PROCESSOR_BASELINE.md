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
---

## 8. P3-22D formatting hygiene note

P3-22D extends the zero technical debt policy with explicit SyncBatchProcessor formatting hygiene checks.
---

## 9. P3-22E pending event dispatch extraction note

P3-22E reduces ProcessAsync responsibility by moving pending event dispatch into ProcessPendingEventAsync. Direct handler expansion remains forbidden.
---

## 10. P3-22F patient sync event handler extraction note

P3-22F starts real domain handler extraction by moving patient/create behavior into PatientSyncEventHandler.
---

## 11. P3-22G patient visit sync event handler extraction note

P3-22G continues real domain handler extraction by moving patient_visit/create behavior into PatientVisitSyncEventHandler.
---

## 12. P3-22G.1 compatibility governance note

P3-22G.1 clarifies that processor-centered verifiers retained during handler extraction are compatibility governance, not accepted technical debt. Current active P3 sync code and governance should not use deprecated compatibility terminology incorrectly.
---

## 13. P3-22H service encounter sync event handler extraction note

P3-22H continues real domain handler extraction by moving service_encounter/create behavior into ServiceEncounterSyncEventHandler.
---

## 14. P3-22I vital signs sync event handler extraction note

P3-22I continues real domain handler extraction by moving vital_signs/create behavior into VitalSignsSyncEventHandler.
---

## 15. P3-22J form response sync event handler extraction note

P3-22J continues real domain handler extraction by moving form_response/create behavior into FormResponseSyncEventHandler.
---

## 16. P3-22K consent document sync event handler extraction note

P3-22K continues real domain handler extraction by moving consent_document/create behavior into ConsentDocumentSyncEventHandler.
---

## 17. P3-22L medical referral sync event handler extraction note

P3-22L continues real domain handler extraction by moving medical_referral/create behavior into MedicalReferralSyncEventHandler.
---

## 18. P3-22M medication delivery sync event handler extraction note

P3-22M completes primary domain handler extraction by moving medication_delivery/create behavior into MedicationDeliverySyncEventHandler.
---

## 19. P3-22N post-extraction hygiene note

P3-22N removes stale SyncBatchProcessor helpers and unused request-contract imports left behind after handler extraction.
---

## 20. P3-22O direct handler dispatch note

P3-22O removes temporary wrappers from SyncBatchProcessor and leaves direct handler dispatch as the required zero-debt state.
---

## 21. P3-22P clinical sync end-to-end test note

P3-22P closes the primary sync processor refactor by validating the extracted handlers together in one processor-level clinical end-to-end test.
---

## 22. P3-23A ordering regression note

P3-23A protects the direct-dispatch sync architecture by validating that same-batch clinical dependencies still succeed when offline clients upload events out of order.
---

## 23. P3-23B conflict regression note

P3-23B protects the sync processor against regression where controlled domain conflicts become database failures, unhandled exceptions, or failed batches.
---

## 24. P3-23C invalid payload regression note

P3-23C protects the sync processor against regression where malformed payload JSON becomes an unhandled exception or failed batch instead of a controlled rejected event.
---

## 25. P3-23D idempotency regression note

P3-23D protects the sync processor against duplicate domain rows caused by repeated processing of already completed batches.
---

## 26. P3-23E failed batch regression note

P3-23E protects the sync processor against unsafe retries of terminal failed batches.
---

## 27. P3-24A API-level endpoint regression note

P3-24A protects the sync process endpoint wiring and removes stale skeleton wording from the public API response.
---

## 28. P3-24B create batch endpoint API regression note

P3-24B protects the sync intake endpoint and confirms that creation receives batches and events without applying clinical writes.
---

## 29. P3-24C list events endpoint API regression note

P3-24C protects sync event listing privacy by ensuring PayloadJson remains internal processing data.
---

## 30. P3-24D tenant boundary endpoint API regression note

P3-24D protects sync API tenant boundaries by ensuring cross-tenant batch access returns 404 and does not process or leak payload content.
