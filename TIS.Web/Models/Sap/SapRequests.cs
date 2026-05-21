namespace TIS.Web.Models.Sap;

public class MarkAsPostedRequest
{
    public List<SapRowRequest> Value { get; set; } = [];
}

public class SapRowRequest
{
    public string BillId          { get; set; } = "";
    public string DeductibleAmount { get; set; } = "";
}
