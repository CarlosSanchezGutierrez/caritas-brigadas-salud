# P3 Sync Processor Integration Hardening Baseline

Status: active  
Scope: sync processor cross-handler hardening, topological ordering, pending-batch reservation atomicity, duplicate conflict behavior, and payload privacy guardrails  
Target phase: P3-21  
Depends on: P3-08 through P3-20 sync processor packages

---

## 1. Purpose

P3-21 pauses new handler expansion and hardens the integrated sync processor.

The sync processor already handles:

- patient/create;
- patient_visit/create;
- service_encounter/create;
- vital_signs/create;
- form_response/create;
- consent_document/create;
- medical_referral/create;
- medication_delivery/create.

This package protects the cross-handler behavior that can fail only when multiple event types are processed together.

---

## 2. Topological order contract

The processor must keep this create ordering:

1. patient;
2. patient_visit;
3. service_encounter;
4. vital_signs;
5. form_response;
6. consent_document;
7. medical_referral;
8. medication_delivery;
9. fallback unsupported events.

This ordering exists because downstream entities may reference upstream entities staged earlier in the same DbContext.

---

## 3. Pending-batch reservation atomicity

Multi-key pending-batch reservations must be atomic.

Rules:

- ServiceEncounter encounter folio and visit-service keys must be reserved only after successful ServiceEncounter construction and reserved atomically.
- If ServiceEncounter visit-service key reservation fails, the encounter folio reservation must be rolled back.
- FormResponse id and encounter-template keys must be reserved only after successful FormResponse construction and reserved atomically.
- If FormResponse encounter-template key reservation fails, the form response id reservation must be rolled back.
- ConsentDocument id and patient-visit-type-version key reservations must remain atomic.
- MedicalReferral id and referral folio reservations must remain atomic.
- Single-key reservations such as MedicationDelivery id must happen only after successful entity construction and required domain transition.

---

## 4. Duplicate behavior

Duplicate checks must mark the event as conflict before SaveChangesAsync.

Rules:

- Duplicate pending-batch keys must never rely on database unique-key exceptions.
- Global primary key uniqueness must be checked globally when the database primary key is global.
- Tenant-scoped unique keys must match the actual database index shape.
- Soft-deleted rows must be included in duplicate checks when the database unique index is not soft-delete filtered.

---

## 5. Payload privacy

Rules:

- processor results must not expose PayloadJson;
- event list read models must not materialize PayloadJson for status flags;
- handler rejection/conflict reasons must not echo raw clinical JSON;
- signatures, consent text, referral reasons, medication instructions, and form response JSON must not be logged or echoed in process results.

---

## 6. Acceptance criteria

P3-21 is complete when:

- SyncBatchProcessor keeps the topological order contract;
- ServiceEncounter pending-batch reservations are atomic with rollback;
- FormResponse pending-batch reservations are atomic with rollback;
- ConsentDocument atomic reservation contract remains protected;
- MedicalReferral atomic reservation contract remains protected;
- MedicationDelivery global id duplicate behavior remains protected;
- P3 sync processor contract tests do not contain implicitly typed empty arrays;
- repository governance and database deployment gates remain green.
---

## 7. P3-22B component extraction note

P3-22B moves topological ordering into SyncProcessingOrder and pending-batch reservation sets into PendingBatchReservationState without changing behavior.
