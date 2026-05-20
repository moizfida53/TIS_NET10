---
name: migrate-module
description: Migrate a full legacy module (controller + views + JS) from F:\DAD Projects\TIS_MVC into the new TIS.NET10 project. Trigger when the user types /migrate-module followed by a controller name, OR when the user asks to "port", "migrate", or "convert" a specific legacy controller and its views.
---

# /migrate-module — Migrate a legacy module

## When to invoke

User says any of: `/migrate-module BillController`, "migrate BillController", "port the Bill module", "convert the legacy Admin module".

## What to do

Delegate to the `module-migrator` agent. Pass the controller name and any extra constraints from the user (e.g. "skip the SAP-related actions for now").

## Template prompt for the agent

```
Migrate the legacy {ControllerName} module from F:\DAD Projects\TIS_MVC\TIS\ into TIS.NET10.

Follow the workflow defined in .claude/agents/module-migrator.md. Specifically:

1. Read CLAUDE.md, docs/sp_catalog.md, docs/jqx-mapping.md, docs/migration-progress.md first.
2. Spawn an Explore sub-agent to enumerate the legacy controller's actions, SP calls, views, and inline JS.
3. Plan and generate: repository (interface + impl), DTOs, controller, views (using shared partials), module JS, DI registration.
4. Build the solution. Run /verify-no-inline-sql. Update docs/migration-progress.md.
5. Report: files created, build status, verify result, any blockers.

Hard rules: no inline SQL outside TIS.Data/Repositories/, no Session[], no jqx, every action [Authorize].
```

## After agent completes

- Show the user the build status and verify result.
- Ask if they want to commit (do NOT auto-commit).
