using System.Text.Json.Serialization;

namespace TIS.Data.Models.Sap;

public class SapReportRowDto
{
    [JsonPropertyName("billId")]          public string BillId          { get; set; } = "";
    [JsonPropertyName("billDate")]        public string BillDate        { get; set; } = "";
    [JsonPropertyName("telephoneNumber")] public string TelephoneNumber { get; set; } = "";
    [JsonPropertyName("employeeNo")]      public string EmployeeNo      { get; set; } = "";
    [JsonPropertyName("employeeName")]    public string EmployeeName    { get; set; } = "";
    [JsonPropertyName("managerName")]     public string ManagerName     { get; set; } = "";
    [JsonPropertyName("totalAmount")]     public string TotalAmount     { get; set; } = "";
    [JsonPropertyName("businessCharges")]  public string BusinessCharges  { get; set; } = "";
    [JsonPropertyName("personalCharges")]  public string PersonalCharges  { get; set; } = "";
    [JsonPropertyName("deductibleAmount")] public string DeductibleAmount { get; set; } = "";
    [JsonPropertyName("costCenterName")]  public string CostCenterName  { get; set; } = "";
    [JsonPropertyName("costCenterCode")]  public string CostCenterCode  { get; set; } = "";
    [JsonPropertyName("department")]      public string Department      { get; set; } = "";
}

public class SapReportResultDto
{
    public IEnumerable<SapReportRowDto> Bills        { get; set; } = [];
    public string                       PendingCount { get; set; } = "0";
}
