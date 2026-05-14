# P3 Sync Process Endpoint API Regression Baseline

Status: active
Scope: API-level integration regression for sync batch processing
Target phase: P3-24A
Depends on: P3-23E failed batch processor regression

---

## 1. Purpose

P3-24A moves sync validation from processor-level coverage to API-level coverage.

The processor has already been validated directly. This baseline validates that the real HTTP endpoint is correctly wired to authentication, authorization, tenant route data, dependency injection, SyncBatchProcessor, ApiResponse, and persistence.

---

## 2. Endpoint under test

The API-level test targets:

POST /api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}/process

The endpoint must require PermissionCodes.SyncBatchesWrite.

---

## 3. Required regressions

The integration test must include:

1. an unauthenticated request returning 401 Unauthorized;
2. an authenticated request with sync-batches.write reaching the endpoint;
3. a pending SyncBatch seeded in CaritasDbContext;
4. one pending patient SyncEvent;
5. successful processing through the HTTP endpoint;
6. persisted Patient and accepted SyncEvent after the HTTP request.

---

## 4. Required assertions

The successful endpoint test must assert:

- HTTP 200 OK;
- ApiResponse success equals true;
- API message equals Sync batch processed successfully.;
- completed equals true;
- PendingEventsProcessed equals 1;
- AcceptedCount equals 1;
- RejectedCount equals 0;
- ConflictCount equals 0;
- response batch status equals completed;
- one Patient is persisted;
- one SyncEvent is persisted;
- the SyncEvent status is accepted;
- SyncBatch status is completed.

---

## 5. Technical debt cleanup

P3-24A removes stale skeleton wording from SyncBatchesController.

The endpoint must not claim that it is a skeleton processor and must not claim that clinical writes are not applied.

---

## 6. Non-goals

P3-24A does not replace processor-level E2E tests.

P3-24A does not test the full eight-event clinical flow through HTTP.

P3-24A does not add SQL Server-specific integration testing.

---

## 7. Acceptance criteria

P3-24A is complete when:

- P3SyncProcessEndpointIntegrationTests exists;
- the unauthorized endpoint regression exists;
- the authenticated process endpoint regression exists;
- the test uses WebApplicationFactory;
- the test uses Development authentication headers;
- the test uses in-memory CaritasDbContext;
- the endpoint response is validated as ApiResponse JSON;
- stale skeleton wording is removed;
- dotnet build and dotnet test pass.