# P3.5-01 Production Environment Contract

## Current decision

Status: BLOCKED

The backend has reached a P3 functional-contract milestone, but it is not production-ready until the real deployment environment, SQL Server connectivity, authentication, secrets, backups, restore, rollback, monitoring, and incident evidence are completed.

## Scope

This contract governs the backend production path for:

- iOS app.
- Android app.
- Web admin portal.
- API.
- SQL Server.
- Offline sync.
- Reporting.
- Audit.
- Future AI Gateway.
- Future crypto audit lab.

## Core architecture

```text
iOS / Android / Web Admin
        |
        | HTTPS only
        v
ASP.NET Core API
        |
        | private network / controlled server-side credentials
        v
SQL Server

Direct client-to-database access is forbidden.

Production environment inventory
ItemRequiredCurrent statusOwnerEvidence
API hosting targetYesPENDINGPENDINGPENDING
SQL Server targetYesPENDINGPENDINGPENDING
DNS/domainYesPENDINGPENDINGPENDING
TLS certificateYesPENDINGPENDINGPENDING
Reverse proxy / hosting runtimeYesPENDINGPENDINGPENDING
Firewall / ACLYesPENDINGPENDINGPENDING
Deny-by-default network policyYesPENDINGPENDINGPENDING
Secrets providerYesPENDINGPENDINGPENDING
Production auth providerYesPENDINGPENDINGPENDING
Backup policyYesPENDINGPENDINGPENDING
Restore evidenceYesPENDINGPENDINGPENDING
Rollback evidenceYesPENDINGPENDINGPENDING
Observability dashboardYesPENDINGPENDINGPENDING
Alerting ownerYesPENDINGPENDINGPENDING
Incident response ownerYesPENDINGPENDINGPENDING
SQL Server contract

The SQL Server integration must satisfy:

Application user has minimum privileges.
Migration execution is separate from runtime execution or explicitly approved.
Connection string is stored as a secret.
SQL Server is not exposed to mobile or web clients.
API is the only supported application access path.
Backups are encrypted.
Restore is tested.
Migration scripts are reviewed before execution.
No automatic production migration at API startup.
Authentication contract

Production must use real token-based authentication.

Allowed candidates:

Microsoft Entra ID / Azure AD.
Auth0.
Keycloak.
Institutional OIDC provider.

Forbidden in production:

Development authentication headers.
Static admin tokens.
Shared passwords.
Client-side role claims without backend validation.
Production bypass flags.
Mobile client contract

iOS and Android clients must satisfy:

HTTPS only.
No database credentials.
No backend secrets.
Environment-specific configuration.
Offline queue.
Idempotent sync.
Retry-safe sync.
Conflict-aware UI.
Encrypted local storage for sensitive records.
Session expiration.
Remote revoke strategy.
App Store / Play Store release separation.
Web admin contract

The admin web application must satisfy:

HTTPS only.
RBAC.
Export permissions.
Export audit logs.
Sensitive field masking.
No direct SQL Server access.
No production secrets in frontend bundles.
Report access traceability.
Data protection contract

Required before production:

Field-level data classification.
Log redaction list.
Export redaction/masking decision.
Encryption in transit.
Encryption at rest.
Backup encryption.
Key ownership.
Retention policy.
Deletion policy.
Audit retention policy.
Observability contract

Required before production:

Liveness health check.
Readiness health check.
Database connectivity health check.
Structured logs.
Correlation id in response and logs.
Sync failure metrics.
Auth failure metrics.
429/rate-limit metrics.
5xx metrics.
Alert routing.
Incident runbook.
Security testing contract

Required before production:

CodeQL clean or reviewed.
Dependency review clean or reviewed.
Secret scanning clean.
SBOM generated.
OWASP baseline test performed.
Tenant isolation tests.
Authorization bypass tests.
Payload limit tests.
Offline sync retry tests.
SQL injection regression tests.
Rate limiting tests.
AI Gateway decision

AI Gateway is deferred.

Status: DISABLED BY DEFAULT

No AI feature may process PHI until a dedicated privacy/security ADR is approved.

Crypto audit / blockchain decision

Blockchain is deferred.

Allowed work:

Hash-chain or Merkle-root audit proof of concept.
No PHI in hashes without privacy review.
No public-chain dependency for production MVP.
Final readiness states
StateMeaning
BLOCKEDMissing required production evidence
READY FOR STAGINGCan deploy to controlled non-production environment
READY FOR PILOTCan run limited controlled pilot
READY FOR PRODUCTIONCan be used operationally with approved evidence

Current state: BLOCKED

Next action

Create evidence packages for:

SQL Server connectivity.
Secrets/auth.
Backup/restore/rollback.
Observability.
Security test gate.