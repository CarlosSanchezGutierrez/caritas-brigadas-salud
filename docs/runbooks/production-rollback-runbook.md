# Production Rollback Runbook

## Purpose

Define the minimum rollback path for a failed production/staging deployment.

## Rollback decision criteria

Rollback may be triggered by:

- failed startup;
- failed health/readiness;
- failed authentication;
- failed authorization;
- repeated 5xx responses;
- failed database migration;
- unacceptable latency;
- data integrity risk.

## Required information

- failed deployment commit SHA;
- last known good commit SHA;
- migration status;
- backup reference;
- restore reference;
- decision owner;
- incident timestamp.

## Application rollback

1. Stop rollout.
2. Identify last known good version.
3. Redeploy last known good version.
4. Verify `/health/live`.
5. Verify `/health/ready`.
6. Verify authentication.
7. Verify representative endpoint.
8. Register evidence.

## Database rollback

Database rollback must not be improvised.

Use one of:

- forward fix migration;
- restore to validated backup;
- manual remediation approved by owner.

## Evidence

Every rollback must record:

- reason;
- responsible person;
- commands or deployment actions;
- timestamp;
- verification result;
- database action taken;
- final status.