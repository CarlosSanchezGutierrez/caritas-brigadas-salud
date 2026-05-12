# P3 Tenant Boundary & Authorization Hardening Inventory

Status: active  
Scope: backend authorization, tenant isolation, endpoint classification, query scope, and privilege boundaries  
Target phase: P3-01  
Depends on: P3 Architecture & Business Rules Decision Register

---

## 1. Purpose

This inventory defines how the backend must classify and harden authorization and tenant boundaries.

P2 protected relational integrity. P3 must now protect access boundaries.

The goal is to prevent:

- cross-tenant reads;
- cross-tenant writes;
- tenant admins gaining global access;
- global operations being exposed to tenant-scoped roles;
- controller-only security without repository/service tenant enforcement;
- legacy claim drift;
- frontend-only security assumptions;
- missing audit for sensitive operations.

This document is an inventory and hardening plan. It does not change runtime behavior yet.

---

## 2. Access classification model

Every endpoint, command, query, repository method, and service operation must be classified as exactly one of the following:

| Classification | Meaning | Example |
|---|---|---|
| Public | Does not require authentication and does not expose sensitive data. | Health readiness/liveness with no secrets. |
| Authenticated global | Requires authentication but is not tied to one tenant. | Profile discovery, global metadata if later approved. |
| Authenticated tenant-scoped | Requires authentication and OrganizationId scope. | Patient, visit, encounter, forms, documents, sync records. |
| Authenticated self-scoped | User can access only their own resource. | Own profile/session data. |
| Global-only | Requires SuperAdmin or institutional/global role. | Organization creation, global role assignment, global configuration. |
| System/internal only | Not intended for public clients. | Seed, diagnostics, migration helpers, internal jobs. |

Default rule: if classification is unclear, treat it as protected and deny by default.

---

## 3. Tenant boundary principles

Tenant boundary means organization-specific data must not be accessible across organizations.

Rules:

- every tenant-scoped entity must carry or derive OrganizationId;
- every tenant-scoped query must filter by OrganizationId;
- every tenant-scoped command must validate OrganizationId before mutation;
- tenant admins must not operate outside their organization;
- SuperAdmin/global operations must be explicit and auditable;
- repositories must not return cross-tenant data to controllers;
- authorization cannot rely only on frontend visibility;
- authorization cannot rely only on controller checks if service/repository paths can be reused elsewhere.

---

## 4. Claims and principal inventory

Expected user context claims:

| Claim / source | Purpose | Risk if missing or inconsistent |
|---|---|---|
| UserId | Identifies actor. | Audit cannot identify actor. |
| OrganizationId | Tenant boundary. | Cross-tenant data access risk. |
| RoleCode | Role-based behavior. | Privilege ambiguity. |
| LegacyRole | Backward compatibility. | Drift between old/new super admin checks. |
| Permission | Fine-grained authorization. | Overbroad role access. |

Hardening rules:

- CurrentUserContext must normalize supported role claims consistently.
- Legacy claims must remain explicit and tested while supported.
- SuperAdmin checks must treat approved current and legacy claims consistently.
- Tenant-scoped operations must reject missing OrganizationId unless explicitly global-only.
- Global-only operations must not silently fallback to tenant scope.
- Permission policy checks and controller guardrails must agree.

---

## 5. Permission and role inventory

Current role model must distinguish:

| Role class | Intended scope | Notes |
|---|---|---|
| SuperAdmin | Global/institutional | Can perform global-only operations. Must be audited. |
| Admin | Tenant/organization | Must not receive global organization-write grants. |
| Operational roles | Tenant/organization | Must operate only inside assigned organization. |
| Clinical roles | Tenant/organization | Must operate only on clinical records within assigned organization. |
| System actors | Internal | Must be separately governed if introduced. |

Hardening rules:

- global permissions must not be granted to tenant roles;
- seed logic must remove stale global-only permissions from non-global roles;
- role assignment to SuperAdmin must be guarded by SuperAdmin checks;
- tenant admin cannot assign global-only roles;
- permission constants must classify global-only and tenant-scoped permissions.

