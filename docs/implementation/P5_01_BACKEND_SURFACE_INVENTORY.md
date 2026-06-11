# P5.1 Backend Surface Inventory

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

P5.1 starts the backend functional closure phase after P4.7 Backend Runtime Preflight.

This phase inventories the real backend surface before adding or changing functional code.

## Current milestone

The project is moving from runtime preflight into practical backend closure.

The current goal is to identify what already exists and what is missing for:

- patients.
- brigades.
- services.
- clinical encounters.
- consent and privacy.
- longitudinal history.
- offline-first synchronization.
- dashboards.
- analytics.
- reports.
- exports.
- audit trail.
- authorization.
- SQL Server persistence.
- OpenAPI contracts.

## Inventory scope

P5.1 captures:

- solution and project inventory.
- API project location.
- source file inventory.
- controllers.
- endpoint mapping patterns.
- request and response contracts.
- DTOs.
- validators.
- entities.
- DbContext files.
- EF Core configurations.
- migrations.
- repository and service layer files.
- authorization policy surface.
- audit and telemetry surface.
- health and OpenAPI route surface.
- test project inventory.
- detected domain coverage.
- detected missing functional surfaces.

## Mandatory future backend surfaces

The final backend must include all of the following functional areas:

- patient core.
- flexible patient identity for incomplete records.
- consent and privacy capture.
- brigade core.
- brigade service availability.
- clinical encounter capture.
- longitudinal patient history.
- clinical audit trail.
- report endpoints.
- exports.
- offline-first synchronization.
- idempotency.
- conflict resolution.
- dashboards.
- analytics.
- institutional SQL Server readiness.
- production observability.

## Offline-first requirement

Offline-first is mandatory for the final system.

The backend must later support:

- client operation id.
- idempotency key.
- local temporary ids.
- server authoritative ids.
- sync status.
- outbox submission.
- server acknowledgment.
- conflict id.
- conflict resolution.
- retry-safe writes.
- patient deduplication.
- per-brigade sync scope.
- sync audit trail.

## Dashboards and analytics requirement

Dashboards and analytics are mandatory for the final system.

The backend must later support:

- daily patient count.
- monthly patient count.
- patients by brigade.
- patients by service.
- services delivered.
- consent completion metrics.
- longitudinal patient metrics.
- location-level metrics.
- exportable datasets.
- executive reporting.
- impact indicators.

## Longitudinal history requirement

Longitudinal history is mandatory.

The backend must later support:

- patient timeline.
- encounter history.
- service history.
- consent history.
- audit trail history.
- clinical evolution.
- duplicate detection.
- source brigade traceability.

## Guardrails

- No backend production readiness approval.
- No fabricated evidence.
- No secrets in repository.
- No committed real patient data.
- No direct mobile write to SQL Server.
- No client may bypass the API.
- No cloud dependency.
- SQL Server remains the operational source of truth.
- Backend production readiness remains BLOCKED_PENDING_REAL_EVIDENCE.