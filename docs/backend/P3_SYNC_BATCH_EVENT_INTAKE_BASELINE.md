# P3 Sync Batch Event Intake Baseline

Status: active  
Scope: sync batch intake, payload envelope, pending SyncEvent creation, cross-batch idempotency, duplicate suppression, and safe processor staging  
Target phase: P3-10  
Depends on: P3 sync idempotency guardrails, P3 sync payload governance contracts

---

## 1. Purpose

P3-10 defines and implements the first safe intake layer for offline sync batches.

The backend may receive a sync batch and create pending SyncEvent records, but it must not yet apply clinical changes to patient, visit, encounter, vital signs, forms, documents, referrals, or medication tables.

---

## 2. Payload envelope

Accepted payload shape, shown as plain text to avoid Markdown fence drift:

    {
      "events": [
        {
          "localEventId": "device-local-event-001",
          "entityType": "vital_signs",
          "operation": "create",
          "entityId": null,
          "createdAtDevice": "2026-05-13T00:00:00Z",
          "payload": {
            "example": true
          }
        }
      ]
    }

Arrays are also accepted as direct event lists.

Rules:

- payload must contain events or items array;
- each event must have localEventId;
- each event must have entityType;
- each event must have operation;
- each event must have payload;
- entityId is optional but must be a valid GUID when provided;
- createdAtDevice is optional but must be a valid date/time when provided.

---

## 3. Idempotency behavior

Rules:

- SyncEvent.IdempotencyKey must be generated outside SyncBatchId-only scope.
- preferred scope: OrganizationId + DeviceId + LocalEventId.
- fallback scope: OrganizationId + UserId + BrigadeId + ClientInstanceId + LocalEventId.
- ClientInstanceId is required when DeviceId is not provided.
- duplicate idempotency keys inside the same payload must be rejected.
- duplicate idempotency keys already stored for the same organization must not create new SyncEvent records.
- duplicate retry batches may still create SyncBatch evidence, but must not duplicate SyncEvent rows.
- EventsCount is advisory input only and must match the server-parsed payload event count when provided.
- SyncBatch.EventsCount must be persisted from the server-parsed payload event count, not blindly from client input.

---

## 4. Safe staging

P3-10 only stages events.

Allowed:

- create SyncBatch;
- parse payload envelope;
- create pending SyncEvent rows;
- suppress duplicate SyncEvent rows by OrganizationId + IdempotencyKey;
- validate entity type and operation through SyncEvent allowlists.

Not allowed:

- create Patient from sync payload;
- create PatientVisit from sync payload;
- create ServiceEncounter from sync payload;
- create VitalSignsRecord from sync payload;
- create FormResponse from sync payload;
- create ConsentDocument from sync payload;
- create MedicalReferral from sync payload;
- create MedicationDelivery from sync payload;
- log raw PayloadJson;
- send raw PayloadJson to analytics or AI.

---

## 5. Future processor handoff

A future processor must load pending SyncEvent rows and apply entity-specific validators.

The future processor must still validate:

- actor organization;
- SyncBatch.OrganizationId;
- SyncEvent.OrganizationId;
- EntityType;
- Operation;
- payload schema;
- referenced entity tenant ownership;
- ordering dependencies;
- conflict rules;
- duplicate accepted behavior.

---

## 6. Acceptance criteria

P3-10 is complete when:

- CreateSyncBatchRequest supports ClientInstanceId fallback.
- SyncBatchWriteRepository parses payload events.
- SyncBatchWriteRepository rejects EventsCount mismatches when the client-provided count differs from parsed payload events.
- SyncBatchWriteRepository creates pending SyncEvent rows.
- SyncBatchWriteRepository suppresses cross-batch duplicate events by OrganizationId + IdempotencyKey.
- SyncBatchWriteRepository rejects duplicate idempotency keys inside one payload.
- SyncBatchWriteRepository does not apply clinical domain writes.
- tests protect the intake behavior.