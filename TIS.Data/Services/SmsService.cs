using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TIS.Data.Services;

public class SmsService(IHttpClientFactory http, IConfiguration config, ILogger<SmsService> logger)
{
    private readonly string _user     = config["Sms:UserName"]  ?? "";
    private readonly string _password = config["Sms:Password"]  ?? "";
    private readonly string _sender   = config["Sms:SenderID"]  ?? "";

    /// <summary>Sends one SMS. Returns true if the API responded with "OK".</summary>
    public async Task<bool> SendAsync(string mobile, string message, string language)
    {
        try
        {
            var url = $"http://www.brazilboxtech.com/api/send.aspx" +
                      $"?username={Uri.EscapeDataString(_user)}" +
                      $"&password={Uri.EscapeDataString(_password)}" +
                      $"&language={language}" +
                      $"&sender={Uri.EscapeDataString(_sender)}" +
                      $"&mobile=965{mobile}" +
                      $"&message= {Uri.EscapeDataString(message)}";

            using var client = http.CreateClient();
            var response = await client.GetStringAsync(url);
            return response.Contains("OK", StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SMS send failed for mobile '{Mobile}'", mobile);
            return false;
        }
    }
}
