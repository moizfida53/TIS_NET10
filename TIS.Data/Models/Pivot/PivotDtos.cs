using System.Text.Json.Serialization;

namespace TIS.Data.Models.Pivot;

// from sp_vwSub1Pivot Tables[1]
public class PivotRowDto
{
    [JsonPropertyName("mobile")]
    public int     SUB_NO          { get; set; }
    [JsonPropertyName("transType")]
    public string  TRANS_TYPE      { get; set; } = "";
    [JsonPropertyName("amount")]
    public decimal AMOUNT          { get; set; }
    [JsonPropertyName("billDate")]
    public string  BILLDATE        { get; set; } = "";
    [JsonPropertyName("provider")]
    public string  PROVIDER_TEXT   { get; set; } = "";
    [JsonPropertyName("lineStatus")]
    public string  LineStatus      { get; set; } = "";
    [JsonPropertyName("lineType")]
    public string  LineType        { get; set; } = "";
    [JsonPropertyName("costCenter")]
    public string  CostCenter_Name { get; set; } = "";
    [JsonPropertyName("department")]
    public string  Department      { get; set; } = "";
    [JsonPropertyName("country")]
    public string  Country         { get; set; } = "";
}
