using System.Text.Json.Serialization;

namespace TIS.Data.Models.Setting;

public class ConfigDto
{
    public int  EmpReminder       { get; set; }
    public int  ForceBillReminder { get; set; }
    public int  MgrComplaintReminder { get; set; }
    public int  LMReminder        { get; set; }
    public string SMTPSettings    { get; set; } = "";
    public string AdminEmail      { get; set; } = "";
    public string HostUrl         { get; set; } = "";
    public int  SuperGrade        { get; set; }
    public bool EnableGrade       { get; set; }
    public bool NotSendMail       { get; set; }
    public bool HidePersonalCalls { get; set; }
    public bool SkipGMApproval    { get; set; }
    public bool EnableDiscrepancy { get; set; }
    public bool SkipApprovalBuss  { get; set; }
    public bool DedBussinessCharges { get; set; }
    public bool BusinessZeroAsUnlimited { get; set; }
    public bool AllowWaiver       { get; set; }
    public bool DeleteBut         { get; set; }
    public bool AllowTrainForceBill { get; set; }
    public bool HideAllowanceLimit { get; set; }
    public bool HidePersonalLimit  { get; set; }
}

public class PolicyDto
{
    public int    ID              { get; set; }
    public int    Provider        { get; set; }
    public string ProviderName    { get; set; } = "";
    public string ProviderTypeDesc { get; set; } = "";
    public string DestinationDesc  { get; set; } = "";
    public int    CallType         { get; set; }
    public string CallTypeDesc     { get; set; } = "";
    public int    LineType         { get; set; }
    public string LineTypeName     { get; set; } = "";
    public bool   IsAll            { get; set; }
    public bool   SuperimposeTrain { get; set; }
}

public class PolicyDetailDto
{
    public int    SubNoID       { get; set; }
    public int    UID           { get; set; }
    public string EmployeeName  { get; set; } = "";
}

public class TransTypeDto  { public string TransType   { get; set; } = ""; }
public class DescriptionDto { public string Description { get; set; } = ""; }

public class ProviderDto
{
    public int    ID        { get; set; }
    public string Name      { get; set; } = "";
    public bool   IsVoip    { get; set; }
    public int    CountryID { get; set; }
}

public class CallTypeListDto
{
    public int    ID   { get; set; }
    public string Name { get; set; } = "";
}

public class LineTypeDto
{
    public int    Id       { get; set; }
    public string LineType { get; set; } = "";
}

public class EmpSubDto
{
    [JsonPropertyName("uid")]          public int    UID          { get; set; }
    [JsonPropertyName("employeeName")] public string EmployeeName { get; set; } = "";
    [JsonPropertyName("org")]          public string ORG          { get; set; } = "";
    [JsonPropertyName("subNoId")]      public int    SubNoID      { get; set; }
    [JsonPropertyName("subNo")]        public string SubNo        { get; set; } = "";
}
