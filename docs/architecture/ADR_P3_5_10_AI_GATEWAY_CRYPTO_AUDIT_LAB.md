# ADR P3.5-10: AI Gateway and Crypto Audit Lab

## Status

DEFERRED

## Context

Caritas Brigadas de Salud has long-term opportunities for AI-assisted operations, data science, administrative summarization, reporting assistance, cryptographic auditability, integrity proofs, and student research.

However, the production clinical workflow currently requires reliability, privacy, security, offline sync, reporting, SQL Server integration, consent capture, emergency contact capture, insurance/social security capture, audit logging, backup/restore, observability, and mobile readiness before optional AI or blockchain features.

## Decision

AI Gateway and crypto audit lab work are deferred and disabled by default.

No AI Gateway, LLM provider, blockchain, public-chain dependency, hash-chain audit feature, Merkle-root audit feature, or crypto audit lab feature may process production patient data until a dedicated privacy, security, operational, and legal review is approved.

## Non-negotiable rule

Neither AI Gateway nor blockchain is required for production MVP.

Production MVP must not depend on:

- LLM provider availability.
- LLM provider secrets.
- Public blockchain availability.
- Crypto audit lab availability.
- Student research modules.
- Experimental AI features.
- Experimental cryptography features.

## AI Gateway current decision

Current AI Gateway state: DISABLED

Production must not include:

- AI Gateway provider API keys.
- AI Gateway production secrets.
- Silent LLM calls.
- Patient data prompts.
- PHI sent to LLM providers.
- Emergency contact data sent to LLM providers.
- Insurance/social security data sent to LLM providers.
- Automated diagnosis.
- Automated treatment recommendation.
- Clinical decision replacement.

## AI Gateway allowed future scope

Future AI work may be considered only after a dedicated ADR.

Allowed future use cases may include:

- Administrative summaries without PHI.
- Documentation assistance without PHI.
- Report drafting from de-identified aggregates.
- Operational analytics over aggregated data.
- Training/simulation data using synthetic data.
- Internal support tooling.
- Data quality assistance without exposing patient records.

## AI Gateway future approval checklist

| Control | Required | Current status |
|---|---:|---|
| Dedicated AI privacy/security ADR | Yes | PENDING |
| Feature flag | Yes | PENDING |
| Admin-only enablement | Yes | PENDING |
| Provider selection | Yes | PENDING |
| Vendor risk review | Yes | PENDING |
| Data retention review | Yes | PENDING |
| PHI policy | Yes | PENDING |
| Prompt redaction | Yes | PENDING |
| Prompt versioning | Yes | PENDING |
| Human review path | Yes | PENDING |
| Cost limit | Yes | PENDING |
| Rate limit | Yes | PENDING |
| Abuse protection | Yes | PENDING |
| Audit logging | Yes | PENDING |
| Incident response | Yes | PENDING |
| Kill switch | Yes | PENDING |
| Legal/privacy approval | Yes | PENDING |
| Security approval | Yes | PENDING |

## AI Gateway architectural constraints

If implemented later:

- Clients must not call model providers directly.
- Frontend must not contain model provider secrets.
- Mobile apps must not contain model provider secrets.
- API must enforce authorization.
- AI Gateway must be feature-flagged.
- AI Gateway must be auditable.
- Prompts must be versioned.
- PHI must be redacted or excluded by default.
- Provider outage must not break patient intake.
- Provider outage must not break consent capture.
- Provider outage must not break offline sync.
- Provider outage must not break reporting.
- Kill switch must disable AI features without redeploy if possible.

## Crypto audit lab current decision

Current crypto audit lab state: DISABLED FOR PRODUCTION CLINICAL WORKFLOW

Blockchain is not required for production MVP.

Production must not include:

- Patient PHI on-chain.
- Patient identifiers on-chain.
- Consent content on public chain.
- Signature binary data on public chain.
- Emergency contact data on-chain.
- Insurance/social security data on-chain.
- Public blockchain dependency for clinical operations.
- Clinical workflow blocked by blockchain availability.
- Irreversible disclosure of sensitive data.

## Crypto audit allowed future research

Future crypto audit work may be considered only as research or controlled staging experiment first.

Allowed future research:

- Hash chain.
- Merkle root.
- Integrity proof.
- Internal audit digest.
- Tamper-evidence proof of concept.
- Student cryptography module.
- Synthetic dataset proof of concept.
- Non-production lab environment.

## Crypto audit future approval checklist

| Control | Required | Current status |
|---|---:|---|
| Dedicated crypto/privacy/security ADR | Yes | PENDING |
| Data included in digest | Yes | PENDING |
| Data explicitly excluded | Yes | PENDING |
| Hash algorithm | Yes | PENDING |
| Salt/pepper/keyed hash decision | Yes | PENDING |
| Key ownership if keyed | If applicable | PENDING |
| Rotation policy if keyed | If applicable | PENDING |
| Merkle tree design if used | If applicable | PENDING |
| Verification process | Yes | PENDING |
| Privacy risk review | Yes | PENDING |
| Re-identification risk review | Yes | PENDING |
| Storage location | Yes | PENDING |
| Retention | Yes | PENDING |
| Audit access | Yes | PENDING |
| Incident response | Yes | PENDING |
| Legal/privacy approval | Yes | PENDING |
| Security approval | Yes | PENDING |

## Data protection rules

AI Gateway and crypto audit lab must follow:

- No PHI by default.
- No patient identifiers by default.
- No emergency contact data by default.
- No insurance/social security data by default.
- No irreversible sensitive-data disclosure.
- No production secrets in clients.
- No bypass of backend authorization.
- No bypass of audit logging.
- No bypass of retention policy.
- No bypass of incident response.

## Consequences

Positive consequences:

- Prevents overengineering before production readiness.
- Keeps production MVP focused on clinical/operational reliability.
- Avoids false claims about AI or blockchain security.
- Protects PHI and sensitive operational data.
- Allows future research without contaminating production workflow.
- Creates a clean path for student cryptography and data science work later.

Tradeoffs:

- No AI automation in production MVP.
- No blockchain auditability in production MVP.
- Future AI/crypto work requires additional ADRs and evidence.
- Some advanced research value is deferred until core production foundations are stronger.

## Current readiness

| State | Value |
|---|---|
| AI Gateway readiness | DEFERRED |
| AI Gateway production readiness | BLOCKED |
| Crypto audit lab readiness | DEFERRED |
| Blockchain production readiness | BLOCKED |
| Research-only crypto readiness | BLOCKED |
| Production MVP dependency | NOT REQUIRED |

## Next required evidence before revisiting

1. Complete production SQL Server evidence.
2. Complete production auth/secrets evidence.
3. Complete encryption/data protection evidence.
4. Complete backup/restore/rollback evidence.
5. Complete observability/incident response evidence.
6. Complete security testing evidence.
7. Complete mobile/API offline readiness evidence.
8. Complete admin reporting backend evidence.
9. Define AI privacy/security ADR if AI is still desired.
10. Define crypto audit privacy/security ADR if crypto audit is still desired.