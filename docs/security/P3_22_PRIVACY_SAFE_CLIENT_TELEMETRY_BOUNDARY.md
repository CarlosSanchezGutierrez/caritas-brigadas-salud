# P3.22 Privacy Safe Client Telemetry Boundary

## Purpose

This document defines privacy-safe client telemetry rules for Web iOS Android.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Privacy safe client telemetry status: BLOCKED_PENDING_REAL_EVIDENCE

## Privacy-safe telemetry rules

Client telemetry must help support and operations without exposing sensitive payloads.

Allowed telemetry fields:

- environment name.
- client target.
- build profile.
- release channel.
- API contract version.
- endpoint id.
- request id.
- correlation id.
- organization id.
- authorization role.
- standard error envelope code.
- audit trail reference.
- device id when mobile.
- idempotency key when offline sync is involved.
- client operation id when offline sync is involved.
- sync status.
- conflict id.

## Restricted telemetry fields

Restricted telemetry fields include real patient names, real patient addresses, real patient contact data, clinical payload text, identity document values, consent signatures, credentials, tokens, connection strings, raw database values, and unsupported sensitive fixtures.

## Required controls

Required controls:

- telemetry redaction boundary.
- support diagnostic boundary.
- evidence sanitization boundary.
- local log retention boundary.
- mobile offline telemetry boundary.
- standard error envelope boundary.
- request id and correlation id preservation.
- organization id preservation.

## P3.22 conclusion

Client telemetry must be privacy-safe before diagnostic evidence is accepted.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
