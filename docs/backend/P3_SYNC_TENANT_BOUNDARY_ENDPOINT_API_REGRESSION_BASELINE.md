# P3 Sync Tenant Boundary Endpoint API Regression Baseline

Status: active
Scope: API-level integration regression for sync tenant boundary
Target phase: P3-24D
Depends on: P3-24C sync list events endpoint privacy regression

---

## 1. Purpose

P3-24D validates the HTTP tenant boundary for sync batch endpoints.

An authenticated user from one organization must not be able to inspect or process sync batches owned by another organization.

---

## 2. Endpoints under test

The API-level tests target:

GET /api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}

POST /api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}/process

The GET endpoint must require PermissionCodes.SyncBatchesRead.

The process endpoint must require PermissionCodes.SyncBatchesWrite.

---

## 3. Required regressions

The integration test must include:

1. a sync batch owned by organization A;
2. a request through organization B route;
3. GET by id returns 404 NotFound;
4. POST process returns 404 NotFound;
5. no clinical rows are created;
6. the original SyncBatch remains received;
7. the original SyncEvent remains pending;
8. response bodies do not leak PayloadJson or sensitive payload content.

---

## 4. Required assertions

The GET by id tenant mismatch test must assert:

- HTTP 404 NotFound;
- response contains Sync batch was not found.;
- response does not contain the syncBatchId;
- response does not contain sensitive payload values;
- response does not contain payloadJson.

The process tenant mismatch test must assert:

- HTTP 404 NotFound;
- response contains Sync batch was not found.;
- response does not contain sensitive payload values;
- response does not contain payloadJson;
- Patient count remains 0;
- SyncBatch status remains received;
- SyncBatch counters remain zero;
- SyncEvent status remains pending.

---

## 5. Tenant boundary rule

Sync APIs must behave as if cross-tenant resources do not exist.

They must return 404 NotFound, not 403 Forbidden with resource hints, and not leak identifiers, PayloadJson, patient names, phone numbers, or clinical content.

---

## 6. Non-goals

P3-24D does not test every sync endpoint.

P3-24D does not add SQL Server-specific integration testing.

P3-24D does not change production code unless a tenant boundary bug is discovered.

---

## 7. Acceptance criteria

P3-24D is complete when:

- P3SyncTenantBoundaryEndpointIntegrationTests exists;
- GET by id tenant mismatch regression exists;
- process tenant mismatch regression exists;
- cross-tenant responses are 404;
- cross-tenant responses do not leak payload content;
- the process mismatch test does not create Patient rows;
- the original batch and event remain unchanged;
- dotnet build and dotnet test pass.