using System.Text.Json.Serialization;

namespace TIS.Data.Models.EmailSms;

public class EmailGroupDto
{
    [JsonPropertyName("groupId")]   public int    GroupID   { get; set; }
    [JsonPropertyName("groupName")] public string GroupName { get; set; } = "";
}

public class EmailTemplateDto
{
    [JsonPropertyName("templateId")]   public int    TemplateId   { get; set; }
    [JsonPropertyName("templateName")] public string TemplateName { get; set; } = "";
    [JsonPropertyName("subject")]      public string Subject      { get; set; } = "";
    [JsonPropertyName("templateText")] public string TemplateText { get; set; } = "";
}

public class EmailEmployeeDto
{
    [JsonPropertyName("uid")]      public int    UID      { get; set; }
    [JsonPropertyName("username")] public string USERNAME { get; set; } = "";
    [JsonPropertyName("email")]    public string EMAIL    { get; set; } = "";
    [JsonPropertyName("org")]      public string ORG      { get; set; } = "";
}

public class SmsEmployeeDto
{
    [JsonPropertyName("uid")]      public int    UID      { get; set; }
    [JsonPropertyName("username")] public string USERNAME { get; set; } = "";
    [JsonPropertyName("subNo")]    public string SUB_NO   { get; set; } = "";
    [JsonPropertyName("org")]      public string ORG      { get; set; } = "";
}

public class SmsTemplateDto
{
    [JsonPropertyName("smsTemplateId")]   public int    SMSTemplateId   { get; set; }
    [JsonPropertyName("smsTemplateName")] public string SMSTemplateName { get; set; } = "";
    [JsonPropertyName("message")]         public string Message         { get; set; } = "";
    [JsonPropertyName("language")]        public int    Language        { get; set; }
}

public class LogEmailDto
{
    [JsonPropertyName("id")]           public int    Id           { get; set; }
    [JsonPropertyName("templateId")]   public int    TemplateId   { get; set; }
    [JsonPropertyName("templateName")] public string TemplateName { get; set; } = "";
    [JsonPropertyName("subject")]      public string Subject      { get; set; } = "";
    [JsonPropertyName("emailText")]    public string EmailText    { get; set; } = "";
    [JsonPropertyName("emailFrom")]    public string EmailFrom    { get; set; } = "";
    [JsonPropertyName("emailTo")]      public string EmailTo      { get; set; } = "";
    [JsonPropertyName("isSent")]       public int    IsSent       { get; set; }
    [JsonPropertyName("sentOn")]       public string SentOn       { get; set; } = "";
}

public class LogSmsDto
{
    [JsonPropertyName("id")]              public int    ID              { get; set; }
    [JsonPropertyName("smsTemplateId")]   public int    SMSTemplateId   { get; set; }
    [JsonPropertyName("smsTemplateName")] public string SMSTemplateName { get; set; } = "";
    [JsonPropertyName("smsTo")]           public string SMSTo           { get; set; } = "";
    [JsonPropertyName("message")]         public string Message         { get; set; } = "";
}

public class SmsLogResultDto
{
    public IEnumerable<LogSmsDto> Rows    { get; set; } = [];
    public string                 Balance { get; set; } = "0";
}

public class SendEmailRowDto
{
    [JsonPropertyName("id")]         public int    Id       { get; set; }
    [JsonPropertyName("templateId")] public int    TemplateId { get; set; }
    [JsonPropertyName("billId")]     public int    BillId   { get; set; }
    [JsonPropertyName("billDate")]   public string BillDate { get; set; } = "";
    [JsonPropertyName("subject")]    public string Subject  { get; set; } = "";
    [JsonPropertyName("emailText")]  public string EmailText{ get; set; } = "";
    [JsonPropertyName("emailFrom")]  public string EmailFrom{ get; set; } = "";
    [JsonPropertyName("emailTo")]    public string EmailTo  { get; set; } = "";
    [JsonPropertyName("cc")]         public string CC       { get; set; } = "";
    [JsonPropertyName("sent")]       public bool   Sent     { get; set; }
    [JsonPropertyName("sentOn")]     public string SentOn   { get; set; } = "";
}

public class EmployeesResultDto
{
    public IEnumerable<EmailEmployeeDto> EmailEmployees { get; set; } = [];
    public IEnumerable<SmsEmployeeDto>   SmsEmployees   { get; set; } = [];
}

// Internal: email rows + SMTP host returned by sending SPs
public class EmailSendPacketDto
{
    public string SmtpHost { get; set; } = "";
    public List<EmailToSendDto> Emails { get; set; } = [];
}

public class EmailToSendDto
{
    public string  Id      { get; set; } = "";
    public string  To      { get; set; } = "";
    public string  From    { get; set; } = "";
    public string  Subject { get; set; } = "";
    public string  Body    { get; set; } = "";
    public string? Cc      { get; set; }
}
