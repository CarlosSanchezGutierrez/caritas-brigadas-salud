# Technical Decisions

This document captures project-level decisions that should not be re-litigated without a clear reason.

## Backend

- ASP.NET Core backend.
- Layered architecture.
- Contracts project for DTOs.
- Domain project for entities.
- Infrastructure project for persistence and repositories.
- API project for controllers and filters.

## Database

- SQL Server is the target operational database.
- `ConnectionStrings__SqlServer` is the required runtime connection string key.
- `ConnectionStrings__CaritasDatabase` is legacy/forbidden for current production-like configuration.
- EF migrations are used.
- SQL Server baseline scripts are maintained for controlled deployment evidence.

## Patient module

P5 Patient Backend Module is closed as a controlled milestone.

Important patient capabilities:

- Offline/source metadata.
- Organization-scoped patient reads and writes.
- Patient create idempotency.
- SQL Server unique filtered index backstop.
- Exact violated-index replay handling.
- Patient longitudinal timeline.
- Patient write audit evidence.

Important patient idempotency indexes:

- `IX_patients_OrganizationId_ClientOperationId_UQ`
- `IX_patients_OrganizationId_IdempotencyKey_UQ`
- `IX_patients_OrganizationId_SourceBrigadeId_LocalPatientId_UQ`

The non-unique snapshot entries for `ClientOperationId` and `IdempotencyKey` must not remain in the EF snapshot after P5.9.1, because the migration explicitly drops them and replaces them with filtered unique indexes.

## Audit

Audit evidence is part of backend trust.

Patient writes must map to clinical audit action codes and preserve:

- organization id
- actor/user context when available
- entity id
- route/action
- correlation metadata
- timestamps
- request metadata where supported

## Guardrail routes

The API guardrail routes are:

- `/health/live`
- `/health/ready`
- `/openapi/v1/openapi.json`
- `/swagger`

## Mobile / frontend boundary

- Mobile and frontend clients must use the API.
- No direct SQL Server writes from mobile.
- No API bypass.
- Store release readiness is not closed.
- Full offline sync processor is not closed.

## Next architecture direction

P6 should build from the patient foundation into brigade operations.

The likely domain sequence is:

1. Brigades
2. Patient visits within brigades
3. Service encounters
4. Staff/resources
5. Inventory/medication deliveries
6. Dashboards/reports
7. Offline sync processor/conflict queues
8. Production/pilot evidence