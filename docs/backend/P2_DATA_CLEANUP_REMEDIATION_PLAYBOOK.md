# P2 Data Cleanup Remediation Playbook

Status: active  
Scope: backend database integrity  
Target database: SQL Server  
Related script: `database/diagnostics/sqlserver/p2_detect_fk_orphans.sql`

---

## 1. Purpose

This playbook defines what to do when the P2 orphan detection script reports rows that would violate reviewed foreign key constraints.

The goal is to avoid improvisation, unsafe deletes, undocumented repairs, and production data loss.

This playbook is intentionally conservative because the system handles clinical, consent, document, operational, and sync records.

---

## 2. Non-negotiable rules

Do not apply P2 FK migrations when orphan counts are greater than zero.

Do not delete clinical, consent, document, audit, or sync data without explicit authorization.

Do not repair production data without:

- backup evidence;
- exported affected rows;
- remediation plan;
- approval;
- rollback plan;
- post-remediation validation.

Do not run automatic migrations at API startup.

---

## 3. Required input evidence

Before remediation, collect:

- database name;
- environment;
- script version or commit SHA;
- full orphan detection output;
- affected relationship name;
- orphan count;
- sample affected rows;
- principal table expected;
- dependent table affected;
- business owner or module owner;
- proposed remediation action.

---

## 4. Remediation decision tree

### 4.1 Principal row exists elsewhere

Use this when the missing parent record exists under a different ID, duplicate record, or corrected tenant.

Recommended action:

1. confirm the correct principal row;
2. validate tenant boundary;
3. update dependent FK to the correct principal;
4. run orphan detection again.

This should be done only through a reviewed repair script.

---

### 4.2 Principal row was accidentally deleted

Use this when a parent row is missing but should still exist.

Recommended action:

1. restore the principal row from backup, audit history, or authoritative source;
2. preserve original IDs when possible;
3. avoid generating new IDs unless explicitly approved;
4. run orphan detection again.

---

### 4.3 Dependent row is invalid test or seed data

Use this when data belongs to a non-production seed/test flow and is not clinically or operationally meaningful.

Recommended action:

1. confirm environment and source;
2. archive before deletion if needed;
3. document why it is safe to remove;
4. delete only with approval;
5. run orphan detection again.

Production deletes require stricter authorization.

---

### 4.4 Dependent row is historical and must remain

Use this when the dependent record must remain for clinical, legal, audit, or sync history.

Recommended action:

1. do not delete;
2. restore or reconstruct the principal row;
3. create an archival parent record if allowed by policy;
4. document the reason;
5. run orphan detection again.

---

### 4.5 DeviceId relationship appears as orphan-like data

DeviceId references are deferred in P2.

Do not create strong DeviceId FK repairs yet.

Recommended action:

1. report counts only;
2. confirm whether devices are offline, revoked, or not-yet-synced;
3. defer enforcement until device lifecycle policy is approved.

---

## 5. Approved remediation actions

Allowed actions, when approved:

- restore missing principal rows;
- relink dependent rows to the correct principal;
- archive invalid records;
- delete non-production test data;
- create documented placeholder parent records only when policy explicitly allows it.

Disallowed actions:

- blind deletes;
- mass update without transaction;
- changing tenant ownership without evidence;
- disabling FK checks permanently;
- applying migrations despite orphan counts;
- modifying clinical history without approval.

---

## 6. Repair script requirements

Every repair script must:

- run inside an explicit transaction;
- include pre-check queries;
- include post-check queries;
- include row counts;
- include rollback instructions;
- be reviewed before execution;
- be stored or attached as deployment evidence.

Minimum shape:

```sql
BEGIN TRANSACTION;

-- Pre-check
SELECT COUNT_BIG(*) AS affected_rows
FROM ...

-- Repair
UPDATE ...
WHERE ...

-- Post-check
SELECT COUNT_BIG(*) AS remaining_orphans
FROM ...

-- COMMIT only after review
-- COMMIT TRANSACTION;
-- ROLLBACK TRANSACTION;
7. Required post-remediation validation

After remediation:

run p2_detect_fk_orphans.sql;
confirm total_orphans = 0;
confirm required_fk_orphans = 0;
confirm optional_fk_orphans = 0;
archive the output;
obtain approval to continue migration.
8. Migration readiness rule

A database is ready for P2 FK migration only when:

orphan detection returns zero total orphans;
repair evidence is documented;
backup exists;
rollback plan exists;
migration user is ready;
runtime API user is not responsible for applying migrations;
post-migration validation is planned.
9. Evidence template

Use this template for every cleanup event.

Environment:
Database:
Date:
Commit SHA:
Operator:
Reviewer:
Backup location:
Orphan detection output:
Affected relationship:
Dependent table:
Principal table:
Orphan count:
Root cause:
Remediation decision:
Repair script location:
Pre-check result:
Post-check result:
Final orphan count:
Approval:
Rollback notes:
10. Final rule

If the data owner cannot explain why a row is safe to modify or delete, do not modify or delete it.

Escalate instead.