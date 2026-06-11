# P3.8 SQL Server On-Prem Operational Evidence

## Purpose

P3.8 defines the operational evidence baseline for SQL Server on-premise or institutional data center execution.

This phase does not claim real environment validation. It defines the evidence required before backend promotion.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Backend freeze status: NOT_FROZEN_PENDING_ON_PREM_EVIDENCE_AND_CONTRACTS

## Core decision

SQL Server is the operational source of truth.

Runtime configuration must use:

- ConnectionStrings__SqlServer

## Required evidence

The future real evidence package must include:

- SQL Server on-premise target reference.
- migration execution evidence.
- backup and restore evidence.
- restore validation evidence.
- least privilege evidence.
- app runtime user evidence.
- migration user evidence.
- read-only reporting user evidence.
- health endpoint evidence.
- smoke test evidence.
- controlled data injection evidence.
- accepted records count.
- rejected records count.
- quarantine evidence.
- idempotency key behavior.
- audit trail reference.
- RPO.
- RTO.

## Evidence integrity rules

- No secrets in repository.
- No patient data in evidence.
- No fabricated logs.
- No backend closure claim.
- No cloud dependency.
- No external AI dependency.
- No blockchain dependency.

## Required blockers

Backend promotion remains blocked until real SQL Server on-premise evidence exists.