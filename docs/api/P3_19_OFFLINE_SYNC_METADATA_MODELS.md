# P3.19 Offline Sync Metadata Models

## Purpose

This document defines offline sync metadata models for iOS and Android.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Offline sync model scope

Offline sync metadata applies to iOS client and Android client.

The Web client may review conflicts and sync status but must not impersonate mobile offline operation metadata.

## Required offline sync fields

| Field | Required | Purpose |
|---|---|---|
| deviceId | yes | identifies the mobile device |
| idempotencyKey | yes | prevents duplicated accepted writes |
| clientOperationId | yes | links local operation to server acknowledgment |
| localDraftId | yes for local draft | links local draft to outbox operation |
| syncStatus | yes | tracks pending accepted rejected conflicted quarantined or reconciled state |
| serverAcknowledgment | yes after accepted sync | confirms server acceptance |
| conflictId | yes for conflict | supports explicit conflict handling |
| retryCount | yes for retry behavior | supports bounded retry policy |
| lastAttemptAt | yes for retry behavior | supports operational evidence |

## Required sync statuses

Required sync statuses:

- draft.
- pending sync.
- submitted.
- accepted.
- rejected.
- conflicted.
- quarantined.
- reconciled.

## Blocked offline behavior

Blocked behavior includes sync without device id, sync without idempotency key, sync without client operation id, duplicate accepted writes, silent conflict overwrite, dropping server acknowledgment, treating local draft as server evidence, and bypassing audit trail creation.

## P3.19 conclusion

Offline sync metadata must be explicit before iOS and Android offline workstreams implement local queues.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
