namespace TIS.Web.Models.Setting;

public class EmpSubRequest
{
    public int UID    { get; set; }
    public int SubNoID { get; set; }
}

public class AddPolicyRequest
{
    public int    ProviderID   { get; set; }
    public string TransType    { get; set; } = "";
    public int    CallTypeID   { get; set; }
    public int    LineTypeID   { get; set; }
    public bool   IsAll        { get; set; }
    public bool   IsSupImp     { get; set; }
    public bool   IsAllDesc    { get; set; }
    public string[] Descriptions { get; set; } = [];
    public EmpSubRequest[]? Employees { get; set; }
}

public class UpdatePolicyRequest
{
    public int    ID        { get; set; }
    public bool   IsAll     { get; set; }
    public bool   IsSupImp  { get; set; }
    public EmpSubRequest[]? Employees { get; set; }
}
