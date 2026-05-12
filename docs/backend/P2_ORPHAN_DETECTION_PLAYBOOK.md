# P2 Orphan Detection Playbook

Status: active  
Scope: backend database integrity  
Target database: SQL Server  
Script: `database/diagnostics/sqlserver/p2_detect_fk_orphans.sql`

---

## 1. Purpose

This playbook explains how to run orphan detection before applying the P2 foreign key migrations to any real database.

The script is read-only. It does not repair, delete, update, archive, or migrate data.

---

## 2. When to run it

Run the script before:

- applying P2 migrations to staging;
- applying P2 migrations to production;
- restoring old data into a database with P2 constraints;
- importing legacy brigade, patient, document, or sync data;
- validating a database that was modified manually.

---

## 3. Expected result

Before applying FK migrations, the expected result is:

- total_orphans = 0
- required_fk_orphans = 0
- optional_fk_orphans = 0

If any orphan count is greater than zero, do not apply the FK migration yet.

---

## 4. Deferred references

The script does not enforce deferred DeviceId references.

Deferred references:

- SyncBatch.DeviceId -> Device.Id
- FormResponse.DeviceId -> Device.Id
- DocumentSignature.DeviceId -> Device.Id

These remain deferred because offline, revoked, or not-yet-synced devices need a lifecycle policy before strong FK enforcement.

---

## 5. Blocking rule

If P2_FK_ORPHAN_SUMMARY.total_orphans is greater than zero, the database is not ready for P2 FK migration.

The migration must be blocked until data cleanup is completed and verified.