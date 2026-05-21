namespace TIS.Web.Models.BillReport;

public class BillReportSearchRequest
{
    public int Month     { get; set; }
    public int Year      { get; set; }
    public int Status    { get; set; }
    public int CompanyId { get; set; }
}

public class PendingBillSearchRequest
{
    public int Month    { get; set; }
    public int Year     { get; set; }
    public int Provider { get; set; }
    public int Status   { get; set; }
}
