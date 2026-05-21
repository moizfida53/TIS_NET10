using System.Text.Json.Serialization;

namespace TIS.Data.Models.BillReport;

// from sp_SearchBillReport
public class BillReportRowDto
{
    [JsonPropertyName("billId")]
    public int      BILL_ID            { get; set; }
    [JsonPropertyName("billDate")]
    public DateTime BILLDATE           { get; set; }
    [JsonPropertyName("empNo")]
    public string   EMPLOYEENO         { get; set; } = "";
    [JsonPropertyName("empName")]
    public string   EMPLOYEENAME       { get; set; } = "";
    [JsonPropertyName("mobile")]
    public string   SUB_NO             { get; set; } = "";
    [JsonPropertyName("mobileDesc")]
    public string   SUB_DESC           { get; set; } = "";
    [JsonPropertyName("totalAmount")]
    public double   TOTALAMOUNT        { get; set; }
    [JsonPropertyName("businessCharges")]
    public double   BUSINESSCHARGES    { get; set; }
    [JsonPropertyName("personalCharges")]
    public double   PERSONALCHARGES    { get; set; }
    [JsonPropertyName("personalLimitCharges")]
    public double   PERSONALLIMITCHARGES { get; set; }
    [JsonPropertyName("deductibleAmount")]
    public double   DEDUCTIBLEAMOUNT   { get; set; }
    [JsonPropertyName("costCenter")]
    public string   COSTCENTER         { get; set; } = "";
    [JsonPropertyName("costCenterCode")]
    public string   Code               { get; set; } = "";
    [JsonPropertyName("department")]
    public string   DESCRIPTION        { get; set; } = "";
    [JsonPropertyName("payrollCategory")]
    public string   PAYROLLCATEGORY    { get; set; } = "";
    [JsonPropertyName("status")]
    public string   BillStatus         { get; set; } = "";
    [JsonPropertyName("company")]
    public string   CompanyName        { get; set; } = "";
    [JsonPropertyName("providerName")]
    public string   PROVIDERNAME       { get; set; } = "";
    [JsonPropertyName("forcedBy")]
    public string   Forced_by_UID      { get; set; } = "";
    [JsonPropertyName("forcedDate")]
    public DateTime? Forced_Date       { get; set; }
}

// from sp_BillReport(@IsStatus) result-set 0
public class BillReportStatusDto
{
    [JsonPropertyName("id")]
    public int    Id   { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}

// from sp_GetSalesReportFilterData result-set 0
public class BillReportCompanyDto
{
    [JsonPropertyName("id")]
    public int    ID      { get; set; }
    [JsonPropertyName("name")]
    public string COMPANY { get; set; } = "";
}

// from sp_Report(@IsStatus) result-set 0: providers; result-set 1: statuses
public class ReportFilterDto
{
    public IEnumerable<ReportProviderDto> ProviderList { get; set; } = [];
    public IEnumerable<ReportStatusDto>   StatusList   { get; set; } = [];
}

public class ReportProviderDto
{
    [JsonPropertyName("id")]
    public int    ID   { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}

public class ReportStatusDto
{
    [JsonPropertyName("id")]
    public int    ID   { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}

// from sp_SearchPendingBills (replaces inline SQL in old ReportController.Search)
public class PendingBillRowDto
{
    [JsonPropertyName("billId")]
    public int      BILL_ID      { get; set; }
    [JsonPropertyName("mobile")]
    public string   SUB_NO       { get; set; } = "";
    [JsonPropertyName("empName")]
    public string   EMPLOYEENAME { get; set; } = "";
    [JsonPropertyName("billDate")]
    public DateTime BILLDATE     { get; set; }
    [JsonPropertyName("totalAmount")]
    public double   TOTALAMOUNT  { get; set; }
    [JsonPropertyName("lmEmail")]
    public string   LMEmail      { get; set; } = "";
}

// from sp_ReportChart
public class ReportChartDto
{
    [JsonPropertyName("transType")]
    public string TRANS_TYPE { get; set; } = "";
    [JsonPropertyName("amount")]
    public string Amount     { get; set; } = "";
    [JsonPropertyName("org")]
    public string ORG        { get; set; } = "";
}
