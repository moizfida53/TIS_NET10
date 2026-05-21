using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIS.Data.Repositories;
using TIS.Data.Services;
using TIS.Web.Models.EmailSms;

namespace TIS.Web.Controllers;

[Authorize(Roles = "Administrator,SuperAdmin")]
public class EmailSmsController(
    IEmailSmsRepository repo,
    EmailService         emailSvc,
    SmsService           smsSvc,
    ILogger<EmailSmsController> logger) : Controller
{
    // ── Views ──────────────────────────────────────────────────────────────
    public IActionResult Index()     => View();
    public IActionResult SendEmail() => View();

    // ── Email groups ───────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> GetGroups()
    {
        var groups = await repo.GetGroupsAsync();
        return Json(new { dtGroups = groups });
    }

    [HttpPost]
    public async Task<IActionResult> GetEmails([FromBody] GroupIdsRequest req)
    {
        var emails = await repo.GetEmailsByGroupAsync(req.GroupIDs);
        return Json(new { dtEmail = emails.Select(e => new { Emails = e }) });
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployees()
    {
        var result = await repo.GetEmployeesAsync();
        return Json(new { dtEmpList = result.EmailEmployees, dtEmpList1 = result.SmsEmployees });
    }

    [HttpPost]
    public async Task<IActionResult> GetGroupDetails([FromBody] GroupIdRequest req)
    {
        var uids = await repo.GetGroupDetailsAsync(req.GroupID);
        return Json(new { dtUIDs = uids.Select(u => new { UID = u }) });
    }

    [HttpPost]
    public async Task<IActionResult> AddUpdateGroup([FromBody] AddUpdateGroupRequest req)
    {
        var uids = string.Join(",", req.Emp);
        await repo.AddUpdateGroupAsync(uids, req.GroupName, req.GroupID, req.IsUpdated);
        return Json(new { Message = "Success" });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteGroup([FromBody] GroupIdRequest req)
    {
        await repo.DeleteGroupAsync(req.GroupID);
        return Json(new { Message = "Deleted Successfuly" });
    }

    // ── Email templates ────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> GetTemplate()
    {
        var templates = await repo.GetTemplatesAsync();
        return Json(new { dtTemplate = templates });
    }

    [HttpPost]
    public async Task<IActionResult> CreateTemplate([FromBody] EmailTemplateRequest req)
    {
        await repo.CreateTemplateAsync(req.TemplateName, req.Subject, req.TemplateText);
        return Json(new { Message = "Added Successfuly" });
    }

    [HttpPost]
    public async Task<IActionResult> SaveTemplate([FromBody] EmailTemplateRequest req)
    {
        await repo.SaveTemplateAsync(req.TemplateName, req.Subject, req.TemplateText, req.TemplateId);
        return Json(new { Message = "Added Successfuly" });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEmailTemplate([FromBody] DeleteTemplateRequest req)
    {
        await repo.DeleteEmailTemplateAsync(req.TemplateId);
        return Json(new { Message = "Deleted" });
    }

    // ── Send email (compose + group) ───────────────────────────────────────
    [HttpPost]
    public async Task<IActionResult> SendGroupEmail([FromBody] SendGroupEmailRequest req)
    {
        var emails = req.Emails.Replace(" ", "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Distinct().ToList();

        var packet = await repo.GetGroupEmailsToSendAsync(
            req.CheckedGroupList, req.Subject, req.TemplateID, string.Join(",", emails));

        foreach (var email in packet.Emails)
        {
            bool sent = await emailSvc.SendAsync(
                email.To, email.Cc, email.From, email.Subject, email.Body, packet.SmtpHost);
            if (sent)
                await repo.MarkGroupEmailAsSentAsync(int.Parse(email.Id));
            else
                logger.LogWarning("Email queue ID {Id} send failed", email.Id);
        }

        return Json(new { Message = "Success" });
    }

    // ── Email log ──────────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> GetLogEmails()
    {
        var rows = await repo.GetLogEmailsAsync();
        return Json(new { dtSendEmail = rows });
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] EmailIdsRequest req)
    {
        foreach (var id in req.EmailID)
        {
            var packet = await repo.GetLogEmailToResendAsync(id);
            foreach (var email in packet.Emails)
            {
                bool sent = await emailSvc.SendAsync(
                    email.To, email.Cc, email.From, email.Subject, email.Body, packet.SmtpHost);
                if (sent)
                    await repo.MarkGroupEmailAsSentAsync(int.Parse(email.Id));
            }
        }
        return Json(new { Message = "Email Sent" });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEmail([FromBody] EmailIdsRequest req)
    {
        foreach (var id in req.EmailID)
            await repo.DeleteLogEmailAsync(id);
        return Json(new { Message = "Deleted" });
    }

    // ── SMS groups ─────────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> GetSMSGroups()
    {
        var groups = await repo.GetSmsGroupsAsync();
        return Json(new { dtSMSGroups = groups });
    }

    [HttpPost]
    public async Task<IActionResult> GetMobileNo([FromBody] SmsGroupIdsRequest req)
    {
        var mobiles = await repo.GetMobileNosAsync(req.GroupIDs);
        return Json(new { dtMobile = mobiles.Select(m => new { MobileNo = m }) });
    }

    [HttpPost]
    public async Task<IActionResult> GetSMSGroupDetails([FromBody] SmsGroupIdRequest req)
    {
        var subs = await repo.GetSmsGroupDetailsAsync(req.GroupID);
        return Json(new { dtSUB_NOs = subs.Select(s => new { SUB_NO = s }) });
    }

    [HttpPost]
    public async Task<IActionResult> SMSAddUpdateGroup([FromBody] AddUpdateSmsGroupRequest req)
    {
        var subNos = string.Join(",", req.SUB_NOs.Select(s => s.Replace("+", "")));
        await repo.AddUpdateSmsGroupAsync(subNos, req.GroupName, req.GroupID, req.IsUpdated);
        return Json(new { Message = "Success" });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSMSGroup([FromBody] SmsGroupIdRequest req)
    {
        await repo.DeleteSmsGroupAsync(req.GroupID);
        return Json(new { Message = "Deleted Successfuly" });
    }

    // ── SMS templates ──────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> GetSMSTemplate()
    {
        var templates = await repo.GetSmsTemplatesAsync();
        return Json(new { dtTemplate = templates });
    }

    [HttpPost]
    public async Task<IActionResult> CreateSMSTemplate([FromBody] SmsTemplateRequest req)
    {
        await repo.CreateSmsTemplateAsync(req.SMSTemplateName, req.Message, req.Language);
        return Json(new { Message = "Added Successfuly" });
    }

    [HttpPost]
    public async Task<IActionResult> SaveSMSTemplate([FromBody] SmsTemplateRequest req)
    {
        await repo.SaveSmsTemplateAsync(req.SMSTemplateName, req.Message, req.SMSTemplateId, req.Language);
        return Json(new { Message = "Added Successfuly" });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSMSTemplate([FromBody] DeleteSmsTemplateRequest req)
    {
        await repo.DeleteSmsTemplateAsync(req.TemplateId);
        return Json(new { Message = "Deleted" });
    }

    // ── Send SMS (compose + group) ─────────────────────────────────────────
    [HttpPost]
    public async Task<IActionResult> SendSMS([FromBody] SendSmsRequest req)
    {
        var mobiles = req.MobileNos.Replace(" ", "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Distinct();

        string lang = req.Language.ToString();
        foreach (var mobile in mobiles)
        {
            bool sent = await smsSvc.SendAsync(mobile, req.SMS, lang);
            await repo.LogSmsAsync(req.TemplateID, mobile, req.SMS, sent ? 1 : 0, req.Language);
        }

        return Json(new { Message = "Success" });
    }

    // ── SMS log ────────────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> GetLogSMS()
    {
        var result = await repo.GetSmsLogAsync();
        return Json(new { dtSendSMS = result.Rows, SMSBalance = result.Balance });
    }

    [HttpPost]
    public async Task<IActionResult> SendSMS2([FromBody] SmsIdsRequest req)
    {
        foreach (var id in req.SMSID)
        {
            var (_, items) = await repo.GetLogSmsToResendAsync(id);
            foreach (var (itemId, to, message, lang) in items)
            {
                bool sent = await smsSvc.SendAsync(to, message, lang);
                await repo.MarkSmsAsSentAsync(int.Parse(itemId), sent ? 1 : 0,
                    int.TryParse(lang, out var l) ? l : 1, message);
            }
        }
        return Json(new { Message = "Success" });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSMS([FromBody] SmsIdsRequest req)
    {
        foreach (var id in req.SMSID)
            await repo.DeleteLogSmsAsync(id);
        return Json(new { Message = "Deleted" });
    }

    // ── Bill reminder emails ───────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> GetEmail()
    {
        var rows = await repo.GetSendEmailsAsync();
        return Json(new { dtSendEmail = rows });
    }

    [HttpPost]
    public async Task<IActionResult> SendBillEmail([FromBody] SendBillEmailRequest req)
    {
        foreach (var bid in req.BID)
        {
            var packet = await repo.GetBillEmailsToSendAsync(bid);
            foreach (var email in packet.Emails)
            {
                bool sent = await emailSvc.SendAsync(
                    email.To, email.Cc, email.From, email.Subject, email.Body, packet.SmtpHost);
                if (sent)
                    await repo.MarkBillEmailAsSentAsync(int.Parse(email.Id));
            }
        }
        return Json(new { Message = "Email Sent" });
    }

    [HttpPost]
    public async Task<IActionResult> SaveBillEmail([FromBody] SaveBillEmailRequest req)
    {
        await repo.SaveBillEmailAsync(req.BillId, req.EmailText, req.EmailTo, req.CC);
        return Json(new { Message = "Success" });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteBillEmail([FromBody] DeleteBillEmailRequest req)
    {
        foreach (var id in req.EmailID)
            await repo.DeleteBillEmailAsync(id);
        return Json(new { Message = "Deleted" });
    }

    [HttpPost]
    public async Task<IActionResult> SetReminder()
    {
        await repo.SetReminderAsync();
        return Json(new { Message = "Success" });
    }

    [HttpPost]
    public async Task<IActionResult> SetForceBillReminder()
    {
        await repo.SetForceBillReminderAsync();
        return Json(new { Message = "Success" });
    }
}
