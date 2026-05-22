# P3.7 On-Prem Security and Vulnerability Map Baseline

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## On-prem threat model scope

The backend must account for internal network exposure, reverse proxy misconfiguration, SQL Server credential leakage, excessive SQL permissions, unauthorized report access, broken organization boundaries, offline device loss, sync replay, duplicate data injection, malicious file injection, audit tampering, sensitive logs, unapproved analytics exports, unapproved AI data transfer, dashboard overexposure, and backup exposure.

## Minimum controls

- Least privilege SQL Server user.
- No committed secrets.
- Auth required for protected endpoints.
- Role-based access control.
- Organization-scope authorization.
- Structured logs without sensitive payload leakage.
- Correlation ID on requests.
- Idempotency for sync and data injection.
- Audit log for critical actions.
- Backup and restore evidence.
- Export evidence.
- Security smoke tests.
- Dependency review.
- Code scanning.
- Secret scanning.

## Social vulnerability map governance

A social vulnerability and needs map must not expose identifiable patient data.

Required controls:

- Aggregation by approved territorial level.
- Small population suppression.
- Data quality confidence score.
- No exact household-level clinical exposure by default.
- Approved indicator dictionary.
- Approved interpretation guide.
- Export audit.
- Access control for direction-level dashboards.
- Research review for advanced analysis.

## AI API Gateway risk boundary

AI features must be blocked until governance exists for use case, data classification, prompt/input policy, output review policy, human approval, provider risk, logging policy, redaction policy, and incident response.

## Crypto-audit risk boundary

Blockchain or crypto-audit features must be blocked until governance exists for audit hash scope, hash algorithm, verification procedure, storage location, no patient data on-chain, no cryptocurrency dependency, and incident handling.
