using TIS.Data.Infrastructure;
using TIS.Data.Models.EmailSms;

namespace TIS.Data.Repositories;

public class EmailSmsRepository(ISpRunner sp) : IEmailSmsRepository
{
    // helpers
    private static string Val(IDictionary<string, object> d, params string[] keys)
    {
        foreach (var k in keys)
            if (d.TryGetValue(k, out var v)) return v?.ToString() ?? "";
        return "";
    }

    private static object? Get(IDictionary<string, object> d, string key)
        => d.TryGetValue(key, out var v) ? v : null;

    private static EmailToSendDto ToEmailToSend(dynamic row)
    {
        var d = (IDictionary<string, object>)row;
        return new EmailToSendDto
        {
            Id      = Val(d, "ID", "Id"),
            To      = Val(d, "EmailTo"),
            From    = Val(d, "EmailFrom"),
            Subject = Val(d, "Subject"),
            Body    = Val(d, "Body", "EmailText"),
            Cc      = d.TryGetValue("CC", out var cc) ? cc?.ToString() : null
        };
    }

    private static string SmtpFromRow(dynamic row)
        => Val((IDictionary<string, object>)row, "smtpsettings");

    // Email groups
    public Task<IEnumerable<EmailGroupDto>> GetGroupsAsync()
        => sp.QueryAsync<EmailGroupDto>("sp_GetGroups");

    public async Task<IEnumerable<string>> GetEmailsByGroupAsync(string groupIDs)
    {
        var rows = await sp.QueryAsync<dynamic>("sp_GetEmails", new { GroupID = groupIDs });
        return rows.Select(r => ((IDictionary<string, object>)r)["Emails"]?.ToString() ?? "");
    }

    public Task<EmployeesResultDto> GetEmployeesAsync()
        => sp.QueryMultipleAsync("sp_GetEmployeesForGroup", null, async grid =>
        {
            var email = await grid.ReadAsync<EmailEmployeeDto>();
            var sms   = await grid.ReadAsync<SmsEmployeeDto>();
            return new EmployeesResultDto { EmailEmployees = email, SmsEmployees = sms };
        });

    public async Task<IEnumerable<int>> GetGroupDetailsAsync(int groupID)
    {
        var rows = await sp.QueryAsync<dynamic>("sp_GetGroupDetails", new { GroupID = groupID });
        return rows.Select(r => Convert.ToInt32(((IDictionary<string, object>)r)["UID"]));
    }

    public Task AddUpdateGroupAsync(string uids, string groupName, int groupID, int isUpdated)
        => sp.ExecuteAsync("sp_AddUpdateGroup",
            new { UIDs = uids, GroupName = groupName, GroupID = groupID, IsUpdated = isUpdated });

    public Task DeleteGroupAsync(int groupID)
        => sp.ExecuteAsync("sp_DeleteGroup", new { GroupID = groupID });

    // Email templates
    public Task<IEnumerable<EmailTemplateDto>> GetTemplatesAsync()
        => sp.QueryMultipleAsync("sp_GroupEmailTemplate", null, async grid =>
        {
            var rows = await grid.ReadAsync<dynamic>();
            return rows.Select(r =>
            {
                var d = (IDictionary<string, object>)r;
                return new EmailTemplateDto
                {
                    TemplateId   = Convert.ToInt32(d["Template_ID"]),
                    TemplateName = d["TemplateName"]?.ToString() ?? "",
                    Subject      = d["TemplateName"]?.ToString() ?? "",
                    TemplateText = d["Body"]?.ToString() ?? ""
                };
            });
        });

    public Task CreateTemplateAsync(string name, string subject, string text)
        => sp.ExecuteAsync("sp_CreateTemplate",
            new { TemplateName = name, Subject = subject, TemplateText = text });

    public Task SaveTemplateAsync(string name, string subject, string text, int templateId)
        => sp.ExecuteAsync("sp_SaveTemplate",
            new { TemplateName = name, Subject = subject, TemplateText = text, TemplateId = templateId });

    public Task DeleteEmailTemplateAsync(int id)
        => sp.ExecuteAsync("sp_DeleteEmailTemplate", new { ID = id });

    // Email sending
    public Task<EmailSendPacketDto> GetGroupEmailsToSendAsync(
        string groupList, string subject, int templateId, string emails)
        => sp.QueryMultipleAsync("sp_SendGroupEmail",
            new { GroupList = groupList, Subject = subject, TemplateID = templateId, Email = emails },
            async grid =>
            {
                var rows     = (await grid.ReadAsync<dynamic>()).ToList();
                var settings = (await grid.ReadAsync<dynamic>()).FirstOrDefault();
                return new EmailSendPacketDto
                {
                    SmtpHost = settings is null ? "" : SmtpFromRow(settings),
                    Emails   = rows.Select(ToEmailToSend).ToList()
                };
            });

    public Task MarkGroupEmailAsSentAsync(int id)
        => sp.ExecuteAsync("sp_GroupEmailMarkAsSent", new { ID = id });

    public Task<EmailSendPacketDto> GetLogEmailToResendAsync(int id)
        => sp.QueryMultipleAsync("sp_GetGroupEmail", new { ID = id }, async grid =>
        {
            var rows     = (await grid.ReadAsync<dynamic>()).ToList();
            var settings = (await grid.ReadAsync<dynamic>()).FirstOrDefault();
            return new EmailSendPacketDto
            {
                SmtpHost = settings is null ? "" : SmtpFromRow(settings),
                Emails   = rows.Select(ToEmailToSend).ToList()
            };
        });

    // Email log
    public Task<IEnumerable<LogEmailDto>> GetLogEmailsAsync()
        => sp.QueryMultipleAsync("sp_GetLogEmails", null, async grid =>
        {
            var rows = await grid.ReadAsync<dynamic>();
            return rows.Select(r =>
            {
                var d = (IDictionary<string, object>)r;
                return new LogEmailDto
                {
                    Id           = Convert.ToInt32(Get(d, "Id") ?? 0),
                    TemplateId   = Convert.ToInt32(Get(d, "TemplateId") ?? 0),
                    TemplateName = Get(d, "TemplateName")?.ToString() ?? "",
                    Subject      = Get(d, "Subject")?.ToString() ?? "",
                    EmailText    = Get(d, "Body")?.ToString() ?? "",
                    EmailFrom    = Get(d, "EmailFrom")?.ToString() ?? "",
                    EmailTo      = Get(d, "EmailTo")?.ToString() ?? "",
                    IsSent       = Convert.ToInt32(Get(d, "IsSent") ?? 0),
                    SentOn       = Get(d, "SentOn")?.ToString() ?? ""
                };
            });
        });

    public Task DeleteLogEmailAsync(int id)
        => sp.ExecuteAsync("sp_DeleteLogEmail", new { Id = id });

    // SMS groups
    public Task<IEnumerable<EmailGroupDto>> GetSmsGroupsAsync()
        => sp.QueryAsync<EmailGroupDto>("sp_GetSMSGroups");

    public async Task<IEnumerable<string>> GetMobileNosAsync(string groupIDs)
    {
        var rows = await sp.QueryAsync<dynamic>("sp_GetMobileNos", new { GroupID = groupIDs });
        return rows.Select(r => ((IDictionary<string, object>)r)["MobileNo"]?.ToString() ?? "");
    }

    public async Task<IEnumerable<string>> GetSmsGroupDetailsAsync(int groupID)
    {
        var rows = await sp.QueryAsync<dynamic>("sp_GetSMSGroupDetails", new { GroupID = groupID });
        return rows.Select(r => ((IDictionary<string, object>)r)["Mobile"]?.ToString() ?? "");
    }

    public Task AddUpdateSmsGroupAsync(string subNos, string groupName, int groupID, int isUpdated)
        => sp.ExecuteAsync("sp_SMSAddUpdateGroup",
            new { SUB_NOs = subNos, GroupName = groupName, GroupID = groupID, IsUpdated = isUpdated });

    public Task DeleteSmsGroupAsync(int groupID)
        => sp.ExecuteAsync("sp_DeleteSMSGroup", new { GroupID = groupID });

    // SMS templates
    public Task<IEnumerable<SmsTemplateDto>> GetSmsTemplatesAsync()
        => sp.QueryAsync<SmsTemplateDto>("sp_GroupSMSTemplate");

    public Task CreateSmsTemplateAsync(string name, string message, int language)
        => sp.ExecuteAsync("sp_CreateSMSTemplate",
            new { SMSTemplateName = name, Message = message, Language = language });

    public Task SaveSmsTemplateAsync(string name, string message, int id, int language)
        => sp.ExecuteAsync("sp_SaveSMSTemplate",
            new { SMSTemplateName = name, Message = message, SMSTemplateId = id, Language = language });

    public Task DeleteSmsTemplateAsync(int id)
        => sp.ExecuteAsync("sp_DeleteSMSTemplate", new { ID = id });

    // SMS log
    public Task LogSmsAsync(int templateId, string smsTo, string message, int isSent, int language)
        => sp.ExecuteAsync("sp_SMSLog",
            new { TemplateID = templateId, SMSTo = smsTo, Message = message, IsSent = isSent, Language = language });

    public Task<SmsLogResultDto> GetSmsLogAsync()
        => sp.QueryMultipleAsync("sp_GetLogSMS", null, async grid =>
        {
            var rows    = await grid.ReadAsync<LogSmsDto>();
            var balance = (await grid.ReadAsync<dynamic>()).FirstOrDefault();
            string bal  = balance is null
                ? "0"
                : ((IDictionary<string, object>)balance).Values.FirstOrDefault()?.ToString() ?? "0";
            return new SmsLogResultDto { Rows = rows, Balance = bal };
        });

