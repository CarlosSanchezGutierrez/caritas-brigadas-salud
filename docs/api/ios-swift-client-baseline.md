# iOS Swift Client Baseline

## Purpose

This document defines the baseline for the official iOS client of Cáritas Brigadas de Salud.

The official iOS version must be implemented in Swift.

## Required stack

- Swift
- SwiftUI
- URLSession or approved networking abstraction
- OpenAPI-generated Swift client when practical
- Keychain for sensitive token storage
- Centralized API service layer

## Non-negotiable rules

1. The official iOS app must be written in Swift.
2. SwiftUI is the preferred UI framework.
3. API calls must go through a centralized networking layer.
4. DTOs must match backend OpenAPI contracts.
5. Authentication handling must be centralized.
6. Authorization must remain enforced by the backend.
7. Tokens must not be stored in UserDefaults.
8. PHI, PII, signatures, clinical payloads, and tokens must not be logged.
9. Pagination must use backend pagination metadata.
10. Errors must preserve backend `traceId`.

## Architecture expectation

Recommended iOS layering:

- Presentation
- ViewModels
- Domain/UI models
- API client
- DTO mapping
- Secure storage

## API contract rule

The iOS app must not define independent API behavior.

The source of truth is:

- Backend OpenAPI
- `ApiResponse<T>`
- `PaginatedResponse<T>`
- `ApiErrorResponse`
- Permission policies
- Organization-scoped routes

## Future work

Before iOS implementation starts:

1. Export stable OpenAPI.
2. Decide generated Swift client vs manual typed API client.
3. Define authentication flow.
4. Define secure token storage.
5. Define offline requirements.
6. Define local persistence boundaries.
7. Define test strategy for API integration.