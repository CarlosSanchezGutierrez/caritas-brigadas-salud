# P3.5-09 Admin Reporting Backend Contract

## Current decision

Status: BLOCKED

Admin reporting backend readiness is not approved until real evidence exists for reporting endpoints, dashboard metrics, exports, role-based access, audit logging, privacy controls, data quality indicators, performance limits, and web admin API dependency.

## Scope

This contract applies to:

- ASP.NET Core API.
- Web Admin.
- SQL Server reporting queries.
- Patient intake data.
- Consent/signature evidence.
- Emergency contact fields.
- Insurance/social security fields.
- Sync metadata.
- Audit logs.
- Exports.
- Dashboards.
- Operational reporting.
- Future analytics.

## Non-negotiable rule

Web Admin and reporting users must never connect directly to SQL Server.

Approved architecture:

Web Admin -> HTTPS -> API -> SQL Server

Forbidden:

- Browser to SQL Server.
- Web Admin direct database credentials.
- Production SQL credentials in frontend bundles.
- Routine manual SQL reporting without audit trail.
- Unapproved raw production database access.

## Administrative reporting goal

Central offices must be able to understand brigada operations through the API without uncontrolled spreadsheets or manual database access.

## Reporting roles and permissions

| Capability | Required permission | Current status |
|---|---|---|
| View dashboard | reporting.dashboard.view | PENDING |
| View aggregated reports | reporting.summary.view | PENDING |
| View patient-level report | reporting.patient.view | PENDING |
| View sensitive fields | reporting.sensitive.view | PENDING |
| Export CSV | reporting.export.csv | PENDING |
| Export XLSX | reporting.export.xlsx | PENDING |
| View export history | reporting.export.history.view | PENDING |
| Delete export artifact | reporting.export.delete | PENDING |
| View audit trail | audit.reporting.view | PENDING |
| Manage reporting config | reporting.config.manage | PENDING |

## Dashboard metric evidence

| Metric | Required | Current status |
|---|---:|---|
| Total patients by date | Yes | PENDING |
| Total patients by brigade | Yes | PENDING |
| Total patients by service | Yes | PENDING |
| Consent completed count | Yes | PENDING |
| Consent refused count | Yes | PENDING |
| Unable-to-sign count | Yes | PENDING |
| Partial record count | Yes | PENDING |
| Migrant/incomplete record count | Yes | PENDING |
| Emergency contact completion count | Yes | PENDING |
| Insurance/social security completion count | Yes | PENDING |
| Sync accepted count | Yes | PENDING |
| Sync rejected count | Yes | PENDING |
| Sync conflict count | Yes | PENDING |
| Data quality indicators | Yes | PENDING |
| Export activity count | Yes | PENDING |

## Report endpoint evidence

| Endpoint area | Required | Current status |
|---|---:|---|
| Daily summary report | Yes | PENDING |
| Brigade summary report | Yes | PENDING |
| Service summary report | Yes | PENDING |
| Patient intake completeness report | Yes | PENDING |
| Consent status report | Yes | PENDING |
| Emergency contact completeness report | Yes | PENDING |
| Insurance/social security report | Yes | PENDING |
| Sync health report | Yes | PENDING |
| Data quality report | Yes | PENDING |
| Export history report | Yes | PENDING |

## Export evidence

| Export control | Required | Current status |
|---|---:|---|
| CSV export decision | Yes | PENDING |
| XLSX export decision | Yes | PENDING |
| Field selection | Yes | PENDING |
| Sensitive field masking | Yes | PENDING |
| Export authorization | Yes | PENDING |
| Export audit logging | Yes | PENDING |
| Export retention | Yes | PENDING |
| Export deletion | Yes | PENDING |
| Download expiration | Yes | PENDING |
| Export file encryption decision | Yes | PENDING |
| Watermarking decision | Decision required | PENDING |
| Maximum export size | Yes | PENDING |

## Privacy controls

Reports and exports must not expose sensitive data by default.

| Data group | Default reporting behavior | Current status |
|---|---|---|
| Patient identity | Restricted / minimized | PENDING |
| Phone/contact | Masked or restricted | PENDING |
| Emergency contact | Masked or restricted | PENDING |
| Insurance/social security | Masked or restricted | PENDING |
| Consent/signature evidence | Restricted | PENDING |
| Clinical notes | Restricted by default | PENDING |
| Sync metadata | Operational only | PENDING |
| Audit logs | Restricted | PENDING |

Required controls:

- Role-based sensitive field access.
- Aggregated reporting by default where possible.
- Raw patient-level export only with explicit permission.
- Export audit logging.
- Data minimization.
- Re-identification risk review for analytics exports.

