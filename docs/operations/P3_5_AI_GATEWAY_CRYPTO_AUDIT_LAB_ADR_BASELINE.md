# P3.5-10 AI Gateway and Crypto Audit Lab ADR Baseline

## Status

Required before any AI Gateway, LLM feature, blockchain feature, crypto audit feature, hash-chain feature, or Merkle-root feature is implemented.

This document is not a production approval.

## Purpose

Define the architectural decision record baseline for future AI Gateway and crypto audit lab work in Caritas Brigadas de Salud.

## Core rule

AI Gateway and crypto audit work must be disabled by default.

No AI Gateway, LLM provider, blockchain, public-chain dependency, hash-chain audit feature, Merkle-root audit feature, or crypto audit lab feature may process production patient data until a dedicated privacy, security, operational, and legal review is approved.

## AI Gateway decision

The AI Gateway is deferred.

Default state:

- DISABLED.
- No production secrets.
- No provider API keys.
- No PHI processing.
- No patient data prompts.
- No clinical decision automation.
- No autonomous medical advice.
- No production dependency.
- No silent enablement.

Allowed future use cases after approval:

- Administrative summaries without PHI.
- Documentation assistance without PHI.
- Operational analytics over aggregated data.
- Internal support tooling.
- Report drafting from de-identified aggregates.
- Training/simulation data generation using synthetic data.

Forbidden until approved:

- Sending PHI to an LLM provider.
- Sending patient identifiers to an LLM provider.
- Sending emergency contact data to an LLM provider.
- Sending insurance/social security data to an LLM provider.
- Automated diagnosis.
- Automated treatment recommendation.
- Clinical decision replacement.
- Silent model calls from production endpoints.
- AI features without audit trail.
- AI features without cost/rate limit.
- AI features without kill switch.

## AI Gateway minimum future controls

A future AI Gateway ADR must define:

- Feature flag.
- Admin-only enablement.
- Environment separation.
- Provider selection.
- Data retention review.
- PHI policy.
- Prompt redaction.
- Prompt versioning.
- Output review.
- Human review path.
- Cost limit.
- Rate limit.
- Abuse protection.
- Audit logging.
- Incident response.
- Kill switch.
- Model fallback decision.
- Vendor risk review.
- Legal/privacy approval.
- Security approval.

## AI Gateway architecture constraints

If implemented later, the AI Gateway must be isolated from core clinical workflows.

Required constraints:

- API-controlled access.
- No direct client-to-provider secret exposure.
- No model provider secret in frontend.
- No model provider secret in mobile app.
- Server-side authorization.
- Request/response audit.
- Prompt template versioning.
- Redaction before provider call.
- Configurable provider.
- Provider outage must not break patient intake.
- Provider outage must not break consent capture.
- Provider outage must not break offline sync.
- Provider outage must not break reporting.

## Crypto audit lab decision

Crypto audit and blockchain work is deferred.

Default state:

- DISABLED.
- No public blockchain dependency.
- No PHI on-chain.
- No patient identifiers on-chain.
- No emergency contact data on-chain.
- No insurance/social security data on-chain.
- No production clinical dependency.
- No irreversible disclosure of sensitive data.

Allowed future research:

- Hash chain.
- Merkle root.
- Integrity proof.
- Internal audit digest.
- Tamper-evidence proof of concept.
- Student cryptography module.
- Synthetic dataset proof of concept.
- Non-production lab environment.

Forbidden:

- Patient PHI on-chain.
- Consent content on public chain.
- Signature binary data on public chain.
- Emergency contact data on-chain.
- Insurance/social security data on-chain.
- Public blockchain dependency for production MVP.
- Clinical workflow blocked by blockchain availability.
- Irreversible publication of sensitive data.

## Crypto audit minimum future controls

A future crypto audit ADR must define:

- Data included in digest.
- Data explicitly excluded.
- Hash algorithm.
- Salt/pepper/keyed hash decision.
- Key ownership if keyed.
- Rotation policy if keyed.
- Merkle tree design if used.
- Verification process.
- Privacy risk review.
- Re-identification risk review.
- Storage location.
- Retention.
- Audit access.
- Incident response.
- Legal/privacy approval.
- Security approval.

## Production MVP rule

Neither AI Gateway nor blockchain is required for production MVP.

Production MVP must prioritize:

- Secure patient intake.
- Consent/signature evidence.
- Emergency contact and insurance/social security capture.
- Offline sync.
- SQL Server integration.
- Auth.
- Reporting.
- Audit.
- Backup/restore/rollback.
- Observability.
- Security testing.

## Required ADR readiness states

- DEFERRED.
- APPROVED FOR RESEARCH ONLY.
- APPROVED FOR STAGING EXPERIMENT.
- APPROVED FOR PILOT.
- APPROVED FOR PRODUCTION.

Default state is DEFERRED.