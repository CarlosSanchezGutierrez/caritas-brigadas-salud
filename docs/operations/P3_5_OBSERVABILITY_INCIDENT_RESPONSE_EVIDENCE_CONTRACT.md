# P3.5-06 Observability and Incident Response Evidence Contract

## Current decision

Status: BLOCKED

Observability and incident response are not approved until real evidence exists for health checks, logs, metrics, dashboards, alerts, runbooks, incident ownership, escalation, and post-incident review.

## Scope

This contract applies to:

- ASP.NET Core API.
- SQL Server connectivity.
- Offline sync.
- Auth.
- Rate limiting.
- CORS/AllowedHosts.
- iOS app.
- Android app.
- Web admin.
- Exports.
- Backups.
- Deployments.
- Security incidents.
- Operational incidents.

## Non-negotiable rule

A production system without observability is operationally blind.

Production readiness requires observable behavior, not just successful builds.

## Health check evidence

| Evidence item | Required | Current status |
|---|---:|---|
| Liveness endpoint | Yes | PENDING |
| Readiness endpoint | Yes | PENDING |
| Database connectivity health check | Yes | PENDING |
| Dependency readiness behavior | Yes | PENDING |
| Load balancer health path | Yes | PENDING |
| Deployment smoke health check | Yes | PENDING |
| Rollback health check | Yes | PENDING |
| SQL outage readiness behavior | Yes | PENDING |
| Health check exposure policy | Yes | PENDING |

Required behavior:

- Liveness should indicate the process is alive.
- Readiness should indicate whether the API can safely receive traffic.
- SQL Server outage should fail readiness, not necessarily liveness.
- Deployment smoke must validate readiness before release approval.

## Structured logging evidence

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
- Safe user reference when authenticated.
- Organization id when available.
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

## Metrics evidence

| Metric area | Required | Current status |
|---|---:|---|
| Request count | Yes | PENDING |
| Request latency | Yes | PENDING |
| Error rate | Yes | PENDING |
| 4xx rate | Yes | PENDING |
| 5xx rate | Yes | PENDING |
| 401 rate | Yes | PENDING |
| 403 rate | Yes | PENDING |
| 429 rate | Yes | PENDING |
| SQL connectivity failures | Yes | PENDING |
| Sync accepted count | Yes | PENDING |
| Sync rejected count | Yes | PENDING |
| Sync conflict count | Yes | PENDING |
| Failed batch count | Yes | PENDING |
| Deployment smoke result | Yes | PENDING |

## Tracing and correlation evidence

Required evidence:

| Evidence item | Required | Current status |
|---|---:|---|
| Correlation id propagation | Yes | PENDING |
| Trace id strategy | Yes | PENDING |
| Request scope | Yes | PENDING |
| Sync batch traceability | Yes | PENDING |
| Audit log linkage | Yes | PENDING |
| Incident investigation path | Yes | PENDING |
| OpenTelemetry decision | Decision required | PENDING |
| Distributed tracing decision | Decision required | PENDING |

## Monitoring stack decision

Allowed options:

- OpenTelemetry.
- Prometheus.
- Grafana.
- Application Insights.
- CloudWatch.
- ELK.
- Loki.
- Institutional monitoring stack approved by Caritas/Tec.
- Equivalent documented monitoring platform.

Current selected stack: PENDING

A tool name alone is not evidence.

Required evidence:

- Data source.
- Dashboard.
- Alerting.
- Retention.
- Owner.
- Access control.
- Incident use case.

## Dashboard evidence

| Dashboard item | Required | Current status |
|---|---:|---|
| API availability | Yes | PENDING |
| Readiness status | Yes | PENDING |
| Error rate | Yes | PENDING |
| Latency | Yes | PENDING |
| SQL connectivity | Yes | PENDING |
| Auth failures | Yes | PENDING |
| Rate limiting | Yes | PENDING |
| Sync failures | Yes | PENDING |
| Deployment smoke | Yes | PENDING |
| Recent critical incidents | Yes | PENDING |

## Alerting evidence

