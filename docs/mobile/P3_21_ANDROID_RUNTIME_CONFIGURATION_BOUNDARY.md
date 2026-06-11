# P3.21 Android Runtime Configuration Boundary

## Purpose

This document defines the Android runtime configuration boundary.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Android runtime configuration status: BLOCKED_PENDING_REAL_EVIDENCE

## Android configuration responsibilities

The Android runtime configuration boundary must define:

- environment name.
- API base URL.
- API contract version.
- OpenAPI artifact reference.
- feature flag boundary.
- offline mode toggle boundary.
- sync mode toggle boundary.
- request timeout policy.
- retry policy.
- secure storage boundary.
- secret injection boundary.
- build profile boundary.
- release channel boundary.
- evidence package reference.

## Android runtime rules

The Android client must preserve request id, correlation id, organization id, authorization role, standard error envelope, audit trail reference, device id, idempotency key, client operation id, sync status, server acknowledgment, and conflict id.

## Android blocked configuration behavior

The Android client must not hardcode production URLs in feature code, write directly to SQL Server, bypass the API, bypass authorization, bypass audit trail creation, sync without device id, sync without idempotency key, drop client operation id, drop server acknowledgment, silently overwrite conflicts, or treat local draft configuration as server evidence.

## Android evidence requirement

Required evidence includes environment mapping evidence, API base URL evidence, API contract version evidence, offline mode toggle evidence, sync mode toggle evidence, device id evidence, idempotency key evidence, client operation id evidence, standard error envelope evidence, and contract test evidence.

## P3.21 conclusion

Android runtime configuration must be centralized and evidence-backed before Android offline-first implementation expands.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
