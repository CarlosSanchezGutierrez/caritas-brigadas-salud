# P3.5-06 Observability and Incident Response Evidence Contract Baseline

## Status

Required before staging, pilot, production, App Store, Play Store, or web admin production release.

This document is not a production approval.

## Purpose

Define the required evidence for observability, health checks, structured logs, metrics, traces, dashboards, alerts, incident response, on-call ownership, runbooks, escalation, and post-incident review for Caritas Brigadas de Salud.

## Core rule

A production system without observability is operationally blind.

Production readiness requires:

- Liveness health check.
- Readiness health check.
- Database connectivity health check.
- Structured logs.
- Correlation id.
- Request telemetry.
- Error telemetry.
- Auth failure visibility.
- Rate limit visibility.
- Sync failure visibility.
- SQL connectivity visibility.
- Dashboard or equivalent operational view.
- Alert routing.
- Incident runbook.
- Incident owner.
- Escalation path.
- Post-incident review process.

## Health check requirements

Production must define:

- Liveness endpoint.
- Readiness endpoint.
- Database readiness behavior.
- Dependency readiness behavior.
- Health check authentication decision.
- Health check exposure policy.
- Load balancer health check path.
- Deployment smoke health check.
- Rollback health check.
- SQL outage behavior.
- Degraded mode decision.

Required behavior:

- Liveness must not fail just because SQL Server is temporarily unavailable.
- Readiness must fail when SQL Server or required dependencies are unavailable.
- Deployment smoke must validate readiness before release approval.

## Logging requirements

Production logs must include:

- Timestamp.
- Severity.
- Environment.
- Service name.
- Request path.
- HTTP method.
- Status code.
- Duration.
- CorrelationId.
- RequestId or TraceId.
- User id or safe user reference when authenticated.
- Organization id when available.
- Error category.
- Safe exception summary.

Production logs must not include:

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
- Raw clinical request bodies.
- Raw clinical response bodies.

## Metrics requirements

Production must define visibility for:

- Request count.
- Request latency.
- Error rate.
- 4xx rate.
- 5xx rate.
- 401 rate.
- 403 rate.
- 429 rate.
- SQL connectivity failures.
- Sync accepted count.
- Sync rejected count.
- Sync conflict count.
- Failed batch count.
- Deployment health smoke result.
- Background job failures if jobs exist.
- Export failures if exports exist.

## Tracing requirements

Production must define:

- Correlation id propagation.
- Trace id strategy.
- Request scope.
- Sync batch traceability.
- Audit log linkage.
- Incident investigation path.
- Whether OpenTelemetry is used.
- Whether distributed tracing is required for MVP.

OpenTelemetry, Prometheus, Grafana, Application Insights, CloudWatch, ELK, Loki, or another stack may be selected, but a tool name alone is not evidence.

## Dashboard requirements

Production must define at least one operational view for:

- API availability.
- Readiness status.
- Error rate.
- Latency.
- SQL connectivity.
- Auth failures.
- Rate limiting.
- Sync failures.
- Deployment smoke.
- Recent critical incidents.

Grafana is allowed if backed by real metrics/log sources.

Grafana is not required if another approved monitoring stack provides equivalent evidence.

## Alerting requirements

Production must define alerts for:

- API down.
- Readiness failing.
- Elevated 5xx.
- SQL connectivity failure.
- Authentication failure spike.
- Rate limit spike.
- Sync failure spike.
- Failed deployment smoke.
- Backup failure if observable.
- Disk/storage pressure if available.
- Certificate expiration if available.

Each alert must define:

- Trigger.
- Severity.
- Owner.
- Destination.
- Response time.
- Runbook link.
- Escalation path.

## Incident response requirements

Production must define:

- Incident owner.
- Severity levels.
- Triage process.
- Escalation path.
- Communication channel.
- Decision owner.
- Rollback trigger.
- Data breach escalation.
- Security incident escalation.
- Patient data exposure response.
- Post-incident review.
- Corrective action tracking.

## Security incident requirements

Required incident paths:

- Secret leakage.
- Unauthorized access.
- Token abuse.
- Admin account compromise.
- SQL credential compromise.
- Patient data exposure.
- Suspicious export.
- Repeated failed login spike.
- Authorization bypass suspicion.
- Dependency vulnerability requiring emergency patch.

## Mobile incident requirements

iOS and Android incidents must define:

- Lost device.
- Stolen device.
- Offline queue corruption.
- App version with critical bug.
- Forced update decision.
- Remote session revocation.
- Local data wipe decision.
- App Store / Play Store emergency release path.

## Web admin incident requirements

Web admin incidents must define:

- Admin account compromise.
- Wrong export permissions.
- Report data leakage.
- Dashboard unavailability.
- Export failure.
- Permission rollback.
- Session revocation.
- Audit review.

## Evidence package requirements

Observability and incident response evidence must include:

- Environment.
- Owner.
- Tool or stack selected.
- Dashboard link or screenshot reference.
- Alert definitions.
- Runbook link.
- Test incident record.
- Deployment smoke evidence.
- SQL outage simulation or equivalent decision.
- Post-incident review template.
- Approval.

## Production readiness states

- BLOCKED.
- READY FOR STAGING OBSERVABILITY.
- READY FOR PILOT OBSERVABILITY.
- READY FOR PRODUCTION OBSERVABILITY.

Default state is BLOCKED.