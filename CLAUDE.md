# TIS.NET10 — Project Context for Claude

## What this repo is

A full rewrite of the legacy `TIS_MVC` (ASP.NET MVC 5 / .NET Framework 4.8 / jqWidgets / Bootstrap 3) into a modern **.NET 10** solution. Domain: telecom bill management — Excel-imported provider bills, employee charge assignment, personal-limit enforcement, SAP master data sync, email/SMS notifications, bill reports.

Legacy source (read-only reference): `F:\DAD Projects\TIS_MVC\TIS\`
Approved migration plan: `C:\Users\Moiz Taha\.claude\plans\i-have-an-old-sharded-curry.md`

## Stack (locked in — do not deviate)

| Layer | Choice |
|---|---|
| Runtime | **.NET 10** (LTS) |
| Web | ASP.NET Core MVC (Controllers + Razor) — no Minimal APIs, no Blazor |
| Data | **Dapper** via `ISpRunner` — **stored procedures only** |
| Auth | Windows AD (Negotiate) + `AdRoleClaimsTransformer` → claims |
| UI | Bootstrap 5.3, DataTables 2.x (BS5 theme), Select2, flatpickr, SweetAlert2, Chart.js |
| Reports | Razor HTML views + **QuestPDF** (PDF) + **ClosedXML** (Excel) |
| Excel import | ExcelDataReader 3.x |
| SAP | SAP.Connector.Rfc (NCo 3.1) — isolate via shim if .NET 10 compat fails |
| Email | MailKit |
| Logging | Serilog → SQL Server sink + file |

## Solution layout

```
TIS.slnx
├── TIS.Web/   ← MVC, Razor views, wwwroot, Program.cs, controllers, auth
└── TIS.Data/  ← ISpRunner, repositories, DTOs, services (email/excel/PDF)
```

`TIS.Web` references `TIS.Data`. No reverse dependency.

## Hard rules — enforced by skills and code review

1. **No inline SQL outside `TIS.Data/Repositories/`.** Controllers, services, and views must call repository methods only. Repository methods may only invoke stored procedures via `ISpRunner`. There is a `/verify-no-inline-sql` skill that greps for `SqlConnection`, `SqlCommand`, `FromSqlRaw`, `ExecuteSqlRaw`, raw `SELECT`/`INSERT`/`UPDATE`/`DELETE`/`EXEC` strings outside the repository folder — it must return zero hits before a module is considered done.
2. **No `Session[...]`.** Auth state lives in `ClaimsPrincipal`. Read role/empId via `User.FindFirst("EmpId")`, `User.IsInRole("Finance")`, etc. The old `Session["EmpRoleID"]`, `Session["EmpLoginName"]`, `Session["CountryID"]` patterns are forbidden.
3. **No jqWidgets references** (jqx*, jqxGrid, jqxDropDownList, etc.). Use the mapping in `docs/jqx-mapping.md`.
4. **Every controller action requires `[Authorize]`** plus a role policy (e.g. `[Authorize(Roles="Finance,SuperAdmin")]`). Anonymous endpoints must be explicitly opted out with `[AllowAnonymous]` and justified in a comment.
5. **No hardcoded connection strings or credentials.** Use `appsettings.json` for non-secrets; User Secrets in dev; environment variables in prod.
6. **SP naming follows the existing catalog.** Don't rename SPs during port — keep the old names (e.g. `sp_GetForceBills`) so SQL Server schema diffs stay clean. See `docs/sp_catalog.md`.

## Conventions

- **Repositories** live in `TIS.Data/Repositories/` as one file per domain area: `BillRepository.cs`, `EmployeeRepository.cs`, etc. Each exposes an interface `I{Entity}Repository`.
- **DTOs** live in `TIS.Data/Models/`. They are plain records or POCOs — no EF mappings.
- **Controllers** are thin orchestrators: validate input, call repo/service, return view or `IActionResult`. No business logic in controllers.
- **Razor views** must use shared partials (`_DataTable.cshtml`, `_FormGroup.cshtml`, `_Modal.cshtml`) instead of inlining HTML for repeated patterns.
- **Per-module JS** lives at `TIS.Web/wwwroot/js/modules/{module}.js`. Never script-tag-inline in `.cshtml`.
- **Per-module CSS** at `TIS.Web/wwwroot/css/modules/{module}.css` only if module-specific; otherwise extend `site.css`.

## Pointers

- `docs/sp_catalog.md` — full list of 143 stored procedures with parameters. Read this before adding any repository method.
- `docs/jqx-mapping.md` — old jqx widget → new Bootstrap/DataTables/etc. mapping with code snippets.
- `docs/migration-progress.md` — checklist of which modules have been ported. Update after each module.
- `docs/old-vs-new-routes.md` — URL parity table; ensures bookmarks/redirects keep working post-cutover.

## Skills available in this repo

- `/migrate-module {OldControllerName}` — orchestrates a full module port from legacy to new
- `/migrate-jqx-view {path/to/old/View.cshtml}` — converts a single view to Bootstrap5 + DataTables
- `/add-sp-repo {SpName}` — scaffolds a repository method around an existing SP from `sp_catalog.md`
- `/new-datatable {EndpointName}` — generates a controller endpoint + Razor partial usage for a DataTables grid
- `/new-report {ReportName}` — scaffolds an HTML report view + QuestPDF export action
- `/verify-no-inline-sql` — grep-based gate to ensure no SQL outside `TIS.Data/Repositories/`

## When in doubt

- Reading legacy code: open it from `F:\DAD Projects\TIS_MVC\TIS\` — that path is allowed in `.claude/settings.json`.
- New code patterns: prefer the simplest thing that works. No premature abstractions, no "future-proofing", no helpers for the hypothetical second caller.
- Database changes: avoid schema/SP changes during the migration. If a new SP is needed, add it to `TIS_KDD_NEW` and update `docs/sp_catalog.md` in the same commit.
