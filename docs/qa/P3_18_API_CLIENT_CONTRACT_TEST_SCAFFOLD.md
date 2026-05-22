# P3.18 API Client Contract Test Scaffold

## Purpose

This document defines the contract test scaffold for Web iOS Android API clients.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

API client contract test scaffold status: BLOCKED_PENDING_REAL_EVIDENCE

## Contract test scope

Contract tests must validate:

- API contract version.
- endpoint id mapping.
- request schema.
- response schema.
- standard error envelope.
- authentication requirement.
- authorization role.
- organization id propagation.
- request id propagation.
- correlation id propagation.
- audit trail reference handling.
- device id propagation for mobile.
- idempotency key propagation for offline sync.
- client operation id propagation for offline sync.
- sync status handling for mobile.
- schema drift detection.
- breaking change detection.

## Contract test evidence

Required evidence includes passing contract test evidence, failing scenario evidence, schema drift evidence, standard error envelope evidence, cross-client metadata evidence, and blocked scenario evidence.

## P3.18 conclusion

API client scaffolds must be contract-tested before feature implementation depends on them.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
