# P3 Sync List Events Endpoint API Regression Baseline

Status: active
Scope: API-level integration regression for sync event listing privacy
Target phase: P3-24C
Depends on: P3-24B sync create batch endpoint API regression

---

## 1. Purpose

P3-24C validates the real HTTP sync event listing endpoint.

The goal is to prove that sync event metadata can be inspected by authorized users without exposing PayloadJson or sensitive clinical payload content.

---

## 2. Endpoint under test

The API-level test targets:

GET /api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}/events

The endpoint must require PermissionCodes.SyncBatchesRead.

---

## 3. Required regressions

The integration test must include:

1. an unauthenticated request returning 401 Unauthorized;
2. an authenticated request with sync-batches.read returning event metadata;
3. a seeded SyncBatch and SyncEvent containing sensitive PayloadJson;
4. HTTP 200 OK;
5. ApiResponse with PaginatedResponse items;
6. response body must not contain payloadJson;
7. response body must not contain sensitive payload values;
8. tenant mismatch returns 404 NotFound without leaking payload content.

---

## 4. Required assertions

The successful endpoint test must assert:

- HTTP 200 OK;
- ApiResponse success equals true;
- pageNumber equals 1;
- pageSize equals 10;
- totalCount equals 1;
- one item is returned;
- item syncBatchId matches;
- item organizationId matches;
- item localEventId matches;
- item entityType matches;
- item operation matches;
- item status equals pending;
- item isPending equals true;
- item does not contain payloadJson;
- item does not contain payload;
- raw response body does not contain sensitive payload values.

---

## 5. Privacy rule

SyncEvent.PayloadJson is internal processing data.

List-events API responses must use SyncEventSummaryDto and must never expose PayloadJson, raw payload, patient names, phone numbers, or clinical form body data.

---

## 6. Tenant boundary rule

If the route organizationId does not own the sync batch, the endpoint must return 404 NotFound and must not leak event content.

---

## 7. Non-goals

P3-24C does not process sync events.

P3-24C does not test every field of SyncEventSummaryDto.

P3-24C does not add SQL Server-specific integration testing.

---

## 8. Acceptance criteria

P3-24C is complete when:

- P3SyncListEventsEndpointIntegrationTests exists;
- the unauthorized list events regression exists;
- the authenticated list events regression exists;
- the tenant mismatch 404 regression exists;
- the response is validated to exclude PayloadJson;
- sensitive payload values are not present in the response body;
- the test uses WebApplicationFactory;
- the test uses Development authentication headers;
- the test uses in-memory CaritasDbContext;
- the test registers ISyncBatchReadRepository;
- dotnet build and dotnet test pass.