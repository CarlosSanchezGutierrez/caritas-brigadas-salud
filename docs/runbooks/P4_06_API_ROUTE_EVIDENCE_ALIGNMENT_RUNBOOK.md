# P4.6 API Route Evidence Alignment Runbook

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

This runbook explains how to use the P4.5 collector after P4.6 route alignment.

## Correct API routes

Use these implemented routes for evidence collection:

/health/live
/health/ready
/openapi/v1/openapi.json
/swagger

## Collector without API

& "scripts/p4/collect-p4-05-api-runtime-openapi-evidence.ps1"

## Collector with running API

& "scripts/p4/collect-p4-05-api-runtime-openapi-evidence.ps1" -ApiBaseUrl "https://localhost:7044"

## Collector with startup attempt

& "scripts/p4/collect-p4-05-api-runtime-openapi-evidence.ps1" -StartLocalApi -ApiBaseUrl "https://localhost:7044"

## SQL Server boundary

If /health/live succeeds but /health/ready fails because database connectivity is unavailable, this is valid evidence.

Do not fake readiness.

Reference P4.4 Real Environment SQL Server Access Blocker until institutional SQL Server access exists.

## Guardrails

- No secrets in repository.
- No fabricated evidence.
- No backend production readiness approval.
- No direct mobile write to SQL Server.
- No client may bypass the API.
- SQL Server remains the operational source of truth.
- Backend production readiness remains BLOCKED_PENDING_REAL_EVIDENCE.