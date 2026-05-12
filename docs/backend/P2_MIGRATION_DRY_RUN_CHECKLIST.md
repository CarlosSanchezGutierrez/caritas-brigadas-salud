# P2 Migration Dry-Run Checklist

Status: active  
Scope: backend database deployment  
Target database: SQL Server  
Related baseline: P2 data integrity

---

## 1. Purpose

This checklist defines the minimum controlled process for dry-running and applying P2 database migrations.

It exists to prevent unsafe production changes, unverified FK failures, missing backups, undocumented rollback paths, and runtime-driven migrations.

---

## 2. Non-negotiable rules

Do not run automatic migrations at API startup.

Do not apply migrations with the runtime application user.

Do not apply P2 FK migrations when orphan counts are greater than zero.

Do not apply migrations without backup evidence.

Do not apply migrations without a rollback plan.

Do not apply migrations without post-migration validation.

---

## 3. Required artifacts before dry-run

Before running a dry-run, collect:

- target environment;
- database server;
- database name;
- current deployed application version;
- target commit SHA;
- migration script path;
- generated SQL script SHA or checksum;
- backup evidence;
- rollback plan;
- orphan detection output;
- deployment operator;
- reviewer or approver.

---

## 4. Pre-dry-run checklist

### 4.1 Repository validation

Run:

~~~powershell
powershell -ExecutionPolicy Bypass -File "scripts/verify-no-mojibake.ps1"
powershell -ExecutionPolicy Bypass -File "scripts/verify-p2-orphan-detection-sql.ps1"
powershell -ExecutionPolicy Bypass -File "scripts/verify-p2-data-cleanup-remediation-playbook.ps1"
powershell -ExecutionPolicy Bypass -File "scripts/verify-database-foreign-key-baseline.ps1"
powershell -ExecutionPolicy Bypass -File "scripts/validate-database-deployment-baseline.ps1"

Set-Location "services/api-dotnet"
dotnet build "Caritas.Brigadas.sln" -warnaserror
dotnet test "Caritas.Brigadas.sln" -warnaserror --no-build
~~~

Expected result: all commands pass.

---

### 4.2 Database backup

Confirm:

- backup exists;
- backup is restorable;
- restore process has an owner;
- RPO and RTO are acceptable;
- backup location is documented.

Do not continue without backup evidence.

---

### 4.3 Orphan detection

Run:

~~~powershell
sqlcmd -S "<server>" -d "<database>" -E -i "database/diagnostics/sqlserver/p2_detect_fk_orphans.sql" -o "p2_orphan_detection_output.txt"
~~~

Expected result:

- total_orphans = 0;
- required_fk_orphans = 0;
- optional_fk_orphans = 0.

If any value is greater than zero, stop and follow `docs/backend/P2_DATA_CLEANUP_REMEDIATION_PLAYBOOK.md`.

---

## 5. Generate migration script

Generate the reviewed SQL script using the approved repository script.

~~~powershell
powershell -ExecutionPolicy Bypass -File "scripts/db-generate-migration-script.ps1"
~~~

Confirm the generated SQL:

- is idempotent;
- targets the expected migration range;
- does not include destructive data cleanup;
- does not include `ON DELETE CASCADE`;
- does not include `ON DELETE SET NULL`;
- does not create DeviceId strong FKs;
- matches the expected commit SHA.

---

## 6. Dry-run execution

Run the generated migration script against staging or a restored production copy.

Capture:

- start time;
- end time;
- executing user;
- SQL output;
- errors or warnings;
- affected object count;
- migration history result.

Do not use the runtime application user.

---

## 7. Post-dry-run validation

After dry-run:

1. run orphan detection again;
2. confirm total orphan count remains zero;
3. validate EF migration history table;
4. validate expected FK constraints exist;
5. validate API startup without applying migrations;
6. validate critical application flows;
7. archive output logs.

Critical flows:

- health endpoint;
- organization creation restrictions;
- roles and permissions;
- patient creation;
- patient visit creation;
- service encounter creation;
- form response creation;
- document signature creation;
- sync batch and sync event creation.

---

## 8. Production migration readiness

Production is ready only when:

- staging dry-run passed;
- backup evidence exists;
- orphan detection passed;
- generated SQL was reviewed;
- rollback plan exists;
- migration user is available;
- runtime user does not own migration execution;
- monitoring is available;
- post-migration validation owner is assigned;
- approval is documented.

---

## 9. Rollback expectations

Rollback strategy must define:

- whether rollback is restore-based or script-based;
- who approves rollback;
- who executes rollback;
- maximum acceptable downtime;
- data written after migration start;
- communication path;
- validation after rollback.

For FK migrations, restore-based rollback is usually safer than ad-hoc constraint removal.

---

## 10. Evidence template

~~~text
Environment:
Database:
Server:
Commit SHA:
Migration script path:
Migration script checksum:
Operator:
Reviewer:
Approval:
Backup evidence:
Orphan detection output before:
Dry-run start:
Dry-run end:
Dry-run result:
Post-dry-run orphan detection:
FK validation result:
API validation result:
Rollback plan:
Decision:
Notes:
~~~

---

## 11. Final rule

If any validation step is skipped, failed, or undocumented, the migration is not ready.