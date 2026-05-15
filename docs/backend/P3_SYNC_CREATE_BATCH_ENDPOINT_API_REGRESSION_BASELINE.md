# P3 Sync Create Batch Endpoint API Regression Baseline

Status: active
Scope: API-level integration regression for sync batch intake
Target phase: P3-24B
Depends on: P3-24A sync process endpoint API regression

---

## 1. Purpose

P3-24B validates the real HTTP sync batch intake endpoint.

This moves beyond processor-level validation and verifies that mobile/web clients can submit a sync batch through the API boundary.

---

## 2. Endpoint under test

The API-level test targets:

POST /api/v1/organizations/{organizationId}/sync-batches

The endpoint must require PermissionCodes.SyncBatchesWrite.

---

## 3. Required regressions

The integration test must include:

1. an unauthenticated request returning 401 Unauthorized;
2. an authenticated request with sync-batches.write reaching the endpoint;
3. seeded Organization, User, and Brigade;
4. a CreateSyncBatchRequest with one patient create event inside PayloadJson;
5. HTTP 201 Created;
6. ApiResponse message Sync batch received successfully.;
7. persisted SyncBatch with status received;
8. persisted SyncEvent with status pending;
9. zero Patient rows because create endpoint only receives the batch and does not process it.

---

## 4. Required assertions

The successful endpoint test must assert:

- HTTP 201 Created;
- Location header is present;
- ApiResponse success equals true;
- API message equals Sync batch received successfully.;
- response batch status equals received;
- response batch isCompleted equals false;
- response batch eventsCount equals 1;
- one SyncBatch is persisted;
- one SyncEvent is persisted;
- zero Patient rows exist;
- SyncBatch counters are zero;
- SyncEvent status is pending;
- SyncEvent idempotency key includes organization, device, and local event id.

---

## 5. Architecture rule

Create sync batch endpoint is intake only.

It must persist the batch and events, but it must not apply clinical writes. Processing belongs to the process endpoint.

---

## 6. Non-goals

P3-24B does not process the batch.

P3-24B does not test the full eight-event clinical flow through HTTP.

P3-24B does not add SQL Server-specific integration testing.

---

## 7. Acceptance criteria

P3-24B is complete when:

- P3SyncCreateBatchEndpointIntegrationTests exists;
- the unauthorized create endpoint regression exists;
- the authenticated create endpoint regression exists;
- the test uses WebApplicationFactory;
- the test uses Development authentication headers;
- the test uses in-memory CaritasDbContext;
- the test registers ISyncBatchWriteRepository;
- the endpoint response is validated as ApiResponse JSON;
- dotnet build and dotnet test pass.
---

## 8. P3-24C list events endpoint API regression note

P3-24C validates that the HTTP sync event listing route exposes metadata only and does not leak PayloadJson.
