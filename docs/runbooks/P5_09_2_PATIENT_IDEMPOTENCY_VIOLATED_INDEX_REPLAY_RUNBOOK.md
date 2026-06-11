# P5.9.2 Patient Idempotency Violated Index Replay Runbook

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Validation command

Run from repository root:

    powershell -ExecutionPolicy Bypass -File "scripts/verify-p5-09-2-patient-idempotency-violated-index-replay.ps1"

## Expected result

The verifier confirms that concurrent replay re-reads by the violated SQL Server unique index identity.

## Guardrails

No backend production readiness approval.

No fabricated evidence.

No secrets in repository.

No committed real patient data.

No direct mobile write to SQL Server.

No client may bypass the API.

No cloud dependency.

SQL Server remains the operational source of truth.

Backend production readiness remains BLOCKED_PENDING_REAL_EVIDENCE.