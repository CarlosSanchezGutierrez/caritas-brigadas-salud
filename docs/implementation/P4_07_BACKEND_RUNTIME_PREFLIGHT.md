# P4.7 Backend Runtime Preflight

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

P4.7 starts the practical backend runtime validation phase after P4.6 route alignment.

This phase stops relying only on documentation and begins collecting real execution evidence from the backend.

## Current position

The latest closed governance and evidence phases are:

- P3.43 final production governance evidence index.
- P4.1 real evidence execution baseline.
- P4.2 real evidence package classification.
- P4.4 real environment SQL Server access blocker.
- P4.5 API runtime and OpenAPI evidence boundary.
- P4.6 API route evidence alignment.

## Runtime preflight scope

P4.7 collects evidence for:

- git revision.
- repository cleanliness.
- .NET SDK availability.
- solution or project discovery.
- dotnet restore.
- dotnet build.
- dotnet test.
- API project path existence.
- API startup attempt.
- /health/live.
- /health/ready.
- /openapi/v1/openapi.json.
- /swagger.
- SQL Server configuration presence without printing secret values.
- EF Core migration surface discovery when available.
- endpoint/controller inventory.
- blocker classification for runtime closure.

## Correct API routes

The implemented API evidence routes are:

- /health/live
- /health/ready
- /openapi/v1/openapi.json
- /swagger

## SQL Server boundary

P4.7 does not close the institutional SQL Server blocker.

If /health/live succeeds and /health/ready fails because SQL Server access is unavailable, that is valid evidence and should remain classified under the P4.4 SQL Server access blocker.

## Offline-first boundary

P4.7 does not implement offline-first synchronization.

Offline-first is mandatory for the final system and must later include:

- local IDs.
- idempotency keys.
- local outbox.
- server inbox.
- sync endpoint.
- conflict detection.
- conflict resolution.
- retry policy.
- local encrypted storage.
- sync audit trail.
- patient deduplication strategy.
- per-brigade sync scope.

## Dashboards and analytics boundary

P4.7 does not implement dashboards or analytics.

Dashboards and analytics are mandatory for the final system and must later include:

- daily patient counts.
- monthly patient counts.
- service-level metrics.
- brigade-level metrics.
- location-level metrics.
- longitudinal patient metrics.
- executive reporting.
- exportable datasets.
- social impact indicators.
- cost per beneficiary when cost data exists.
- risk and operational decision indicators.

## Longitudinal history boundary

P4.7 does not close longitudinal history.

Longitudinal history is mandatory for the backend functional closure and must later include:

- patient timeline.
- encounter history.
- service history.
- consent history.
- audit trail.
- duplicate detection.
- clinical evolution.
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