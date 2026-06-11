# P3.19 Request and Response Metadata Models

## Purpose

This document defines request metadata model and response metadata model expectations for Web iOS Android.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Request metadata model

| Field | Required | Applies to | Purpose |
|---|---|---|---|
| apiContractVersion | yes | all clients | identifies the API contract version |
| endpointId | yes | all clients | identifies the endpoint contract being used |
| requestId | yes | all clients | supports traceability and support investigation |
| correlationId | yes | all clients | connects client flow with backend logs and audit evidence |
| organizationId | yes for scoped data | all clients | prevents cross-organization data leakage |
| authorizationRole | yes for protected actions | all clients | preserves role-sensitive behavior |
| deviceId | yes for mobile sync | iOS Android | identifies mobile device participation |
| idempotencyKey | yes for offline writes | iOS Android | prevents duplicated accepted writes |
| clientOperationId | yes for offline writes | iOS Android | links local operation to server acknowledgment |

## Response metadata model

| Field | Required | Applies to | Purpose |
|---|---|---|---|
| requestId | yes | all clients | confirms request traceability |
| correlationId | yes | all clients | confirms cross-system traceability |
| serverTimestamp | yes when available | all clients | supports evidence and ordering |
| auditTrailReference | yes for accepted writes | all clients | connects accepted write to audit trail |
| serverAcknowledgment | yes for mobile sync | iOS Android | confirms accepted sync operation |
| syncStatus | yes for mobile sync | iOS Android | communicates accepted pending rejected conflicted or quarantined state |

## Blocked metadata behavior

Blocked behavior includes missing request id, missing correlation id, missing organization id for scoped data, missing device id for mobile sync, missing idempotency key for offline sync, missing audit trail reference for accepted writes, and treating synthetic metadata as evidence.

## P3.19 conclusion

Request and response metadata must remain consistent across Web iOS Android.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
