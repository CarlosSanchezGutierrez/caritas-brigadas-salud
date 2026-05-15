# P3 Offline Sync and Conflict Policy Baseline

Status: active  
Scope: backend offline sync, conflict handling, tenant boundary, device policy, retries, payload governance, and operational evidence  
Target phase: P3-07  
Depends on: P3 clinical business rules, P3 data governance, P3 operational access matrix, P3 vital signs model, P3 clinical record read model

---

## 1. Purpose

This document defines the offline sync and conflict policy before implementing deeper sync processing behavior.

The platform must support field brigades, office capture, intermittent internet, iPad/iPhone/Android/Web clients, and future analytics without weakening privacy, tenant scope, or clinical data integrity.

P3-07 is a policy and governance baseline. It does not implement the sync processor.

---

## 2. Current sync model

The backend already has core sync concepts:

| Concept | Purpose |
|---|---|
| SyncBatch | Groups one sync attempt from a user/device/organization/brigade context. |
| SyncEvent | Represents one entity operation inside a sync batch. |
| LocalEventId | Client-generated idempotency key inside the device/local queue. |
| EntityType | The target domain entity type for the event. |
| EntityId | Optional server entity id or known entity id. |
| Operation | Create, update, void, sign, or sync operation. |
| PayloadJson | Event payload submitted by the client. |
| Status | Processing status such as pending, accepted, rejected, or conflict. |
| ConflictReason | Human-readable conflict explanation for review. |

Rules:

- SyncBatch belongs to one OrganizationId.
- SyncEvent belongs to one OrganizationId.
- SyncEvent belongs to one SyncBatch.
- OrganizationId must not be inferred only from payload JSON.
- OrganizationId from route/auth context must match batch and event OrganizationId.
- PayloadJson must be treated as untrusted input until validated.

---

## 3. Online/offline mode policy

The system should not constantly waste bandwidth trying to sync without control.

Recommended client behavior:

- support explicit Online mode;
- support explicit Offline mode;
- support Sync now action;
- support automatic retry with bounded backoff when enabled;
- show pending, failed, and conflict counts clearly;
- avoid infinite busy loops;
- avoid aggressive polling on unstable connections.

Backend rules:

- backend must accept batches idempotently;
- backend must reject tenant-mismatched payloads;
- backend must expose enough status for clients to show pending/errors/conflicts;
- backend must not require clients to be always online to preserve clinical workflow.

---

## 4. Tenant boundary rules

Offline sync must never bypass tenant isolation.

Rules:

- actor OrganizationId is authoritative.
- SyncBatch.OrganizationId must match actor OrganizationId unless a global-only system operation explicitly allows otherwise.
- SyncEvent.OrganizationId must match SyncBatch.OrganizationId.
- payload OrganizationId must match SyncEvent.OrganizationId when present.
- patient, visit, encounter, vital signs, form response, consent, referral, and medication payloads must validate related entity ownership.
- cross-tenant entity references must be rejected, not silently corrected.
- conflicts must not expose data from another organization.

---

## 5. Device policy

DeviceId remains policy-sensitive.

Current baseline:

- DeviceId can be nullable.
- DeviceId must not be Guid.Empty when provided.
- DeviceId strong FK policy remains deferred for offline/revoked/not-yet-synced devices.
- DeviceId must not become a tenant bypass.
- Device trust must be evaluated separately from user trust.

Future device states:

| State | Meaning |
|---|---|
| Registered | Device is known by the organization. |
| Active | Device can submit sync batches. |
| Suspended | Device cannot submit new batches but historical data remains. |
| Revoked | Device is no longer trusted. |
| Unknown | Device is not yet registered or cannot be verified. |

Rules:

- revoked devices should not submit new accepted clinical data unless an explicit recovery policy exists.
- unknown devices may be rejected, quarantined, or accepted with review depending on later product policy.
- device identity must not replace user authorization.

---

## 6. Idempotency policy

Sync must be idempotent across retries, including retries that create a new SyncBatch.

Rules:

- LocalEventId must be stable across client retries for the same offline event.
- LocalEventId idempotency scope must exist outside a single SyncBatch.
- per-batch idempotency scope is not allowed as the only duplicate-prevention mechanism.
- preferred scope when DeviceId is present: OrganizationId + DeviceId + LocalEventId.
- fallback scope when DeviceId is null must still be outside a single batch, such as OrganizationId + UserId + LocalEventId + client installation key or another approved client identity key.
- fallback scope must be documented before sync processor implementation.
- duplicate LocalEventId submissions within the approved idempotency scope must not create duplicate clinical records.
- duplicate accepted events should return consistent status.
- retrying the same offline event in a new batch should not duplicate patients, visits, encounters, vital signs, documents, forms, referrals, or medication deliveries.
- server-generated ids must be returned or traceable after acceptance.
- accepted events must not later become different records because of retry order.

Required future contract:

- unique or logical idempotency constraint outside SyncBatchId-only scope;
- deterministic duplicate detection across batch retries;
- explicit duplicate result behavior;
- explicit fallback idempotency behavior when DeviceId is null.

---

## 7. Ordering policy

Offline events may arrive out of order.

Rules:

- parent records must exist before child records are accepted.
- patient must exist before visit.
- visit must exist before encounter.
- visit must exist before vital signs.
- encounter must exist before form response when form response requires encounter.
- patient must exist before consent document.
- encounter must exist before referral or medication delivery.
- missing parent references should be rejected or marked conflict based on event type.
- processing order must be deterministic.

