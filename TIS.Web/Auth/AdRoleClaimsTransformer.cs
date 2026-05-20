using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using TIS.Data.Repositories;

namespace TIS.Web.Auth;

public class AdRoleClaimsTransformer(IAdminRepository repo, ILogger<AdRoleClaimsTransformer> log)
    : IClaimsTransformation
{
    private static readonly string[] RoleNames =
    [
        "", "Employee", "LineManager", "Administrator",
        "AdminService", "Finance", "Secretary", "", "SuperAdmin"
    ];

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true) return principal;
        if (principal.HasClaim(c => c.Type == "EmpId")) return principal;

        var winUser = principal.Identity.Name ?? "";
        var samAccount = winUser.Contains('\\') ? winUser.Split('\\')[^1] : winUser;

        try
        {
            var emp = await repo.GetEmployeeByUsernameAsync(samAccount);
            if (emp is null)
            {
                log.LogWarning("No TIS employee record for Windows user {User}", samAccount);
                return principal;
            }

            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim("EmpId", emp.UID.ToString()));
            identity.AddClaim(new Claim("EmpRoleId", emp.ROLEID.ToString()));
            identity.AddClaim(new Claim("CountryId", emp.COUNTRYID.ToString()));
            identity.AddClaim(new Claim("EmpLoginName", emp.USERNAME ?? samAccount));
            identity.AddClaim(new Claim("EmpLoginAs", emp.USERNAME ?? samAccount));

            var roleName = emp.ROLEID is >= 1 and <= 8 ? RoleNames[emp.ROLEID] : "Employee";
            identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
            if (emp.ROLEID == 8)
                identity.AddClaim(new Claim(ClaimTypes.Role, "SuperAdmin"));
            if (emp.ROLEID is 3 or 8)
                identity.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
            if (emp.ROLEID == 5)
                identity.AddClaim(new Claim(ClaimTypes.Role, "Finance"));

            principal.AddIdentity(identity);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Failed to load claims for {User}", samAccount);
        }

        return principal;
    }
}
