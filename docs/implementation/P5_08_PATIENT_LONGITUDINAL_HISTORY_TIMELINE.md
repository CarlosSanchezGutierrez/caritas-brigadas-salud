# P5.8 Patient Longitudinal History Timeline

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Purpose

P5.8 adds an explicit longitudinal timeline to the existing patient clinical record read model.

This keeps the current clinical-record endpoint and source tables, but makes the patient history easier to consume by web, mobile, reporting, and future care-continuity workflows.

## Scope

P5.8 adds or validates:

- PatientClinicalRecordDto exposes Timeline.
- Timeline entries are derived from existing clinical record collections.
- Timeline entries include occurred time when known, event type, entity id, organization id, patient id, visit id, encounter id, label, status, offline flag, device id, and sync status where available.
- Timeline includes visits, including visits without arrival time.
- Timeline includes service encounters, including encounters without started time.
- Timeline includes vital signs.
- Timeline includes form responses.
- Timeline includes consent documents.
- Timeline includes medical referrals.
- Timeline includes medication deliveries.
- Summary includes TimelineEventCount, FirstTimelineEventAt, and LastTimelineEventAt.
- Timeline orders known-time events first, newest first, and preserves unknown-time events after known-time events.
- Existing clinical-record collections remain available.
- P5.8 implementation documentation.
- P5.8 acceptance matrix.
- P5.8 runbook.
- P5.8 verifier.

## Required behavior

The clinical record response must preserve the existing typed collections and add a derived longitudinal timeline.

The timeline must not replace visits, encounters, vital signs, form responses, consent documents, medical referrals, or medication deliveries.

The timeline must be scoped to the same organization and patient as the clinical record.

The timeline must preserve visits and encounters even when their event timestamp is not yet known.

Events with unknown occurred time must remain in the timeline and be ordered after events with known timestamps.

The timeline must be generated server-side from SQL Server-backed data already returned by the patient clinical record query.

## Boundary

P5.8 does not close:

- Offline sync processor behavior.
- Conflict resolution strategy.
- Patient merge or deduplication.
- Dashboards.
- Analytics.
- Production readiness.
- New persistence tables.

P5.8 does not add a new route. It extends the existing clinical record read model.

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