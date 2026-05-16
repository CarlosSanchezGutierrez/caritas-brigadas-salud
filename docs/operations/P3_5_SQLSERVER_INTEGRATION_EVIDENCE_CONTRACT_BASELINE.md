# P3.5-02 SQL Server Integration Evidence Contract Baseline

## Status

Required before connecting the backend to a real Caritas/Tec SQL Server environment.

This document is not a production approval and must not include secrets.

## Purpose

Define the required evidence to connect the ASP.NET Core backend to the SQL Server infrastructure owned or operated by Caritas/Tec.

## Core rule

SQL Server is not the backend.

The backend is the API. SQL Server is the persistence layer.

Allowed production path:

- Client -> HTTPS -> API -> SQL Server.

Forbidden production path:

- Client -> SQL Server.
- Mobile app -> SQL Server.
- Web admin -> SQL Server.
- SQL credentials embedded in any client.

## Evidence package required

The SQL Server evidence package must include:

- Environment name.
- SQL Server owner.
- SQL Server version.
- SQL Server edition if available.
- Hostname or private endpoint.
- Network path from API host to SQL Server.
- Database name.
- Application login name.
- Migration login name or approved migration execution process.
- Minimum-privilege permissions.
- Connection encryption setting.
- TrustServerCertificate decision.
- Backup policy.
- Restore test evidence.
- Rollback plan.
- Migration script review evidence.
- Deployment smoke test evidence.
- Database connectivity health evidence.
- Audit/logging configuration evidence.

## Secrets rule

No connection string, password, token, certificate private key, or secret value may be committed to the repository.

Allowed evidence:

- Secret name.
- Secret provider.
- Environment variable name.
- Key Vault reference.
- Rotation owner.
- Rotation cadence.

Forbidden evidence:

- Plaintext SQL password.
- Plaintext connection string with password.
- Private key.
- Production certificate material.
- Shared admin credentials.

## SQL login separation

The runtime API login should not own schema migrations by default.

Required decision:

- Separate runtime and migration users; or
- Explicitly approved single-user temporary pilot exception.

The preferred model is:

- Runtime login: read/write only to required application tables and stored procedures if used.
- Migration login: DDL permissions only during controlled deployment windows.
- Human DBA/admin: break-glass only.

## Migration execution rule

Migrations must not run automatically at API startup in production.

Allowed migration execution paths:

- Reviewed SQL script.
- EF migration bundle.
- DBA-reviewed deployment script.
- Controlled CI/CD deployment step.

Required migration evidence:

- Script generated.
- Script reviewed.
- Script applied to staging.
- Rollback script or rollback decision.
- Backup exists before migration.
- Migration timestamp recorded.

## Network security requirements

The SQL Server network path must define:

- Source host.
- Destination host.
- Port.
- Protocol.
- Firewall rule.
- ACL owner.
- Deny-by-default posture.
- Whether access is private network only.
- Whether VPN/site-to-site/private endpoint is used.
- Whether SQL Server is publicly exposed.

SQL Server should not be publicly exposed.

## Connection string policy

The application connection string must define:

- Encrypt=True or equivalent encryption requirement.
- TrustServerCertificate decision.
- Connection timeout.
- Command timeout.
- Database name.
- Application name.
- Least-privilege user.
- No secrets in source code.
- No secrets in mobile apps.
- No secrets in web bundles.

## Backup and restore requirements

Required before production:

- Backup frequency.
- Backup retention.
- Backup encryption.
- Backup storage location.
- Restore procedure.
- Restore test date.
- Restore test owner.
- Recovery time objective.
- Recovery point objective.

## Observability requirements

Required SQL observability:

- Database connectivity health check.
- Failed connection visibility.
- Slow query visibility or decision.
- Migration failure visibility.
- Sync write failure visibility.
- Deadlock/error visibility or decision.
- Alert routing.

## Data protection requirements

Required SQL data protection:

- Encryption in transit.
- Encryption at rest or explicit infrastructure decision.
- Backup encryption.
- Sensitive field classification.
- Access audit or compensating control.
- Export control.
- Retention policy.
- Deletion/soft-delete policy.

## Load and resilience requirements

Required before production pilot:

- Expected daily patients.
- Expected events per batch.
- Expected concurrent users.
- Expected offline retry volume.
- Maximum request payload.
- SQL connection pool policy.
- Baseline load smoke.
- Retry behavior.
- Dead-letter or failed sync evidence.

## Final readiness states

- BLOCKED.
- READY FOR STAGING SQL.
- READY FOR PILOT SQL.
- READY FOR PRODUCTION SQL.

Default state is BLOCKED.