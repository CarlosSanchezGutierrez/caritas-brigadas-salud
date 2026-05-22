# P3.7 Operational and Analytical Pipelines Baseline

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Operational pipeline

The operational pipeline supports live institutional execution.

Flow:

1. Capture or inject data.
2. Validate payload.
3. Enforce authorization and organization scope.
4. Persist in SQL Server.
5. Write audit evidence.
6. Update operational read models.
7. Produce reports or exports.
8. Monitor health and data quality.

## Operational data categories

- Patients.
- Consents.
- Brigades.
- Encounters.
- Services.
- Vital signs.
- Form responses.
- Referrals.
- Medication delivery.
- Documents.
- Users.
- Roles.
- Organizations.
- Audit logs.
- Sync events.
- Data injection batches.

## Analytical pipeline

The analytical pipeline supports direction, monitoring, reporting, research, and social impact measurement.

Flow:

1. Select approved extraction window.
2. Snapshot operational data.
3. Apply privacy rules.
4. Apply de-identification or aggregation when required.
5. Create analytical staging dataset.
6. Produce indicators.
7. Generate dashboards.
8. Register export evidence.
9. Store reproducibility metadata.

## KPI families

### Direction

- Total people served.
- Total encounters.
- Services delivered by brigade.
- Services delivered by location.
- Follow-up required.
- Referral volume.
- Referral completion.

### Clinical monitoring

- Vital sign abnormality rate.
- Service recurrence.
- Medication delivery continuity.
- Follow-up compliance.
- Clinical data completeness.

### Social vulnerability

- Needs by territory.
- Service demand by territory.
- Recurrent unmet needs.
- Coverage gaps.
- Priority score by zone.
- Confidence score by data completeness.

### Operations

- Capture throughput.
- Average encounter duration.
- Pending records.
- Failed sync batches.
- Data injection rejected records.
- Export freshness.
- Dashboard freshness.

### Security and audit

- Failed auth attempts.
- Protected endpoint access.
- Admin actions.
- Sensitive data access.
- Audit events per workflow.
- Incident records.

## Research lab readiness

Research exports require approved purpose, extraction window, data dictionary, de-identification rule, owner, reproducibility metadata, export audit event, and retention rule.
