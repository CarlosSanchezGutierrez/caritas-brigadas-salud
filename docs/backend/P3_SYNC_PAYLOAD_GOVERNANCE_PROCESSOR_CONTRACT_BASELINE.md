# P3 Sync Payload Governance and Processor Contract Baseline

Status: active  
Scope: sync payload governance, entity type allowlist, operation allowlist, processor guardrails, safe diagnostics, and contract tests  
Target phase: P3-09  
Depends on: P3 Offline Sync and Conflict Policy Baseline, P3 Sync Idempotency Guardrails

---

## 1. Purpose

This document defines the minimum guardrails required before implementing the real sync processor.

The sync processor must not accept arbitrary entity types, arbitrary operations, raw unvalidated payloads, tenant-mismatched records, unsafe diagnostics, or duplicate offline events.

---

## 2. Non-negotiable rules

Rules:

- PayloadJson is sensitive and untrusted.
- EntityType must come from an explicit allowlist.
- Operation must come from an explicit allowlist.
- unknown EntityType must be rejected.
- unknown Operation must be rejected.
- raw PayloadJson must not be logged.
- raw PayloadJson must not be sent to analytics.
- raw PayloadJson must not be sent to LLM systems.
- payload OrganizationId must match SyncEvent.OrganizationId when present.
- SyncEvent.OrganizationId must match SyncBatch.OrganizationId.
- idempotency must use OrganizationId + IdempotencyKey outside a single SyncBatch.
- duplicate accepted events must not create duplicate clinical records.

---

## 3. Allowed EntityType values

Allowed values:

| EntityType | Meaning |
|---|---|
| patient | Patient identity or administrative record. |
| patient_visit | Patient visit to brigade or care event. |
| service_encounter | Service delivered during a visit. |
| vital_signs | Historical vital signs record. |
| form_response | Structured form response metadata and payload. |
| consent_document | Consent evidence metadata. |
| document_signature | Signed document evidence metadata. |
| medical_referral | Clinical referral need. |
| medication_delivery | Medication delivery record. |
| media_release | Media consent/release record. |

No other EntityType may be processed without updating this baseline, the allowlist, and contract tests.

---

## 4. Allowed Operation values

Allowed values:

| Operation | Meaning |
|---|---|
| create | Create a new entity. |
| update | Update allowed mutable fields. |
| void | Void/cancel allowed records with audit trail. |
| sign | Capture signature/document evidence. |
| sync | System sync/status operation. |

No other Operation may be processed without updating this baseline, the allowlist, and contract tests.

---

## 5. Processor contract expectations

The future sync processor must:

- load SyncBatch;
- validate actor organization;
- validate SyncBatch.OrganizationId;
- validate SyncEvent.OrganizationId;
- validate EntityType allowlist;
- validate Operation allowlist;
- validate payload schema by EntityType and Operation;
- validate tenant ownership of referenced records;
- validate idempotency before applying changes;
- mark duplicate accepted events without duplicating records;
- reject unknown entity types;
- reject unknown operations;
- mark conflicts for recoverable ordering or stale update issues;
- reject tenant mismatch;
- avoid raw payload logging;
- record safe error codes and conflict reasons.

---

## 6. Payload validation expectations

Payload validation must be entity-specific.

Minimum future validators:

- patient payload validator;
- patient_visit payload validator;
- service_encounter payload validator;
- vital_signs payload validator;
- form_response payload validator;
- consent_document payload validator;
- document_signature payload validator;
- medical_referral payload validator;
- medication_delivery payload validator;
- media_release payload validator.

Rules:

- free-form JSON must not be persisted directly into domain tables without validation;
- unknown JSON fields must be ignored or rejected according to explicit policy;
- required fields must be validated;
- tenant fields must be cross-checked;
- clinical units must be canonical.

---

## 7. Safe diagnostics

Allowed diagnostics:

- SyncBatchId;
- SyncEventId;
- OrganizationId;
- DeviceId if allowed;
- UserId if allowed;
- EntityType;
- Operation;
- Status;
- safe error code;
- safe conflict code;
- duration;
- retry count.

Forbidden diagnostics by default:

- raw PayloadJson;
- patient names;
- phone numbers;
- CURP;
- signatures;
- document image data;
- clinical free text;
- raw form response JSON;
- clinical justification text.

---

## 8. Acceptance criteria

P3-09 is complete when:

- allowed EntityType values exist in code;
- allowed Operation values exist in code;
- SyncEvent rejects unknown EntityType;
- SyncEvent rejects unknown Operation;
- contract tests verify allowlists;
- contract tests verify PayloadJson is treated as sensitive in policy;
- repository governance and database deployment gates remain green.