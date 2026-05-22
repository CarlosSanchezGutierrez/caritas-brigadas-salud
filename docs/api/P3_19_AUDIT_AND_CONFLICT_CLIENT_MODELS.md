# P3.19 Audit and Conflict Client Models

## Purpose

This document defines audit reference model and conflict model expectations for Web iOS Android.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Audit reference model

Accepted writes must expose or preserve audit trail reference when applicable.

Required audit fields:

- auditTrailReference.
- request id.
- correlation id.
- endpoint id.
- organization id.
- user role.
- server timestamp when available.

## Conflict model

Conflict responses must be explicit and must not be treated as success.

Required conflict fields:

- conflict id.
- request id.
- correlation id.
- endpoint id.
- conflicting resource id.
- client operation id when mobile.
- device id when mobile.
- conflict reason.
- resolution requirement.

## Client behavior

Web client may review and resolve conflicts only when authorized.

iOS client and Android client must preserve conflict id and show explicit conflict handling state.

## Blocked behavior

Blocked behavior includes missing audit trail reference for accepted writes, unaudited accepted writes, treating conflict as success, silent conflict overwrite, missing conflict id, missing organization id, and missing authorization role for conflict review.

## P3.19 conclusion

Audit and conflict client models must be consistent before client features implement accepted writes or conflict resolution.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
