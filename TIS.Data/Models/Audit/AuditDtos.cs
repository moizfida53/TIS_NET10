using System.Text.Json.Serialization;

namespace TIS.Data.Models.Audit;

public class AuditEmployeeDto
{
    [JsonPropertyName("uid")]        public int    Uid      { get; set; }
    [JsonPropertyName("userName")]   public string UserName { get; set; } = "";
    [JsonPropertyName("empName")]    public string EmpName  { get; set; } = "";
    [JsonPropertyName("empNo")]      public string EmpNo    { get; set; } = "";
}

public class AuditReportRowDto
{
    [JsonPropertyName("id")]         public int    Id         { get; set; }
    [JsonPropertyName("actionName")] public string ActionName { get; set; } = "";
    [JsonPropertyName("result")]     public string Result     { get; set; } = "";
    [JsonPropertyName("user")]       public string User       { get; set; } = "";
    [JsonPropertyName("userId")]     public string UserId     { get; set; } = "";
    [JsonPropertyName("date")]       public string Date       { get; set; } = "";
    [JsonPropertyName("formId")]     public int    FormId     { get; set; }
}

public class AuditDetailDto
{
    [JsonPropertyName("id")]         public int    Id        { get; set; }
    [JsonPropertyName("sno")]        public int    Sno       { get; set; }
    [JsonPropertyName("atId")]       public int    AtId      { get; set; }
    [JsonPropertyName("oldValue")]   public string OldValue  { get; set; } = "";
    [JsonPropertyName("newValue")]   public string NewValue  { get; set; } = "";
    [JsonPropertyName("fieldName")]  public string FieldName { get; set; } = "";
}

public class AuditSearchRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate   { get; set; }
    public int      Event     { get; set; }
    public int      Uid       { get; set; }
    public string   Status    { get; set; } = "";
}
