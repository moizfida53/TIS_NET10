namespace TIS.Web.Models.Bill;

public class ForceBillRequest
{
    public int[]  BillID      { get; set; } = [];
    public int    Status      { get; set; }
    public int    CallType    { get; set; }
    public bool   WavRental   { get; set; }
    public bool   WavBusiness { get; set; }
    public bool   Train       { get; set; }
}

public class BillSearchRequest
{
    public int Month    { get; set; }
    public int Year     { get; set; }
    public int UID      { get; set; }
    public int Status   { get; set; }
    public int Provider { get; set; }
}

public class ReassignBillRequest
{
    public int BillId { get; set; }
    public int Uid    { get; set; }
}

public class ReimburseBillRequest
{
    public int[] BillID { get; set; } = [];
}
