# P3.19 Mobile Model Mapping

## Purpose

This document maps shared API client models to iOS client and Android client.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Mobile model mapping status: BLOCKED_PENDING_REAL_EVIDENCE

## Mobile model mapping

| Shared model | iOS usage | Android usage |
|---|---|---|
| request metadata model | Swift API boundary | Kotlin API boundary |
| response metadata model | Swift API boundary | Kotlin API boundary |
| standard error envelope model | Swift error model | Kotlin error model |
| authentication context model | authenticated field workflow | authenticated field workflow |
| authorization context model | role-aware local UI | role-aware local UI |
| organization scope model | organization-scoped field capture | organization-scoped field capture |
| mobile device model | device id lifecycle | device id lifecycle |
| offline operation model | local draft outbox sync | local draft outbox sync |
| audit reference model | accepted write reconciliation | accepted write reconciliation |
| conflict model | explicit conflict handling | explicit conflict handling |

## Mobile blocked mapping

iOS and Android must not sync without device id, sync without idempotency key, sync without client operation id, silently overwrite conflicts, drop server acknowledgment, drop request id, drop correlation id, or treat local draft as server evidence.

## P3.19 conclusion

Mobile model mapping must remain aligned with offline-first, auditable, idempotent API contracts.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
