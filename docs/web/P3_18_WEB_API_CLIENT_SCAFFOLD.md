# P3.18 Web API Client Scaffold

## Purpose

This document defines the Web API client scaffold governance.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Web API client scaffold status: BLOCKED_PENDING_REAL_EVIDENCE

## Web scaffold responsibilities

The Web API client scaffold must provide:

- configuration boundary.
- API contract version propagation.
- endpoint id mapping.
- typed request model.
- typed response model.
- standard error envelope handler.
- authentication metadata boundary.
- authorization role metadata boundary.
- organization id propagation.
- request id propagation.
- correlation id propagation.
- audit trail reference handling.
- pagination convention support.
- filtering convention support.
- sorting convention support.
- contract test boundary.

## Web scaffold blocked behavior

The Web API client scaffold must not bypass the API, write directly to SQL Server, call undocumented endpoints, ignore authorization role, ignore organization id, drop request id, drop correlation id, hide standard error envelope, treat exports as unrestricted, or treat UI completion as evidence.

## Web scaffold evidence

Required evidence includes typed model evidence, standard error envelope evidence, request id evidence, correlation id evidence, organization id evidence, authorization role evidence, audit trail reference evidence, contract test evidence, and schema drift evidence.

## P3.18 conclusion

The Web API client scaffold must centralize API access before Web feature implementation expands.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