    public Task<(string SmtpHost, List<(string Id, string To, string Message, string Language)> Items)>
        GetLogSmsToResendAsync(int id)
        => sp.QueryMultipleAsync("sp_GetGroupSMS", new { ID = id }, async grid =>
        {
            var rows     = (await grid.ReadAsync<dynamic>()).ToList();
            var settings = (await grid.ReadAsync<dynamic>()).FirstOrDefault();
            string host  = settings is null ? "" : SmtpFromRow(settings);
            var items = rows.Select(r =>
            {
                var d = (IDictionary<string, object>)r;
                return (
                    d["ID"]?.ToString() ?? "",
                    d["SMSTo"]?.ToString() ?? "",
                    d["Message"]?.ToString() ?? "",
                    d["Language"]?.ToString() ?? "1"
                );
            }).ToList();
            return (host, items);
        });

    public Task MarkSmsAsSentAsync(int id, int isSent, int language, string message)
        => sp.ExecuteAsync("sp_SMSLogMarkAsSent",
            new { ID = id, IsSent = isSent, Language = language, Message = message });

    public Task DeleteLogSmsAsync(int id)
        => sp.ExecuteAsync("sp_DeleteLogSMS", new { ID = id });

    // Bill reminder emails
    public Task<IEnumerable<SendEmailRowDto>> GetSendEmailsAsync()
        => sp.QueryMultipleAsync("sp_GetSendEmail", null, async grid =>
        {
            var rows = await grid.ReadAsync<dynamic>();
            return rows.Select(r =>
            {
                var d       = (IDictionary<string, object>)r;
                var rawDate = Get(d, "BillDate")?.ToString() ?? "";
                string billDate = DateTime.TryParse(rawDate, out var dt)
                    ? dt.ToString("MMM-yyyy")
                    : rawDate;
                return new SendEmailRowDto
                {
                    Id         = Convert.ToInt32(Get(d, "Id") ?? 0),
                    TemplateId = Convert.ToInt32(Get(d, "TemplateId") ?? 0),
                    BillId     = Convert.ToInt32(Get(d, "Bill_Id") ?? 0),
                    BillDate   = billDate,
                    Subject    = Get(d, "Subject")?.ToString() ?? "",
                    EmailText  = Get(d, "EmailText")?.ToString() ?? "",
                    EmailFrom  = Get(d, "EmailFrom")?.ToString() ?? "",
                    EmailTo    = Get(d, "EmailTo")?.ToString() ?? "",
                    CC         = Get(d, "CC")?.ToString() ?? "",
                    Sent       = Convert.ToBoolean(Get(d, "sent") ?? false),
                    SentOn     = Get(d, "senton")?.ToString() ?? ""
                };
            });
        });

    public Task<EmailSendPacketDto> GetBillEmailsToSendAsync(int bid)
        => sp.QueryMultipleAsync("sp_GetEmail", new { bid }, async grid =>
        {
            var rows     = (await grid.ReadAsync<dynamic>()).ToList();
            await grid.ReadAsync<dynamic>();           // skip Tables[1]
            var settings = (await grid.ReadAsync<dynamic>()).FirstOrDefault();
            return new EmailSendPacketDto
            {
                SmtpHost = settings is null ? "" : SmtpFromRow(settings),
                Emails   = rows.Select(ToEmailToSend).ToList()
            };
        });

    public Task MarkBillEmailAsSentAsync(int id)
        => sp.ExecuteAsync("sp_MarkAsSent", new { id });

    public Task SaveBillEmailAsync(int billId, string emailText, string emailTo, string cc)
        => sp.ExecuteAsync("sp_Save",
            new { Bill_Id = billId, EmailText = emailText, EmailTo = emailTo, CC = cc });

    public Task DeleteBillEmailAsync(int id)
        => sp.ExecuteAsync("sp_DeleteEmail", new { Id = id });

    public Task SetReminderAsync()
        => sp.ExecuteAsync("sp_BillIdentification_Reminder");

    public Task SetForceBillReminderAsync()
        => sp.ExecuteAsync("sp_ForceBill_Reminder");

    public Task GetAndSendPendingEmailsAsync(Func<EmailSendPacketDto, Task> sender)
        => sp.QueryMultipleAsync("sp_GetPendingEmail", null, async grid =>
        {
            var rows     = (await grid.ReadAsync<dynamic>()).ToList();
            await grid.ReadAsync<dynamic>();           // skip Tables[1]
            var settings = (await grid.ReadAsync<dynamic>()).FirstOrDefault();
            string host  = settings is null ? "" : SmtpFromRow(settings);
            var packet   = new EmailSendPacketDto
            {
                SmtpHost = host,
                Emails   = rows.Select(ToEmailToSend).ToList()
            };
            await sender(packet);
            return 0;
        });
}
