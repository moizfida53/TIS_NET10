---
name: new-datatable
description: Generate a DataTables.net grid (Bootstrap 5 themed) backed by a controller endpoint and a repository SP call. Trigger when user types /new-datatable followed by an entity name, OR asks for "a new grid for X", "a DataTable showing Y".
---

# /new-datatable — Add a new DataTables grid

## When to invoke

User: `/new-datatable Bills`, "add a grid showing pending bills", "I need a DataTable for the Employee list".

## What to do — inline

1. Ask user (only if not given): which controller hosts this grid, what columns are needed, which SP returns the rows.
2. Verify the SP exists in `docs/sp_catalog.md`.
3. Add (or reuse) a repository method that returns `IEnumerable<{Dto}>`. Use `/add-sp-repo` pattern if the method doesn't exist.
4. Add a controller action:
```csharp
[HttpPost]
public async Task<IActionResult> DataTable{Entity}([FromForm] DataTablesRequest req)
{
    var rows = await _repo.Get{Entity}Async(/* filters from req */);
    return Json(new DataTablesResponse<{Dto}>(req.Draw, rows.Count(), rows.Count(), rows));
}
```
5. In the view, drop in the shared partial:
```cshtml
<partial name="_DataTable" model="@(new DataTableViewModel {
    Id = "{entity}Grid",
    AjaxUrl = Url.Action("DataTable{Entity}", "{Controller}"),
    Columns = new[] {
        new DtColumn("{Field}", "{Header}"),
        ...
    }
})" />
```
6. Build. Show user the URL to test.

If `_DataTable.cshtml`, `DataTablesRequest`, or `DataTablesResponse<T>` don't exist yet, scaffold them under `TIS.Web/Views/Shared/` and `TIS.Web/Infrastructure/DataTables/`. Reply ≤ 100 words.