## Audit logging evidence

Required reporting audit events:

| Audit event | Required | Current status |
|---|---:|---|
| Report viewed | Yes | PENDING |
| Dashboard viewed if sensitive | Yes | PENDING |
| Export requested | Yes | PENDING |
| Export generated | Yes | PENDING |
| Export downloaded | Yes | PENDING |
| Export failed | Yes | PENDING |
| Export deleted | Yes | PENDING |
| Sensitive field included | Yes | PENDING |
| Permission denied | Yes | PENDING |
| Filter/date range used | Yes | PENDING |
| User id | Yes | PENDING |
| Organization id | Yes | PENDING |
| Correlation id | Yes | PENDING |
| Timestamp | Yes | PENDING |

## Data quality indicators

| Indicator | Required | Current status |
|---|---:|---|
| Missing required patient fields | Yes | PENDING |
| Partial patient records | Yes | PENDING |
| Migrant or incomplete patient data handling | Yes | PENDING |
| Missing consent evidence | Yes | PENDING |
| Refused consent without reason | Yes | PENDING |
| Unable-to-sign without reason | Yes | PENDING |
| Missing emergency contact | Yes | PENDING |
| Missing insurance/social security decision | Yes | PENDING |
| Sync conflicts | Yes | PENDING |
| Rejected sync events | Yes | PENDING |
| Duplicate patient folio conflicts | Yes | PENDING |
| Stale pending batches | Yes | PENDING |

## Reporting API requirements

| API requirement | Required | Current status |
|---|---:|---|
| Versioned endpoints | Yes | PENDING |
| Authenticated access | Yes | PENDING |
| Server-side authorization | Yes | PENDING |
| Organization scoping | Yes | PENDING |
| Query parameter validation | Yes | PENDING |
| Date range validation | Yes | PENDING |
| Pagination | Yes | PENDING |
| Sorting | Yes | PENDING |
| Filtering | Yes | PENDING |
| Safe error responses | Yes | PENDING |
| Rate limiting | Yes | PENDING |
| OpenAPI documentation | Yes | PENDING |
| Response contracts | Yes | PENDING |

## Performance and scalability evidence

| Evidence item | Required | Current status |
|---|---:|---|
| Expected report date ranges | Yes | PENDING |
| Expected data volume | Yes | PENDING |
| Query timeout | Yes | PENDING |
| Export timeout | Yes | PENDING |
| Maximum export size | Yes | PENDING |
| Async export decision | Yes | PENDING |
| Caching decision | Decision required | PENDING |
| Indexing decision | Yes | PENDING |
| SQL performance review | Yes | PENDING |
| Reporting load smoke | Yes | PENDING |

## Web Admin dependency evidence

The Web Admin must depend on the API only.

| Requirement | Required | Current status |
|---|---:|---|
| HTTPS-only API access | Yes | PENDING |
| No direct SQL Server access | Yes | PENDING |
| No production secrets in frontend bundle | Yes | PENDING |
| Backend authorization source of truth | Yes | PENDING |
| Export UI respects backend permissions | Yes | PENDING |
| Report UI handles empty states | Yes | PENDING |
| Report UI handles permission denial | Yes | PENDING |
| Report UI handles server unavailable | Yes | PENDING |
| Report UI handles export failures | Yes | PENDING |

## Analytics and science data controls

| Evidence item | Required | Current status |
|---|---:|---|
| De-identification decision | Yes | PENDING |
| Aggregation decision | Yes | PENDING |
| Minimum cell size decision | Decision required | PENDING |
| PHI exclusion decision | Yes | PENDING |
| Research dataset approval | If applicable | PENDING |
| Data sharing approval | If applicable | PENDING |
| Re-identification risk review | Yes | PENDING |
| Data lineage | Yes | PENDING |

## Current readiness

| State | Value |
|---|---|
| Reporting API readiness | BLOCKED |
| Dashboard readiness | BLOCKED |
| Export readiness | BLOCKED |
| Reporting audit readiness | BLOCKED |
| Reporting privacy readiness | BLOCKED |
| Data quality readiness | BLOCKED |
| Reporting performance readiness | BLOCKED |
| Web Admin reporting readiness | BLOCKED |
| Production reporting readiness | BLOCKED |

## Next required evidence

1. Define report endpoint list.
2. Define dashboard metrics.
3. Define reporting permissions.
4. Define export permissions.
5. Define sensitive field masking.
6. Define export audit events.
7. Define date range and pagination limits.
8. Define reporting indexes.
9. Define export retention/deletion.
10. Define Web Admin report UX error handling.