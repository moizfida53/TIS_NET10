using System.Text.Json.Serialization;

namespace TIS.Data.Models.Import;

public class UploadHistoryDto
{
    public long     ID           { get; set; }
    [JsonPropertyName("fileName")]
    public string   UploadFileName { get; set; } = "";
    public DateTime UploadDate   { get; set; }
    public DateTime BillDate     { get; set; }
    [JsonPropertyName("status")]
    public string   STatus       { get; set; } = "";
    [JsonPropertyName("providerName")]
    public string   Name         { get; set; } = "";
    [JsonPropertyName("providerId")]
    public int      Provider     { get; set; }
    public double   BillAmount   { get; set; }
}

public class ImportRowDto
{
    public int       ID          { get; set; }
    [JsonPropertyName("subNo")]
    public string?   SUB_NO      { get; set; }
    [JsonPropertyName("billDate")]
    public DateTime? BILLDATE    { get; set; }
    [JsonPropertyName("callDate")]
    public string?   CALLDATE    { get; set; }
    [JsonPropertyName("transType")]
    public string?   TRANS_TYPE  { get; set; }
    [JsonPropertyName("description")]
    public string?   DESCRIPTION { get; set; }
    [JsonPropertyName("callTime")]
    public string?   CALLTIME    { get; set; }
    [JsonPropertyName("duration")]
    public string?   DURATION    { get; set; }
    [JsonPropertyName("amount")]
    public string?   AMOUNT      { get; set; }
    [JsonPropertyName("billNumber")]
    public string?   BILLNUMBER  { get; set; }
}

public class UnassignedBillDto
{
    public DateTime BillDate    { get; set; }
    public string   Mobile      { get; set; } = "";
    [JsonPropertyName("providerName")]
    public string   Provider    { get; set; } = "";
    [JsonPropertyName("totalAmount")]
    public double   BillAmount  { get; set; }
}

public class ProviderSettingDto
{
    public string  Col1        { get; set; } = "";
    public string  Col2        { get; set; } = "";
    public string  Col3        { get; set; } = "";
    public string  Col4        { get; set; } = "";
    public string  Col5        { get; set; } = "";
    public string  Col6        { get; set; } = "";
    public string  Col7        { get; set; } = "";
    public string  Col8        { get; set; } = "";
    public string  Col9        { get; set; } = "";
    public bool    DbBased     { get; set; }
    public string? DbConstr    { get; set; }
    public string? DbTableName { get; set; }
}

public class ImportNullResult
{
    public IEnumerable<ImportRowDto> NullRows    { get; set; } = [];
    public double                    TotalAmount { get; set; }
}

public class BillDetailDto
{
    public int    BilledButNotInSystem_Count           { get; set; }
    public double BilledButNotInSystem_Amount          { get; set; }
    public int    InSystemButNotAssigned_Count         { get; set; }
    public double InSystemButNotAssigned_Amount        { get; set; }
    public int    AssignedButOutsideValidDates_Count   { get; set; }
    public double AssignedButOutsideValidDates_Amount  { get; set; }
}
