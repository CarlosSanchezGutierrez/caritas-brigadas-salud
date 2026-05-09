# Frontend API Contracts Baseline

## Purpose

This document defines how Web, Android, and iOS clients must integrate with the backend API.

The backend is the source of truth for:

- authentication;
- authorization;
- roles;
- permissions;
- tenant isolation;
- validation;
- pagination;
- auditability;
- clinical data consistency.

Frontend clients must not duplicate business rules as security boundaries.

## Official client strategy

### Web client

Required direction:

- Next.js
- TypeScript
- Centralized API client
- OpenAPI-generated types or typed wrappers matching OpenAPI

### iOS client

Required direction:

- Swift
- SwiftUI
- URLSession or approved networking layer
- OpenAPI-generated Swift client when practical
- Typed models aligned with backend DTOs

Swift and SwiftUI are mandatory for the official iOS client because they are a project requirement from Tec and Cáritas.

React Native, Expo, Flutter, or other cross-platform alternatives may be useful for prototypes, but they do not replace the official Swift iOS deliverable.

### Android client

Recommended direction:

- Kotlin
- Jetpack Compose
- Retrofit, Ktor, or approved networking layer
- OpenAPI-generated Kotlin client when practical
- Typed models aligned with backend DTOs

## Mandatory integration rules

1. Do not invent DTOs independently in each client.
2. Do not call random URLs directly from UI components.
3. Use a centralized API client per platform.
4. Handle `ApiResponse<T>` globally.
5. Handle `PaginatedResponse<T>` globally.
6. Handle `ApiErrorResponse` globally.
7. Handle 401 and 403 globally.
8. Do not trust frontend permission checks as security boundaries.
9. Do not store tokens in unsafe storage.
10. Do not log PHI, PII, tokens, signatures, or clinical payloads.
11. Use pagination metadata for list screens.
12. Use `traceId` when reporting errors.
13. Keep API contracts aligned with OpenAPI.

## API response contracts

Success responses use `ApiResponse<T>`.

Paginated success responses use `ApiResponse<PaginatedResponse<T>>`.

Error responses use `ApiErrorResponse`.

## Breaking change rule

Once frontend work starts, these are breaking changes:

- changing DTO property names;
- changing enum values;
- changing required fields;
- changing route shape;
- changing pagination response shape;
- changing authentication behavior;
- changing permission requirements;
- removing fields used by clients;
- changing error response structure.

Breaking changes require:

1. issue;
2. pull request note;
3. updated OpenAPI;
4. regenerated clients or typed wrappers;
5. migration note;
6. affected frontend validation.

## Student contributor rule

Students may build frontend modules only against documented contracts.

They may not:

- bypass the API client;
- hardcode permissions as security logic;
- use production data;
- store sensitive data locally without approved storage;
- create unofficial API routes;
- copy backend DTOs manually without review.