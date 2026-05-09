# Branch Protection Baseline

## Objetivo

Definir reglas mínimas para proteger develop y main.

## main

Reglas recomendadas:

- Require pull request before merging.
- Require approvals.
- Require review from CODEOWNERS.
- Require status checks to pass.
- Require branches to be up to date before merging.
- Block force pushes.
- Block deletions.
- Restrict who can push.

## develop

Reglas recomendadas:

- Require pull request before merging cuando haya más de un contribuidor activo.
- Require status checks to pass.
- Require branches to be up to date before merging.
- Block force pushes.
- Block deletions.

## required checks

Checks mínimos:

- Backend security and quality gate.
- Frontend security and quality gate.
- Deployment baseline metadata gate.
- Database deployment baseline metadata gate.
- Supply chain baseline metadata gate.
- Repository governance metadata gate.
- Testing baseline metadata gate.
- Docker image build gate.

## Nota

La protección real debe configurarse en GitHub Settings o mediante GitHub Rulesets con permisos administrativos.

## Repository security required checks

Agregar después de validar el workflow:

- Repository security metadata gate.
- Dependency Review.

Dependency Review solo corre en pull requests.
