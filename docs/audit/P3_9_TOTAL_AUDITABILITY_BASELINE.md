# P3.9 Total Auditability Baseline

## Purpose

P3.9 defines the total auditability baseline for Caritas Brigadas de Salud.

The system must preserve an audit trail for clinically relevant, operationally relevant, security relevant, and data governance relevant actions.

This phase does not claim final implementation evidence. It defines the auditable evidence contract that future backend, web, iOS, Android, reporting, and data pipeline work must respect.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Backend freeze status: NOT_FROZEN_PENDING_ON_PREM_EVIDENCE_AND_CONTRACTS

## Core principle

Every meaningful domain action must be auditable.

The minimum audit event must preserve:

- actor
- action
- entity
- entity id
- timestamp
- correlation id
- request id
- source ip
- device id
- organization id
- user role
- result
- reason
- before snapshot reference
- after snapshot reference
- audit trail reference

## Audit event categories

| Category | Examples |
|---|---|
| Identity and access | login, logout, failed login, token refresh, permission denied |
| Patient | create patient, update patient, identity partial update, merge candidate review |
| Consent | consent captured, consent version changed, consent revoked, privacy notice accepted |
| Encounter | create encounter, update encounter, close encounter, reopen encounter |
| Clinical | vital signs captured, diagnosis note recorded, medication recorded, reference created |
| Documents | document uploaded, document linked, document rejected, document corrected |
| Brigade operations | brigade created, brigade opened, brigade closed, service availability changed |
| Controlled data injection | batch received, accepted records, rejected records, quarantine, idempotency key replay |
| Reporting | export generated, dashboard dataset refreshed, analytical snapshot created |
| Security | forbidden access, suspicious request, rate limit event, privilege change |
| Administration | role changed, user disabled, organization changed, configuration changed |

## Audit event integrity

Audit events must be append-oriented.

The system must not silently overwrite audit history.

Corrections must create correction events.

Destructive operations must preserve:

- actor
- action
- entity
- reason
- timestamp
- correlation id
- request id
- audit trail

## Required audit guarantees

1. No silent overwrite.
2. No unaudited privileged action.
3. No unaudited patient merge.
4. No unaudited consent change.
5. No unaudited clinical correction.
6. No unaudited data export.
7. No unaudited controlled data injection.
8. No unaudited role or permission change.
9. No audit trail bypass.
10. No secrets in repository.

## Sensitive data rule

Audit events must avoid unnecessary patient data duplication.

Audit events should store references, hashes, metadata, and controlled snapshots where appropriate.

Raw sensitive patient data must not be copied into logs unnecessarily.

## Evidence required later

Future implementation evidence must prove:

- audit trail exists for core patient actions.
- audit trail exists for consent actions.
- audit trail exists for encounter actions.
- audit trail exists for controlled data injection.
- audit trail exists for exports.
- audit trail exists for role changes.
- correction event exists for edited clinical information.
- correlation id links API request and domain event.
- request id links HTTP request and audit event.
- device id exists for mobile or offline-originated actions.
- organization id exists for tenant and institutional separation.

## P3.9 conclusion

Total auditability remains a hard backend closure requirement.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE