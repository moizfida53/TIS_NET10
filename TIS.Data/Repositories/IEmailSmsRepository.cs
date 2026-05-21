using TIS.Data.Models.EmailSms;

namespace TIS.Data.Repositories;

public interface IEmailSmsRepository
{
    // ── Email groups ────────────────────────────────────────────────────
    Task<IEnumerable<EmailGroupDto>> GetGroupsAsync();
    Task<IEnumerable<string>> GetEmailsByGroupAsync(string groupIDs);
    Task<EmployeesResultDto> GetEmployeesAsync();
    Task<IEnumerable<int>> GetGroupDetailsAsync(int groupID);
    Task AddUpdateGroupAsync(string uids, string groupName, int groupID, int isUpdated);
    Task DeleteGroupAsync(int groupID);

    // ── Email templates ─────────────────────────────────────────────────
    Task<IEnumerable<EmailTemplateDto>> GetTemplatesAsync();
    Task CreateTemplateAsync(string name, string subject, string text);
    Task SaveTemplateAsync(string name, string subject, string text, int templateId);
    Task DeleteEmailTemplateAsync(int id);

    // ── Email sending ───────────────────────────────────────────────────
    /// Returns emails queued by sp_SendGroupEmail + SMTP host.
    Task<EmailSendPacketDto> GetGroupEmailsToSendAsync(string groupList, string subject, int templateId, string emails);
    Task MarkGroupEmailAsSentAsync(int id);
    /// Returns emails from log entry by ID for resend.
    Task<EmailSendPacketDto> GetLogEmailToResendAsync(int id);

    // ── Email log ───────────────────────────────────────────────────────
    Task<IEnumerable<LogEmailDto>> GetLogEmailsAsync();
    Task DeleteLogEmailAsync(int id);

    // ── SMS groups ──────────────────────────────────────────────────────
    Task<IEnumerable<EmailGroupDto>> GetSmsGroupsAsync();
    Task<IEnumerable<string>> GetMobileNosAsync(string groupIDs);
    Task<IEnumerable<string>> GetSmsGroupDetailsAsync(int groupID);
    Task AddUpdateSmsGroupAsync(string subNos, string groupName, int groupID, int isUpdated);
    Task DeleteSmsGroupAsync(int groupID);

    // ── SMS templates ───────────────────────────────────────────────────
    Task<IEnumerable<SmsTemplateDto>> GetSmsTemplatesAsync();
    Task CreateSmsTemplateAsync(string name, string message, int language);
    Task SaveSmsTemplateAsync(string name, string message, int id, int language);
    Task DeleteSmsTemplateAsync(int id);

    // ── SMS log ─────────────────────────────────────────────────────────
    Task LogSmsAsync(int templateId, string smsTo, string message, int isSent, int language);
    Task<SmsLogResultDto> GetSmsLogAsync();
    /// Returns SMS row from log for resend.
    Task<(string SmtpHost, List<(string Id, string To, string Message, string Language)> Items)> GetLogSmsToResendAsync(int id);
    Task MarkSmsAsSentAsync(int id, int isSent, int language, string message);
    Task DeleteLogSmsAsync(int id);

    // ── Bill reminder emails (SendEmailController) ───────────────────────
    Task<IEnumerable<SendEmailRowDto>> GetSendEmailsAsync();
    Task<EmailSendPacketDto> GetBillEmailsToSendAsync(int bid);
    Task MarkBillEmailAsSentAsync(int id);
    Task SaveBillEmailAsync(int billId, string emailText, string emailTo, string cc);
    Task DeleteBillEmailAsync(int id);
    Task SetReminderAsync();
    Task SetForceBillReminderAsync();
    Task GetAndSendPendingEmailsAsync(Func<EmailSendPacketDto, Task> sender);
}
