# P3.5-02 SQL Server Integration Evidence Contract

## Current decision

Status: BLOCKED

The backend must not be connected to a real Caritas/Tec SQL Server environment until this evidence package is completed.

## Scope

This contract applies to:

- ASP.NET Core API.
- SQL Server.
- EF Core migrations.
- SQL Server smoke tests.
- Deployment pipeline.
- Health checks.
- Backup/restore evidence.
- Rollback evidence.
- Mobile and web clients that depend on API persistence.

## Architecture decision

SQL Server is the database, not the backend.

The only approved application path is:

```text
iOS / Android / Web Admin
        |
        | HTTPS
        v
ASP.NET Core API
        |
        | controlled server-side connection
        v
SQL Server

Direct SQL Server access from clients is forbidden.

Required SQL Server inventory
Evidence itemRequiredCurrent statusOwnerEvidence link
Environment nameYesPENDINGPENDINGPENDING
SQL Server ownerYesPENDINGPENDINGPENDING
SQL Server versionYesPENDINGPENDINGPENDING
SQL Server hostname/private endpointYesPENDINGPENDINGPENDING
Database nameYesPENDINGPENDINGPENDING
API host network pathYesPENDINGPENDINGPENDING
Runtime SQL loginYesPENDINGPENDINGPENDING
Migration execution identityYesPENDINGPENDINGPENDING
Backup policyYesPENDINGPENDINGPENDING
Restore testYesPENDINGPENDINGPENDING
Rollback planYesPENDINGPENDINGPENDING
SQL connectivity smokeYesPENDINGPENDINGPENDING
Readiness health evidenceYesPENDINGPENDINGPENDING
Secrets and connection strings

Repository rule:

No plaintext SQL passwords.
No plaintext production connection strings.
No private keys.
No production certificate material.
No secrets in mobile apps.
No secrets in web frontend bundles.

Required evidence:

Secret itemRequiredCurrent statusOwnerEvidence
Secret provider selectedYesPENDINGPENDINGPENDING
Connection string secret nameYesPENDINGPENDINGPENDING
Runtime credential ownerYesPENDINGPENDINGPENDING
Rotation cadenceYesPENDINGPENDINGPENDING
Emergency rotation processYesPENDINGPENDINGPENDING
Runtime SQL login

The runtime API login must be minimum privilege.

Required decision:

Permission areaRuntime allowed?Current decision
SELECT app tablesPENDINGPENDING
INSERT app tablesPENDINGPENDING
UPDATE app tablesPENDINGPENDING
DELETE hard deleteNo by defaultPENDING
DDL schema changesNo by defaultPENDING
db_ownerNoPENDING
sysadminNoPENDING
backup operatorNo by defaultPENDING
Migration execution

Production migrations must not run automatically at API startup.

Allowed migration paths:

Reviewed SQL script.
EF migration bundle.
DBA-reviewed deployment script.
Controlled CI/CD deployment step.

Required migration evidence:

Evidence itemRequiredCurrent status
Migration script generatedYesPENDING
Migration script reviewedYesPENDING
Backup completed before migrationYesPENDING
Migration applied to stagingYesPENDING
Rollback script or decision existsYesPENDING
Migration result recordedYesPENDING
Network and ACL evidence

Required before staging SQL connection:

Evidence itemRequiredCurrent status
SQL Server not publicly exposedYesPENDING
API host source IP or network identifiedYesPENDING
SQL port documentedYesPENDING
Firewall rule documentedYesPENDING
Deny-by-default posture documentedYesPENDING
VPN/private network/private endpoint decisionYesPENDING
Owner of ACL changes documentedYesPENDING
SQL connection policy

Required connection policy:

Server-side only.
Secret-backed.
Environment-specific.
Encrypt=True or equivalent.
TrustServerCertificate must be explicitly approved.
Application Name must identify Caritas Brigadas API.
Connection timeout must be explicit.
Command timeout must be explicit or justified.
Pooling behavior must be accepted or configured.
Backup, restore and rollback evidence

Production is blocked until:

Backup policy exists.
Backup encryption exists or infrastructure decision exists.
Restore test passes.
Restore owner is defined.
RTO is defined.
RPO is defined.
Rollback path exists.
Deployment rollback smoke exists.
SQL smoke test evidence

Required smoke tests:

API can connect to SQL Server.
API can run readiness health check.
EF model can validate schema.
Migration script can be generated.
Migration history table exists or creation plan exists.
Basic read/write transaction succeeds in staging.
Failed credential produces controlled failure.
SQL unavailability produces readiness failure, not liveness failure.
Mobile and web implication

Mobile and web clients are allowed to depend on this backend only after:

API environment exists.
API uses HTTPS.
SQL credentials remain server-side.
Offline sync persistence is validated.
Idempotency retry behavior is validated.
Conflict behavior is validated.
Readiness health is operational.
Current readiness
StateValue
SQL Server integration stateBLOCKED
Staging SQL readinessBLOCKED
Pilot SQL readinessBLOCKED
Production SQL readinessBLOCKED
Next required evidence
Identify real SQL Server target.
Identify API hosting target.
Define network path.
Define runtime SQL login.
Define migration execution process.
Configure secret provider.
Run SQL Server smoke test.
Capture backup/restore/rollback evidence.