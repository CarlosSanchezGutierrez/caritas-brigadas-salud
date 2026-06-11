# P3.13 Endpoint Catalog

## Purpose

This document defines the baseline endpoint catalog required for Web, iOS, Android, reporting, administration, and offline sync.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Endpoint catalog rule

Every endpoint must define:

- endpoint id.
- HTTP method.
- route.
- API version.
- client support.
- authentication requirement.
- authorization role.
- organization id requirement.
- request schema.
- response schema.
- standard error envelope.
- validation rules.
- audit trail requirement.
- idempotency key requirement.
- offline sync compatibility.
- pagination convention when applicable.
- filtering convention when applicable.
- sorting convention when applicable.

## Endpoint groups

| Group | Purpose |
|---|---|
| health | Operational health and readiness visibility |
| identity | Authenticated identity and role context |
| organizations | Organization-scoped configuration |
| brigades | Brigade setup, opening, closing, and service availability |
| patients | Patient registration, partial identity, identity enrichment |
| consent | Consent capture, privacy notice version, revocation |
| encounters | Encounter creation, updates, close, reopen, corrections |
| clinical records | Vital signs, notes, referrals, medications, corrections |
| documents | Document metadata, document references, upload state |
| offline sync | Outbox submission, acknowledgment, conflicts, replay detection |
| audit | Audit trail lookup and security review |
| reports | Governed reports, CSV/XLSX exports, evidence packages |
| dashboards | Dashboard datasets and metric lineage |
| administration | Roles, permissions, users, configuration |

## Baseline endpoint matrix

| Endpoint id | Method | Route | Client support | Notes |
|---|---|---|---|---|
| health.read | GET | /api/v1/health | Web/iOS/Android/Ops | Public or protected according to environment |
| identity.me | GET | /api/v1/identity/me | Web/iOS/Android | Authenticated user context |
| organizations.list | GET | /api/v1/organizations | Web/Admin | Organization-scoped access |
| brigades.create | POST | /api/v1/brigades | Web/Admin | Audited write |
| brigades.close | POST | /api/v1/brigades/{brigadeId}/close | Web/Admin | Audited write |
| patients.create | POST | /api/v1/patients | Web/iOS/Android | Supports partial identity |
| patients.update | PATCH | /api/v1/patients/{patientId} | Web/iOS/Android | Correction-aware |
| consent.capture | POST | /api/v1/patients/{patientId}/consents | Web/iOS/Android | Consent timeline |
| encounters.create | POST | /api/v1/encounters | Web/iOS/Android | Offline-compatible |
| encounters.update | PATCH | /api/v1/encounters/{encounterId} | Web/iOS/Android | Correction-aware |
| clinical.vitals.create | POST | /api/v1/encounters/{encounterId}/vital-signs | Web/iOS/Android | Clinical timeline |
| clinical.referrals.create | POST | /api/v1/encounters/{encounterId}/referrals | Web/iOS/Android | Clinical continuity |
| sync.outbox.submit | POST | /api/v1/sync/outbox | iOS/Android | Requires idempotency key |
| sync.status.read | GET | /api/v1/sync/operations/{clientOperationId} | iOS/Android | Reconciliation |
| sync.conflicts.read | GET | /api/v1/sync/conflicts | iOS/Android/Web | Conflict review |
| reports.export | POST | /api/v1/reports/exports | Web/Admin | Governed CSV/XLSX export |
| dashboards.dataset.read | GET | /api/v1/dashboards/{dashboardId}/dataset | Web/Admin | Read-only governed dataset |
| audit.events.search | GET | /api/v1/audit/events | Web/Admin/Auditor | Scoped audit review |

## Endpoint freeze rule

New endpoints after P3.13 must include:

- endpoint catalog entry.
- request schema.
- response schema.
- standard error envelope.
- security rule.
- audit rule.
- client compatibility statement.
- evidence requirement.

## P3.13 conclusion

The endpoint catalog is the source for client integration planning.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE