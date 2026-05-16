# P3.5-09 Admin Reporting Backend Contract Baseline

## Status

Required before Web Admin, staging pilot, production reporting, dashboards, exports, App Store, Play Store, or operational release.

This document is not a production approval.

## Purpose

Define the backend requirements for administrative reporting, central office visibility, daily summaries, exports, dashboards, permission enforcement, audit logging, data minimization, privacy controls, and traceable operational metrics for Caritas Brigadas de Salud.

## Core rule

Web Admin and reporting users must never connect directly to SQL Server.

Approved path:

- Web Admin -> HTTPS -> API -> SQL Server.
- Internal reports -> API or controlled backend job -> SQL Server.
- Exports -> API authorization -> audited export generation.

Forbidden path:

- Web Admin -> SQL Server.
- Browser -> SQL Server.
- Admin user with direct production database access for routine reporting.
- Production SQL credentials in frontend bundles.
- Unapproved manual reporting queries with no audit trail.

## Administrative reporting goal

The system must help central offices understand daily brigada operations without waiting for manual spreadsheets, WhatsApp messages, or uncontrolled ad hoc database access.

Required reporting outcomes:

- Daily patient counts.
- Daily service counts.
- Brigade-level summaries.
- Service-level summaries.
- Patient intake completeness.
- Consent/signature status.
- Emergency contact capture status.
- Insurance/social security capture status.
- Sync status.
- Rejected/conflicted records.
- Exportable operational reports.
- Audit trail for report access and exports.

## Reporting access requirements

Production reporting must define:

- Admin reporting roles.
- Report viewer permissions.
- Export permissions.
- Sensitive field visibility.
- Organization boundary enforcement.
- Brigade boundary enforcement if applicable.
- Date range restrictions.
- Export approval if required.
- Audit logging.
- Data masking.

## Dashboard requirements

Admin dashboards must define:

- Total patients by date.
- Total patients by brigade.
- Total patients by service.
- Consent completed count.
- Consent refused count.
- Unable-to-sign count.
- Partial record count.
- Migrant/incomplete record handling.
- Emergency contact completion count.
- Insurance/social security completion count.
- Sync accepted count.
- Sync rejected count.
- Sync conflict count.
- Data quality indicators.
- Export activity.
- Operational exceptions.

## Export requirements

Exports must define:

- CSV export.
- XLSX export if supported.
- Field selection.
- Sensitive field masking.
- Export authorization.
- Export audit logging.
- Export retention.
- Export deletion.
- Download expiration.
- Export file encryption decision.
- Watermarking decision.
- Data minimization.
- Re-identification risk review for analytics exports.

## Privacy requirements

Reports and exports must not expose sensitive data by default.

Required privacy controls:

- Patient PHI/PII minimization.
- Emergency contact masking decision.
- Insurance/social security masking decision.
- Consent/signature data access restriction.
- Clinical notes export restriction.
- Role-based sensitive field access.
- Aggregated reporting by default where possible.
- Raw patient-level export only with explicit permission.

## Audit requirements

Reporting must be auditable.

Required audit events:

- Report viewed.
- Dashboard viewed if sensitive.
- Export requested.
- Export generated.
- Export downloaded.
- Export failed.
- Export deleted.
- Sensitive field included.
- Permission denied.
- Filter/date range used.
- User id.
- Organization id.
- Correlation id.
- Timestamp.

## Data quality requirements

Reporting must expose operational data quality signals:

- Missing required patient fields.
- Partial patient records.
- Migrant or incomplete patient data handling.
- Missing consent evidence.
- Refused consent without reason.
- Unable-to-sign without reason.
- Missing emergency contact.
- Missing insurance/social security decision.
- Sync conflicts.
- Rejected sync events.
- Duplicate patient folio conflicts.
- Stale pending batches.

## Performance requirements

Reporting must define:

- Expected report date ranges.
- Expected data volume.
- Pagination.
- Query timeout.
- Export timeout.
- Maximum export size.
- Async export decision.
- Caching decision.
- Indexing decision.
- SQL performance review.
- Load test for reporting endpoints.

## API requirements

Admin reporting API must define:

- Versioned endpoints.
- Authenticated access.
- Server-side authorization.
- Organization scoping.
- Query parameter validation.
- Date range validation.
- Pagination.
- Sorting.
- Filtering.
- Safe error responses.
- Rate limiting.
- OpenAPI documentation.
- Response contracts.

## Web Admin requirements

The Web Admin client must depend on the API only.

Required:

- HTTPS-only API access.
- No direct SQL Server access.
- No production secrets in frontend bundle.
- Role-based UI as convenience only.
- Backend remains authorization source of truth.
- Export UI respects backend permissions.
- Report UI handles empty states.
- Report UI handles permission denial.
- Report UI handles server unavailable.
- Report UI handles export generation failures.

## Analytics and science data requirements

Reporting data may support analytics only after privacy review.

Required:

- De-identification decision.
- Aggregation decision.
- Minimum cell size decision.
- PHI exclusion decision.
- Research dataset approval if applicable.
- Data sharing approval.
- Re-identification risk review.
- Export retention.
- Data lineage.

## Production readiness states

- BLOCKED.
- READY FOR STAGING REPORTING.
- READY FOR PILOT REPORTING.
- READY FOR PRODUCTION REPORTING.

Default state is BLOCKED.