---

## 6. Endpoint classification inventory

This inventory must include every current controller before P3-02 converts it into endpoint authorization contract tests.

Canonical classifications allowed in this table:

- Public
- Authenticated global
- Authenticated tenant-scoped
- Authenticated self-scoped
- Global-only
- System/internal only

No composite classifications are allowed. Each row must use exactly one canonical classification.

| Controller / area | Endpoint/action type | Classification | Required enforcement |
|---|---|---|---|
| HealthController | Health, liveness, readiness | Public | Must not expose secrets, connection strings, tenant data, patient data, role data, permission data, or deployment internals. |
| OrganizationsController | Create organization | Global-only | Requires SuperAdmin/global permission. Tenant admins must not create organizations. Must be audited. |
| OrganizationsController | List organizations | Global-only | Tenant users must not enumerate all organizations. Any future own-organization read must be a separate tenant-scoped action. |
| OrganizationsController | Read own organization if supported | Authenticated tenant-scoped | Must filter by actor OrganizationId unless actor is global. |
| OrganizationsController | Update organization if supported | Global-only | Tenant-level organization updates require a separate approved tenant-scoped rule before implementation. |
| RolesController | List roles | Authenticated tenant-scoped | Must return roles only for actor OrganizationId unless actor is global. |
| RolesController | Assign tenant role | Authenticated tenant-scoped | Must validate target user, role, and actor organization. |
| RolesController | Assign SuperAdmin/global role | Global-only | Must require SuperAdmin actor and explicit global guardrail. |
| RolesController | Permission assignment | Global-only | Must not allow tenant admins to grant global-only permissions. |
| UsersController | List users | Authenticated tenant-scoped | Must filter users by actor OrganizationId unless actor is global. |
| UsersController | Read user | Authenticated tenant-scoped | Must validate target user belongs to actor OrganizationId unless actor is global. |
| UsersController | Create user | Authenticated tenant-scoped | Must create user only inside actor OrganizationId unless global provisioning is explicitly approved. |
| UsersController | Update user | Authenticated tenant-scoped | Must validate target user belongs to actor OrganizationId and audit sensitive changes. |
| ServicesController | List services | Authenticated tenant-scoped | Must filter by actor OrganizationId. |
| ServicesController | Create service | Authenticated tenant-scoped | Must create service only inside actor OrganizationId. |
| ServicesController | Update service | Authenticated tenant-scoped | Must validate service belongs to actor OrganizationId. |
| ReportsController | Generate/read reports | Authenticated tenant-scoped | Must filter report data by actor OrganizationId. Must not expose cross-tenant aggregates unless a separate global report endpoint is approved. |
| AuditLogsController | Read tenant audit logs | Authenticated tenant-scoped | Must filter audit logs by actor OrganizationId and avoid sensitive payload leakage. |
| AuditLogsController | Read global audit logs if supported | Global-only | Must require SuperAdmin/global permission and be audited. |
| CommunitiesController | List communities | Authenticated tenant-scoped | Must filter by actor OrganizationId. |
| CommunitiesController | Create community | Authenticated tenant-scoped | Must create community only inside actor OrganizationId. |
| CommunitiesController | Update community | Authenticated tenant-scoped | Must validate community belongs to actor OrganizationId. |
| MobileUnitsController | List mobile units | Authenticated tenant-scoped | Must filter by actor OrganizationId. |
| MobileUnitsController | Create mobile unit | Authenticated tenant-scoped | Must create mobile unit only inside actor OrganizationId. |
| MobileUnitsController | Update mobile unit | Authenticated tenant-scoped | Must validate mobile unit belongs to actor OrganizationId. |
| BrigadesController | List brigades | Authenticated tenant-scoped | Must filter by actor OrganizationId. |
| BrigadesController | Create brigade | Authenticated tenant-scoped | Must validate organization, community, and mobile unit tenant ownership. |
| BrigadesController | Update brigade | Authenticated tenant-scoped | Must validate brigade and related entities belong to actor OrganizationId. |
| BrigadeServicesController | List brigade services | Authenticated tenant-scoped | Must filter through brigade/service tenant ownership. |
| BrigadeServicesController | Attach service to brigade | Authenticated tenant-scoped | Must validate brigade and service belong to actor OrganizationId. |
| BrigadeServicesController | Remove service from brigade | Authenticated tenant-scoped | Must validate brigade and service belong to actor OrganizationId. |
| Seed/admin endpoints | Seed defaults or internal setup actions | System/internal only | Must not be publicly exposed. Must require explicit seed/system permission or controlled environment guardrail. |
| Future PatientsController | Patient CRUD | Authenticated tenant-scoped | Must validate Patient.OrganizationId against actor OrganizationId. |
| Future PatientVisitsController | Visit CRUD | Authenticated tenant-scoped | Must validate Patient, Brigade, and Visit tenant ownership. |
| Future ServiceEncountersController | Encounter CRUD | Authenticated tenant-scoped | Must validate Patient, Visit, Brigade, Service, and Encounter tenant ownership. |
| Future FormTemplatesController | Form template management | Authenticated tenant-scoped | Must validate OrganizationId and Service tenant ownership. |
| Future FormResponsesController | Form response submission/read | Authenticated tenant-scoped | Must validate OrganizationId, FormTemplate, and Encounter tenant ownership. |
| Future DocumentTemplatesController | Document template management | Authenticated tenant-scoped | Must validate OrganizationId and optional service ownership. |
| Future DocumentSignaturesController | Document signature capture/read | Authenticated tenant-scoped | Must validate OrganizationId, patient, visit, encounter, and document template ownership. |
| Future MediaReleasesController | Media release capture/read | Authenticated tenant-scoped | Must validate OrganizationId, patient, and visit ownership. |
| Future SyncController | Sync batch/event submission | Authenticated tenant-scoped | Must validate actor OrganizationId, payload OrganizationId, and deferred DeviceId policy. |

