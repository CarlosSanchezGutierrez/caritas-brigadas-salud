# P3.5-04 Encryption and Data Protection Contract Baseline

## Status

Required before staging, pilot, production, App Store, Play Store, or web admin production release.

This document is not a production approval.

## Purpose

Define the non-negotiable encryption, key management, data classification, logging, export, backup, mobile local storage, and SQL Server data protection rules for Caritas Brigadas de Salud.

## Core rule

The project must not claim full end-to-end encryption for the entire clinical record if the backend must read data for reporting, validation, synchronization, administration, analytics, or operational support.

Required production model:

- Encryption in transit.
- Encryption at rest.
- Backup encryption.
- Mobile local storage encryption.
- Field-level protection decision for sensitive fields.
- Secret-backed key management.
- Log redaction.
- Export controls.
- Server-side authorization.
- Audit trail.

## End-to-end encryption decision

Full end-to-end encryption is not the default architecture for the complete patient record.

Reason:

- The API must validate and process clinical records.
- The API must generate reports.
- The API must synchronize offline records.
- Offices must produce administrative summaries.
- Search, deduplication, conflict resolution, and reporting require server-side processing.

Allowed future limited E2EE scopes:

- Specific attachments.
- Specific consent artifacts.
- Specific sealed documents.
- Specific research module approved by privacy/security review.

Forbidden claims:

- Do not claim full E2EE for all data unless the backend cannot decrypt that data.
- Do not claim zero-knowledge clinical processing while the API reads clinical records.
- Do not put PHI into public blockchain or public immutable stores.

## Data classification requirements

Production must classify fields as:

- Public.
- Operational.
- Internal.
- PII.
- PHI.
- Sensitive health data.
- Emergency contact sensitive data.
- Insurance sensitive data.
- Audit/security data.
- Secret/credential material.

Required classification areas:

- Patient identity.
- Patient contact data.
- Emergency contact data.
- Insurance/social security data.
- Clinical notes.
- Vital signs.
- Services provided.
- Consent and signature evidence.
- Referrals.
- Medication delivery.
- Sync metadata.
- Audit logs.
- Exports.
- Backups.
- Telemetry.

## Encryption in transit

Required:

- HTTPS only.
- TLS certificate.
- No plaintext HTTP in production.
- HSTS decision.
- Secure cookies if cookies are used.
- No SQL Server exposure to clients.
- SQL Server connection encryption decision.
- Mobile clients must reject non-production insecure endpoints unless explicitly using local development.

## Encryption at rest

Required:

- SQL Server encryption at rest decision.
- Backup encryption.
- Secret storage encryption.
- Mobile local storage encryption.
- Export storage encryption.
- Telemetry credential protection.
- Key ownership.
- Key rotation.
- Emergency key rotation.

Allowed SQL Server options:

- Transparent Data Encryption if available.
- Infrastructure disk encryption.
- Backup encryption.
- Column/field-level encryption where justified.
- Always Encrypted only if query/reporting limitations are accepted.

## Field-level encryption decision

Field-level encryption is not automatic for every column.

Required decision per field group:

- Encrypt at database/storage layer only.
- Encrypt at application field level.
- Mask in API responses.
- Exclude from analytics.
- Exclude from exports by default.
- Redact from logs.
- Retain in plaintext only with explicit justification.

Field-level encryption must consider:

- Search requirements.
- Reporting requirements.
- Deduplication requirements.
- Sync conflict requirements.
- Key rotation requirements.
- Performance.
- Operational recovery.

## Mobile local data protection

iOS and Android offline mode must define:

- Local database/storage engine.
- Local encryption strategy.
- Keychain/Keystore usage.
- Biometric/PIN decision.
- Session timeout.
- Lost device procedure.
- Remote revoke procedure.
- Offline queue retention.
- Local data wipe.
- Background screenshot/cache decision.
- Clipboard policy if applicable.
- Jailbreak/root detection decision.

## Logging and telemetry protection

Never log:

- Access tokens.
- Refresh tokens.
- Authorization headers.
- Cookies.
- SQL passwords.
- Connection strings.
- Private keys.
- Client secrets.
- Patient PHI/PII.
- Emergency contact data.
- Insurance/social security data.
- Consent signature binary data.
- Raw clinical notes unless explicitly approved for a secure audit reason.

Required:

- Structured logs.
- Correlation id.
- Redacted error payloads.
- No raw request body logging for clinical endpoints.
- No raw response body logging for clinical endpoints.
- Security event logging without secret leakage.

## Export protection

Exports must define:

- Who can export.
- What fields can be exported.
- Whether sensitive fields are masked.
- Export audit logging.
- Export file encryption decision.
- Export retention.
- Export deletion.
- Download expiry.
- Watermarking decision.
- Re-identification risk review for analytics exports.

## Backup protection

Backups must define:

- Encryption.
- Storage location.
- Retention.
- Access owner.
- Restore owner.
- Restore test.
- Emergency access.
- Key dependency.
- RTO.
- RPO.
- Deletion process.

## Key management requirements

Production must define:

- Key provider.
- Key owner.
- Key purpose.
- Key rotation cadence.
- Emergency rotation process.
- Key backup/escrow decision.
- Key access audit or compensating control.
- Separation of duties.
- Break-glass process.
- Revocation process.

Allowed providers:

- Azure Key Vault.
- AWS KMS.
- AWS Secrets Manager for secret material.
- HashiCorp Vault.
- Institutional KMS/secret manager approved by Caritas/Tec.

## Analytics and science data protection

Analytics must not bypass privacy.

Required:

- De-identification decision.
- Aggregation decision.
- Minimum cell size decision.
- PHI exclusion decision.
- Export approval.
- Re-identification risk review.
- Data retention.
- Data sharing approval.
- Research dataset approval if used.

## Crypto audit / blockchain protection

Blockchain is not required for production MVP.

Allowed:

- Hash chain.
- Merkle root.
- Integrity proof.
- Internal audit digest.
- No PHI on-chain.
- No public blockchain dependency for clinical workflow.

Forbidden:

- Patient PHI on-chain.
- Consent content on public chain.
- Emergency contact data on-chain.
- Insurance data on-chain.
- Any irreversible disclosure of clinical data.

## AI Gateway data protection

AI Gateway must remain disabled until a dedicated ADR exists.

Minimum future requirements:

- No PHI by default.
- Prompt redaction.
- Model provider risk review.
- Data retention review.
- Cost/rate limit.
- Human review.
- Kill switch.
- Audit trail.

## Final readiness states

- BLOCKED.
- READY FOR STAGING DATA PROTECTION.
- READY FOR PILOT DATA PROTECTION.
- READY FOR PRODUCTION DATA PROTECTION.

Default state is BLOCKED.