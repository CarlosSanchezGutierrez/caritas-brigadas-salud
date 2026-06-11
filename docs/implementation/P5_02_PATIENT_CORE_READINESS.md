# P5.2 Patient Core Readiness

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

P5.2 starts patient core functional closure after P5.1 backend surface inventory.

This phase validates whether the backend already has the necessary patient core surface before adding patient write endpoints and persistence changes.

## Patient core requirements

The backend must support patient creation, patient lookup, patient detail, patient update, flexible identity when CURP, phone, or complete name is unavailable, organization scoped access, audit trail for patient writes, consent linkage, clinical encounter linkage, longitudinal history linkage, offline-first safe identifiers, idempotency safe creation, OpenAPI contract visibility, and SQL Server persistence.

## Required patient fields

The patient core must support patient id, organization id, display name, given names, family names, date of birth when known, sex or gender when collected, CURP when available, phone when available, locality or community when available, notes when clinically safe, created timestamp, updated timestamp, created by, updated by, source brigade id when applicable, client operation id when applicable, and idempotency key when applicable.

## Offline-first requirements

Patient creation must later support local temporary id, client operation id, idempotency key, retry-safe POST behavior, server authoritative id, server acknowledgment, sync status, conflict detection, duplicate candidate detection, merge decision evidence, and sync audit trail.

## Longitudinal history requirements

Patient core must later connect to patient timeline, clinical encounter history, services received, consent history, brigade history, audit history, and duplicate and merge history.

## P5.2 scope

P5.2 captures evidence for existing patient core readiness: patient domain files, patient contracts, patient controllers or endpoints, patient persistence surface, patient DbSet or configuration, patient migration surface, patient validation surface, patient authorization surface, patient audit surface, patient tests, patient OpenAPI visibility, and missing implementation backlog.

## P5.2 does not fake completion

P5.2 does not approve production readiness.

P5.2 does not claim patient core is implemented unless real code exists.

P5.2 creates the execution evidence needed to implement patient core safely.

## Guardrails

No backend production readiness approval.

No fabricated evidence.

No secrets in repository.

No committed real patient data.

No direct mobile write to SQL Server.

No client may bypass the API.

No cloud dependency.

SQL Server remains the operational source of truth.

Backend production readiness remains BLOCKED_PENDING_REAL_EVIDENCE.