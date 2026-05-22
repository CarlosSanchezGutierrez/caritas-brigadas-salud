# Production Evidence Register

## Global status

`Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE`

## Evidence table

| ID | Category | Required evidence | Status | Reference | Owner | Date |
|---|---|---|---|---|---|---|
| P3.6-EV-001 | Deployment | Deployed commit SHA | Pending | TBD | TBD | TBD |
| P3.6-EV-002 | Deployment | Environment URL or internal endpoint | Pending | TBD | TBD | TBD |
| P3.6-EV-003 | Deployment | Deployment log or CI run | Pending | TBD | TBD | TBD |
| P3.6-EV-004 | Configuration | Production environment variables documented without secrets | Pending | TBD | TBD | TBD |
| P3.6-EV-005 | Configuration | CORS origins verified | Pending | TBD | TBD | TBD |
| P3.6-EV-006 | Configuration | Forwarded headers known proxies/networks verified | Pending | TBD | TBD | TBD |
| P3.6-EV-007 | Security | Secrets stored outside repository | Pending | TBD | TBD | TBD |
| P3.6-EV-008 | Security | CodeQL clean | Pending | TBD | TBD | TBD |
| P3.6-EV-009 | Security | Dependency review clean or justified | Pending | TBD | TBD | TBD |
| P3.6-EV-010 | Security | Secret scanning clean | Pending | TBD | TBD | TBD |
| P3.6-EV-011 | Security | Anonymous protected request rejected | Pending | TBD | TBD | TBD |
| P3.6-EV-012 | Security | Authenticated protected request accepted | Pending | TBD | TBD | TBD |
| P3.6-EV-013 | Database | SQL Server target documented | Pending | TBD | TBD | TBD |
| P3.6-EV-014 | Database | Migrations applied | Pending | TBD | TBD | TBD |
| P3.6-EV-015 | Database | Application user least privilege documented | Pending | TBD | TBD | TBD |
| P3.6-EV-016 | Database | Backup executed | Pending | TBD | TBD | TBD |
| P3.6-EV-017 | Database | Restore tested | Pending | TBD | TBD | TBD |
| P3.6-EV-018 | Observability | Health live verified | Pending | TBD | TBD | TBD |
| P3.6-EV-019 | Observability | Health ready verified | Pending | TBD | TBD | TBD |
| P3.6-EV-020 | Observability | Correlation id visible in logs | Pending | TBD | TBD | TBD |
| P3.6-EV-021 | Observability | Structured logs verified | Pending | TBD | TBD | TBD |
| P3.6-EV-022 | Smoke tests | Root endpoint verified | Pending | TBD | TBD | TBD |
| P3.6-EV-023 | Smoke tests | Representative organization endpoint verified | Pending | TBD | TBD | TBD |
| P3.6-EV-024 | Smoke tests | Representative report/export endpoint verified | Pending | TBD | TBD | TBD |
| P3.6-EV-025 | Rollback | Rollback runbook completed | Pending | TBD | TBD | TBD |
| P3.6-EV-026 | Rollback | Database restore/rollback path documented | Pending | TBD | TBD | TBD |

## Rule

Do not change global status to ready until every evidence row has a real reference.