# P5.4 Patient Persistence Offline Source Runbook

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

Validate patient offline/source fields are now persisted and projected by backend code.

## Validation command

Run from repository root:

```powershell
powershell -ExecutionPolicy Bypass -File "scripts/verify-p5-04-patient-persistence-offline-source.ps1"
Expected persisted fields
SourceBrigadeId
LocalPatientId
ClientOperationId
IdempotencyKey
SyncStatus
DataCaptureSource
Next implementation

After P5.4:

P5.5 patient API endpoint hardening
P5.6 patient validation and organization authorization
P5.7 patient write audit proof
P5.8 longitudinal patient history linkage
P6 offline-first synchronization behavior
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