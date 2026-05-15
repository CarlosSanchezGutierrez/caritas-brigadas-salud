# P3 Security and Product Readiness Gap Audit

Status: active
Phase: P3-26L
Backend production readiness conclusion: NOT_PRODUCTION_READY
Frontend readiness conclusion: NOT_READY_FOR_FULL_FRONTEND
Recommended next execution path: security/product gap closure, functional contract closure, staging, then frontend

---

## 1. Executive conclusion

The backend does not appear to have severe technical debt from the P3-26 work.

The repository now has strong governance, validation gates, health endpoints, request telemetry, correlation id hardening, production configuration validation, deployment evidence templates, incident response templates, final blocker matrix, and production readiness closure reporting.

The remaining risk is not mainly code quality.

The remaining risk is incomplete environment evidence and incomplete functional product contract.

The project should not move directly into full frontend implementation until the backend contract for patient intake, consent/signature, insurance/social security, emergency contact, optional patient data, and offline sync is clear.

The project should not move to production until staging and production evidence exists.

---

## 2. Phase plan

| Phase | Name | Goal | Blocks frontend? | Blocks staging? | Blocks production? |
|---|---|---|---:|---:|---:|
| P3-26L | Security and product readiness gap audit | Classify all remaining gaps | Yes | Yes | Yes |
| P3-30A | Patient intake functional contract | Freeze patient fields and validation | Yes | No | Yes |
| P3-30B | Consent and signature evidence contract | Freeze privacy notice and signature evidence | Yes | No | Yes |
| P3-30C | Emergency contact and insurance fields | Add/verify emergency contact and insurance data | Yes | No | Yes |
| P3-30D | OpenAPI/frontend contract freeze | Freeze API contracts for frontend | Yes | No | Yes |
| P3-31A | SQL Server VM connectivity baseline | Connect backend to Tec/Cáritas SQL Server target | No | Yes | Yes |
| P3-31B | SQL least privilege baseline | Define app SQL user permissions | No | Yes | Yes |
| P3-31C | Network ACL/firewall baseline | Define deny-by-default traffic rules | No | Yes | Yes |
| P3-31D | TLS/encryption/secrets baseline | Validate transport encryption and secrets | No | Yes | Yes |
| P3-32A | CodeQL/static analysis | Add static security scanning | No | Recommended | Yes |
| P3-32B | Dependabot/dependency scanning | Automate dependency monitoring | No | Recommended | Yes |
| P3-32C | Secret scanning guard | Prevent secrets in repository | No | Recommended | Yes |
| P3-32D | OWASP ZAP staging scaffold | Prepare pentest baseline for deployed API | No | After staging | Yes |
| P3-33A | Staging environment readiness scaffold | Prepare real staging evidence | No | Yes | Yes |
| P3-33B | Staging SQL smoke execution | Validate SQL Server connectivity | No | Yes | Yes |
| P3-33C | Staging health smoke execution | Validate deployed API health | No | Yes | Yes |
| P3-33D | Staging deployment evidence record | Complete release evidence | No | Yes | Yes |
| P4-01A | Frontend scaffold | Start frontend only after contract freeze | Starts frontend | No | No |

---

## 3. Security gap matrix

| Area | Current state | Classification | Required next action | Owner |
|---|---|---|---|---|
| Rate limiting | Production validation exists | COMPLETE_FOR_CODE / REQUIRED_BEFORE_PRODUCTION_EVIDENCE | Validate in staging/deployment evidence | Backend/DevOps |
| Dependency Review | GitHub gate exists | PARTIAL | Add Dependabot or equivalent automated dependency update policy | Repository owner |
| Static analysis | Not confirmed as CodeQL/SAST | REQUIRED_BEFORE_PRODUCTION | Add CodeQL or equivalent static analysis workflow | Repository owner |
| Secret scanning | Not confirmed | REQUIRED_BEFORE_PRODUCTION | Add secret scanning policy/guard and document secret handling | Repository owner |
| Penetration testing | Not executable before staging | REQUIRED_AFTER_STAGING | Add OWASP ZAP baseline against deployed staging API | Security/QA |
| SQL Server VM connectivity | Not evidenced against real VM | REQUIRED_BEFORE_STAGING | Validate connection string, firewall, TLS, app identity | Tec/Cáritas infra + backend |
| SQL least privilege | Not evidenced | REQUIRED_BEFORE_STAGING | Create app SQL user/role with minimum required permissions | DBA/infra |
| Network ACL/firewall | Not evidenced | REQUIRED_BEFORE_STAGING | Deny any by default; allow API host to SQL Server only | Tec/Cáritas infra |
| Deny-by-default traffic posture | Not evidenced | REQUIRED_BEFORE_STAGING | Document allowed flows and blocked flows | Tec/Cáritas infra |
| TLS backend to SQL Server | Production validation expects encryption | REQUIRED_BEFORE_STAGING_EVIDENCE | Test Encrypt=True/Strict against SQL Server VM | Backend/infra |
| Encryption at rest | Infrastructure-dependent | OWNED_BY_INFRASTRUCTURE | Confirm SQL Server disk/TDE/backups encryption | Tec/Cáritas infra |
| Production secrets source | Not evidenced | REQUIRED_BEFORE_STAGING | Use environment secrets, Key Vault, GitHub Secrets, or equivalent | DevOps/infra |
| Grafana/dashboarding | Not necessary yet | OPTIONAL_LATER | Add only after real metrics/logs exist | DevOps |
| Health endpoints | Implemented | COMPLETE_FOR_CODE | Validate in staging | Backend/DevOps |
| Structured logging | Implemented | COMPLETE_FOR_CODE | Validate in staging logs | Backend/DevOps |
| Request telemetry | Implemented | COMPLETE_FOR_CODE | Validate in staging logs | Backend/DevOps |
| Correlation id | Implemented | COMPLETE_FOR_CODE | Validate in staging logs and response headers | Backend/DevOps |

