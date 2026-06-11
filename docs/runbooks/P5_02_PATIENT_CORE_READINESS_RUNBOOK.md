# P5.2 Patient Core Readiness Runbook

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

Run the patient core readiness collector before implementing patient core changes.

## Collector command

Run from repository root:

```powershell
& "scripts/p5/collect-p5-02-patient-core-readiness.ps1"
Output

The collector writes to:

artifacts/p5/p5-02-patient-core-readiness/<timestamp>/

Expected files:

manifest.json
patient-core-readiness-summary.md
patient-core-surface-inventory.json
patient-core-gap-backlog.md
How to use the output

Use the gap backlog to decide whether the next PR should implement patient entity, patient request and response contracts, patient controller or endpoints, patient persistence configuration, patient migration, patient validation, patient authorization, patient audit trail, patient tests, idempotency-safe patient creation, offline-first patient synchronization fields, and longitudinal patient history linkage.

Guardrails

No backend production readiness approval.

No fabricated evidence.

No secrets in repository.

No committed real patient data.

No direct mobile write to SQL Server.

No client may bypass the API.

No cloud dependency.

SQL Server remains the operational source of truth.

Backend production readiness remains BLOCKED_PENDING_REAL_EVIDENCE.