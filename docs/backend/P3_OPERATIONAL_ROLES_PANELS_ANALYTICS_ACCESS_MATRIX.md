# P3 Operational Roles, Panels, and Analytics Access Matrix

Status: active  
Scope: backend access design, web panels, operational workflows, analytics access, and developer/data team boundaries  
Target phase: P3-04.2  
Depends on: P3 Clinical Data Governance, Privacy, and Analytics Baseline

---

## 1. Purpose

This document defines how operational users, clinical users, office capturers, administrators, developers, analysts, data engineers, and data scientists should interact with the platform.

The goal is to make the system useful without requiring programmers for daily operations, while preserving tenant boundaries, privacy, auditability, and analytics readiness.

---

## 2. Core principle

Every user must see only what they need, do only what their role allows, and access only the data purpose approved for that role.

Default rule: deny by default.

---

## 3. Role groups

| Role group | Main purpose | Default access posture |
|---|---|---|
| SuperAdmin | Institutional/global control. | Global-only, audited. |
| OrganizationAdmin | Manage one organization's users, services, brigades, and reports. | Tenant-scoped. |
| MedicalUser | Capture and review clinical care during brigades. | Tenant-scoped clinical access. |
| OfficeCapturer | Capture or clean administrative patient/visit/document data in offices. | Tenant-scoped limited operational access. |
| AuditReviewer | Review audit and traceability evidence. | Tenant-scoped or global-only depending on assignment. |
| DataAnalyst | Analyze approved datasets and dashboards. | Aggregated or de-identified by default. |
| DataEngineer | Build controlled data pipelines and curated datasets. | Approved data access only. |
| DataScientist | Work on approved statistical/ML datasets. | De-identified, pseudonymized, aggregated, or synthetic by default. |
| Developer | Build and test the system. | Synthetic or anonymized data only by default. |
| SystemActor | Internal job, sync process, migration process, or automation. | System/internal only. |

---

## 4. Web panels

| Panel | Primary users | Purpose |
|---|---|---|
| Admin Panel | SuperAdmin, OrganizationAdmin | Manage organization, users, roles, services, brigades, reports, evidence. |
| Medical Panel | MedicalUser | Clinical workflow: patient, visit, encounter, vital signs, forms, referrals, medication, documents. |
| Office Capture Panel | OfficeCapturer | Patient search, administrative confirmation, document capture, visit cleanup, data quality corrections. |
| Audit Panel | AuditReviewer, OrganizationAdmin, SuperAdmin | Review sensitive actions, changes, exports, corrections, sync conflicts. |
| Analytics Panel | OrganizationAdmin, DataAnalyst | Aggregated operational/clinical dashboards. |
| Data Engineering Panel | DataEngineer | Controlled exports, pipeline health, dataset generation evidence. |
| Developer Support Panel | Developer, SystemActor | Non-production diagnostics, test data, technical health, documentation links. |

No panel should expose data only because the frontend hides a button. Backend authorization remains authoritative.

---

## 5. SuperAdmin access

SuperAdmin can:

- create organizations;
- manage global configuration;
- manage global roles and permissions;
- review global audit when approved;
- perform controlled seed or system setup operations;
- access cross-organization views only when explicitly required.

SuperAdmin must not be treated as a normal tenant user.

Rules:

- all global actions must be auditable;
- global-only operations must use explicit authorization;
- SuperAdmin access must not be granted to tenant admins;
- production access should be exceptional and traceable.

---

## 6. OrganizationAdmin access

OrganizationAdmin can manage only their organization.

Allowed:

- manage users within the organization;
- assign non-global roles within the organization;
- configure services;
- configure brigades;
- review tenant-scoped reports;
- review tenant-scoped audit;
- manage operational corrections if approved.

Not allowed:

- create global organizations;
- assign SuperAdmin;
- grant global-only permissions;
- view other organizations;
- export identified patient data without approved purpose;
- bypass clinical or privacy rules.

---

## 7. MedicalUser access

MedicalUser can work with clinical care inside their organization.

Allowed:

- search patient inside tenant scope;
- create patient visit;
- create service encounter;
- capture vital signs;
- complete clinical forms;
- generate referrals;
- record medication delivery;
- capture consent or document signatures when workflow requires it;
- view relevant clinical history for care.

Not allowed by default:

