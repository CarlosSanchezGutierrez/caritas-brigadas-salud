# P3.13 Request, Response, and Error Conventions

## Purpose

This document defines standard request, response, validation, pagination, filtering, sorting, and error conventions.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Request conventions

Every request that changes state must include or derive:

- request id.
- correlation id.
- organization id.
- actor.
- user role.
- API version.
- device id when mobile or offline-originated.
- idempotency key when replay protection is required.
- client operation id when offline sync is involved.

## Response conventions

Every successful response must provide:

- result status.
- server timestamp.
- request id.
- correlation id.
- API version.
- entity id when applicable.
- server version when applicable.
- audit trail reference when applicable.
- sync status when applicable.

## Standard response envelope

Standard response envelope fields:

- success.
- data.
- metadata.
- warnings.
- request id.
- correlation id.
- API version.

## Standard error envelope

Every error response must use a standard error envelope.

Required error fields:

- success.
- error code.
- error message.
- error category.
- validation errors.
- request id.
- correlation id.
- API version.
- retryable.
- audit trail reference when applicable.

## Error categories

| Category | Meaning |
|---|---|
| validation_error | Request failed validation |
| authorization_error | Actor lacks permission |
| authentication_error | Authentication missing or invalid |
| organization_scope_error | Organization boundary violation |
| not_found | Entity not found or unavailable in scope |
| conflict_error | Conflict detection triggered |
| idempotency_error | Idempotency key replay or mismatch |
| sync_error | Offline sync failure |
| rate_limit_error | Request exceeded allowed limits |
| server_error | Unexpected server failure |

## Pagination convention

Paginated endpoints must define:

- page size.
- continuation token or page number.
- total count when safe and feasible.
- next page indicator.
- sort order.
- filter summary.

## Filtering convention

Filterable endpoints must define:

- allowed fields.
- allowed operators.
- date range behavior.
- organization id scope.
- privacy restrictions.
- default filters.

## Sorting convention

Sortable endpoints must define:

- allowed sort fields.
- default sort field.
- default sort direction.
- stable ordering requirement.

## Validation convention

Validation must be server-side authoritative.

Clients may perform local validation, but server validation remains authoritative.

Validation errors must preserve:

- field.
- rule.
- message.
- rejected value category when safe.
- request id.
- correlation id.

## P3.13 conclusion

Consistent request, response, and error conventions are required before client contract freeze.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE