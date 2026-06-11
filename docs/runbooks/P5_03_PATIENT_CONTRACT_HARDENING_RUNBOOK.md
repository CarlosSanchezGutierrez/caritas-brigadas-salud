# P5.3 Patient Contract Hardening Runbook

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

Validate that patient contracts include offline-first and longitudinal-readiness fields.

## Validation command

Run from repository root:

```powershell
powershell -ExecutionPolicy Bypass -File "scripts/verify-p5-03-patient-contract-hardening.ps1"
Expected contract fields

CreatePatientRequest and PatientSummaryDto must include:

SourceBrigadeId
LocalPatientId
ClientOperationId
IdempotencyKey
SyncStatus
DataCaptureSource
Next implementation

After P5.3, continue with:

P5.4 patient persistence and migration.
P5.5 patient API endpoint hardening.
P5.6 patient validation and organization authorization.
P5.7 patient write audit.
P5.8 longitudinal patient history linkage.
P6 offline-first patient synchronization.
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