| Alert | Required | Current status |
|---|---:|---|
| API down | Yes | PENDING |
| Readiness failing | Yes | PENDING |
| Elevated 5xx | Yes | PENDING |
| SQL connectivity failure | Yes | PENDING |
| Authentication failure spike | Yes | PENDING |
| Rate limit spike | Yes | PENDING |
| Sync failure spike | Yes | PENDING |
| Failed deployment smoke | Yes | PENDING |
| Backup failure if observable | Yes | PENDING |
| Certificate expiration if available | Decision required | PENDING |

Each alert must define:

- Trigger.
- Severity.
- Owner.
- Destination.
- Response time.
- Runbook link.
- Escalation path.

## Incident response evidence

| Evidence item | Required | Current status |
|---|---:|---|
| Incident owner | Yes | PENDING |
| Severity levels | Yes | PENDING |
| Triage process | Yes | PENDING |
| Escalation path | Yes | PENDING |
| Communication channel | Yes | PENDING |
| Decision owner | Yes | PENDING |
| Rollback trigger | Yes | PENDING |
| Data breach escalation | Yes | PENDING |
| Security incident escalation | Yes | PENDING |
| Post-incident review | Yes | PENDING |
| Corrective action tracking | Yes | PENDING |

## Security incident paths

Required security incident response paths:

| Scenario | Required response | Current status |
|---|---|---|
| Secret leakage | Rotation and incident review | PENDING |
| Unauthorized access | Containment and audit review | PENDING |
| Token abuse | Revocation and investigation | PENDING |
| Admin account compromise | Session revoke and credential rotation | PENDING |
| SQL credential compromise | Rotation and SQL access review | PENDING |
| Patient data exposure | Data breach escalation | PENDING |
| Suspicious export | Export audit review | PENDING |
| Failed login spike | Auth investigation | PENDING |
| Authorization bypass suspicion | Emergency patch and audit | PENDING |
| Emergency dependency patch | Security release process | PENDING |

## Mobile incident paths

| Scenario | Required response | Current status |
|---|---|---|
| Lost device | Revoke/wipe decision | PENDING |
| Stolen device | Revoke/wipe decision | PENDING |
| Offline queue corruption | Reset/replay decision | PENDING |
| Critical app bug | Forced update decision | PENDING |
| Unsupported app version | Minimum version enforcement decision | PENDING |
| Local data exposure | Security incident path | PENDING |
| App Store emergency release | Required path | PENDING |
| Play Store emergency release | Required path | PENDING |

## Web admin incident paths

| Scenario | Required response | Current status |
|---|---|---|
| Admin account compromise | Session revocation and audit | PENDING |
| Wrong export permissions | Permission rollback | PENDING |
| Report data leakage | Data breach escalation | PENDING |
| Dashboard unavailable | Operational incident | PENDING |
| Export failure | Retry/recovery policy | PENDING |
| Permission misconfiguration | Rollback and review | PENDING |

## Evidence record template

Each observability or incident response validation must record:

| Field | Required |
|---|---:|
| Date | Yes |
| Environment | Yes |
| Owner | Yes |
| Tool/stack | Yes |
| Procedure executed | Yes |
| Result | Yes |
| Alert triggered | If applicable |
| Dashboard evidence | If applicable |
| Runbook used | If applicable |
| Duration | Yes |
| Failure notes | Yes |
| Corrective actions | Yes |
| Evidence link | Yes |
| Approval | Yes |

## Current readiness

| State | Value |
|---|---|
| Health check readiness | BLOCKED |
| Logging readiness | BLOCKED |
| Metrics readiness | BLOCKED |
| Dashboard readiness | BLOCKED |
| Alerting readiness | BLOCKED |
| Incident response readiness | BLOCKED |
| Security incident readiness | BLOCKED |
| Mobile incident readiness | BLOCKED |
| Web admin incident readiness | BLOCKED |
| Production observability readiness | BLOCKED |

## Next required evidence

1. Select monitoring stack.
2. Confirm log destination.
3. Confirm metrics destination.
4. Confirm dashboard owner.
5. Confirm alert owner.
6. Define incident severity levels.
7. Define escalation path.
8. Define SQL outage simulation or equivalent test.
9. Define deployment smoke evidence.
10. Execute test incident and post-incident review.