---

## 8. Conflict policy

Conflicts must be explicit.

Conflict examples:

- payload references another OrganizationId;
- referenced Patient does not exist;
- referenced Visit does not exist;
- referenced Encounter does not exist;
- record was already completed or voided;
- update attempts to overwrite immutable clinical history;
- signed document is being replaced without correction workflow;
- duplicate patient candidate requires human review;
- stale client update conflicts with newer server version;
- revoked device attempts to submit data.

Rules:

- conflicts must preserve the submitted event.
- conflict reason must be understandable.
- conflict resolution must be auditable.
- conflicts must not expose other-tenant details.
- automatic merge is allowed only for approved low-risk fields.
- clinical history must not be silently overwritten.

---

## 9. Accepted, rejected, and conflict semantics

| Result | Meaning |
|---|---|
| Accepted | Event passed validation and was applied or recognized as idempotent duplicate. |
| Rejected | Event is invalid and should not be retried without correction. |
| Conflict | Event needs review or conflict-resolution workflow. |
| Processing | Event is being processed. |
| Pending | Event has not been processed. |

Rules:

- invalid schema should be rejected.
- tenant mismatch should be rejected.
- missing required parent may be conflict or rejected depending on whether parent may still arrive.
- stale update should usually be conflict.
- duplicate accepted event should not be rejected.
- rejected events should include safe error messages only.

---

## 10. Payload governance

PayloadJson is sensitive and untrusted.

Rules:

- payload schema must be validated per EntityType and Operation.
- payload must not be logged raw.
- payload must not include unnecessary direct identifiers.
- payload must not be sent to analytics directly.
- payload must not be sent to LLMs.
- payload must be redacted in diagnostics.
- payload storage retention must be reviewed later.

EntityType allowlist must be explicit before processing.

Candidate allowed entity types:

- patient;
- patient_visit;
- service_encounter;
- vital_signs;
- form_response;
- consent_document;
- medical_referral;
- medication_delivery;
- media_release.

---

## 11. Clinical data rules

Offline clinical data must follow the same rules as online clinical data.

Rules:

- vital signs remain historical records.
- systolic and diastolic blood pressure remain separate mmHg fields.
- temperature remains TemperatureCelsius.
- oxygen saturation remains percent.
- form response raw JSON must not be exposed in general clinical record views.
- signature data must not be exposed in general clinical record views.
- clinical corrections must be auditable.
- completed or signed records need explicit correction workflow.

---

## 12. Office capture and central review workflow

Office users may help clean data after brigades.

Expected workflow:

- review pending sync batches;
- review rejected or conflict events;
- correct administrative fields when allowed;
- merge duplicate patient candidates only after approved rules;
- request medical review for clinical conflicts;
- avoid exposing full clinical history to office capturers unless explicitly needed.

Rules:

- office correction must be tenant-scoped.
- office correction must be audited.
- office correction must not silently overwrite clinical measurements.
- office correction must not bypass medical permissions.

---

## 13. Analytics and data engineering implications

Sync data supports operational intelligence.

Useful metrics:

- pending sync batches;
- failed sync batches;
- conflict count;
- rejected event count;
- accepted event count;
- average sync delay;
- device error rate;
- entity type with most conflicts;
- brigade sync quality;
- data quality issue count.

Rules:

- analytics should use aggregated sync metrics by default.
- raw payloads must not be analytics source.
- patient-level sync errors must be protected.
- sync conflict reports must be tenant-scoped.

---

## 14. Security policy

Sync endpoints are high-risk.

Rules:

- require authentication;
- require PermissionCodes-based authorization;
- require tenant scope;
- apply request size limits;
- apply rate limits;
- validate JSON schema;
- reject unknown entity types;
- reject unknown operations;
- avoid raw payload logging;
- return safe error messages;
- audit sensitive failures;
- protect against replay using idempotency keys.

Future hardening:

- per-device throttling;
- per-user throttling;
- batch size limit;
- event count limit;
- payload byte limit;
- checksum/hash validation;
- replay window policy.

---

## 15. Observability policy

Observability must be useful without leaking sensitive data.

Allowed telemetry:

- batch id;
- organization id;
- device id when allowed;
- user id;
- entity type;
- operation;
- status;
- counters;
- durations;
- safe error codes.

Not allowed by default:

- raw PayloadJson;
- patient names;
- phone numbers;
- CURP;
- signatures;
- document text snapshots;
- clinical free text;
- raw form JSON.

---

## 16. Developer and testing policy

Developers must test sync with synthetic data.

Required future tests:

- duplicate event idempotency;
- tenant mismatch rejection;
- payload OrganizationId mismatch rejection;
- missing parent conflict;
- accepted duplicate behavior;
- rejected invalid schema behavior;
- conflict reason redaction;
- no raw payload logs;
- device id empty rejection;
- batch counter integrity.

---

## 17. Explicitly out of scope for P3-07

P3-07 does not implement:

- sync processor;
- sync endpoint changes;
- device registry;
- conflict resolution UI;
- client offline queue;
- mobile encryption;
- production retry policy;
- analytics dashboard;
- raw payload retention policy.

Those belong to later P3/P4 packages.

---

## 18. Acceptance criteria

P3-07 is complete when:

- this offline sync and conflict policy baseline exists;
- a verifier protects required sections;
- repository governance gate validates it;
- database and security gates remain green;
- future sync processor work can be implemented without inventing tenant, conflict, idempotency, or payload rules.