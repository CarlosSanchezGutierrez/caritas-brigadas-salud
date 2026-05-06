# Required Checks Baseline

## Checks obligatorios

- Backend security and quality gate.
- Frontend security and quality gate.
- Deployment baseline metadata gate.
- Database deployment baseline metadata gate.
- Supply chain baseline metadata gate.
- Repository governance metadata gate.
- Docker image build gate.

## Backend security and quality gate

Valida restore, build, tests y NuGet vulnerable scan.

## Frontend security and quality gate

Valida npm ci, npm audit, typecheck y build.

## Deployment baseline metadata gate

Valida documentación y metadata de deployment.

## Database deployment baseline metadata gate

Valida documentación y scripts de migraciones SQL Server.

## Supply chain baseline metadata gate

Valida scanning, SBOM y supply chain baseline.

## Repository governance metadata gate

Valida PR template, CODEOWNERS y documentos de gobernanza.

## Docker image build gate

Valida Docker build, Trivy scan y generación de SBOM.
