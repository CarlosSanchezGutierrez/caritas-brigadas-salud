# Verification Commands

Run these from repository root:

`C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud`

## Core backend validation

```powershell
dotnet restore "services/api-dotnet/Caritas.Brigadas.sln"

dotnet build "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Caritas.Brigadas.Infrastructure.csproj" --configuration Release /p:TreatWarningsAsErrors=true --no-restore

dotnet build "services/api-dotnet/src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj" --configuration Release /p:TreatWarningsAsErrors=true --no-restore

dotnet test "services/api-dotnet/Caritas.Brigadas.sln" --configuration Release /p:TreatWarningsAsErrors=true --no-restore
P5 closure verifier
powershell -ExecutionPolicy Bypass -File "scripts/verify-p5-10-patient-module-closure.ps1"
Git conflict marker scan
git grep -n -E "^(<<<<<<<|=======|>>>>>>>)" -- .

Expected:

No results.

Diff whitespace check
git diff --check

Expected:

Exit code 0.

Required branch hygiene
git status --short
git branch --show-current
git branch -r

Expected:

Work should happen in a feature/fix/docs branch.
main and develop should remain protected.
No random stale release branches should remain after promotion.
Open PR check
gh pr list --state open

Expected before new module work:

No old blocking PRs related to P5 promotion.