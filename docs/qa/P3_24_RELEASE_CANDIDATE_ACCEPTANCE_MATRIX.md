# P3.24 Release Candidate Acceptance Matrix

## Purpose

This document defines release candidate acceptance criteria for Web iOS Android.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Release candidate acceptance matrix status: BLOCKED_PENDING_REAL_EVIDENCE

## Acceptance matrix

| Acceptance area | Web | iOS | Android |
|---|---|---|---|
| artifact reference | required | required | required |
| deployed commit SHA | required | required | required |
| environment name | required | required | required |
| build profile | required | required | required |
| release channel | required | required | required |
| API contract version | required | required | required |
| OpenAPI artifact reference | required | required | required |
| dependency review evidence | required | required | required |
| secret scan evidence | required | required | required |
| static analysis evidence | required | required | required |
| build reproducibility evidence | required | required | required |
| unit test evidence | required | required | required |
| contract test evidence | required | required | required |
| runtime configuration test evidence | required | required | required |
| observability test evidence | required | required | required |
| privacy-safe telemetry test evidence | required | required | required |
| schema drift evidence | required | required | required |
| breaking change evidence | required | required | required |
| signing boundary evidence | review only | required | required |
| release notes evidence | required | required | required |
| rollback plan | required | required | required |
| support diagnostic evidence | required | required | required |

## Rejection criteria

Reject release candidate approval when evidence is missing, stale, synthetic, unsanitized, untraceable, inconsistent with API contract version, missing contract test evidence, missing runtime configuration test evidence, missing observability test evidence, missing privacy-safe telemetry test evidence, missing rollback plan, or claiming production readiness without real evidence.

## P3.24 conclusion

Release candidate acceptance must remain evidence-driven across Web iOS Android.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
