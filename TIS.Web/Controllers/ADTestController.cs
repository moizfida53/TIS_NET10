using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TIS.Web.Controllers;

[Authorize(Roles = "SuperAdmin")]
[SupportedOSPlatform("windows")]
public class ADTestController(IConfiguration config) : TisController
{
    // â”€â”€ AD settings (read from appsettings.json ActiveDirectory section) â”€â”€â”€â”€â”€â”€

    private string AdDomain     => config["ActiveDirectory:Domain"]             ?? string.Empty;
    private string AttrDisplay  => config["ActiveDirectory:Attributes:DisplayName"]    ?? "displayName";
    private string AttrEmail    => config["ActiveDirectory:Attributes:Email"]          ?? "mail";
    private string AttrDept     => config["ActiveDirectory:Attributes:Department"]     ?? "department";
    private string AttrEmpNo    => config["ActiveDirectory:Attributes:EmployeeNumber"] ?? "employeeNumber";

    // â”€â”€ Pages â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    public IActionResult Index() => View();

    // â”€â”€ AJAX â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpPost]
    public JsonResult TestConnection()
    {
        if (string.IsNullOrWhiteSpace(AdDomain))
            return Json(new { success = false, message = "FAIL â€” 'ActiveDirectory:Domain' key is missing from appsettings.json." });

        try
        {
            using var ctx = new PrincipalContext(ContextType.Domain, AdDomain);
            bool ok = ctx.ConnectedServer != null;
            return Json(new
            {
                success = ok,
                message = ok
                    ? "SUCCESS â€” Connected to: " + ctx.ConnectedServer
                    : "FAIL â€” Could not reach a domain controller."
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "FAIL â€” " + ex.Message });
        }
    }

    [HttpPost]
    public JsonResult SearchUser(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return Json(new { success = false, message = "Please enter a username." });

        if (string.IsNullOrWhiteSpace(AdDomain))
            return Json(new { success = false, message = "FAIL â€” 'ActiveDirectory:Domain' key is missing from appsettings.json." });

        try
        {
            using var ctx  = new PrincipalContext(ContextType.Domain, AdDomain);
            using var user = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, username.Trim());

            if (user == null)
                return Json(new { success = false, message = $"No user found for '{username}'." });

            var entry = user.GetUnderlyingObject() as DirectoryEntry;

            return Json(new
            {
                success = true,
                attributeMap = new
                {
                    displayName    = AttrDisplay,
                    email          = AttrEmail,
                    department     = AttrDept,
                    employeeNumber = AttrEmpNo
                },
                data = new
                {
                    displayName    = GetProperty(entry, AttrDisplay),
                    email          = GetProperty(entry, AttrEmail),
                    department     = GetProperty(entry, AttrDept),
                    employeeNumber = GetProperty(entry, AttrEmpNo),
                    samAccount     = user.SamAccountName ?? string.Empty
                }
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public JsonResult UpdateMobile(string username, string mobileNumber)
    {
        if (string.IsNullOrWhiteSpace(username))
            return Json(new { success = false, message = "Please enter a username." });

        if (string.IsNullOrWhiteSpace(mobileNumber))
            return Json(new { success = false, message = "Please enter a mobile number." });

        if (string.IsNullOrWhiteSpace(AdDomain))
            return Json(new { success = false, message = "FAIL â€” 'ActiveDirectory:Domain' key is missing from appsettings.json." });

        try
        {
            using var ctx  = new PrincipalContext(ContextType.Domain, AdDomain);
            using var user = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, username.Trim());

            if (user == null)
                return Json(new { success = false, message = $"No user found for '{username}'." });

            var entry = user.GetUnderlyingObject() as DirectoryEntry;
            if (entry == null)
                return Json(new { success = false, message = "FAIL â€” Could not retrieve directory entry for user." });

            entry.Properties["mobile"].Value = mobileNumber.Trim();
            entry.CommitChanges();

            return Json(new { success = true, message = $"SUCCESS â€” Mobile number updated for '{username}'." });
        }
        catch (UnauthorizedAccessException)
        {
            return Json(new { success = false, message = "FAIL â€” The application account does not have write permission to Active Directory." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "FAIL â€” " + ex.Message });
        }
    }

    // â”€â”€ Helpers â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    private static string GetProperty(DirectoryEntry? entry, string attr)
    {
        if (entry == null || string.IsNullOrWhiteSpace(attr)) return string.Empty;
        try
        {
            if (entry.Properties.Contains(attr))
            {
                var val = entry.Properties[attr].Value;
                return val?.ToString() ?? string.Empty;
            }
        }
        catch { /* attribute may not exist in this AD schema */ }
        return string.Empty;
    }
}

