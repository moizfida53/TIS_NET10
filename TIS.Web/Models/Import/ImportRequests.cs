using TIS.Data.Models.Import;

namespace TIS.Web.Models.Import;

public class UploadFileRequest
{
    public string FileName   { get; set; } = "";
    public string SheetName  { get; set; } = "";
    public int    ProviderID { get; set; }
    public int    Month      { get; set; }
    public int    Year       { get; set; }
    public bool   DbBased    { get; set; }
}

public class ProcessBillRequest
{
    public string FileName   { get; set; } = "";
    public string BillDate   { get; set; } = "";
    public int    ProviderID { get; set; }
}

public class UpdateImportRequest
{
    public int    ID       { get; set; }
    public double Amount   { get; set; }
    public string SubNo    { get; set; } = "";
    public string CallDate { get; set; } = "";
}

public class DeleteBillRequest
{
    public int      ProviderID { get; set; }
    public DateTime BillDate   { get; set; }
}

public class ColumnSettingRequest
{
    public int    Provider    { get; set; }
    public string Col1        { get; set; } = "";
    public string Col2        { get; set; } = "";
    public string Col3        { get; set; } = "";
    public string Col4        { get; set; } = "";
    public string Col5        { get; set; } = "";
    public string Col6        { get; set; } = "";
    public string Col7        { get; set; } = "";
    public string Col8        { get; set; } = "";
    public string Col9        { get; set; } = "";
    public string DbConstr    { get; set; } = "";
    public string DbTableName { get; set; } = "";

    public ProviderSettingDto ToDto() => new()
    {
        Col1 = Col1, Col2 = Col2, Col3 = Col3, Col4 = Col4,
        Col5 = Col5, Col6 = Col6, Col7 = Col7, Col8 = Col8, Col9 = Col9,
        DbConstr = DbConstr, DbTableName = DbTableName
    };
}
