# P3.11 Insight and Direction Reporting Governance

## Purpose

This document defines how metrics and dashboards become insights, action recommendations, executive summaries, direction reports, and institutional evidence packages.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Insight principle

An insight is not just a chart.

An insight is an evidence-backed interpretation tied to a decision owner and action recommendation.

## Required insight metadata

Every insight must define:

- insight id
- insight owner
- insight statement
- supporting metric ids
- source snapshot id
- pipeline id
- pipeline version
- evidence package reference
- dashboard id when applicable
- decision owner
- action recommendation
- confidence level
- limitations
- audit trail reference

## Insight classes

| Class | Purpose |
|---|---|
| operational insight | Immediate brigade or service decision |
| tactical insight | Weekly or monthly program improvement |
| strategic insight | Direction, board, donor, partner decision |
| data quality insight | Completeness, rejected records, quarantine, correction issues |
| research insight | ODS, causal, Bayesian, statistical, or academic interpretation |
| risk insight | Security, compliance, operational, or clinical risk |

## Direction reporting structure

Every direction report must include:

- direction report id.
- report owner.
- executive summary.
- reporting period.
- organization id.
- source snapshot ids.
- KPI summary.
- dashboard references.
- key insights.
- action recommendations.
- risks and blockers.
- data quality caveats.
- evidence package reference.
- audit trail reference.

## Report types

| Report type | Purpose |
|---|---|
| operational report | Field operations and daily review |
| tactical report | Program coordination and resource planning |
| strategic report | Direction and institutional governance |
| donor report | Aggregate and sanitized partner communication |
| board report | Governance-level decisions |
| research report | Approved analytical work |

## Executive summary requirements

An executive summary must include:

- what happened.
- why it matters.
- what changed.
- what decision is needed.
- what action is recommended.
- what data quality limitations exist.
- what evidence supports the statement.

## Action recommendation requirements

Every action recommendation must define:

- action recommendation id.
- decision owner.
- recommended action.
- supporting insight id.
- expected impact.
- required resources.
- urgency.
- risk.
- evidence package reference.
- audit trail reference.

## Guardrails

- No insight without evidence.
- No action recommendation without decision owner.
- No direction reporting without data quality caveats.
- No external report without minimization.
- No report export without audit trail.
- No silent overwrite.
- No secrets in repository.
- No cloud dependency.

## P3.11 conclusion

Direction reporting must be evidence-backed, auditable, scoped, and actionable.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE