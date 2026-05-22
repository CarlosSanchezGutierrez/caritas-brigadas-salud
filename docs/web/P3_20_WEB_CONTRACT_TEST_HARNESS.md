# P3.20 Web Contract Test Harness

## Purpose

This document defines the Web contract test harness expectations.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Web contract test harness status: BLOCKED_PENDING_REAL_EVIDENCE

## Web harness scope

The Web contract test harness must validate request schema, response schema, standard error envelope model, authentication requirement, authorization role, organization id, request id, correlation id, audit trail reference, pagination convention, filtering convention, sorting convention, schema drift detection, and breaking change detection.

## Web-specific required tests

Required tests:

- protected route API call includes authenticated context.
- role-sensitive API call preserves authorization role.
- organization-scoped API call preserves organization id.
- table and report endpoints preserve pagination convention.
- table and report endpoints preserve filtering convention.
- table and report endpoints preserve sorting convention.
- validation error uses standard error envelope.
- authorization error uses standard error envelope.
- conflict response preserves conflict id.
- accepted write preserves audit trail reference.

## Web blocked behavior

The Web harness must reject undocumented endpoints, direct database access, missing organization scope, missing request id, missing correlation id, hidden standard error envelope, unrestricted export behavior, and UI-only evidence.

## P3.20 conclusion

The Web client must have contract test harness coverage before feature implementation relies on backend behavior.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
