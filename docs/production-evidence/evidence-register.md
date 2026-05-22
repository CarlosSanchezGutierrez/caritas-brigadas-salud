# Production Evidence Register

## Global status

`Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE`

## Evidence table

| ID | Category | Required evidence | Status | Reference | Owner | Date |
|---|---|---|---|---|---|---|
| P3.6-EV-001 | Deployment | Environment name | Pending | TBD | TBD | TBD |
| P3.6-EV-002 | Deployment | Provider or infrastructure target | Pending | TBD | TBD | TBD |
| P3.6-EV-003 | Deployment | Deployed commit SHA | Pending | TBD | TBD | TBD |
| P3.6-EV-004 | Deployment | Deployment date | Pending | TBD | TBD | TBD |
| P3.6-EV-005 | Deployment | Deployment responsible | Pending | TBD | TBD | TBD |
| P3.6-EV-006 | Deployment | API URL or internal endpoint | Pending | TBD | TBD | TBD |
| P3.6-EV-007 | Deployment | Deployment logs or CI reference | Pending | TBD | TBD | TBD |
| P3.6-EV-008 | Deployment | Rollback reference | Pending | TBD | TBD | TBD |
| P3.6-EV-009 | Configuration | ASPNETCORE_ENVIRONMENT documented | Pending | TBD | TBD | TBD |
| P3.6-EV-010 | Configuration | CORS indexed origins verified | Pending | TBD | TBD | TBD |
| P3.6-EV-011 | Configuration | Forwarded headers indexed known proxies verified | Pending | TBD | TBD | TBD |
| P3.6-EV-012 | Configuration | Forwarded headers indexed known networks verified | Pending | TBD | TBD | TBD |
| P3.6-EV-013 | Configuration | Rate limiting status documented | Pending | TBD | TBD | TBD |
| P3.6-EV-014 | Configuration | Max request body size documented | Pending | TBD | TBD | TBD |
| P3.6-EV-015 | Configuration | Swagger exposure status documented | Pending | TBD | TBD | TBD |
| P3.6-EV-016 | Configuration | Authentication mode documented | Pending | TBD | TBD | TBD |
| P3.6-EV-017 | Configuration | Secrets provider documented without secret values | Pending | TBD | TBD | TBD |
| P3.6-EV-018 | Security | Secrets stored outside repository | Pending | TBD | TBD | TBD |
| P3.6-EV-019 | Security | CodeQL clean | Pending | TBD | TBD | TBD |
| P3.6-EV-020 | Security | Dependency review clean or justified | Pending | TBD | TBD | TBD |
| P3.6-EV-021 | Security | Secret scanning clean | Pending | TBD | TBD | TBD |
| P3.6-EV-022 | Security | Authentication smoke test | Pending | TBD | TBD | TBD |
| P3.6-EV-023 | Security | Authorization smoke test | Pending | TBD | TBD | TBD |
| P3.6-EV-024 | Security | Security headers verification | Pending | TBD | TBD | TBD |
| P3.6-EV-025 | Security | Rate limiting verification | Pending | TBD | TBD | TBD |
| P3.6-EV-026 | Security | Sensitive logs verification | Pending | TBD | TBD | TBD |
| P3.6-EV-027 | Database | SQL Server target documented | Pending | TBD | TBD | TBD |
| P3.6-EV-028 | Database | Database name documented | Pending | TBD | TBD | TBD |
| P3.6-EV-029 | Database | Migration status documented | Pending | TBD | TBD | TBD |
| P3.6-EV-030 | Database | Application user least privilege documented | Pending | TBD | TBD | TBD |
| P3.6-EV-031 | Database | Backup executed | Pending | TBD | TBD | TBD |
| P3.6-EV-032 | Database | Restore tested | Pending | TBD | TBD | TBD |
| P3.6-EV-033 | Database | Recovery time notes documented | Pending | TBD | TBD | TBD |
| P3.6-EV-034 | Database | Data retention notes documented | Pending | TBD | TBD | TBD |
| P3.6-EV-035 | Observability | Health live verified | Pending | TBD | TBD | TBD |
| P3.6-EV-036 | Observability | Health ready verified | Pending | TBD | TBD | TBD |
| P3.6-EV-037 | Observability | Structured logging evidence | Pending | TBD | TBD | TBD |
| P3.6-EV-038 | Observability | Propagated correlation id evidence | Pending | TBD | TBD | TBD |
| P3.6-EV-039 | Observability | 4xx and 5xx traceability evidence | Pending | TBD | TBD | TBD |
| P3.6-EV-040 | Observability | Latency evidence | Pending | TBD | TBD | TBD |
| P3.6-EV-041 | Observability | Startup log evidence | Pending | TBD | TBD | TBD |
| P3.6-EV-042 | Smoke tests | Root endpoint verified | Pending | TBD | TBD | TBD |
| P3.6-EV-043 | Smoke tests | Health live endpoint verified | Pending | TBD | TBD | TBD |
| P3.6-EV-044 | Smoke tests | Health ready endpoint verified | Pending | TBD | TBD | TBD |
| P3.6-EV-045 | Smoke tests | Anonymous request to protected endpoint fails | Pending | TBD | TBD | TBD |
| P3.6-EV-046 | Smoke tests | Authenticated request to protected endpoint succeeds | Pending | TBD | TBD | TBD |
| P3.6-EV-047 | Smoke tests | Representative organization endpoint succeeds | Pending | TBD | TBD | TBD |
| P3.6-EV-048 | Smoke tests | Representative report/export endpoint succeeds when applicable | Pending | TBD | TBD | TBD |
| P3.6-EV-049 | Rollback | Rollback criteria documented | Pending | TBD | TBD | TBD |
| P3.6-EV-050 | Rollback | Rollback command or procedure documented | Pending | TBD | TBD | TBD |
| P3.6-EV-051 | Rollback | Database rollback policy documented | Pending | TBD | TBD | TBD |
| P3.6-EV-052 | Rollback | Restore procedure documented | Pending | TBD | TBD | TBD |
| P3.6-EV-053 | Rollback | Decision owner documented | Pending | TBD | TBD | TBD |
| P3.6-EV-054 | Rollback | Incident record template documented | Pending | TBD | TBD | TBD |

## Rule

Do not change global status to ready until every evidence row has a real reference.

Every required evidence item listed in 
`
P3_6_PRODUCTION_EVIDENCE_IMPLEMENTATION.md
`
 must have a corresponding row in this register.