---

## 4. Product and medical workflow gap matrix

| Area | Current state | Classification | Required next action | Owner |
|---|---|---|---|---|
| Patient signature | Needs explicit frontend/backend contract confirmation | REQUIRED_BEFORE_FRONTEND | Define signature storage, consent version, timestamp, evidence hash/path | Product/backend |
| Privacy notice consent | Partially represented by consent flow | REQUIRED_BEFORE_FRONTEND | Confirm consent fields and legal evidence requirements | Product/legal/backend |
| Social security / insurance | Not confirmed as stable contract | REQUIRED_BEFORE_FRONTEND | Add/verify hasSocialSecurity and socialSecurityProvider fields | Product/backend |
| Emergency contact | Not confirmed as stable contract | REQUIRED_BEFORE_FRONTEND | Add/verify name, phone, relationship, optional notes | Product/backend |
| Migrant/incomplete data handling | Requirement known | REQUIRED_BEFORE_FRONTEND | Confirm optional fields and minimum patient identity rule | Product/backend |
| Patient intake validation | Needs contract freeze | REQUIRED_BEFORE_FRONTEND | Define required/optional fields and validation messages | Product/backend/frontend |
| Services per brigade | Core requirement known | REQUIRED_BEFORE_FRONTEND | Confirm endpoint and DTO shape for enabled services | Product/backend |
| Offline sync workflow | Backend has sync processing work | REQUIRED_BEFORE_FRONTEND | Freeze payload format and conflict/rejection UX contract | Backend/frontend |
| OpenAPI contract | Not declared frozen | REQUIRED_BEFORE_FRONTEND | Generate/review Swagger/OpenAPI for frontend team | Backend/frontend |

---

## 5. What is unnecessary right now

| Item | Decision | Reason |
|---|---|---|
| Grafana dashboards | OPTIONAL_LATER | No real staging metrics/log export yet |
| Full external pentest | LATER | Requires deployed staging target |
| WAF/CDN rules | LATER | Deployment topology not finalized |
| Production go-live approval | NOT_ALLOWED | Evidence incomplete |
| Frontend full build | NOT_YET | Functional API contract not frozen |
| Blockchain audit trail | OPTIONAL_LATER | Not needed for MVP health brigade workflow |
| LLM API gateway | OPTIONAL_LATER | Not needed for MVP production readiness |

---

## 6. SQL Server VM interpretation

The SQL Server VM does not create the backend.

The SQL Server VM hosts the database.

The backend API must connect to that SQL Server VM.

Required connection evidence:

- SQL Server hostname or internal IP;
- database name;
- app SQL user;
- least privilege role;
- TLS support;
- firewall rule from API host to SQL Server;
- deny direct public SQL access;
- connection string from secret source;
- smoke test result;
- backup and restore evidence.

---

## 7. Frontend readiness decision

Frontend readiness conclusion: NOT_READY_FOR_FULL_FRONTEND

The frontend can start only after:

- patient intake contract is frozen;
- consent/signature contract is frozen;
- insurance/social security fields are frozen;
- emergency contact fields are frozen;
- optional patient data rules are frozen;
- offline sync contract is frozen;
- OpenAPI contract is reviewed.

Allowed frontend work before full readiness:

- design system;
- shell layout;
- navigation;
- mock screens;
- static Figma-like implementation;
- API client scaffolding against mocked contracts.

Not allowed yet:

- full patient intake implementation against unstable fields;
- full consent/signature implementation without evidence contract;
- production API integration;
- assuming SQL Server/staging readiness.

---

## 8. Production readiness decision

Production readiness conclusion: NOT_PRODUCTION_READY

Production remains blocked until:

- staging deployment exists;
- SQL Server smoke passes against real target;
- health smoke passes against deployed API;
- production-like auth is configured;
- production-like CORS and AllowedHosts are configured;
- rate limiting is validated;
- secrets source is configured;
- SQL least privilege is evidenced;
- ACL/firewall deny-by-default posture is evidenced;
- backup/restore is evidenced;
- rollback is evidenced;
- observability is evidenced;
- incident response drill or tabletop is evidenced;
- final blocker matrix is completed;
- deployment evidence record is completed.

---

## 9. Recommended immediate next PRs

Recommended execution order:

1. P3-30A patient intake functional contract.
2. P3-30B consent and signature evidence contract.
3. P3-30C emergency contact and insurance fields.
4. P3-30D OpenAPI/frontend contract freeze.
5. P3-31A SQL Server VM connectivity baseline.
6. P3-31B SQL least privilege baseline.
7. P3-31C network ACL/firewall baseline.
8. P3-32A CodeQL/static analysis.
9. P3-32B Dependabot/dependency scanning.
10. P3-33A staging environment readiness scaffold.

After P3-30D, frontend can begin safely in parallel with staging/security work.