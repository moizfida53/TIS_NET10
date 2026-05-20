# jqWidgets → Modern Stack Mapping

This file is the single source of truth for converting legacy jqx widgets into the new Bootstrap 5 + DataTables stack. The `/migrate-jqx-view` skill consults this file. **Do not** introduce a new replacement without updating this file first.

## Library inventory (loaded via libman)

| Library | Version | Purpose |
|---|---|---|
| jQuery | 3.7 | DataTables dependency only |
| Bootstrap | 5.3 | Layout, forms, modals, tabs |
| DataTables (Bootstrap 5) | 2.x | All grids |
| DataTables Buttons | 2.x | Export to CSV/Excel/PDF |
| Select2 (BS5 theme) | 4.x | Searchable / AJAX dropdowns |
| flatpickr | 4.6 | All date / time / datetime / range pickers |
| SweetAlert2 | 11.x | Toasts, confirms, success/error dialogs |
| Chart.js | 4.x | Dashboard charts |

## Widget mapping

### jqxGrid → DataTables

**Use the `_DataTable.cshtml` shared partial** with these options:
- Server-side processing for grids > 200 rows
- Column definitions in the partial's `columns` array
- AJAX URL points to a controller action returning `DataTablesResult<T>` from the repository

```cshtml
@* OLD jqxGrid *@
<div id="jqxBillsGrid"></div>
<script>
$("#jqxBillsGrid").jqxGrid({ width:'100%', source:billsAdapter, columns:[...] });
</script>

@* NEW DataTables *@
<partial name="_DataTable" model="@(new DataTableViewModel {
    Id = "billsGrid",
    AjaxUrl = Url.Action("DataTable", "Bill"),
    Columns = new[] {
        new DtColumn("BillId", "Bill #"),
        new DtColumn("EmployeeName", "Employee"),
        new DtColumn("Amount", "Amount") { Render = "money" },
        new DtColumn("Status", "Status") { Render = "badge" }
    }
})" />
```

### jqxDropDownList → Bootstrap select / Select2

- **Static < 20 options:** plain `<select class="form-select">`
- **Searchable or > 20 options:** Select2 with `theme: 'bootstrap-5'`
- **AJAX-loaded:** Select2 with `ajax: { url: ... }` returning `{results: [...]}`

```cshtml
@* OLD *@
<div id="jqxCountry"></div>
<script>$("#jqxCountry").jqxDropDownList({ source:countries, ... });</script>

@* NEW *@
<select id="country" class="form-select" asp-for="CountryId" asp-items="Model.Countries"></select>
<script>$("#country").select2({ theme: "bootstrap-5" });</script>
```

### jqxDateTimeInput → flatpickr

```cshtml
@* OLD *@
<div id="jqxFromDate"></div>
<script>$("#jqxFromDate").jqxDateTimeInput({ formatString:'dd-MMM-yyyy' });</script>

@* NEW *@
<input id="fromDate" class="form-control flatpickr-date" asp-for="FromDate" />
<script>flatpickr("#fromDate", { dateFormat: "d-M-Y" });</script>
```

Datetime: `enableTime: true`. Range: `mode: "range"`. Time-only: `noCalendar: true, enableTime: true`.

### jqxWindow → Bootstrap modal

Use `_Modal.cshtml` partial. Open with `bootstrap.Modal(el).show()`. Listen for `hidden.bs.modal` to clean up.

### jqxTabs → Bootstrap nav-tabs

Pure CSS — no JS library needed.
```html
<ul class="nav nav-tabs" role="tablist">
  <li class="nav-item"><button class="nav-link active" data-bs-toggle="tab" data-bs-target="#tab1">Tab 1</button></li>
</ul>
<div class="tab-content">
  <div class="tab-pane fade show active" id="tab1">…</div>
</div>
```

### jqxButton → `<button class="btn btn-*">`

Drop the wrapper; use Bootstrap utility classes directly: `btn-primary`, `btn-outline-secondary`, `btn-danger`, etc.

### jqxComboBox → Select2 with `tags: true`

Allows free-text plus pick-from-list. Use only where the old combo truly needs that — most "combo" usages can be plain Select2 dropdowns.

### jqxNotification → SweetAlert2

```js
// OLD
$("#notify").jqxNotification("open"); $("#notify").html("Saved");

// NEW (toast)
Swal.fire({ toast: true, position: "top-end", icon: "success", title: "Saved", timer: 2500, showConfirmButton: false });

// NEW (confirm)
const r = await Swal.fire({ icon: "question", title: "Delete?", showCancelButton: true });
if (r.isConfirmed) { ... }
```

### jqxChart → Chart.js

Map series → `datasets`, category axis → `labels`. For pie/donut use `type: "doughnut"`. For multi-axis bar/line, set per-dataset `yAxisID`.

## Known gaps from jqx that need workarounds

| jqx feature | Workaround |
|---|---|
| Nested column headers | DataTables ColumnGroup extension (free) |
| Frozen left columns | DataTables FixedColumns extension |
| Inline cell editing | Add `editor` plugin (Editor is paid — alternative: open a modal on row click for edit) |
| Hierarchical / tree grid | Use Bootstrap tree-view or render as flat grid with parent column |
| Export to PDF with column widths | DataTables Buttons + pdfmake (built-in) |
