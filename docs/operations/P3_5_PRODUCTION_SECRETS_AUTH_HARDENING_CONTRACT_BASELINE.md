# P3.5-03 Production Secrets and Auth Hardening Contract Baseline

## Status

Required before staging, pilot, production, App Store, Play Store, or web admin production release.

This document is not a production approval.

## Purpose

Define the non-negotiable production rules for secrets management, authentication, authorization, token validation, bootstrap access, break-glass access, environment separation, and client configuration.

## Core rule

Production must not depend on development-only authentication.

Forbidden in production:

- Development authentication headers.
- Static admin tokens.
- Shared admin passwords.
- Hardcoded secrets.
- SQL credentials in mobile apps.
- SQL credentials in web frontend bundles.
- Backend secrets in iOS, Android, or web clients.
- Auth bypass flags.
- Localhost-only authentication assumptions.
- Public anonymous write access to clinical endpoints.

## Secret management requirements

Production must define:

- Secret provider.
- Secret owner.
- Secret rotation owner.
- Rotation cadence.
- Emergency rotation process.
- Secret naming convention.
- Environment variable mapping.
- CI/CD secret injection process.
- Local development secret process.
- Staging secret process.
- Production secret process.
- Audit trail for secret access or compensating control.

Allowed providers:

- Azure Key Vault.
- AWS Secrets Manager.
- HashiCorp Vault.
- GitHub Actions secrets for CI/CD only.
- Institutional secret manager approved by Caritas/Tec.

Forbidden storage:

- appsettings.Production.json with plaintext secrets.
- source code.
- committed .env files.
- mobile app bundles.
- frontend bundles.
- screenshots in repository.
- documentation with real secret values.

## Required secret inventory

The production inventory must classify:

- SQL Server runtime connection string.
- SQL Server migration credential if used.
- OIDC authority/client/audience values.
- JWT signing/validation material if applicable.
- API encryption keys if applicable.
- backup encryption keys if applicable.
- external API keys if applicable.
- AI Gateway keys if future feature is enabled.
- telemetry exporter credentials if applicable.
- email/SMS credentials if applicable.

## Authentication requirements

Production authentication must use a real token-based provider.

Allowed candidates:

- Microsoft Entra ID / Azure AD.
- Auth0.
- Keycloak.
- Institutional OIDC provider.
- Another approved OIDC/OAuth2 provider.

Required OIDC evidence:

- Authority.
- Issuer.
- Audience.
- JWKS validation.
- Token lifetime.
- Clock skew.
- Role/group claim mapping.
- Permission mapping.
- Admin bootstrap process.
- Logout/session invalidation decision.
- Refresh/session renewal decision.
- Emergency access procedure.
- Disabled-user revocation process.

## Authorization requirements

Authorization must remain server-enforced.

Required rules:

- Backend validates tokens.
- Backend validates roles/permissions.
- Frontend role checks are usability only, not security.
- Mobile role checks are usability only, not security.
- Organization/tenant boundary is enforced in backend queries.
- Admin operations require explicit permissions.
- Exports require explicit permissions.
- Sync writes require explicit permissions.
- Reporting endpoints require explicit permissions.
- Development bypasses are blocked in production.

## Bootstrap admin requirements

Production must define how the first administrator is created.

Required evidence:

- Bootstrap owner.
- Bootstrap method.
- Bootstrap expiry.
- Audit trail.
- Manual approval.
- Rollback/removal process.
- No permanent bootstrap backdoor.

## Break-glass requirements

Break-glass access must be controlled.

Required evidence:

- Break-glass owner.
- Allowed emergency scenarios.
- Approval process.
- Expiration process.
- Audit logging.
- Post-incident review.
- Credential rotation after use.

## Environment separation

Required environments:

- Local development.
- CI.
- Staging.
- Pilot if applicable.
- Production.

Each environment must define:

- API base URL.
- Auth authority.
- Auth audience.
- SQL Server secret reference.
- CORS origins.
- AllowedHosts.
- Logging level.
- Telemetry destination.
- Feature flags.
- AI Gateway disabled by default.
- Crypto audit lab disabled by default unless explicitly enabled outside production clinical workflow.

## Mobile client configuration

iOS and Android must not contain production secrets.

Allowed in mobile apps:

- API base URL.
- OIDC client id if public-client flow requires it.
- Redirect URI.
- Non-secret environment identifiers.
- Feature flags that do not grant access by themselves.

Forbidden in mobile apps:

- SQL credentials.
- Backend API secrets.
- Client secrets.
- Admin tokens.
- Migration credentials.
- Production connection strings.
- Private keys.

Required mobile auth decisions:

- Public client flow.
- Redirect URI.
- Token storage.
- Secure enclave/keychain/keystore decision.
- Session timeout.
- Offline access policy.
- Logout behavior.
- Remote revocation behavior.
- Lost device procedure.

## Web admin configuration

The web admin must not contain production secrets.

Required web decisions:

- Public runtime config only.
- No backend secrets in frontend bundles.
- API base URL per environment.
- OIDC provider per environment.
- Role-based UI only as convenience.
- Backend remains source of truth for authorization.
- Export permission enforced by API.

## Token validation requirements

API token validation must define:

- Validate issuer.
- Validate audience.
- Validate lifetime.
- Validate signing key.
- Validate algorithm policy.
- Clock skew.
- Required claims.
- Role/group mapping.
- Organization/tenant resolution.
- Rejected token logging without token leakage.
- No token values in logs.

## Logging requirements

Never log:

- Access tokens.
- Refresh tokens.
- Authorization headers.
- Cookies.
- SQL passwords.
- Connection strings.
- Private keys.
- Client secrets.
- OIDC secrets.
- Patient PHI/PII.
- Emergency contact sensitive data.
- Insurance sensitive data.

## CI/CD requirements

CI/CD must define:

- Secret injection.
- Least-privilege CI credentials.
- No secrets printed in logs.
- Secret scanning.
- Dependency review.
- CodeQL/security scans.
- Environment approvals.
- Production deploy approval.
- Rollback authority.

## Final readiness states

- BLOCKED.
- READY FOR STAGING AUTH.
- READY FOR PILOT AUTH.
- READY FOR PRODUCTION AUTH.

Default state is BLOCKED.