## 7. Data domain tenant scope inventory

| Domain | Tenant boundary rule |
|---|---|
| core.organizations | Global root; access is global-only except own-org read if approved. |
| core.users | Tenant-scoped by OrganizationId unless global operator. |
| core.roles | Tenant-scoped by OrganizationId; SuperAdmin role assignment is global-guarded. |
| core.permissions | Global catalog; grants are controlled. |
| core.services | Tenant-scoped by OrganizationId. |
| brigades.communities | Tenant-scoped by OrganizationId. |
| brigades.mobile_units | Tenant-scoped by OrganizationId. |
| brigades.brigades | Tenant-scoped by OrganizationId. |
| brigades.brigade_services | Derived tenant scope through Brigade and Service. |
| clinical.patients | Tenant-scoped by OrganizationId. |
| clinical.patient_guardians | Derived tenant scope through Patient. |
| clinical.patient_visits | Tenant-scoped by OrganizationId; must match Patient and Brigade tenant. |
| clinical.service_encounters | Tenant-scoped by OrganizationId; must match Patient, Visit, Brigade, Service tenant. |
| clinical.medical_referrals | Tenant-scoped by OrganizationId; must match Patient and Encounter. |
| clinical.medication_deliveries | Tenant-scoped by OrganizationId; must match Patient and Encounter. |
| forms.form_templates | Tenant-scoped by OrganizationId and Service. |
| forms.form_responses | Tenant-scoped by OrganizationId and Encounter. |
| documents.document_templates | Tenant-scoped by OrganizationId. |
| documents.document_signatures | Tenant-scoped by OrganizationId; must match patient/visit/encounter. |
| documents.media_releases | Tenant-scoped by OrganizationId; must match patient/visit. |
| sync.sync_batches | Tenant-scoped by OrganizationId; DeviceId policy deferred. |
| sync.sync_events | Tenant-scoped by OrganizationId and SyncBatch. |
| audit.* | Tenant-scoped when tied to tenant data; global audit for global actions. |
| operations.* | Depends on operational object; default tenant-scoped unless explicitly global. |

P3-03 must convert this inventory into query/service/repository tenant-scope contract tests.

---

## 8. Authorization hardening risks

