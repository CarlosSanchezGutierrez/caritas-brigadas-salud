# P3.21 Web Runtime Configuration Boundary

## Purpose

This document defines the Web runtime configuration boundary.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Web runtime configuration status: BLOCKED_PENDING_REAL_EVIDENCE

## Web configuration responsibilities

The Web runtime configuration boundary must define:

- environment name.
- API base URL.
- API contract version.
- OpenAPI artifact reference.
- feature flag boundary.
- telemetry toggle boundary.
- request timeout policy.
- retry policy.
- build profile boundary.
- release channel boundary.
- evidence package reference.

## Web runtime rules

The Web client must preserve request id, correlation id, organization id, authorization role, standard error envelope, audit trail reference, pagination convention, filtering convention, and sorting convention.

## Web blocked configuration behavior

The Web client must not hardcode production URLs in feature code, bypass the API, write directly to SQL Server, ignore organization id, ignore authorization role, drop request id, drop correlation id, hide standard error envelope, treat exports as unrestricted, or treat local configuration as production evidence.

## Web evidence requirement

Required evidence includes environment mapping evidence, API base URL evidence, API contract version evidence, feature flag evidence, standard error envelope evidence, request id evidence, correlation id evidence, organization id evidence, and contract test evidence.

## P3.21 conclusion

Web runtime configuration must be centralized and evidence-backed before Web feature implementation expands.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
