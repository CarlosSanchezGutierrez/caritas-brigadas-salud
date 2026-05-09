# Quality Gates

## Purpose

This document defines the minimum quality gates required before accepting changes into `develop` or `main`.

The goal is to keep the repository maintainable for Tec students, Cáritas technical reviewers, future service-social contributors, and production operators.

## Non-negotiable rules

1. No direct commits to `main`.
2. No direct commits to `develop` once branch protection is enabled.
3. All changes must go through pull requests.
4. Pull requests must be small, reviewable, and scoped.
5. Security, database, authentication, authorization, tenant isolation, and migration changes require extra review.
6. Failed checks must be fixed before merge.
7. Conversations from code review must be resolved before merge.
8. No force push to protected branches.
9. No secrets, real patient data, PHI, PII, private keys, tokens, credentials, or production connection strings may be committed.
10. Any intentional technical debt must be documented in the technical debt register.

## Required checks before merge

### Backend API

Required for backend changes:

- `dotnet build` with warnings as errors.
- `dotnet test` with warnings as errors.
- Authorization and tenant isolation review for controller/repository changes.
- Migration review for database changes.
- Secret scan review for configuration changes.
- CodeQL review when code scanning is available.
- Dependency review when package files change.

### Database

Required for database changes:

- Migration file reviewed.
- Model snapshot reviewed.
- Nullability reviewed.
- Index impact reviewed.
- Rollback impact reviewed.
- No duplicate-column migration sequence.
- No migration that silently weakens tenant isolation.

### Security

Required for security-sensitive changes:

- Permission code alignment reviewed.
- Role seed map reviewed.
- No privilege escalation path.
- SUPER_ADMIN-only operations explicitly protected.
- Organization-scoped endpoints enforce `organizationId`.
- ID-only routes must be justified or removed.
- Logs must not include sensitive identifiers or clinical data.

### Frontend clients

Required for future web, Android, and iOS changes:

- API contract compatibility reviewed.
- Error handling reviewed.
- Authentication flow reviewed.
- No secrets in client bundles.
- Environment variables documented.
- Build/typecheck must pass.

## Merge policy

### Into develop

Allowed when:

- Scope is clear.
- Build and tests pass.
- No unresolved review comments.
- No high-risk security or database concerns remain.
- Technical debt is documented if accepted.

### Into main

Allowed only through release PRs or release snapshots.

`main` should represent the latest stable, reviewed baseline.

Direct `develop -> main` PRs may be avoided when squash-merge history causes artificial conflicts. In that case, use a release snapshot branch based on `main` that applies the current `develop` tree.

## Risk levels

### Low

Examples:

- Documentation corrections.
- Comments.
- Formatting.
- Non-functional scripts.

Required:

- Basic review.
- Build/test if code-adjacent.

### Medium

Examples:

- New endpoints.
- DTO changes.
- Repository changes.
- Local tooling changes.
- Non-critical configuration.

Required:

- Build/test.
- API compatibility review.
- Security checklist.

### High

Examples:

- Authentication.
- Authorization.
- Role assignment.
- Permissions.
- Tenant isolation.
- Database migrations.
- Logging of sensitive data.
- Production configuration.
- Deployment workflows.

Required:

- Build/test.
- Security review.
- Migration review when applicable.
- Explicit technical debt review.
- Maintainer approval.

## Definition of done

A change is done only when:

- It compiles.
- Tests pass.
- API behavior is documented or intentionally unchanged.
- Security implications are reviewed.
- Database impact is reviewed.
- No secrets or real data are committed.
- Accepted debt is recorded.
- The PR can be understood by a future student contributor.