# P3.5-01 Production Environment Contract Baseline

## Status

Baseline required before frontend/iOS/Android implementation is treated as production-bound.

This document is not a production approval.

## Purpose

Define the minimum production environment contract for Caritas Brigadas de Salud before connecting the backend to real Caritas/Tec infrastructure and before treating any mobile or web client as production-ready.

## Non-negotiable architecture rule

Clients must never connect directly to SQL Server.

Allowed flow:

- iOS client -> HTTPS -> API -> SQL Server.
- Android client -> HTTPS -> API -> SQL Server.
- Web admin client -> HTTPS -> API -> SQL Server.

Forbidden flow:

- iOS client -> SQL Server.
- Android client -> SQL Server.
- Web client -> SQL Server.
- Direct database credentials embedded in clients.

## Required environment layers

The production environment must define:

- DNS and domain strategy.
- TLS certificate strategy.
- Reverse proxy or hosting strategy.
- API hosting location.
- SQL Server location.
- SQL Server network path.
- Firewall and ACL policy.
- Deny-by-default ingress rule.
- Secrets provider.
- Authentication provider.
- Backup location.
- Restore process.
- Rollback process.
- Monitoring and alerting owner.
- Incident response owner.
- Deployment owner.
- Database migration owner.

## SQL Server requirements

The backend must connect to SQL Server through controlled server-side credentials only.

Required SQL Server evidence:

- Target SQL Server version.
- Hostname or private endpoint.
- Environment name.
- Database name.
- Minimum-privilege application login.
- Migration login separation decision.
- Backup policy.
- Restore test evidence.
- Connection string stored as secret.
- TLS/encryption setting.
- Network restrictions.
- Audit/logging settings.

## Deployment requirements

Production deployment must define:

- Deployment mechanism.
- Artifact source.
- Configuration source.
- Secret injection.
- Health check URL.
- Rollback command.
- Release approver.
- Migration execution process.
- Deployment smoke test.
- Deployment evidence record.

Migrations must not run automatically at API startup.

## Authentication requirements

Production must not use development-only headers or bypasses.

Required production auth decision:

- OIDC provider.
- Token issuer.
- Audience.
- Role/group mapping.
- Permission mapping.
- Admin bootstrap process.
- Break-glass procedure.
- Token lifetime.
- Refresh strategy.
- Logout/session invalidation strategy.

## Mobile readiness requirements

Mobile clients must use:

- HTTPS only.
- No SQL credentials.
- No embedded production secrets.
- Device/client instance identifier strategy.
- Offline queue policy.
- Idempotency key policy.
- Retry policy.
- Conflict display policy.
- Local storage encryption policy.
- Remote wipe or session revocation decision.
- App Store / Play Store release configuration separation.

## Web admin readiness requirements

The web admin client must use:

- HTTPS only.
- Role-based access.
- Export authorization.
- Export audit logging.
- Sensitive field masking.
- No direct database access.
- No production secrets in frontend bundles.

## Data protection requirements

Production must define:

- Data classification.
- PHI/PII fields.
- Fields forbidden in logs.
- Fields forbidden in analytics.
- Encryption in transit.
- Encryption at rest.
- Backup encryption.
- Key management.
- Data retention.
- Data deletion policy.
- Export controls.
- Audit retention.

## Observability requirements

Production must provide:

- /health/live.
- /health/ready.
- Structured logs.
- Correlation id.
- Error rate visibility.
- Latency visibility.
- 401/403/429/5xx visibility.
- Database connectivity visibility.
- Sync failure visibility.
- Alert owner.
- Incident runbook link.

## Security requirements

Production must define:

- CORS allowed origins.
- AllowedHosts.
- Rate limiting.
- Request body size limits.
- File upload limits if enabled.
- Secret scanning.
- Dependency scanning.
- CodeQL/security scanning.
- SBOM.
- Vulnerability review cadence.
- OWASP baseline test plan.
- Tenant isolation test plan.
- Authorization bypass test plan.

## AI Gateway decision

The AI Gateway must remain disabled until a dedicated ADR exists.

Minimum future rules:

- Feature flag required.
- Admin-only enablement.
- No PHI by default.
- Prompt/version audit.
- Cost limits.
- Rate limits.
- Human review path.
- Incident kill switch.

## Crypto audit / blockchain decision

Blockchain must not be required for production MVP.

Allowed near-term approach:

- Hash-only audit integrity research.
- Merkle root or hash chain proof of concept.
- No PHI on-chain.
- No dependency on public blockchain for clinical operations.
- Educational cryptography module only after production backend is stable.

## Production readiness decision

Backend is not production-ready until this contract has real evidence, not just documentation.

Required final decision states:

- BLOCKED.
- READY FOR STAGING.
- READY FOR PILOT.
- READY FOR PRODUCTION.

Default state is BLOCKED.