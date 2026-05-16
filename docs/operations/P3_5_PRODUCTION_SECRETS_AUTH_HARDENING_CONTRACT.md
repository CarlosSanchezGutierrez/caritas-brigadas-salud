# P3.5-03 Production Secrets and Auth Hardening Contract

## Current decision

Status: BLOCKED

Production secrets and authentication are not approved until real secret provider, OIDC provider, role mapping, bootstrap process, break-glass process, and environment separation evidence exist.

## Scope

This contract applies to:

- ASP.NET Core API.
- SQL Server connection strings.
- EF migration credentials.
- iOS app.
- Android app.
- Web admin.
- CI/CD.
- Staging.
- Pilot.
- Production.
- Future AI Gateway.
- Future crypto audit lab.

## Non-negotiable production rules

Production must not use:

- Development authentication headers.
- Static admin tokens.
- Hardcoded secrets.
- Shared passwords.
- Auth bypass flags.
- SQL credentials in clients.
- Backend secrets in clients.
- Plaintext connection strings in repository.
- Production secrets in appsettings files.

## Secret provider decision

| Evidence item | Required | Current status | Owner | Evidence |
|---|---:|---|---|---|
| Secret provider selected | Yes | PENDING | PENDING | PENDING |
| Secret naming convention | Yes | PENDING | PENDING | PENDING |
| Secret owner | Yes | PENDING | PENDING | PENDING |
| Rotation owner | Yes | PENDING | PENDING | PENDING |
| Rotation cadence | Yes | PENDING | PENDING | PENDING |
| Emergency rotation process | Yes | PENDING | PENDING | PENDING |
| CI/CD secret injection process | Yes | PENDING | PENDING | PENDING |
| Staging secret separation | Yes | PENDING | PENDING | PENDING |
| Production secret separation | Yes | PENDING | PENDING | PENDING |

## Required secret inventory

| Secret | Required | Repository allowed? | Client allowed? | Current status |
|---|---:|---:|---:|---|
| SQL runtime connection string | Yes | No | No | PENDING |
| SQL migration credential | Yes if migrations use separate identity | No | No | PENDING |
| OIDC authority | Yes | Yes if non-secret | Yes if non-secret | PENDING |
| OIDC audience | Yes | Yes if non-secret | Yes if non-secret | PENDING |
| OIDC client id | Yes if public client | Yes if non-secret | Yes if public client | PENDING |
| OIDC client secret | If confidential client | No | No | PENDING |
| JWT validation material | If applicable | No if secret/private | No | PENDING |
| Backup encryption key | If applicable | No | No | PENDING |
| Telemetry credential | If applicable | No | No | PENDING |
| AI Gateway key | Future only | No | No | DISABLED |
| External notification credential | If applicable | No | No | PENDING |

## Authentication provider decision

Allowed candidates:

- Microsoft Entra ID / Azure AD.
- Auth0.
- Keycloak.
- Institutional OIDC provider.
- Approved OAuth2/OIDC provider.

Current selected provider: PENDING

Required evidence:

| Evidence item | Required | Current status |
|---|---:|---|
| Authority | Yes | PENDING |
| Issuer | Yes | PENDING |
| Audience | Yes | PENDING |
| JWKS validation | Yes | PENDING |
| Token lifetime | Yes | PENDING |
| Clock skew | Yes | PENDING |
| Role/group claim mapping | Yes | PENDING |
| Permission mapping | Yes | PENDING |
| Disabled-user revocation process | Yes | PENDING |
| Logout/session invalidation decision | Yes | PENDING |
| Refresh/session renewal decision | Yes | PENDING |

## Authorization policy

Authorization must be enforced by the backend.

Frontend and mobile role checks are not security controls.

Required API rules:

- Backend validates tokens.

- Validate token issuer.
- Validate token audience.
- Validate token lifetime.
- Validate signing key.
- Validate required claims.
- Map external groups/roles to internal permissions.
- Enforce organization boundary in backend queries.
- Require explicit permissions for admin endpoints.
- Require explicit permissions for sync writes.
- Require explicit permissions for exports.
- Require explicit permissions for reports.
- Reject development auth bypasses in production.

## Bootstrap admin process

| Evidence item | Required | Current status |
|---|---:|---|
| Bootstrap owner | Yes | PENDING |
| Bootstrap method | Yes | PENDING |
| Manual approval | Yes | PENDING |
| Audit trail | Yes | PENDING |
| Expiration/removal process | Yes | PENDING |
| No permanent backdoor | Yes | PENDING |

