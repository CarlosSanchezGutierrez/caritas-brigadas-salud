# P3.18 API Client Security Scaffold

## Purpose

This document defines the API client security scaffold for Web iOS Android.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

API client security scaffold status: BLOCKED_PENDING_REAL_EVIDENCE

## Security scaffold scope

The security scaffold governs authentication metadata, authorization role metadata, organization scope, auditability, privacy-safe fixtures, offline sync metadata, and error handling.

## Required security behavior

Required behavior:

- preserve authentication requirement.
- preserve authorization role.
- preserve organization id.
- preserve request id.
- preserve correlation id.
- preserve standard error envelope.
- preserve audit trail reference when applicable.
- preserve device id when mobile.
- preserve idempotency key when offline sync is involved.
- preserve client operation id when offline sync is involved.
- prevent direct database access.
- prevent undocumented endpoint usage.
- prevent silent conflict overwrite.

## Blocked security behavior

Blocked behavior includes bypassing the API, bypassing authorization, bypassing organization scope, bypassing audit trail creation, credential persistence in source code, real patient data in fixtures, unrestricted export behavior, unaudited patient-level access, missing device id for mobile sync, and missing idempotency key for offline sync.

## P3.18 conclusion

API client security must be scaffolded before Web iOS Android implementation expands.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
