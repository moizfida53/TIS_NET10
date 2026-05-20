namespace TIS.Web.Models.DataTables;

public class DataTableViewModel
{
    public string Id { get; set; } = "dt";
    public string AjaxUrl { get; set; } = "";
    public IEnumerable<DtColumn> Columns { get; set; } = [];
    public bool ServerSide { get; set; } = false;
    public int PageLength { get; set; } = 25;
    public bool ShowExport { get; set; } = true;
}

public class DtColumn(string data, string title)
{
    public string Data { get; } = data;
    public string Title { get; } = title;
    public bool Orderable { get; init; } = true;
    public bool Searchable { get; init; } = true;
    public string? Render { get; init; }  // "money" | "date" | "bool" | "badge:{color}" | null
    public string? Width { get; init; }
    public string? ClassName { get; init; }
}
