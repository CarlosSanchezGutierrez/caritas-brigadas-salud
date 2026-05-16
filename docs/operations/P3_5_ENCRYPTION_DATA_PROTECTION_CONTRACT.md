# P3.5-04 Encryption and Data Protection Contract

## Current decision

Status: BLOCKED

Encryption and data protection are not approved for staging, pilot, production, App Store, Play Store, or web admin production release until real evidence exists for transit encryption, rest encryption, backup encryption, mobile local encryption, key management, logging redaction, export controls, and field-level data classification.

## Scope

This contract applies to:

- ASP.NET Core API.
- SQL Server.
- EF Core persistence.
- Sync events.
- Patient records.
- Consent/signature evidence.
- Emergency contact fields.
- Insurance/social security fields.
- Audit logs.
- Exports.
- Backups.
- iOS app.
- Android app.
- Web admin.
- Future analytics.
- Future AI Gateway.
- Future crypto audit lab.

## Non-negotiable rule

Do not claim full end-to-end encryption for the complete clinical record unless the backend cannot decrypt the complete clinical record.

Current architecture requires the backend to process patient data for:

- Validation.
- Offline sync.
- Conflict handling.
- Reporting.
- Administration.
- Exports.
- Audit.
- Operational support.

Therefore, the default production claim must be:

- Encryption in transit.
- Encryption at rest.
- Backup encryption.
- Mobile local storage encryption.
- Field-level protection where justified.
- Strict authorization.
- Redacted logging.
- Audited exports.

## Data classification matrix

| Data group | Classification | Current protection decision | Status |
|---|---|---|---|
| Patient name | PII | PENDING | BLOCKED |
| Patient phone | PII | PENDING | BLOCKED |
| CURP or national identifier | PII sensitive | PENDING | BLOCKED |
| Birth date / approximate age | PII/clinical | PENDING | BLOCKED |
| Address/community | PII/location | PENDING | BLOCKED |
| Migrant status | Sensitive operational/health context | PENDING | BLOCKED |
| Clinical notes | PHI | PENDING | BLOCKED |
| Vital signs | PHI | PENDING | BLOCKED |
| Service encounters | PHI/operational | PENDING | BLOCKED |
| Consent status | Legal/PHI-adjacent | PENDING | BLOCKED |
| Signature evidence | Legal sensitive data | PENDING | BLOCKED |
| Emergency contact | Emergency contact sensitive data | PENDING | BLOCKED |
| Insurance/social security | Insurance sensitive data | PENDING | BLOCKED |
| Sync metadata | Operational/audit | PENDING | BLOCKED |
| Audit logs | Audit/security data | PENDING | BLOCKED |
| Exports | Derived sensitive data | PENDING | BLOCKED |
| Backups | Full sensitive dataset | PENDING | BLOCKED |

## Encryption in transit

Required evidence:

| Evidence item | Required | Current status |
|---|---:|---|
| HTTPS only | Yes | PENDING |
| TLS certificate | Yes | PENDING |
| No plaintext HTTP in production | Yes | PENDING |
| HSTS decision | Yes | PENDING |
| SQL Server connection encryption | Yes | PENDING |
| Mobile rejects insecure production endpoints | Yes | PENDING |
| Web admin uses HTTPS only | Yes | PENDING |

## Encryption at rest

Required evidence:

| Evidence item | Required | Current status |
|---|---:|---|
| SQL Server encryption at rest decision | Yes | PENDING |
| Backup encryption | Yes | PENDING |
| Secret storage encryption | Yes | PENDING |
| Export storage encryption decision | Yes | PENDING |
| Mobile local storage encryption | Yes | PENDING |
| Key ownership | Yes | PENDING |
| Key rotation | Yes | PENDING |
| Emergency key rotation | Yes | PENDING |

## Field-level protection decision

Field-level encryption must be explicitly decided, not assumed.

| Field group | Protection options | Current decision |
|---|---|---|
| Patient identity | Storage encryption / field encryption / masking | PENDING |
| Phone/contact | Storage encryption / field encryption / masking | PENDING |
| Emergency contact | Storage encryption / field encryption / masking / restricted export | PENDING |
| Insurance/social security | Storage encryption / field encryption / masking / restricted export | PENDING |
| Clinical notes | Storage encryption / field encryption / restricted export | PENDING |
| Consent/signature evidence | Storage encryption / artifact encryption / restricted export | PENDING |
| Audit logs | Storage encryption / retention / restricted access | PENDING |
| Exports | File encryption / masking / expiry | PENDING |

Required analysis:

- Search requirements.
- Reporting requirements.
- Deduplication requirements.
- Sync conflict requirements.
- Key rotation requirements.
- Restore requirements.
- Performance impact.
- Operational support impact.

## Mobile local data protection

