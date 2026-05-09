# Pagination Baseline

## Purpose

This document defines the standard pagination contract for Cáritas Brigadas de Salud API.

Pagination is required before production use for endpoints that may return large collections.

## Current state

The backend already has `PaginatedResponse<T>`.

Several list endpoints still return `ApiResponse<IReadOnlyCollection<T>>`.

This is acceptable for early MVP work but must be resolved before production-scale use.

## Standard request

List endpoints should accept:

- `pageNumber`
- `pageSize`

Default behavior:

- `pageNumber = 1`
- `pageSize = 50`
- `maxPageSize = 250`
- `maxPageNumber = int.MaxValue / maxPageSize`

Invalid values must be normalized server-side.

The API must never allow clients to create unbounded production queries or integer-overflow offsets.

## Standard response

Paginated list endpoints should return `ApiResponse<PaginatedResponse<T>>`.

Expected shape:

{
  "success": true,
  "data": {
    "items": [],
    "pageNumber": 1,
    "pageSize": 50,
    "totalCount": 0,
    "totalPages": 0
  },
  "traceId": "...",
  "timestampUtc": "..."
}

## Required query behavior

Repository list queries should use:

- `AsNoTracking()`
- `Where(...)`
- `OrderBy(...)`
- `Skip(...)`
- `Take(...)`

Rules:

1. Every paginated query must have deterministic ordering.
2. Every organization-scoped query must filter by `OrganizationId`.
3. Deleted records must be excluded using `IsDeleted` when applicable.
4. High-volume endpoints must not return unbounded lists.
5. `TotalCount` must be calculated with the same filters as `Items`.
6. `PageSize` must be capped server-side.
7. `PageNumber` must be capped server-side.
8. `Skip` must be computed without integer overflow.
9. Pagination must be documented in OpenAPI before frontend integration.

## Priority endpoints

P0 pagination targets:

1. Patients.
2. Patient visits.
3. Form responses.
4. Consent documents.
5. Sync batches.
6. Audit logs.
7. Users.

P1 pagination targets:

1. Brigades.
2. Communities.
3. Services.
4. Form templates.
5. Mobile units.
6. Service encounters.
7. Roles.

## Compatibility rule

Because frontend clients are not fully implemented yet, changing list response shape is acceptable only if done before web, Android, and iOS clients depend on the endpoints.

After frontend integration starts, pagination response changes must be handled as breaking API changes.

## Frontend rule

Web, Android, and iOS must not manually invent pagination DTOs.

They must use generated OpenAPI clients when practical or typed wrappers matching the OpenAPI contract.

## Technical debt link

Pagination gaps are tracked as D2 debt until all high-volume endpoints use `PaginatedResponse<T>`.