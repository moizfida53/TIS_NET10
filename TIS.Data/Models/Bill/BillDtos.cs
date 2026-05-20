using System.Text.Json.Serialization;

namespace TIS.Data.Models.Bill;

// from sp_GetForceBills
public class ForceBillRowDto
{
    [JsonPropertyName("id")]
    public int     BILL_ID      { get; set; }
    [JsonPropertyName("billDate")]
    public DateTime BILLDATE    { get; set; }
    public string  Mobile       { get; set; } = "";
    public string  ProviderName { get; set; } = "";
    [JsonPropertyName("totalAmount")]
    public double  Amount       { get; set; }
    [JsonPropertyName("empName")]
    public string  EmployeeName { get; set; } = "";
    public string  ManagerName  { get; set; } = "";
    [JsonPropertyName("department")]
    public string  ORG          { get; set; } = "";
}

// from SP_ChangeBillStatus_Search / sp_ReAssignBill_Search / sp_SearchReimburseBill
public class BillSearchRowDto
{
    [JsonPropertyName("id")]
    public int     BILL_ID      { get; set; }
    [JsonPropertyName("billDate")]
    public DateTime BILLDATE    { get; set; }
    [JsonPropertyName("mobile")]
    public string  SUB_NO       { get; set; } = "";
    [JsonPropertyName("empName")]
    public string  EMPLOYEENAME { get; set; } = "";
    [JsonPropertyName("managerName")]
    public string  Appr_Manager { get; set; } = "";
    [JsonPropertyName("totalAmount")]
    public double  TOTALAMOUNT  { get; set; }
    [JsonPropertyName("statusName")]
    public string  STATUSNAME   { get; set; } = "";
    [JsonPropertyName("statusId")]
    public int     STATUS       { get; set; }
    [JsonPropertyName("uid")]
    public int     UID          { get; set; }
}

// from sp_SearchBill result-set 1
public class SearchEmpDto
{
    [JsonPropertyName("empId")]
    public int    UID        { get; set; }
    [JsonPropertyName("empNo")]
    public string EMPLOYEENO { get; set; } = "";
    [JsonPropertyName("empName")]
    public string NAME       { get; set; } = "";
}

// from sp_SearchBill result-set 2
public class SearchProviderDto
{
    [JsonPropertyName("id")]
    public int    ID   { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}

// from sp_SearchBill result-set 3 (IsStatus=true only)
public class SearchStatusDto
{
    [JsonPropertyName("id")]
    public int    ID   { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}

public class SearchDataDto
{
    public IEnumerable<SearchEmpDto>      EmpList      { get; set; } = [];
    public IEnumerable<SearchProviderDto> ProviderList { get; set; } = [];
    public IEnumerable<SearchStatusDto>   StatusList   { get; set; } = [];
}
