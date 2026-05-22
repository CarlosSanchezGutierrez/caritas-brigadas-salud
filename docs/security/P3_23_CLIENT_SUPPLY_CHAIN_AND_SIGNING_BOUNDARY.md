# P3.23 Client Supply Chain and Signing Boundary

## Purpose

This document defines the Client supply chain and signing boundary for Web iOS Android.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Client supply chain and signing status: BLOCKED_PENDING_REAL_EVIDENCE

## Supply chain scope

Supply chain controls must cover:

- dependency review.
- lockfile review.
- secret scan.
- static analysis.
- build reproducibility.
- artifact retention.
- release channel.
- build profile.
- environment name.
- API contract version.
- OpenAPI artifact reference.
- signing boundary.
- evidence package reference.

## Signing boundary

Signing material must be environment-controlled, reviewable, and excluded from source code.

Signing evidence must identify client target, build profile, release channel, artifact reference, responsible owner, and date.

## Required security controls

Required controls:

- No secrets in repository.
- secret scan must pass.
- dependency review must pass.
- static analysis must pass.
- signing boundary must be documented.
- artifact retention must be documented.
- release channel must be documented.
- production approval must require real evidence.

## Blocked supply chain behavior

Blocked behavior includes credential persistence in source code, unchecked dependency updates, missing lockfile review, missing secret scan, missing artifact retention, undocumented signing behavior, unsigned release candidate evidence, and local build output presented as production approval.

## P3.23 conclusion

Client supply chain and signing boundaries must be governed before Web iOS Android artifacts become release candidates.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
