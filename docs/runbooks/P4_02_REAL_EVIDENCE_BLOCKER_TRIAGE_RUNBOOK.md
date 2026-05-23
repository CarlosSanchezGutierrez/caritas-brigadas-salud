# P4.2 Real Evidence Blocker Triage Runbook

## Purpose

This runbook explains how to classify a P4.1 `manifest.json` into a P4.2 blocker backlog.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Step 1: Run the P4.1 collector

From the repository root:

```powershell
& "scripts/p4/collect-p4-01-real-evidence-baseline.ps1"
```

When the API is already running:

```powershell
& "scripts/p4/collect-p4-01-real-evidence-baseline.ps1" -ApiBaseUrl "https://localhost:7044"
```

The collector writes evidence under:

```text
artifacts/p4/p4-01-real-evidence-baseline
```

## Step 2: Locate the latest manifest

The required input is:

```text
artifacts/p4/p4-01-real-evidence-baseline/<timestamp>/manifest.json
```

## Step 3: Classify the evidence package

Run:

```powershell
& "scripts/p4/classify-p4-01-evidence-package.ps1" -ManifestPath "artifacts/p4/p4-01-real-evidence-baseline/<timestamp>/manifest.json"
```

## Step 4: Review the outputs

The classifier writes:

- `p4-02-classification.json`
- `p4-02-blocker-backlog.md`

## Step 5: Triage blockers

Triage order:

1. P0 required blocker.
2. P1 blocker candidate.
3. P2 optional evidence gap.
4. UNKNOWN classification.
5. PASS accepted evidence.

## Required triage evidence

- P4.2 real evidence classification report.
- P4.2 blocker backlog JSON.
- P4.2 blocker backlog Markdown.
- blocker severity.
- blocker category.
- blocker owner group.
- remediation type.
- evidence source.
- required blocker flag.
- optional evidence gap flag.
- pass classification.
- skipped classification.
- failed classification.
- unknown classification.
- P0 required blocker.
- P1 blocker candidate.
- P2 optional evidence gap.
- PASS accepted evidence.
- real evidence only.
- sanitized evidence only.
- Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE.

## Guardrails

- No secrets in repository.
- No cloud dependency.
- No fabricated evidence.
- No backend production readiness approval.
- No direct mobile write to SQL Server.
- No client may bypass the API.
- No undocumented endpoints.
- No silent overwrite.
- SQL Server remains the operational source of truth.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE