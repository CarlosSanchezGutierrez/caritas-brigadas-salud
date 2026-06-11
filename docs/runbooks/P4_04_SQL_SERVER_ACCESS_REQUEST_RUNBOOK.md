# P4.4 SQL Server Access Request Runbook

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

This runbook gives the project team a safe request format for asking Cáritas, Tec, or the accountable infrastructure owner for the SQL Server access package required to close the P1 database-config blocker.

## Do not request secrets through GitHub

Never request passwords, tokens, private keys, or connection string values through:

- GitHub issues
- GitHub pull requests
- Markdown documents
- chat screenshots
- public project boards
- committed `.env` files

Secrets must be transmitted through an approved secure channel.

## Access package request

Send the following request to the accountable institutional owner:

```text
Subject: Cáritas Brigadas de Salud - SQL Server access package required for P4 real evidence

Hello,

The P4 real evidence run for Cáritas Brigadas de Salud detected a P1 database-config blocker:

ConnectionStrings__SqlServer missing.

To continue with real backend evidence without fabricating readiness, we need the approved SQL Server access package for the correct environment.

Required information:

1. SQL Server host and instance.
2. Database name.
3. Environment classification: development, test, staging, or production.
4. Authentication mode.
5. Least-privilege runtime credential or approved integrated authentication path.
6. Migration permission boundary.
7. Network access method: VPN, allowlist, firewall, or private route.
8. TLS or certificate trust requirements.
9. Backup and restore owner.
10. Data owner.
11. Operations owner.
12. Security owner.
13. Privacy owner.
14. Whether this access is approved only for testing or also for deployment validation.

Please do not send secrets through GitHub or public channels.
```

## Local validation after access is received

After the institutional access package is received, run the P4.1 collector from a clean terminal session where `ConnectionStrings__SqlServer` is present.

Standard command:

```powershell
& "scripts/p4/collect-p4-01-real-evidence-baseline.ps1"
```

Then classify the generated manifest:

```powershell
& "scripts/p4/classify-p4-01-evidence-package.ps1" -ManifestPath "<path-to-manifest.json>"
```

## Expected closure evidence

The next classification must show:

```text
P1 blocker candidates: 0
```

And the blocker backlog must not contain:

```text
ConnectionStrings__SqlServer missing.
```

## Guardrails

- No secrets in repository.
- No fabricated evidence.
- No backend production readiness approval.
- No direct mobile write to SQL Server.
- No client may bypass the API.
- SQL Server remains the operational source of truth.
- Backend production readiness remains BLOCKED_PENDING_REAL_EVIDENCE.