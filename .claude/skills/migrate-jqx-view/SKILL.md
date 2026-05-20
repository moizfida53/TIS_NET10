---
name: migrate-jqx-view
description: Convert a single legacy .cshtml view containing jqWidgets into a modern Bootstrap 5 + DataTables view. Trigger when user types /migrate-jqx-view followed by a path, OR asks to "convert this view to Bootstrap" or "remove jqx from {file}".
---

# /migrate-jqx-view — Convert one jqx-heavy view

## When to invoke

User says: `/migrate-jqx-view Views/User/Index.cshtml`, "convert this view to Bootstrap", "remove jqx from Email/Templates.cshtml".

## What to do

Delegate to the `jqx-view-converter` agent. Pass the source view path.

## Template prompt for the agent

```
Convert the legacy view at {path} into the new TIS.NET10 project.

Follow .claude/agents/jqx-view-converter.md exactly:
- Read docs/jqx-mapping.md
- Apply the widget mapping table
- Move inline JS to wwwroot/js/modules/{controller}.js
- Drop jqx scripts/CSS
- Use shared partials (_DataTable, _Modal, _FormGroup)

Report: new file paths + a short list of features that don't map cleanly.
```