- export raw datasets;
- view cross-tenant data;
- manage roles/permissions;
- change global configuration;
- access analytics datasets outside approved dashboards.

---

## 8. OfficeCapturer access

OfficeCapturer supports administrative capture and cleanup.

Allowed:

- search patients inside tenant scope;
- create or update administrative patient fields;
- confirm patient information;
- attach documents when approved;
- help complete missing administrative data;
- review data quality issues assigned to them.

Restricted:

- should not see full clinical history unless explicitly approved;
- should not modify clinical measurements without role approval;
- should not export patient-level datasets;
- should not access other organizations;
- should not assign roles or permissions.

Workflow goal:

- office capturers should be able to keep data clean without needing programmers.

---

## 9. DataAnalyst access

DataAnalyst works with dashboards and approved datasets.

Allowed by default:

- aggregated dashboards;
- tenant-scoped reporting;
- de-identified or pseudonymized datasets when approved;
- operational metrics;
- data quality summaries.

Not allowed by default:

- direct patient identifiers;
- raw clinical free text;
- raw document signatures;
- raw production tables;
- cross-tenant patient-level datasets.

Analytics examples:

- patients by sex;
- patients by age band;
- patients by minor/adult status;
- visits by location;
- services delivered by brigade;
- vital signs distributions;
- referrals by service;
- medication delivery counts;
- missing data rates;
- sync failure rates.

Small cohort outputs must be controlled to reduce re-identification risk.

---

## 10. DataEngineer access

DataEngineer builds controlled pipelines.

Allowed:

- build curated views;
- build de-identified datasets;
- build pseudonymized datasets when approved;
- build aggregated marts;
- validate data quality rules;
- monitor pipeline health.

Rules:

- production access must be approved;
- exports must be documented;
- re-identification keys must be protected separately;
- raw operational tables should not be the default analytics interface;
- pipelines must preserve tenant boundaries;
- pipelines must avoid leaking direct identifiers.

---

## 11. DataScientist access

DataScientist works with approved analytical or model datasets.

Allowed by default:

- synthetic datasets;
- aggregated datasets;
- de-identified datasets;
- pseudonymized datasets only when approved.

Not allowed by default:

- raw patient names;
- phone numbers;
- CURP;
- signatures;
- full clinical free text;
- raw production database dumps;
- patient-identifiable LLM prompts;
- model training on identifiable clinical data without governance approval.

Rules:

- model experiments must not write back to operational clinical records;
- datasets must document purpose, owner, fields, and de-identification method;
- results must avoid small-cohort re-identification.

---

## 12. Developer access

Developers build the product but should not depend on production patient data.

Allowed:

- synthetic seed data;
- anonymized sample data;
- fake patients;
- fake clinical values;
- schema and migrations;
- logs without sensitive patient data;
- technical documentation.

Not allowed by default:

- production backups;
- raw production patient tables;
- real signatures;
- real document images;
- production sync payloads;
- screenshots with real patient information.

Developer support material must include:

- entity diagrams;
- module map;
- endpoint inventory;
- permission map;
- tenant boundary rules;
- database migration playbooks;
- testing strategy;
- data classification guide.

---

## 13. SystemActor access

SystemActor includes jobs, sync processes, migration users, and automation.

Rules:

- system access must be explicit;
- migration user must be separate from runtime user;
- sync actor must be tenant-scoped;
- background jobs must not bypass tenant boundaries;
- system operations must be auditable where sensitive;
- system actors must not use broad permissions casually.

---

## 14. Data pipeline expectations

The system should support professional data usage from day one.

Data must be usable for:

- operational reporting;
- clinical summaries;
- quality control;
- service planning;
- public health-style aggregated insights;
- data engineering pipelines;
- data science experiments after governance approval.

Required analytical dimensions:

- organization;
- brigade;
- community;
- municipality;
- colony;
- service;
- patient sex;
- minor/adult status;
- age or age band;
- migrant/partial-record status when approved;
- visit date;
- encounter/service date;
- vital signs values with canonical units;
- referral status;
- medication delivery status;
- consent/document completion;
- sync status;
- data quality status.

Rules:

- direct identifiers must not be part of default analytics datasets;
- analytics should use de-identified, pseudonymized, aggregated, or synthetic data depending on purpose;
- identified operational data remains separate from analytics-ready data;
- analytics views must be intentionally designed, not created from unrestricted raw table access.

