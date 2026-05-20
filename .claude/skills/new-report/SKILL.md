---
name: new-report
description: Scaffold a new report as an HTML Razor view plus a QuestPDF export action (and optional ClosedXML Excel export). Replaces the legacy RDLC pattern. Trigger when user types /new-report followed by a report name, OR asks for "a new report", "a PDF report for X".
---

# /new-report — Scaffold a new HTML + PDF report

## When to invoke

User: `/new-report MonthlyBillSummary`, "add a PDF report for force bills", "convert the legacy Report2.rdlc".

## What to do — inline

1. Ask which SP feeds the report and what filters apply (date range, country, role).
2. Create or extend the relevant repository with a method returning the report's row DTO.
3. Add the controller actions (typically in `ReportController` or a module-specific one):
   - `GET /Reports/{Name}` → renders the HTML view
   - `GET /Reports/{Name}/Pdf?...filters` → returns `FileContentResult` from QuestPDF
   - `GET /Reports/{Name}/Excel?...filters` → returns `FileContentResult` from ClosedXML (optional)
4. The Razor view uses a `print.css`-style stylesheet so the same markup looks clean on screen and on the QuestPDF render.
5. QuestPDF document goes in `TIS.Data/Services/Reports/{Name}Pdf.cs` implementing `IDocument`. Compose with `.Header()`, `.Content()`, `.Footer()`.
6. ClosedXML export goes in the same `TIS.Data/Services/Reports/{Name}Excel.cs` returning `byte[]`.
7. Register the QuestPDF community license in `Program.cs` (`QuestPDF.Settings.License = LicenseType.Community;`) — once, not per-report.

Build, show URLs, reply ≤ 100 words.
