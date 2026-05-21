namespace TIS.Web.Models.EmailSms;

public class GroupIdsRequest    { public string GroupIDs  { get; set; } = ""; }
public class GroupIdRequest     { public int    GroupID   { get; set; } }
public class SmsGroupIdRequest  { public int    GroupID   { get; set; } }
public class SmsGroupIdsRequest { public string GroupIDs  { get; set; } = ""; }

public class AddUpdateGroupRequest
{
    public int[]  Emp       { get; set; } = [];
    public string GroupName { get; set; } = "";
    public int    GroupID   { get; set; }
    public int    IsUpdated { get; set; }
}

public class AddUpdateSmsGroupRequest
{
    public string[] SUB_NOs   { get; set; } = [];
    public string   GroupName { get; set; } = "";
    public int      GroupID   { get; set; }
    public int      IsUpdated { get; set; }
}

public class EmailTemplateRequest
{
    public string TemplateName { get; set; } = "";
    public string Subject      { get; set; } = "";
    public string TemplateText { get; set; } = "";
    public int    TemplateId   { get; set; }
}

public class DeleteTemplateRequest { public int TemplateId { get; set; } }

public class SendGroupEmailRequest
{
    public string CheckedGroupList { get; set; } = "";
    public string Subject          { get; set; } = "";
    public int    TemplateID       { get; set; }
    public string Emails           { get; set; } = "";
}

public class EmailIdsRequest { public int[] EmailID { get; set; } = []; }

public class SmsTemplateRequest
{
    public string SMSTemplateName { get; set; } = "";
    public string Message         { get; set; } = "";
    public int    SMSTemplateId   { get; set; }
    public int    Language        { get; set; }
}

public class DeleteSmsTemplateRequest { public int TemplateId { get; set; } }

public class SendSmsRequest
{
    public int    TemplateID { get; set; }
    public string MobileNos  { get; set; } = "";
    public string SMS        { get; set; } = "";
    public int    Language   { get; set; }
}

public class SmsIdsRequest { public int[] SMSID { get; set; } = []; }

public class SendBillEmailRequest { public int[] BID { get; set; } = []; }

public class SaveBillEmailRequest
{
    public int    BillId    { get; set; }
    public string EmailText { get; set; } = "";
    public string EmailTo   { get; set; } = "";
    public string CC        { get; set; } = "";
}

public class DeleteBillEmailRequest { public int[] EmailID { get; set; } = []; }
