# P3 Sync Backend Readiness Checklist

Status: active
Scope: backend sync readiness closure
Target phase: P3-25A
Depends on: P3-24D sync tenant boundary endpoint API regression

---

## 1. Purpose

P3-25A closes the backend sync workstream as a verifiable technical package.

This checklist confirms that the offline sync backend is covered across processor behavior, HTTP endpoint wiring, privacy, tenant isolation, governance, and CI gates.

---

## 2. Readiness status

Backend sync readiness status: ready for next backend workstream.

This does not mean the entire product is finished.

It means the sync backend core has reached a controlled, documented, tested, and governance-protected state for the current P3 scope.

---

## 3. Processor-level coverage closed

The processor-level backend sync scope includes:

- complete clinical offline batch processing;
- same-batch patient to visit stable GUID linkage;
- out-of-order event processing;
- controlled conflict without aborting the batch;
- invalid payload rejection without aborting the batch;
- already completed batch idempotency;
- failed batch terminal behavior;
- direct handler dispatch;
- pending batch reservation safety;
- zero technical debt sync processor guard.

---

## 4. API-level coverage closed

The API-level backend sync scope includes:

- create sync batch endpoint;
- process sync batch endpoint;
- list sync events endpoint;
- GET by id tenant boundary;
- process tenant boundary;
- unauthenticated request rejection;
- development-auth integration testing;
- permission-specific read/write endpoint access;
- in-memory integration testing without SQL Server provider leakage.

---

## 5. Privacy coverage closed

The sync privacy scope includes:

- SyncEvent.PayloadJson remains internal processing data;
- list-events API does not expose payloadJson;
- list-events API does not expose raw payload;
- list-events API does not expose sensitive patient names;
- list-events API does not expose sensitive phone values;
- cross-tenant error responses do not leak payload content.

---

## 6. Tenant boundary coverage closed

The sync tenant boundary scope includes:

- organization route ownership validation;
- cross-tenant GET by id returns 404 NotFound;
- cross-tenant list events returns 404 NotFound;
- cross-tenant process returns 404 NotFound;
- cross-tenant process does not create Patient rows;
- cross-tenant process does not mutate original SyncBatch counters;
- cross-tenant process leaves original SyncEvent pending.

---

## 7. Governance and CI coverage closed

The governance scope includes:

- repository governance baseline validation;
- database deployment baseline validation;
- sync compatibility governance validation;
- zero technical debt sync processor validation;
- dependency review REST retry hardening;
- dotnet build with warnaserror;
- dotnet test with warnaserror;
- git diff whitespace validation.

---

## 8. Required evidence files

The following evidence files must exist:

- P3 clinical sync end-to-end integration test;
- P3 sync process endpoint integration test;
- P3 sync create batch endpoint integration test;
- P3 sync list events endpoint integration test;
- P3 sync tenant boundary endpoint integration test;
- P3 sync processor zero technical debt baseline;
- P3 sync endpoint API regression baselines;
- P3 sync compatibility governance verifier;
- P3 backend sync readiness verifier.

---

## 9. Explicit non-goals

P3-25A does not claim that every future sync feature is complete.

P3-25A does not claim production infrastructure is fully deployed.

P3-25A does not replace SQL Server migration validation.

P3-25A does not replace mobile, iOS, Android, or frontend client testing.

P3-25A does not replace real staging environment smoke tests.

---

## 10. Next backend workstreams

After P3-25A, the next backend workstream should move away from sync internals and into one of these tracks:

1. production deployment readiness;
2. authentication and authorization hardening beyond development headers;
3. audit logging API-level validation;
4. reporting/export endpoints;
5. SQL Server integration smoke tests;
6. operational observability baseline;
7. frontend/mobile contract alignment.

---

## 11. Acceptance criteria

P3-25A is complete when:

- this readiness checklist exists;
- the readiness verifier exists;
- the readiness contract test exists;
- all required sync processor regression files exist;
- all required sync API regression files exist;
- governance validation includes the readiness verifier;
- dotnet build and dotnet test pass.
---

## 12. P3-26A production deployment readiness note

P3-26A moves the project from backend sync readiness into production deployment readiness governance.

Production go-live remains blocked until P3-26B authentication hardening and P3-26C SQL Server integration smoke testing are complete.
