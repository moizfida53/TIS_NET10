using System.Text.Json.Serialization;

namespace TIS.Data.Models.Dashboard;

public class DashboardKpiDto
{
    [JsonPropertyName("unidentifiedBills")]
    public string UnidentifiedBills   { get; set; } = "0";
    [JsonPropertyName("unassignedAmount")]
    public string UnassignedAmount    { get; set; } = "0";
    [JsonPropertyName("billsInApproval")]
    public string BillsInApproval     { get; set; } = "0";
    [JsonPropertyName("sapAmount")]
    public string SapAmount           { get; set; } = "0";
}
