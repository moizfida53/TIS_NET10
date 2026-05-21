# Migration Progress

Updated after each module finishes. Status values: ☐ pending · ◐ in-progress · ☑ done.

## Phase 0 — Setup

- ☑ Solution scaffold (TIS.slnx, TIS.Web, TIS.Data)
- ☑ NuGet packages installed
- ☑ Database restored as `TIS_KDD_NEW` on `.\SQLEXPRESS`
- ☑ `docs/sp_catalog.md` generated (143 SPs)
- ☑ `.claude/` folder scaffolded
- ☑ `CLAUDE.md` written
- ☐ Git initialized + first commit

## Phase 1 — Backend foundation

- ☑ `ISpRunner` + Dapper-based `SpRunner`
- ☑ `SqlConnectionFactory`
- ☑ `appsettings.json` with `TIS_KDD_NEW` connection (no hardcoded creds)
- ☑ Windows AD auth wired (`AddNegotiate`)
- ☑ `AdRoleClaimsTransformer` reading role via SP
- ☑ Authorization policies for all 7 legacy roles
- ☑ Serilog logging configured
- ☑ Shared layout + base partials (`_DataTable`, `_FormGroup`, `_Modal`)
- ☑ libman manifest with all client-side libs

## Phase 2 — Module migration

| # | Module | Legacy controllers | Status |
|---|---|---|---|
| 1 | Admin / Users | `AdminController`, `ADTestController` | ☑ done 2026-05-20 |
| 2 | Employee / Telephone | `TelephoneController` | ☐ |
| 3 | Settings | `SettingController` | ☑ done 2026-05-20 |
| 4 | Import (Excel) | `ImportController` | ☑ done 2026-05-20 |
| 5 | Bill management | `BillController` | ☑ done 2026-05-20 |
| 6 | Bill reporting | `BillReportController`, `ReportController` | ☑ done 2026-05-21 |
| 7 | Dashboard & Pivot | `DashboardController`, `PivotController` | ☐ |
| 8 | Email / SMS | `EmailSmsController`, `SendEmailController` | ☐ |
| 9 | SAP integration | `BapiBIController`, `SyncBapiController` | ☐ |
| 10 | Audit & APIs | `AuditReportController`, `MarsaAPIController` | ☐ |

## Phase 3 — Frontend modernization (parallel with Phase 2)

Track per-view jqx removal in commit messages; no separate checklist.

## Phase 4 — Reports / SAP / Excel

- ☐ All RDLC reports re-implemented as Razor HTML + QuestPDF
- ☐ ClosedXML export helper service
- ☐ SAP NCo .NET 10 compatibility confirmed (or shim built)
- ☐ MailKit email service replaces `SmtpClient`

## Phase 5 — Cutover

- ☐ Manual smoke test pass on all modules
- ☐ UAT with Finance + Admin users
- ☐ Performance parity vs old app
- ☐ Production deploy to IIS
- ☐ DNS cutover
- ☐ Old `TIS_KDD` set to read-only for 30 days
