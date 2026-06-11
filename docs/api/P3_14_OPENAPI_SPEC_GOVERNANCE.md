# P3.14 OpenAPI Specification Governance

## Purpose

This document defines how OpenAPI must be governed for Cáritas Brigadas de Salud.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## OpenAPI governance principle

OpenAPI must reflect the approved API contract.

OpenAPI must not invent backend behavior that is not supported by contract documentation or implementation evidence.

## Required OpenAPI sections

The OpenAPI baseline must define:

- openapi version.
- info title.
- info version.
- server placeholders.
- tags.
- paths.
- operation ids.
- request schema.
- response schema.
- standard error envelope.
- security schemes.
- components schemas.
- examples with synthetic data.
- API version.
- correlation id.
- request id.
- organization id.
- audit trail reference.
- idempotency key.
- device id.

## Operation id rules

Every endpoint must have a stable operation id.

Operation id naming convention:

- health_read
- identity_me_read
- patients_create
- patients_update
- consent_capture
- encounters_create
- encounters_update
- sync_outbox_submit
- sync_status_read
- reports_export
- dashboards_dataset_read
- audit_events_search

## Schema governance

Every schema must define:

- name.
- purpose.
- required fields.
- optional fields.
- sensitive fields when applicable.
- validation expectations.
- examples using synthetic data only.
- client compatibility.
- version impact.

## Standard schemas

Required standard schemas:

- StandardResponseEnvelope.
- StandardErrorEnvelope.
- ValidationError.
- AuditMetadata.
- RequestMetadata.
- PaginationMetadata.
- IdempotencyMetadata.
- OfflineSyncMetadata.
- ConflictMetadata.
- ExportMetadata.

## Schema drift policy

Schema drift occurs when documentation, OpenAPI, implementation, or generated clients disagree.

Schema drift must trigger:

- contract review.
- affected endpoint list.
- client impact statement.
- version review.
- evidence update.
- contract testing update.

## Breaking change policy

Breaking changes require:

- API version review.
- migration note.
- affected client list.
- affected endpoint list.
- OpenAPI update.
- client stub update.
- contract testing update.
- evidence package update.

## P3.14 conclusion

OpenAPI must be governed as a controlled contract artifact.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE