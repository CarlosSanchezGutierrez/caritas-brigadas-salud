# P3 Architecture & Business Rules Decision Register

Status: active  
Scope: backend architecture, business rules, authorization, cloud readiness, and traffic governance  
Target phase: P3  
Application: Caritas Brigadas de Salud Backend

---

## 1. Purpose

This document defines the architectural and business-rule decisions that must guide P3.

P2 closed the data integrity baseline. P3 must now harden tenant boundaries, authorization, business behavior, cloud readiness, offline/sync policy, and traffic governance before more endpoints or frontend work increase the cost of change.

This register exists to avoid ambiguity, overengineering, hidden technical debt, and inconsistent business behavior.

---

## 2. P3 execution order

P3 must follow this sequence:

| Step | Name | Purpose |
|---|---|---|
| P3-00 | Architecture & Business Rules Decision Register | Freeze initial decisions before audits and implementation. |
| P3-01 | Tenant Boundary & Authorization Hardening Inventory | Inventory all endpoints, commands, queries, repositories, claims, policies, and tenant access paths. |
| P3-02 | Endpoint Authorization Contract Tests | Add tests that enforce expected authorization policies per controller/action. |
| P3-03 | Query Tenant Scope Contract Tests | Add tests that prevent cross-tenant reads/writes in repository and service paths. |
| P3-04 | Clinical Business Rules Baseline | Define patient, visit, encounter, expediente, and update rules. |
| P3-05 | Vital Signs Domain Model | Add domain model for historical vital signs. |
| P3-06 | Patient Clinical Record Read Model | Define expediente/clinical-record aggregation without collapsing historical data. |
| P3-07 | Offline Sync Policy Baseline | Define manual/automatic sync, backoff, conflict handling, bandwidth policy, and deferred device behavior. |
| P3-08 | Azure Cloud Readiness Baseline | Define Azure SQL, Key Vault, Private Link, Managed Identity, observability, and deployment rules. |
| P3-09 | Traffic Governance / Zero Trust Baseline | Define deny-by-default, ACL, CORS, rate limits, ingress/egress, telemetry, and payload controls. |
| P3-10 | P3 Release Documentation | Close the phase with architecture, validation, and remaining work. |

---

## 3. Architectural decision: database modularity

The database must remain modular by schema and business capability.

Current logical modules:

- core
- brigades
- clinical
- forms
- documents
- sync
- audit
- operations

Rules:

- core owns organizations, users, roles, permissions, and services.
- brigades owns communities, mobile units, brigades, and brigade service availability.
- clinical owns patient records, visits, encounters, referrals, medication deliveries, and future vital signs.
- forms owns configurable clinical/service forms and responses.
- documents owns consent, signatures, document templates, and media releases.
- sync owns offline/sync batches and events.
- audit owns traceability and evidence.
- operations owns operational telemetry and process-level data.

No module should bypass tenant boundaries.

No module should introduce cascade delete.

No module should create DeviceId strong FKs until the device lifecycle policy is approved.

---

## 4. Architectural decision: SQL Server and Azure readiness

The backend must remain compatible with:

- local SQL Server;
- managed SQL Server infrastructure;
- Azure SQL Database;
- Azure SQL Managed Instance if later required.

Cloud readiness rules:

- runtime database credentials must use minimum privilege;
- migration credentials must be separate from runtime credentials;
- production secrets must not live in committed files;
- connection strings must be environment-specific;
- transient failure handling must be supported for cloud database connectivity;
- migrations must not run automatically at API startup;
- SQL deployment must use reviewed scripts and evidence templates;
- Azure Key Vault or equivalent secret store must be used for production secrets;
- public database access should be denied in production-grade environments;
- private connectivity should be preferred for production-grade environments.

---

## 5. Business decision: patient identity and expediente

The patient record must not be treated as a flat form.

A patient expediente is the aggregate history of:

- Patient;
- PatientGuardian;
- PatientVisit;
- ServiceEncounter;
- future VitalSignsRecord;
- MedicalReferral;
- MedicationDelivery;
- FormResponse;
- DocumentSignature;
- MediaRelease.

The expediente should be exposed later through a read model, not by duplicating all clinical history into the Patient table.

Patient identity data can be reused between visits.

Clinical measurements must remain historical.

---

## 6. Business decision: patient data recapture

Patient data must be split into stable, confirmable, and visit-specific data.

Stable or slowly changing data:

- name;
- birth date;
- sex/gender fields if later defined;
- CURP or external identifier when available;
- contact phone;
- guardian/contact relationship.

Visit-specific or clinically changing data:

- weight;
- height;
- blood pressure;
- temperature;
- oxygen saturation;
- heart rate;
- respiratory rate;
- glucose when applicable;
- symptoms;
- diagnosis or service notes.

Rules:

- stable patient data can be reused when an expediente exists;
- stable data may be reconfirmed instead of recaptured;
- changing clinical data must be captured as historical records;
- weight and height should not overwrite previous clinical measurements without history;
- updates to patient identity data must be auditable;
- the system should support a configurable recapture interval for administrative confirmation.

Initial proposed rule:

- if patient administrative data was confirmed within the last 6 months, allow quick confirmation instead of full recapture;
- if older than 6 months, prompt reconfirmation;
- vital signs remain per visit or encounter unless the brigade configuration explicitly marks them not applicable.

---

## 7. Business decision: vital signs

Vital signs must be modeled as historical clinical records.

Future entity candidate:

- VitalSignsRecord

Expected fields:

- Id;
- OrganizationId;
- PatientId;
- VisitId;
- EncounterId optional;
- SystolicBloodPressure;
- DiastolicBloodPressure;
- HeartRate;
- RespiratoryRate;
- Temperature;
- OxygenSaturation;
- WeightKg;
- HeightCm;
- GlucoseMgDl optional;
- MeasuredAt;
- MeasuredByUserId;
- Source;
- Notes;
- CreatedAt;
- UpdatedAt;
- IsDeleted if lifecycle policy requires soft delete.

Rules:

- systolic and diastolic blood pressure must be stored separately;
- vital signs must not be collapsed into the Patient table;
- vital signs must be historical;
- vital signs must be tenant-scoped;
- vital signs must be associated to a visit;
- encounter association may be optional depending on workflow;
- future validation must reject impossible values;
- future audit must track who measured or captured the values.

---

## 8. Business decision: visits and encounters

A PatientVisit represents the patient's attendance to a brigade or care event.

A ServiceEncounter represents a specific service delivered during a visit.

Rules:

- one patient can have many visits;
- one visit can have many service encounters;
- service-specific forms attach to encounters;
- documents and media releases may attach to patient, visit, or encounter depending on context;
- referrals and medication deliveries should attach to encounter when clinically meaningful;
- encounters must remain tenant-scoped.

---

## 9. Business decision: offline and sync

Offline capability is valuable for health brigades, but it must not waste bandwidth or create hidden complexity.

The system should support:

- Online mode;
- Offline mode;
- Degraded connectivity mode;
- manual sync button;
- automatic sync with conservative backoff;
- sync status visibility;
- pending change count;
- conflict review;
- idempotent sync events;
- payload size limits;
- delta sync where possible;
- no aggressive infinite retry loops.

Rules:

- the app must not constantly consume bandwidth trying to reconnect;
- sync retries must use backoff;
- users should be able to trigger sync manually;
- sync events must be auditable;
- DeviceId strong FK remains deferred until lifecycle policy is approved;
- conflict resolution must be explicit;
- sync must not bypass authorization or tenant boundary checks.

---

## 10. Security decision: Zero Trust and deny-by-default

P3 must enforce deny-by-default behavior.

Rules:

- every protected endpoint must have explicit authorization;
- every data access path must be tenant-scoped unless explicitly global-only;
- global-only operations must be limited to SuperAdmin or equivalent institutional role;
- tenant admins must not gain global data access;
- claims must be validated consistently;
- legacy claims must be handled deliberately and documented;
- CORS must be explicit;
- rate limiting must be enabled for exposed APIs;
- sensitive operations must be audited;
- no endpoint should rely on frontend-only hiding.

Zero Trust baseline:

- verify explicitly;
- use least privilege;
- assume breach;
- segment access;
- log and monitor sensitive actions.

---

## 11. Traffic governance decision

The backend must avoid unnecessary traffic and uncontrolled bandwidth consumption.

Rules:

- APIs must support pagination for list endpoints;
- large responses must be avoided by default;
- clients should request only necessary resources;
- sync should use deltas where possible;
- file/document payloads should not be repeatedly downloaded unnecessarily;
- retries must be bounded;
- rate limits must protect expensive endpoints;
- health checks must be lightweight;
- telemetry must not include sensitive patient data;
- future API gateway/WAF rules should be documented before production exposure.

---

## 12. Tenant boundary decision

Tenant boundary is a P3 priority.

Every future P3 test and endpoint review must classify access as one of:

- public;
- authenticated global;
- authenticated tenant-scoped;
- authenticated self-scoped;
- global-only;
- system/internal only.

Rules:

- organization-specific data must require OrganizationId scope;
- tenant users must not query other organizations;
- tenant admins must not act outside their organization;
- SuperAdmin/global operators must be explicit and audited;
- repositories and queries must enforce tenant filters;
- controllers alone are not enough; tenant safety must exist below the controller layer too.

---

## 13. API and endpoint decision

Endpoint design must remain explicit, versioned, and policy-driven.

Rules:

- all new endpoints must live under the approved API versioning convention;
- every endpoint must declare authorization intent;
- every mutation must validate tenant scope;
- every mutation should be auditable when affecting sensitive data;
- no endpoint should expose raw internal EF entities;
- request/response contracts should remain in Contracts/Application layers as appropriate;
- endpoints must not create circular dependencies between modules.

---

## 14. Audit and traceability decision

Sensitive operations must be auditable.

Audit candidates:

- patient identity updates;
- patient visit creation;
- service encounter creation;
- vital signs capture/update;
- document signature creation;
- role assignment;
- permission assignment;
- organization creation/update;
- sync conflict resolution;
- data cleanup/remediation;
- migration/deployment evidence.

Audit records must avoid storing unnecessary sensitive payloads.

---

## 15. Explicitly out of scope for immediate P3

The following are not immediate P3 implementation targets unless explicitly approved:

- blockchain;
- LLM API gateway;
- advanced analytics;
- Power BI dashboards;
- full mobile app implementation;
- frontend redesign;
- production Azure deployment;
- strong DeviceId FK enforcement;
- automated medical diagnosis;
- direct medical recommendations.

These can remain in long-term architecture, but P3 focuses on authorization, tenant safety, clinical business rules, cloud readiness, and traffic governance.

---

## 16. P3 acceptance criteria

P3-00 is complete when:

- this decision register exists;
- a verifier protects the document;
- repository governance gate validates it;
- the Verify workflow passes;
- P3-01 can start with clear rules for tenant boundary and authorization inventory.