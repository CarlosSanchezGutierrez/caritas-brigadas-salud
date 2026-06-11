
Prompt for Claude

You are continuing development on the repository:

CarlosSanchezGutierrez/caritas-brigadas-salud

Local path used by the owner:

C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud

You are working on Cáritas Brigadas de Salud, a modular institutional health brigade operations platform.

Read these files first:

README.md
docs/ai-handoff/CLAUDE_START_HERE.md
docs/ai-handoff/CURRENT_STATUS.md
docs/ai-handoff/NEXT_ACTIONS_P6.md
docs/ai-handoff/GLOBAL_RULES.md
docs/ai-handoff/TECHNICAL_DECISIONS.md
docs/ai-handoff/VERIFICATION_COMMANDS.md

Current milestone status:

P5 Patient Backend Module is closed as a controlled backend milestone and has been promoted to main.

Next planned work:

P6 Brigade Operations.

Do not start coding immediately.

First, inspect the repository and identify the actual current implementation for:

Brigade entities
Brigade DTOs/contracts
Brigade controllers
Brigade repositories
Brigade status/lifecycle
Patient visits
Service encounters
Audit action mappers
Existing docs and verifiers

Then propose P6.1 as a narrow, controlled PR.

Hard rules:

Do not claim production readiness.
Do not fabricate evidence.
Do not commit secrets.
Do not commit real patient data.
Do not create cloud dependency.
Do not allow mobile/frontend clients to bypass the API.
Do not allow direct mobile SQL Server writes.
SQL Server remains the operational source of truth.
Backend production readiness remains BLOCKED_PENDING_REAL_EVIDENCE.

Preferred output format:

Brief repo findings.
Exact P6.1 proposed scope.
Files to edit.
Tests/verifiers/docs to add.
Explicit non-goals.
Exact PowerShell block to implement only after scope is clear.

Tone:

Be practical, precise, and conservative. Do not overbuild. Do not rename architecture without a reason. Do not invent missing files.