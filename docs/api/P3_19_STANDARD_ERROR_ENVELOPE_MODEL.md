# P3.19 Standard Error Envelope Model

## Purpose

This document defines the standard error envelope model for Web iOS Android.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Standard error envelope

Every client must handle a standard error envelope instead of custom endpoint-specific error parsing.

## Required error envelope fields

| Field | Required | Purpose |
|---|---|---|
| errorCode | yes | machine-readable error classification |
| message | yes | safe user-facing or operator-facing message |
| requestId | yes | request traceability |
| correlationId | yes | cross-system traceability |
| endpointId | yes | endpoint contract traceability |
| details | optional | safe structured validation details |
| retryable | yes when applicable | retry decision boundary |
| validationErrors | yes for validation failures | field-level validation feedback |
| conflictId | yes for conflict responses | explicit conflict handling |
| auditTrailReference | yes when applicable | audit traceability |

## Required error categories

Required error categories:

- validation error.
- authentication error.
- authorization error.
- organization scope error.
- not found error.
- conflict error.
- offline sync error.
- duplicate idempotency key error.
- rate limit error.
- server error.

## Blocked error behavior

Blocked behavior includes hiding the standard error envelope, converting conflict responses into success, dropping request id, dropping correlation id, dropping validation details, ignoring retryable status, and using undocumented endpoint-specific error shapes.

## P3.19 conclusion

The standard error envelope must be the single client error contract across Web iOS Android.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
