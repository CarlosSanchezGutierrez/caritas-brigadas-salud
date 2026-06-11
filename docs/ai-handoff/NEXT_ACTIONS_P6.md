# Next Actions: P6 Brigade Operations

The next major module should be P6 Brigades / Brigade Operations.

## Why P6 is next

P5 answers:

- Who is the patient?
- What patient data exists?
- How is patient data created safely?
- How is patient data audited?
- How does patient longitudinal history appear?

P6 should answer:

- Where and when did the brigade happen?
- Who coordinated the brigade?
- What services were available?
- What patients were attended during the brigade?
- What operational status did the brigade have?
- How can the organization close and summarize a brigade day?

## Suggested P6 breakdown

### P6.1 Brigade operational contracts and lifecycle

Goal:

Define and harden the operational backbone for brigades.

Likely scope:

- Inspect existing brigade entities, DTOs, controllers, services, repositories, mappings, tests, and docs.
- Define or harden brigade lifecycle/status.
- Validate organization scoping.
- Validate schedule, community, location, coordinator, and status transitions.
- Create evidence docs, QA matrix, runbook, and verifier.
- Do not add inventory or dashboards yet.
- Do not claim production readiness.

### P6.2 Brigade service encounters / attention flow

Goal:

Connect patients with brigade visits and services.

Likely scope:

- Patient visit inside brigade.
- Service encounter inside visit.
- Provider/user assignment.
- Status flow for encounter.
- Clinical/audit evidence.
- Read models for brigade-level attention lists.

### P6.3 Brigade staff and operational resources

Goal:

Model the team and operational capacity.

Likely scope:

- Staff assigned to brigade.
- Volunteer/doctor/nurse/operator roles.
- Capacity estimates.
- Operational availability.
- Later linkage to inventory and mobile units.

### P6.4 Brigade closure and operational summary

Goal:

Allow controlled closure of a brigade day.

Likely scope:

- Close brigade.
- Count patients attended.
- Count visits.
- Count services.
- Detect incomplete records.
- Produce operational summary.
- Evidence and verifier.

### P6.5 Brigade module closure

Goal:

Close P6 as a controlled backend milestone.

Likely scope:

- Verify all P6 docs.
- Verify tests.
- Verify no production claims.
- Verify no secrets.
- Verify no real patient data.
- Create closure document and runbook.

## Important instruction for Claude

Before implementing P6.1, inspect the current repository state. Do not invent file names, entities, routes, or tests.

Recommended first searches:

- `Brigade`
- `BrigadesController`
- `BrigadeStatus`
- `BrigadeReadRepository`
- `BrigadeWriteRepository`
- `PatientVisit`
- `ServiceEncounter`
- `SourceBrigadeId`
- `AuditActionCodes`
- `OperationalWriteAuditActionMapper`
- `ClinicalWriteAuditActionMapper`

Then propose a narrow P6.1 implementation plan before editing files.