---

## 15. Admin reporting expectations

Administrators need self-service reporting without programmers.

Admin reporting should support:

- total patients captured;
- total visits;
- total encounters by service;
- services delivered by brigade;
- visits by location;
- patients by sex;
- patients by age band;
- minor/adult counts;
- referrals created;
- medication deliveries;
- consent completion rate;
- form completion rate;
- sync pending/error counts;
- data quality issues;
- active users and capture productivity.

Rules:

- reports must be tenant-scoped;
- identified drill-down requires permission;
- exports require explicit access;
- sensitive reports must be auditable;
- dashboards must avoid exposing unnecessary patient identity.

---

## 16. Medical reporting expectations

Medical reporting should help clinical coordination without creating unsafe diagnosis automation.

Allowed:

- vital signs history;
- visit history;
- encounter history;
- referral history;
- medication delivery history;
- form summaries;
- follow-up flags.

Not allowed in P3:

- automatic diagnosis;
- automated treatment recommendation;
- LLM clinical assistant;
- AI triage without approved governance.

---

## 17. Office capture workflow expectations

Office capture should reduce operational friction.

Expected capabilities:

- search patient inside organization;
- create partial patient record when documents are missing;
- update administrative data;
- mark information confirmed;
- identify possible duplicate patients inside organization;
- attach or review required documents when approved;
- see assigned data quality tasks;
- avoid repeated capture of stable data;
- avoid full clinical exposure unless needed.

Rules:

- office capture must preserve audit;
- office capture must not overwrite clinical history;
- office capture must not cross tenant boundaries.

---

## 18. Programmer support material

The repository should maintain programmer-facing material:

- backend master context;
- architecture decision register;
- tenant boundary inventory;
- clinical business rules;
- data governance baseline;
- operational role matrix;
- database migration baseline;
- orphan detection playbook;
- dry-run checklist;
- deployment evidence template;
- endpoint authorization contracts;
- tenant scope contracts.

Future useful artifacts:

- ERD diagrams;
- module dependency diagram;
- endpoint-to-permission matrix;
- role-to-permission matrix;
- data classification dictionary;
- analytics view catalog;
- onboarding guide for new service-social students.

---

## 19. Access matrix

| Capability | SuperAdmin | OrganizationAdmin | MedicalUser | OfficeCapturer | DataAnalyst | DataEngineer | DataScientist | Developer |
|---|---|---|---|---|---|---|---|---|
| Create organization | Yes | No | No | No | No | No | No | No |
| Manage tenant users | Yes | Own tenant | No | No | No | No | No | No |
| Assign SuperAdmin | Yes | No | No | No | No | No | No | No |
| Manage tenant services | Yes | Own tenant | No | No | No | No | No | No |
| Manage brigades | Yes | Own tenant | Limited read | Limited read | Aggregated | Pipeline only | Dataset only | Synthetic only |
| Search patient | Audited | Own tenant | Own tenant | Own tenant | No by default | No by default | No by default | Synthetic only |
| Create patient | Audited | Own tenant if approved | Own tenant | Own tenant | No | No | No | Synthetic only |
| Update patient identity | Audited | Own tenant if approved | Limited | Own tenant if approved | No | No | No | Synthetic only |
| Capture vital signs | No by default | No by default | Own tenant | No by default | No | No | No | Synthetic only |
| View clinical history | Audited | Limited if approved | Own tenant | Limited if approved | Aggregated | Curated only | De-identified only | Synthetic only |
| Export identified patient data | Exceptional | Approved only | No by default | No | No by default | Approved only | No by default | No |
| View dashboards | Global/tenant | Own tenant | Limited | Limited operational | Own approved scope | Pipeline metrics | Approved scope | Non-prod |
| Access raw production DB | Exceptional | No | No | No | No | Exceptional approved | No | No |
| Use analytics dataset | No by default | Aggregated | No by default | No | Approved | Approved | Approved | Synthetic/anonymized |
| Manage migrations | No by default | No | No | No | No | No | No | Approved engineer only |

---

## 20. Acceptance criteria

P3-04.2 is complete when:

- this operational role, panel, and analytics access matrix exists;
- a verifier protects required sections;
- repository governance gate validates it;
- database and security gates remain green;
- P3-05 can implement VitalSignsRecord knowing who captures it, who reads it, and how it is later used for reporting and analytics.