iOS and Android offline mode must satisfy:

| Evidence item | Required | Current status |
|---|---:|---|
| Local storage engine selected | Yes | PENDING |
| Local encryption strategy | Yes | PENDING |
| Keychain/Keystore usage | Yes | PENDING |
| Token storage decision | Yes | PENDING |
| Offline queue retention | Yes | PENDING |
| Local data wipe | Yes | PENDING |
| Lost device procedure | Yes | PENDING |
| Remote revoke procedure | Yes | PENDING |
| Session timeout | Yes | PENDING |
| Background screenshot/cache decision | Yes | PENDING |
| Jailbreak/root detection decision | Decision required | PENDING |

## Logging and telemetry redaction

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
- Raw clinical request bodies.
- Raw clinical response bodies.

Required evidence:

| Evidence item | Required | Current status |
|---|---:|---|
| Redaction review | Yes | PENDING |
| Clinical body logging disabled | Yes | PENDING |
| Token leakage test | Yes | PENDING |
| Secret scanning clean or reviewed | Yes | PENDING |
| Sensitive field log ban reviewed | Yes | PENDING |

## Export controls

Exports require explicit controls.

| Evidence item | Required | Current status |
|---|---:|---|
| Export permission model | Yes | PENDING |
| Export audit logging | Yes | PENDING |
| Sensitive field masking decision | Yes | PENDING |
| Export encryption decision | Yes | PENDING |
| Export retention | Yes | PENDING |
| Export deletion | Yes | PENDING |
| Download expiry decision | Yes | PENDING |
| Analytics de-identification decision | Yes | PENDING |

## Backup protection

Required evidence:

| Evidence item | Required | Current status |
|---|---:|---|
| Backup encryption | Yes | PENDING |
| Backup storage location | Yes | PENDING |
| Backup access owner | Yes | PENDING |
| Restore test | Yes | PENDING |
| Key dependency documented | Yes | PENDING |
| RTO | Yes | PENDING |
| RPO | Yes | PENDING |
| Emergency access | Yes | PENDING |

## Key management

Allowed providers:

- Azure Key Vault.
- AWS KMS.
- AWS Secrets Manager where appropriate.
- HashiCorp Vault.
- Institutional KMS/secret manager approved by Caritas/Tec.

Required evidence:

| Evidence item | Required | Current status |
|---|---:|---|
| Key provider | Yes | PENDING |
| Key owner | Yes | PENDING |
| Key purpose | Yes | PENDING |
| Key rotation cadence | Yes | PENDING |
| Emergency rotation process | Yes | PENDING |
| Key access audit or compensating control | Yes | PENDING |
| Separation of duties | Yes | PENDING |
| Break-glass process | Yes | PENDING |

## Analytics and science data protection

Analytics must not bypass privacy.

Required evidence:

| Evidence item | Required | Current status |
|---|---:|---|
| De-identification decision | Yes | PENDING |
| Aggregation decision | Yes | PENDING |
| PHI exclusion decision | Yes | PENDING |
| Minimum cell size decision | Decision required | PENDING |
| Re-identification risk review | Yes | PENDING |
| Research dataset approval | If applicable | PENDING |

## Crypto audit / blockchain decision

Status: DEFERRED

Blockchain is not required for production MVP.

Allowed future work:

- Hash chain.
- Merkle root.
- Integrity proof.
- Internal audit digest.
- No PHI on-chain.
- No public blockchain dependency for clinical workflow.

Forbidden:

- Patient PHI on-chain.
- Emergency contact data on-chain.
- Insurance data on-chain.
- Consent content on public chain.

## AI Gateway data protection decision

Status: DISABLED

AI Gateway must remain disabled until a dedicated privacy/security ADR exists.

Minimum future requirements:

- No PHI by default.
- Prompt redaction.
- Provider retention review.
- Cost limit.
- Rate limit.
- Human review.
- Kill switch.
- Audit trail.

## Current readiness

| State | Value |
|---|---|
| Transit encryption readiness | BLOCKED |
| Rest encryption readiness | BLOCKED |
| Backup encryption readiness | BLOCKED |
| Mobile local encryption readiness | BLOCKED |
| Export protection readiness | BLOCKED |
| Key management readiness | BLOCKED |
| Production data protection readiness | BLOCKED |

## Next required evidence

1. Confirm HTTPS/TLS production strategy.
2. Confirm SQL Server encryption at rest strategy.
3. Confirm backup encryption strategy.
4. Confirm mobile local encryption strategy.
5. Define key management provider.
6. Classify sensitive fields.
7. Define export masking/encryption.
8. Verify logs never contain PHI/PII/secrets.
9. Define analytics de-identification rules.
10. Keep AI Gateway and blockchain deferred until approved ADRs exist.