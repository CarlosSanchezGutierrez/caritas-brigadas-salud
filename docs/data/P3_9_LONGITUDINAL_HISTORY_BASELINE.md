# P3.9 Longitudinal History Baseline

## Purpose

This document defines the longitudinal history baseline for patients, consent, encounters, clinical records, social support, and corrections.

The system must support a patient timeline that can represent multiple encounters over time while preserving correction history and audit trail evidence.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

## Longitudinal history principle

A patient record is not a single form.

A patient record is a timeline of events.

The timeline must support:

- patient identity timeline
- consent timeline
- encounter timeline
- clinical timeline
- social support timeline
- document timeline
- referral timeline
- correction timeline
- merge and deduplication timeline
- audit trail

## Required longitudinal event fields

Every longitudinal event must include:

- event id
- patient id
- event type
- effective date
- recorded at
- recorded by
- organization id
- brigade id when applicable
- encounter id when applicable
- source
- source system
- device id when applicable
- correlation id
- audit trail reference

## Patient identity timeline

The patient identity timeline must support:

- partial identity.
- later enrichment of missing identity fields.
- identity correction.
- merge and deduplication.
- identity confidence level.
- source of identity data.
- reason for correction.

Partial identity is valid when field conditions require it.

Examples:

- migrant without full documents.
- patient without phone.
- patient without CURP.
- patient with incomplete name.
- emergency or field context.

## Consent timeline

The consent timeline must preserve:

- consent version.
- privacy notice version.
- capture date.
- capture method.
- revocation date if applicable.
- consent exception reason if applicable.
- audit trail reference.

Consent updates must not silently overwrite previous consent records.

## Encounter timeline

The encounter timeline must preserve:

- encounter creation.
- brigade context.
- available services.
- selected service.
- clinical result.
- social support result.
- close status.
- reopen event if applicable.
- correction event if applicable.

## Clinical timeline

The clinical timeline must preserve:

- vital signs.
- clinical notes.
- medication records.
- diagnoses or clinical impressions where applicable.
- referrals.
- follow-up recommendations.
- correction event.
- audit trail.

Clinical corrections must preserve before snapshot reference, after snapshot reference, actor, reason, and timestamp.

## Merge and deduplication

Merge and deduplication must be deliberate and auditable.

Required merge fields:

- source patient id.
- target patient id.
- reviewer.
- reason.
- matching evidence.
- merge date.
- audit trail reference.

No automatic patient merge can happen without governance.

## Correction model

Corrections must never silently overwrite longitudinal history.

Every correction must create:

- correction event.
- corrected entity.
- before snapshot reference.
- after snapshot reference.
- reason.
- actor.
- timestamp.
- audit trail.

## Evidence required later

Implementation evidence must prove:

- patient timeline can show multiple encounters.
- consent timeline preserves version history.
- clinical corrections generate correction events.
- patient merge is auditable.
- partial identity is supported without bypassing audit.
- device id and correlation id exist for offline-originated data when applicable.
- audit trail links longitudinal events to API/domain actions.

## P3.9 conclusion

Longitudinal history is required before API contract freeze.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE