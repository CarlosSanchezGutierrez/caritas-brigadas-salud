# P5.1 Backend Surface Inventory Runbook

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

Run a backend surface inventory before changing functional backend code.

## Collector command

Run from repository root:

```powershell
& "scripts/p5/collect-p5-01-backend-surface-inventory.ps1"
Output

The collector writes to:

artifacts/p5/p5-01-backend-surface-inventory/<timestamp>/

Expected files:

manifest.json
backend-surface-summary.md
project-inventory.json
source-surface-inventory.json
domain-coverage.json
gap-backlog.md
How to use the output

Use the gap backlog to plan the next backend PRs:

P5.2 patient core.
P5.3 brigade and service availability.
P5.4 clinical encounters.
P5.5 consent and privacy.
P5.6 longitudinal history.
P5.7 clinical audit proof.
P5.8 reports and exports.
P6 offline-first synchronization.
P7 dashboards and analytics.
P8 production institutional readiness.
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