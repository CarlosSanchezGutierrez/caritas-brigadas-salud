# P3.25 Pilot Acceptance UAT Matrix

## Purpose

This document defines controlled pilot acceptance and UAT criteria for Web iOS Android.

## Status

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE

Pilot acceptance UAT matrix status: BLOCKED_PENDING_REAL_EVIDENCE

## Acceptance matrix

| Acceptance area | Web | iOS | Android |
|---|---|---|---|
| approved release candidate reference | required | required | required |
| artifact reference | required | required | required |
| deployed commit SHA | required | required | required |
| environment name | required | required | required |
| build profile | required | required | required |
| release channel | required | required | required |
| API contract version | required | required | required |
| OpenAPI artifact reference | required | required | required |
| pilot site or brigade scope | required | required | required |
| pilot participant scope | required | required | required |
| pilot device inventory | review only | required | required |
| UAT acceptance criteria | required | required | required |
| training evidence | required | required | required |
| privacy consent evidence | required | required | required |
| data protection evidence | required | required | required |
| contract test evidence | required | required | required |
| runtime configuration test evidence | required | required | required |
| observability evidence | required | required | required |
| privacy-safe telemetry evidence | required | required | required |
| offline field workflow evidence | review only | required | required |
| sync dry run evidence | review only | required | required |
| rollback plan | required | required | required |
| incident response plan | required | required | required |
| support escalation plan | required | required | required |

## Rejection criteria

Reject controlled pilot readiness when evidence is missing, stale, synthetic, unsanitized, untraceable, inconsistent with API contract version, missing release candidate approval evidence, missing consent evidence, missing training evidence, missing support escalation plan, missing rollback plan, missing incident response plan, or claiming production readiness without real evidence.

## P3.25 conclusion

Controlled pilot acceptance must remain evidence-driven across Web iOS Android.

Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
