# Global Rules

These rules apply to all future work in this repository.

## Repository and workflow rules

- Use Pull Requests.
- Do not push directly to `main`.
- Do not push directly to `develop` unless explicitly authorized by the owner.
- Keep `main` stable.
- Use `develop` as integration branch.
- Use feature/fix/docs branches for work.
- Keep commits focused and explainable.
- Add or update docs when closing a milestone.

## Production readiness rules

Never claim production readiness without real evidence.

Current required wording:

`Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE`

Allowed milestone wording:

`controlled backend milestone`

Forbidden claims:

- production ready
- approved for production
- real deployment complete
- ready for real patient data
- live clinical use approved

## Data and privacy rules

- Do not commit real patient data.
- Do not commit secrets.
- Do not commit real connection strings.
- Do not commit real passwords.
- Do not commit real access tokens.
- Do not commit private keys.
- Use placeholders only.

## Architecture rules

- SQL Server is the operational source of truth.
- Mobile clients must not write directly to SQL Server.
- Frontend/mobile clients must not bypass the API.
- No mandatory cloud dependency.
- Offline-first is allowed as a design goal, but server-side authority remains SQL Server.
- Do not introduce AI/blockchain/productive automation unless the scope explicitly asks for it and guardrails are preserved.

## Evidence rules

- Do not fabricate logs.
- Do not fabricate test results.
- Do not fabricate deployment evidence.
- Do not fabricate security evidence.
- If evidence is missing, say it is missing.
- If a check was not run, say it was not run.

## Code quality rules

- Prefer small controlled modules.
- Add tests where the current test structure supports them.
- Add verifiers for milestone evidence.
- Treat warnings as errors where existing workflows do.
- Keep DTOs, domain, persistence, API, tests, docs, and scripts aligned.

## Communication rules for AI assistants

- Be explicit about what changed.
- Be explicit about what remains blocked.
- Do not overclaim.
- If a Codex/CI review flags a valid issue, fix it rather than arguing around it.
- If a branch/PR is stale, close or clean it safely.