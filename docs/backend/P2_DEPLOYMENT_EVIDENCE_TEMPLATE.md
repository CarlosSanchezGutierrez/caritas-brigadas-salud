# P2 Deployment Evidence Template

Status: active  
Scope: backend database deployment evidence  
Target database: SQL Server  
Related baseline: P2 data integrity

---

## 1. Purpose

This template standardizes the evidence required for staging and production database deployments related to the P2 data integrity baseline.

It must be copied and filled for each real migration event.

Do not store secrets, passwords, connection strings, private keys, tokens, or patient-sensitive data in this document.

---

## 2. Deployment identification

| Field | Value |
|---|---|
| Environment |  |
| Database name |  |
| Database server |  |
| Application/service | Caritas Brigadas de Salud Backend |
| Deployment type | Staging dry-run / Production deployment / Restore validation |
| Deployment date |  |
| Deployment window |  |
| Target commit SHA |  |
| Source branch |  |
| Pull request |  |
| Operator |  |
| Reviewer / approver |  |

---

## 3. Repository validation evidence

Attach or paste command output summary.

Required commands:

~~~powershell
powershell -ExecutionPolicy Bypass -File "scripts/verify-no-mojibake.ps1"
powershell -ExecutionPolicy Bypass -File "scripts/verify-p2-orphan-detection-sql.ps1"
powershell -ExecutionPolicy Bypass -File "scripts/verify-p2-data-cleanup-remediation-playbook.ps1"
powershell -ExecutionPolicy Bypass -File "scripts/verify-p2-migration-dry-run-checklist.ps1"
powershell -ExecutionPolicy Bypass -File "scripts/verify-database-foreign-key-baseline.ps1"
powershell -ExecutionPolicy Bypass -File "scripts/validate-database-deployment-baseline.ps1"

Set-Location "services/api-dotnet"
dotnet build "Caritas.Brigadas.sln" -warnaserror
dotnet test "Caritas.Brigadas.sln" -warnaserror --no-build
~~~

| Check | Result | Evidence location |
|---|---|---|
| Mojibake verification |  |  |
| Orphan detection SQL verifier |  |  |
| Cleanup remediation playbook verifier |  |  |
| Migration dry-run checklist verifier |  |  |
| FK baseline gate |  |  |
| Database deployment baseline gate |  |  |
| Backend build |  |  |
| Backend tests |  |  |

---

## 4. Backup evidence

| Field | Value |
|---|---|
| Backup completed | Yes / No |
| Backup timestamp |  |
| Backup type | Full / Differential / Snapshot / Other |
| Backup location |  |
| Restore tested | Yes / No |
| Restore test evidence |  |
| RPO |  |
| RTO |  |
| Backup owner |  |

Do not continue without backup evidence.

---

## 5. Orphan detection evidence

Script:

`database/diagnostics/sqlserver/p2_detect_fk_orphans.sql`

| Field | Value |
|---|---|
| Script commit SHA |  |
| Execution timestamp |  |
| Execution user |  |
| Output location |  |
| total_orphans |  |
| required_fk_orphans |  |
| optional_fk_orphans |  |
| Deferred DeviceId rows reviewed | Yes / No |

If any orphan count is greater than zero, stop and follow `docs/backend/P2_DATA_CLEANUP_REMEDIATION_PLAYBOOK.md`.

---

## 6. Migration script evidence

| Field | Value |
|---|---|
| Script generated with approved repo script | Yes / No |
| Script path |  |
| Script checksum |  |
| Idempotent script | Yes / No |
| Reviewed by |  |
| Contains ON DELETE CASCADE | No |
| Contains ON DELETE SET NULL | No |
| Contains strong DeviceId FKs | No |
| Uses migration user | Yes / No |
| Runtime user used for migration | No |

Approved generation command:

~~~powershell
powershell -ExecutionPolicy Bypass -File "scripts/db-generate-migration-script.ps1"
~~~

---

## 7. Dry-run evidence

| Field | Value |
|---|---|
| Dry-run environment |  |
| Dry-run database |  |
| Dry-run start time |  |
| Dry-run end time |  |
| Dry-run result | Passed / Failed |
| SQL output location |  |
| Errors or warnings |  |
| Migration history validated | Yes / No |
| FK constraints validated | Yes / No |
| API startup validated | Yes / No |

---

## 8. Production deployment evidence

Complete this section only for production deployment.

| Field | Value |
|---|---|
| Production approval |  |
| Deployment start time |  |
| Deployment end time |  |
| Executing user |  |
| Migration user |  |
| SQL output location |  |
| Deployment result | Passed / Failed |
| Errors or warnings |  |
| Rollback triggered | Yes / No |
| Rollback reason |  |

---

## 9. Post-deployment validation

| Validation | Result | Evidence location |
|---|---|---|
| Orphan detection after migration |  |  |
| FK constraints exist |  |  |
| EF migration history valid |  |  |
| API starts without applying migrations |  |  |
| Health endpoint |  |  |
| Organization security restrictions |  |  |
| Roles and permissions |  |  |
| Patient flow |  |  |
| Patient visit flow |  |  |
| Service encounter flow |  |  |
| Form response flow |  |  |
| Document signature flow |  |  |
| Sync batch/event flow |  |  |

---

## 10. Rollback evidence

| Field | Value |
|---|---|
| Rollback strategy | Restore-based / Script-based / Not needed |
| Rollback owner |  |
| Rollback approval required | Yes / No |
| Rollback plan location |  |
| Rollback tested | Yes / No |
| Rollback executed | Yes / No |
| Rollback result |  |

---

## 11. Final decision

| Field | Value |
|---|---|
| Deployment accepted | Yes / No |
| Accepted by |  |
| Acceptance timestamp |  |
| Follow-up issues created |  |
| Notes |  |

---

## 12. Final rule

If evidence is missing, the deployment is not complete.