Known risk categories to audit in P3:

| Risk | Description | Target follow-up |
|---|---|---|
| Controller-only checks | Controller blocks access but service/repository can still fetch cross-tenant data if reused. | P3-03 |
| Missing OrganizationId claim | Tenant endpoint accepts a request with no tenant scope. | P3-02 |
| Global-only drift | A global permission accidentally appears on tenant role. | Continue P1/P2 guardrails |
| Legacy claim drift | Legacy SuperAdmin claim authorized in one path but denied in another. | P3-02 |
| List endpoint leakage | List endpoint returns all organizations/roles/patients without tenant filter. | P3-03 |
| Relationship mismatch | Mutation links patient from one tenant to brigade/service from another tenant. | P3-04/P3-05 |
| Seed endpoint exposure | Seed endpoint callable outside controlled environment. | P3-02 |
| Sync bypass | Offline sync payload creates records outside actor tenant. | P3-07 |
| Audit gaps | Sensitive action has no audit evidence. | Later P3/P4 audit package |

---

## 9. Required P3-02 endpoint authorization tests

P3-02 must add or extend tests for:

- every controller has explicit authorization posture;
- public endpoints are intentionally allow-anonymous and safe;
- global-only endpoints require SuperAdmin/global policy;
- tenant-scoped endpoints reject missing OrganizationId;
- tenant-scoped endpoints reject unauthenticated users;
- SuperAdmin assignment requires SuperAdmin actor;
- legacy SuperAdmin role claim is consistently recognized while supported;
- seed endpoints require explicit seed permissions or system-only guardrails;
- no mutation endpoint relies only on frontend hiding.

---

## 10. Required P3-03 tenant scope tests

P3-03 must add or extend tests for:

- repository/service queries include OrganizationId filters where required;
- list endpoints cannot leak data across organizations;
- reads by ID validate tenant ownership;
- updates by ID validate tenant ownership;
- creates validate related entity ownership;
- clinical records cannot link entities from different organizations;
- forms/documents cannot attach to patient/visit/encounter from another organization;
- sync events cannot create cross-tenant payloads.

---

## 11. Tenant boundary for expediente and clinical record

Future expediente read model must be tenant-safe.

Rules:

- expediente access requires authenticated tenant scope;
- patient must belong to actor OrganizationId;
- all included visits, encounters, forms, documents, referrals, medications, and vital signs must belong to the same OrganizationId or derive from same patient/visit boundary;
- cross-tenant joins must be impossible or explicitly rejected;
- SuperAdmin access must be explicit and audited.

---

## 12. Tenant boundary for vital signs

Future VitalSignsRecord must be tenant-scoped.

Rules:

- OrganizationId required;
- PatientId required;
- VisitId required;
- EncounterId optional;
- MeasuredByUserId should identify actor when available;
- Patient, Visit, Encounter, and Organization must match;
- values must be historical, not overwritten into Patient;
- updates must be auditable.

---

## 13. Tenant boundary for offline/sync

Offline/sync must not weaken tenant isolation.

Rules:

- sync batch must carry OrganizationId;
- sync event must carry OrganizationId;
- actor must be allowed to sync for that OrganizationId;
- payload records must be validated against tenant scope before persistence;
- SyncBatch.DeviceId remains deferred as strong FK, but DeviceId must not become a tenant bypass;
- conflicts must not leak records from other tenants;
- sync retries must not bypass authorization.

---

## 14. Zero Trust and traffic governance implications

Tenant boundary hardening must align with Zero Trust:

- deny by default;
- least privilege;
- explicit verification;
- no broad wildcard CORS in production;
- no public SQL database exposure in production-grade environments;
- no secrets in repo;
- rate limits for exposed APIs;
- bounded retries;
- lightweight health checks;
- no sensitive patient data in telemetry.

---

## 15. P3-01 output

P3-01 is complete when:

- this inventory exists;
- a verifier protects required sections;
- repository governance gate validates the inventory;
- Verify workflow passes;
- P3-02 can start endpoint authorization contract tests;
- P3-03 can start query tenant-scope contract tests.