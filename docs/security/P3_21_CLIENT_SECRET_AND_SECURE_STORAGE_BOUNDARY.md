# P3.21 Client Secret and Secure Storage Boundary

## Purpose

This document defines the client secret and secure storage boundary for Web iOS Android.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Client secret and secure storage status: BLOCKED_PENDING_REAL_EVIDENCE

## Secret boundary

Client applications must not depend on repository-stored credentials.

Secret injection must be environment-controlled, documented, reviewable, and excluded from source code.

## Secure storage boundary

Secure storage rules must distinguish:

- public configuration.
- environment configuration.
- non-secret feature flags.
- authenticated session material.
- refresh material when applicable.
- local offline drafts.
- local sync metadata.
- evidence package references.

## Required security controls

Required controls:

- No secrets in repository.
- API base URL must be configuration-driven.
- API contract version must be configuration-driven.
- mobile secure storage boundary must be explicit.
- offline drafts must not contain unsupported sensitive fixtures.
- sync metadata must preserve device id.
- offline writes must preserve idempotency key.
- client operation id must be preserved for offline sync.
- standard error envelope must be preserved.
- request id and correlation id must be preserved.

## Blocked behavior

Blocked behavior includes credential persistence in source code, hardcoded production credentials, unsupported patient fixtures, direct database access, undocumented endpoint usage, missing device id for mobile sync, missing idempotency key for offline sync, and local evidence presented as production approval.

## P3.21 conclusion

Client secrets and secure storage must be governed before client runtime configuration becomes operational.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
