# P3.22 Web Observability Telemetry Boundary

## Purpose

This document defines the Web observability telemetry boundary.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Web observability telemetry status: BLOCKED_PENDING_REAL_EVIDENCE

## Web telemetry scope

The Web client must capture support-safe diagnostic context for:

- environment name.
- API base URL reference.
- API contract version.
- endpoint id.
- request id.
- correlation id.
- organization id.
- authorization role.
- standard error envelope.
- audit trail reference.
- pagination convention.
- filtering convention.
- sorting convention.
- contract test status.
- configuration test status.

## Web event categories

Required Web event categories:

- authenticated navigation event.
- protected API request event.
- organization-scoped request event.
- authorization denial event.
- validation error event.
- conflict error event.
- accepted write audit reference event.
- dashboard load event.
- report export request event.
- support diagnostic event.

## Web blocked telemetry behavior

The Web client must not log secrets, log real patient payloads, bypass the API, write directly to SQL Server, ignore organization id, ignore authorization role, drop request id, drop correlation id, hide standard error envelope, treat exports as unrestricted, or treat UI logs as production evidence.

## Web evidence requirement

Required evidence includes request id evidence, correlation id evidence, organization id evidence, standard error envelope evidence, authorization role evidence, audit trail reference evidence, privacy-safe telemetry evidence, contract test evidence, and configuration test evidence.

## P3.22 conclusion

Web observability must be centralized and privacy-safe before Web feature implementation expands.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
