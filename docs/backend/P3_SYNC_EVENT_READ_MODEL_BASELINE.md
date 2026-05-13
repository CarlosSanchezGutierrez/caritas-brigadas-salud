# P3 Sync Event Read Model Baseline

Status: active  
Scope: sync event read model, intake evidence, tenant-scoped batch event visibility, safe diagnostics, and PayloadJson exclusion  
Target phase: P3-11  
Depends on: P3 sync batch event intake, P3 sync payload governance, P3 sync idempotency guardrails

---

## 1. Purpose

P3-11 adds safe visibility into SyncEvent rows created during sync batch intake.

The purpose is to let administrators, support users, audit reviewers, and future processor tooling inspect which events were staged without exposing raw PayloadJson.

---

## 2. Endpoint

Required endpoint:

GET /api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}/events

Rules:

- endpoint must be tenant-scoped by OrganizationId;
- endpoint must require SyncBatchesRead permission;
- endpoint must confirm the SyncBatch belongs to the organization;
- endpoint must return only SyncEvent rows for that organization and batch;
- endpoint must not expose PayloadJson.

---

## 3. DTO rules

SyncEventSummaryDto may expose:

- Id;
- SyncBatchId;
- OrganizationId;
- LocalEventId;
- IdempotencyKey;
- EntityType;
- EntityId;
- Operation;
- Status;
- ErrorMessage;
- ConflictReason;
- CreatedAtDevice;
- ReceivedAtServer;
- ProcessedAt;
- safe status booleans.

SyncEventSummaryDto must not expose:

- PayloadJson;
- raw clinical payload;
- patient names from payload;
- signatures;
- raw form response JSON;
- document binary data.

---

## 4. Repository rules

ISyncBatchReadRepository must expose ListEventsByBatchAsync.

SyncBatchReadRepository must:

- query SyncEvents by OrganizationId and SyncBatchId;
- use AsNoTracking;
- paginate results;
- order results deterministically;
- map to SyncEventSummaryDto;
- exclude PayloadJson.

---

## 5. Acceptance criteria

P3-11 is complete when:

- SyncEventSummaryDto exists and excludes PayloadJson;
- ISyncBatchReadRepository exposes ListEventsByBatchAsync;
- SyncBatchReadRepository implements tenant-scoped event listing;
- SyncBatchesController exposes the read endpoint;
- contract tests protect PayloadJson exclusion;
- repository governance and database deployment gates remain green.