## Break-glass process

| Evidence item | Required | Current status |
|---|---:|---|
| Break-glass owner | Yes | PENDING |
| Emergency scenarios | Yes | PENDING |
| Approval process | Yes | PENDING |
| Expiration process | Yes | PENDING |
| Audit logging | Yes | PENDING |
| Post-incident review | Yes | PENDING |
| Credential rotation after use | Yes | PENDING |

## Environment separation

| Environment | API URL | Auth authority | SQL secret | CORS origins | Status |
|---|---|---|---|---|---|
| Local development | PENDING | PENDING | PENDING | PENDING | PENDING |
| CI | PENDING | PENDING | PENDING | PENDING | PENDING |
| Staging | PENDING | PENDING | PENDING | PENDING | PENDING |
| Pilot | PENDING | PENDING | PENDING | PENDING | PENDING |
| Production | PENDING | PENDING | PENDING | PENDING | PENDING |

## iOS and Android release hardening

Mobile apps must not contain:

- SQL credentials.
- Backend secrets.
- OIDC client secrets.
- Admin tokens.
- Private keys.
- Production connection strings.
- Migration credentials.

Required mobile evidence:

| Evidence item | Required | Current status |
|---|---:|---|
| API base URL per environment | Yes | PENDING |
| OIDC public client flow | Yes | PENDING |
| Redirect URI | Yes | PENDING |
| Token storage decision | Yes | PENDING |
| Keychain/Keystore strategy | Yes | PENDING |
| Session timeout | Yes | PENDING |
| Logout behavior | Yes | PENDING |
| Remote revocation behavior | Yes | PENDING |
| Lost device procedure | Yes | PENDING |
| Offline access policy | Yes | PENDING |
| App Store release config separation | Yes | PENDING |
| Play Store release config separation | Yes | PENDING |

## Web admin release hardening

Web admin must not contain:

- Backend secrets.
- SQL credentials.
- OIDC client secrets unless using confidential flow only server-side.
- Admin tokens.
- Private keys.
- Production connection strings.

Required web evidence:

| Evidence item | Required | Current status |
|---|---:|---|
| API base URL per environment | Yes | PENDING |
| OIDC configuration per environment | Yes | PENDING |
| No secrets in frontend bundle | Yes | PENDING |
| Export authorization enforced by API | Yes | PENDING |
| Report authorization enforced by API | Yes | PENDING |
| Admin permission mapping | Yes | PENDING |

## Logging and redaction policy

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

Required evidence:

- Log redaction review.
- Security review for auth failures.
- Token rejection logs do not contain token values.
- Secret scanning clean or reviewed.

## CI/CD requirements

| Evidence item | Required | Current status |
|---|---:|---|
| GitHub Actions secrets scoped | Yes | PENDING |
| Production environment approval | Yes | PENDING |
| No secrets printed in logs | Yes | PENDING |
| Secret scanning enabled | Yes | PENDING |
| Dependency review enabled | Yes | PENDING |
| CodeQL/security scan enabled or decision | Yes | PENDING |
| Rollback authority defined | Yes | PENDING |

## AI Gateway decision

Status: DISABLED

AI Gateway keys must not exist in production until a dedicated AI privacy/security ADR is approved.

Minimum future requirements:

- Feature flag.
- Admin-only enablement.
- No PHI by default.
- Prompt/version audit.
- Cost limit.
- Rate limit.
- Kill switch.
- Human review path.

## Crypto audit lab decision

Status: DISABLED FOR PRODUCTION CLINICAL WORKFLOW

Crypto audit work may be researched only as:

- Hash chain.
- Merkle root.
- Integrity proof.
- No PHI on-chain.
- No public-chain dependency for production MVP.

## Current readiness

| State | Value |
|---|---|
| Secrets readiness | BLOCKED |
| Auth readiness | BLOCKED |
| Mobile release auth readiness | BLOCKED |
| Web admin auth readiness | BLOCKED |
| Production auth readiness | BLOCKED |

## Next required evidence

1. Select secret provider.
2. Select OIDC provider.
3. Define auth issuer/audience.
4. Define role/group mapping.
5. Define bootstrap admin process.
6. Define break-glass process.
7. Define mobile token storage.
8. Define environment-specific config.
9. Verify dev auth cannot run in production.
10. Verify no secrets are present in client bundles.