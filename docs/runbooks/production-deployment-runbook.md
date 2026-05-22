# Production Deployment Runbook

## Purpose

Deploy Cáritas Brigadas de Salud API to a real environment with auditable evidence.

## Preconditions

- `main` or approved release branch selected.
- CI checks green.
- CodeQL clean.
- Secrets configured outside the repository.
- SQL Server target available.
- Database backup/restore procedure available.
- Rollback procedure available.

## Required configuration

Do not store values here. Store only evidence references.

- `ASPNETCORE_ENVIRONMENT`
- `ConnectionStrings__SqlServer`
- `Cors__AllowedOrigins__0`
- `Cors__AllowedOrigins__1`

Use indexed entries for CORS origins because the API reads Cors:AllowedOrigins as a string array. Add more origins as `Cors__AllowedOrigins__2`, `Cors__AllowedOrigins__3`, and so on.
- `Security__RateLimiting__Enabled`
- `Security__RateLimiting__PermitLimit`
- `Security__RateLimiting__WindowMinutes`
- `Security__RateLimiting__QueueLimit`
- `Security__MaxRequestBodyBytes`
- `ReverseProxy__ForwardedHeaders__KnownProxies__0`
- `ReverseProxy__ForwardedHeaders__KnownProxies__1`
- `ReverseProxy__ForwardedHeaders__KnownIPNetworks__0`
- `ReverseProxy__ForwardedHeaders__KnownIPNetworks__1`

Use indexed entries for forwarded-header allowlists because the API reads ReverseProxy:ForwardedHeaders:KnownProxies and ReverseProxy:ForwardedHeaders:KnownIPNetworks as string arrays. Add more entries as `ReverseProxy__ForwardedHeaders__KnownProxies__2` and `ReverseProxy__ForwardedHeaders__KnownIPNetworks__2`, and so on.
- Authentication/OIDC provider variables.

## Procedure

1. Confirm commit SHA.
2. Confirm CI checks.
3. Confirm runtime configuration.
4. Confirm SQL Server connectivity.
5. Execute migrations.
6. Deploy API.
7. Execute smoke tests.
8. Review logs.
9. Register evidence in `docs/production-evidence/evidence-register.md`.

## Minimum smoke tests

- `/`
- `/health/live`
- `/health/ready`
- protected endpoint without credentials;
- protected endpoint with valid credentials;
- representative organization endpoint;
- representative report/export endpoint when applicable.

## Rollback trigger

Rollback must be considered if:

- deployment fails;
- health/readiness fails;
- authentication breaks;
- authorization breaks;
- migration causes incompatible state;
- repeated 5xx errors appear;
- database connectivity fails;
- critical logs indicate startup/configuration failure.
