# P3.16 Web Implementation Kickoff Boundary

## Purpose

This document defines the Web implementation kickoff boundary.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Web implementation kickoff status: BLOCKED_PENDING_REAL_EVIDENCE

## Web implementation allowed scope

The Web client may begin shell implementation for authenticated navigation, organization context, brigade administration, patient workflows, consent workflows, encounter workflows, dashboards, reports, audit review, and conflict review.

## Web implementation blocked scope

The Web client must not bypass the API, write directly to SQL Server, invent undocumented endpoints, ignore authorization role, ignore organization id, treat exports as unrestricted, or treat UI completion as production evidence.

## Web technical boundary

The Web client must isolate API access through a single API client boundary.

The Web client must preserve API contract version, request id, correlation id, organization id, standard error envelope, audit trail reference, pagination convention, filtering convention, and sorting convention.

## Web Definition of Ready

A Web feature is ready to implement only when endpoint integration status, API contract version, request schema, response schema, standard error envelope, authorization role, organization id requirement, acceptance criteria, and evidence requirement are documented.

## Web Definition of Done

A Web feature is done only when the UI shell, API boundary, typed models, error envelope handling, organization scope handling, role handling, and contract test evidence exist.

## P3.16 conclusion

Web implementation may begin only through contract-backed API boundaries.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
