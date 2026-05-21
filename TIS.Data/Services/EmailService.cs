using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace TIS.Data.Services;

public class EmailService(ILogger<EmailService> logger)
{
    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        int at  = email.IndexOf('@');
        int dot = email.LastIndexOf('.');
        return at > 0 && dot > at + 1 && dot < email.Length - 1;
    }

    public async Task<bool> SendAsync(
        string to, string? cc, string from, string subject, string body, string smtpHost)
    {
        if (!IsValidEmail(to))
        {
            logger.LogWarning("Skipping email: invalid To address '{To}'", to);
            return false;
        }

        try
        {
            var msg = new MimeMessage();
            msg.From.Add(MailboxAddress.Parse(from));
            msg.To.Add(MailboxAddress.Parse(to));
            if (!string.IsNullOrWhiteSpace(cc) && IsValidEmail(cc))
                msg.Cc.Add(MailboxAddress.Parse(cc));
            msg.Subject = subject;
            msg.Body    = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpHost, 25, SecureSocketOptions.None);
            // Windows integrated auth — matches old SmtpClient.UseDefaultCredentials = true
            await client.AuthenticateAsync(System.Net.CredentialCache.DefaultNetworkCredentials);
            await client.SendAsync(msg);
            await client.DisconnectAsync(true);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email To='{To}' Subject='{Subject}'", to, subject);
            return false;
        }
    }
}
