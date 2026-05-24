# P4.4 Real Environment SQL Server Access Blocker

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

P4.4 formalizes the first real P1 blocker discovered by the P4.3 classified evidence run.

The P4.3 real evidence package detected:

- P0 required blockers: 0
- P1 blocker candidates: 1
- P2 optional evidence gaps: 2
- PASS items: 12
- UNKNOWN items: 0

The first P1 blocker is:

- Evidence: SQL Server configuration presence evidence
- Category: database-config
- Owner group: data owner
- Remediation type: configuration or SQL Server remediation
- Blocker: ConnectionStrings__SqlServer missing.

## Decision

This P1 blocker must not be closed with a decorative local environment variable.

A local placeholder such as a localhost development connection string is acceptable for local experiments, but it is not sufficient real environment evidence for institutional readiness.

P4.4 therefore records the blocker as an external institutional dependency until Cáritas, Tec, or the accountable infrastructure owner provides a real SQL Server access package.

## Required SQL Server access package

The accountable data owner or infrastructure owner must provide the following before the P1 can be closed:

1. SQL Server host, instance, and network access method.
2. Database name for the Cáritas Brigadas de Salud environment.
3. Authentication mode.
4. Least-privilege application credential or approved integrated authentication path.
5. Migration permission boundary.
6. Runtime permission boundary.
7. Backup and restore ownership.
8. TLS or certificate trust decision.
9. VPN, allowlist, firewall, or private network requirements.
10. Environment classification: local, development, test, staging, or production.
11. Data classification and privacy boundary.
12. Accountable data owner approval.
13. Accountable operations owner approval.
14. Accountable security owner approval.

## Real evidence needed to close the P1

The P1 may be closed only after a new P4.1 evidence package captures all of the following:

- ConnectionStrings__SqlServer is present.
- ConnectionStrings__SqlServer value is not printed.
- The evidence remains sanitized.
- The evidence package keeps Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE.
- The P4.2 classifier reports zero P1 database-config blockers.
- The result is based on a real institutional SQL Server access package or an explicitly approved institutional test environment.

## Guardrails

- No secrets in repository.
- No fabricated evidence.
- No backend production readiness approval.
- No direct mobile write to SQL Server.
- No client may bypass the API.
- No cloud dependency.
- SQL Server remains the operational source of truth.
- Backend production readiness remains BLOCKED_PENDING_REAL_EVIDENCE.

## Non-goals

P4.4 does not connect to SQL Server.

P4.4 does not approve production readiness.

P4.4 does not create fake evidence.

P4.4 does not replace the P4.1 collector or the P4.2 classifier.

P4.4 only records the institutional access blocker and defines the exact evidence required to close it.