# P5.6 Patient Validation and Organization Authorization Matrix

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Acceptance criteria

| Area | Required evidence | Required for P5.6 merge | Production-closing |
|---|---|---:|---:|
| Organization-scoped get | GetByIdAsync receives organizationId and patientId | Yes | No |
| Query scoping | Patient read query filters by OrganizationId before returning | Yes | No |
| Empty route ids | Empty organizationId or patientId returns null for controller-level NotFound handling | Yes | No |
| Controller boundary | Controller calls organization-scoped GetByIdAsync | Yes | No |
| No post-query mismatch workaround | Controller does not rely on patient.OrganizationId mismatch after broad lookup | Yes | No |
| Create request guard | Write repository rejects null create request | Yes | No |
| Organization id validation | Write repository rejects Guid.Empty organization id | Yes | No |
| Minimum identity validation | Create requires at least one identity signal | Yes | No |
| Partial record validation | Partial record requires reason | Yes | No |
| Source brigade authorization | SourceBrigadeId must belong to the same organization | Yes | No |
| Verifier | P5.6 verifier passes | Yes | No |
| Build | API project builds in Release | Yes | No |

## Rejection criteria

Reject P5.6 if patient reads can return records from another organization, if patient creation accepts empty organization ids, if SourceBrigadeId is not organization-scoped, if validation failures are hidden as success, if backend readiness is approved, if direct mobile SQL Server writes are allowed, or if cloud becomes mandatory.