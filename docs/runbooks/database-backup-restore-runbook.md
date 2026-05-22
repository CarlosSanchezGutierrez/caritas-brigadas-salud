# Database Backup and Restore Runbook

## Purpose

Define minimum backup and restore evidence for SQL Server.

## Backup evidence

Required:

- database name;
- timestamp;
- backup method;
- backup destination;
- backup size;
- responsible person;
- verification result.

## Restore evidence

Required:

- backup reference;
- restore target environment;
- restore timestamp;
- restore responsible;
- validation query or smoke test;
- result.

## Procedure: backup

1. Confirm database target.
2. Execute backup.
3. Verify backup exists.
4. Register metadata.
5. Store backup securely.
6. Register evidence.

## Procedure: restore

1. Select backup.
2. Restore in non-production environment.
3. Validate schema.
4. Validate representative data.
5. Run smoke tests.
6. Register evidence.

## Rule

A backup without a restore test is not sufficient production evidence.