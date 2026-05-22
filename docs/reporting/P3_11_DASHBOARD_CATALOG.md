# P3.11 Dashboard Catalog

## Purpose

This document defines the dashboard catalog for operational, tactical, strategic, quality, and research views.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Dashboard principle

Every dashboard must be traceable to approved metrics, snapshots, pipelines, and audit trail references.

No dashboard becomes the operational source of truth.

SQL Server is the operational source of truth.

## Required dashboard metadata

Every dashboard must define:

- dashboard id
- dashboard owner
- dashboard audience
- dashboard purpose
- metric ids
- source snapshot id
- pipeline id
- pipeline version
- refresh cadence
- organization id
- access role
- export permissions
- data quality caveats
- audit trail reference

## Dashboard 1: Operational Brigade Dashboard

Audience:

- brigade coordinators.
- operational leads.
- service coordinators.

Purpose:

- Monitor daily activity.
- Monitor patients served.
- Monitor encounters completed.
- Monitor services provided.
- Monitor service availability.
- Monitor rejected records and quarantine.

Required metrics:

- patients served.
- encounters completed.
- services provided.
- service availability.
- rejected records.
- quarantine count.

## Dashboard 2: Clinical Continuity Dashboard

Audience:

- clinical coordinators.
- medical leadership.
- follow-up teams.

Purpose:

- Monitor clinical timeline continuity.
- Monitor referrals.
- Monitor repeat visits.
- Monitor vital signs capture.
- Monitor correction event behavior.

Required metrics:

- vital signs captured.
- referrals created.
- follow-up recommendations.
- repeat visits.
- clinical correction events.

## Dashboard 3: Consent and Compliance Dashboard

Audience:

- legal, compliance, privacy, administration.

Purpose:

- Monitor consent capture.
- Monitor consent version distribution.
- Monitor consent revocation.
- Monitor privacy notice status.
- Monitor unaudited risk indicators.

Required metrics:

- consent captured.
- consent version distribution.
- consent revoked.
- consent exceptions.
- denied actions.
- role changes.

## Dashboard 4: Data Quality Dashboard

Audience:

- data team.
- operations.
- administration.
- technology team.

Purpose:

- Monitor completeness.
- Monitor rejected records.
- Monitor quarantine.
- Monitor duplicate candidates.
- Monitor controlled data injection quality.
- Monitor correction events.

Required metrics:

- missing required fields.
- rejected records.
- quarantine count.
- duplicate candidates.
- controlled data injection errors.
- correction event count.

## Dashboard 5: Direction Reporting Dashboard

Audience:

- direction.
- board.
- donors.
- institutional partners.

Purpose:

- Provide strategic report summaries.
- Provide impact measurement indicators.
- Provide service coverage and operational performance.
- Provide aggregate institutional evidence.
- Provide executive summary inputs.

Required metrics:

- patients served.
- encounters completed.
- services provided.
- location coverage.
- evidence package count.
- export count.
- strategic indicators.

## Dashboard 6: Research and ODS Dashboard

Audience:

- academic team.
- ODS lab.
- data science team.
- authorized researchers.

Purpose:

- Support aggregate impact analysis.
- Support statistical analysis.
- Support social vulnerability analysis.
- Support research report preparation.

Required controls:

- aggregate-first reporting.
- minimum necessary data.
- research governance.
- organization id.
- audit trail reference.
- evidence package reference.

## Dashboard conclusion

Dashboards must be auditable, reproducible, scoped, and tied to decision owners.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE