# P5.7 Patient Write Audit Evidence Matrix

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Acceptance criteria

| Area | Required evidence | Required for P5.7 merge | Production-closing |
|---|---|---:|---:|
| Clinical patient audit mapping | /patients POST maps to AuditActionCodes.PatientCreate in ClinicalWriteAuditActionMapper | Yes | No |
| Entity name | Patient create audit uses entityName Patient | Yes | No |
| No duplicate operational mapping | /patients POST is not also mapped in OperationalWriteAuditActionMapper | Yes | No |
| Success-only behavior | Clinical audit filter audits only successful write results | Yes | No |
| Created response coverage | CreatedAtActionResult is treated as 201 success | Yes | No |
| Entity id extraction | Clinical filter can extract Data.Id from ApiResponse payloads | Yes | No |
| Organization id extraction | Clinical filter can extract route or action organization id | Yes | No |
| Existing logger path | HttpAuditLogger writes through IAuditLogWriteRepository | Yes | No |
| Sanitized logging | Audit logging failure omits sensitive audit metadata from application logs | Yes | No |
| Verifier | P5.7 verifier passes | Yes | No |
| Build | API project builds in Release | Yes | No |

## Rejection criteria

Reject P5.7 if patient creation is not mapped to patients.create in the clinical audit mapper, if patient creation is also mapped in the operational audit mapper, if entityName is not Patient, if failed writes are audited as successful writes, if direct mobile SQL Server writes are allowed, if secrets are added, if real patient data is committed, if a cloud dependency is introduced, or if production readiness closure is claimed.