# P3.20 Cross Client Contract Test Scenarios

## Purpose

This document defines shared contract test scenarios for Web iOS Android.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Required scenarios

| Scenario | Web | iOS | Android |
|---|---|---|---|
| health endpoint contract | required | required | required |
| identity endpoint contract | required | required | required |
| organization-scoped request | required | required | required |
| authenticated request metadata | required | required | required |
| authorization failure envelope | required | required | required |
| validation failure envelope | required | required | required |
| not found envelope | required | required | required |
| conflict envelope | required | required | required |
| rate limit envelope | required | required | required |
| server error envelope | required | required | required |
| accepted write audit reference | required | required | required |
| request id preservation | required | required | required |
| correlation id preservation | required | required | required |
| pagination convention | required | review only | review only |
| filtering convention | required | review only | review only |
| sorting convention | required | review only | review only |
| device id propagation | not applicable | required | required |
| idempotency key propagation | review only | required | required |
| client operation id propagation | review only | required | required |
| server acknowledgment handling | review only | required | required |
| sync status handling | review only | required | required |
| conflict id handling | required | required | required |

## Blocked scenario behavior

Blocked behavior includes missing request id, missing correlation id, missing organization id, missing standard error envelope, missing device id for mobile sync, missing idempotency key for offline sync, missing audit trail reference for accepted writes, and silent conflict overwrite.

## P3.20 conclusion

Cross-client scenarios must verify consistent API behavior before feature implementation expands.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
