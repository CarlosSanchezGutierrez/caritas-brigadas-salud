# Repository Security Dependency Review Retry Baseline

Status: active  
Scope: GitHub Dependency Review REST API gate hardening  
Target phase: repository security hardening  
Depends on: scripts/dependency-review-rest.ps1

---

## 1. Purpose

This baseline hardens the repository security dependency review gate against transient GitHub REST API failures.

The gate must still fail closed when dependency review cannot be completed after retries.

---

## 2. Required behavior

The dependency review script must:

- call the GitHub Dependency Review REST API through gh api;
- retry transient API failures;
- use exponential backoff between attempts;
- expose DEPENDENCY_REVIEW_MAX_ATTEMPTS;
- expose DEPENDENCY_REVIEW_INITIAL_DELAY_SECONDS;
- fail closed after all retry attempts fail;
- continue blocking high or critical vulnerabilities according to FAIL_ON_SEVERITY;
- write attempt metadata to the GitHub step summary.

---

## 3. Non-negotiable security rule

Retry hardening must never downgrade real vulnerability findings.

If the API eventually returns valid dependency data and blocking vulnerabilities are found, the job must fail.

If the API never returns valid dependency data after all attempts, the job must fail.

---

## 4. Acceptance criteria

This hardening is complete when:

- dependency-review-rest.ps1 includes Invoke-DependencyReviewApiWithRetry;
- dependency-review-rest.ps1 includes DEPENDENCY_REVIEW_MAX_ATTEMPTS;
- dependency-review-rest.ps1 includes DEPENDENCY_REVIEW_INITIAL_DELAY_SECONDS;
- dependency-review-rest.ps1 uses Start-Sleep for retry backoff;
- dependency-review-rest.ps1 fails after exhausted retries;
- dependency-review-rest.ps1 still throws on blocking vulnerabilities.