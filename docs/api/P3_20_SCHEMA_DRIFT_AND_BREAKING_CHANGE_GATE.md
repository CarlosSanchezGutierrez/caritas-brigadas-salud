# P3.20 Schema Drift and Breaking Change Gate

## Purpose

This document defines schema drift detection and breaking change gate expectations for client API contracts.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Schema drift gate status: BLOCKED_PENDING_REAL_EVIDENCE

## Schema drift detection

Schema drift detection must compare expected contract behavior against actual API behavior before client implementation depends on it.

Schema drift detection must cover:

- request schema.
- response schema.
- standard error envelope model.
- request metadata model.
- response metadata model.
- offline sync metadata models.
- audit reference model.
- conflict model.

## Breaking change gate

Breaking changes require explicit review before merge.

Breaking change examples:

- removing required field.
- renaming required field.
- changing field type.
- changing error envelope shape.
- changing authorization requirement.
- changing organization id requirement.
- changing idempotency key behavior.
- changing sync status behavior.
- changing audit trail reference behavior.

## Blocked behavior

Blocked behavior includes accepting schema drift silently, bypassing breaking change review, treating incompatible API behavior as client bug, treating mocked contract behavior as evidence, and merging client code without contract test evidence.

## P3.20 conclusion

Schema drift and breaking changes must be gated before Web iOS Android implementation expands.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
