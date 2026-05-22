# P3.15 Web Client Readiness Baseline

## Purpose

This document defines Web client readiness expectations before frontend implementation relies on API behavior.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Web client readiness status: BLOCKED_PENDING_REAL_EVIDENCE

## Web client allowed scope

The Web client may plan against health check, identity context, organization context, brigade setup, patient registration, privacy consent capture, encounter capture, clinical timeline review, dashboard datasets, governed reports export, audit review, and conflict review.

## Web client blocked scope

The Web client must not bypass the API, write directly to SQL Server, ignore authorization role, ignore organization id, hide standard error envelope fields, treat exports as unrestricted, treat audit review as public, or treat contract ready as production evidence.

## Web client required metadata

The Web client must preserve API contract version, request id, correlation id, organization id, user role, standard error envelope, pagination convention, filtering convention, sorting convention, and audit trail reference.

## Web client evidence needed

Required evidence includes authenticated identity evidence, role-based navigation evidence, organization-scoped request evidence, validation error handling evidence, authorization error handling evidence, governed report export evidence, dashboard metric lineage evidence, and audit trail reference display evidence.

## P3.15 conclusion

Web client integration may proceed only against contract-backed endpoints and must remain blocked where real evidence is missing.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
