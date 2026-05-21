using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TIS.Data.Models.Setting;
using TIS.Data.Repositories;
using TIS.Web.Models.Setting;

namespace TIS.Web.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class SettingController(ISettingRepository repo, ILogger<SettingController> logger) : TisController
{
    private int EmpRoleId => int.Parse(User.FindFirstValue("EmpRoleId") ?? "0");
    private int CountryId => int.Parse(User.FindFirstValue("CountryId") ?? "0");

    // â”€â”€ Views â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    public IActionResult Index()    => View("Config");
    public IActionResult Policy()   => View("ManageCallType");
    public IActionResult Provider() => View();

    // â”€â”€ Config â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public async Task<IActionResult> GetConfig()
    {
        try
        {
            var cfg = await repo.GetConfigAsync();
            return Json(new { dtConfig = cfg });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetConfig failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveConfig([FromBody] ConfigDto cfg)
    {
        try
        {
            await repo.SaveConfigAsync(cfg);
            return Json(new { myMessage = "succ" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SaveConfig failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    // â”€â”€ Policy â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public async Task<IActionResult> GetPolicyData()
    {
        try
        {
            var data = await repo.GetPolicyPageDataAsync();
            return Json(new
            {
                dtProvider  = data.Providers,
                dtCallType  = data.CallTypes,
                dtEmp       = data.Employees,
                dtLineType  = data.LineTypes
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetPolicyData failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetPolicies()
    {
        try
        {
            var policies = await repo.GetPoliciesAsync();
            return Json(new { dtPolicy = policies });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetPolicies failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTransTypes(int providerId)
    {
        var data = await repo.GetTransTypesAsync(providerId);
        return Json(new { dtTransType = data });
    }

    [HttpGet]
    public async Task<IActionResult> GetDescriptions(int providerId, string transType)
    {
        var data = await repo.GetDescriptionsAsync(providerId, transType);
        return Json(new { dtDesc = data });
    }

    [HttpGet]
    public async Task<IActionResult> GetPolicyDetail(int id)
    {
        var detail = await repo.GetPolicyDetailAsync(id);
        return Json(new { dtDetail = detail });
    }

    [HttpPost]
    public async Task<IActionResult> AddPolicy([FromBody] AddPolicyRequest req)
    {
        try
        {
            var descriptions = req.IsAllDesc
                ? new[] { "" }
                : (req.Descriptions ?? []);

            foreach (var desc in descriptions)
            {
                var newId = await repo.AddPolicyAsync(
                    req.ProviderID, req.TransType, desc,
                    req.CallTypeID, req.LineTypeID, req.IsAll, req.IsSupImp);

                if (!req.IsAll && req.Employees != null)
                {
                    foreach (var emp in req.Employees)
                        await repo.AddPolicyDetailAsync(newId, emp.UID, emp.SubNoID);
                }
            }
            return Json(new { myMessage = "succ" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AddPolicy failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePolicy([FromBody] UpdatePolicyRequest req)
    {
        try
        {
            await repo.UpdatePolicyAsync(req.ID, req.IsAll, req.IsSupImp);

            if (!req.IsAll && req.Employees != null)
            {
                foreach (var emp in req.Employees)
                    await repo.AddPolicyDetailAsync(req.ID, emp.UID, emp.SubNoID);
            }
            return Json(new { myMessage = "succ" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UpdatePolicy failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeletePolicy(int id)
    {
        try
        {
            await repo.DeletePolicyAsync(id);
            return Json(new { myMessage = "succ" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeletePolicy failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ApplyPolicy()
    {
        try
        {
            await repo.ApplyPolicyAsync();
            return Json(new { myMessage = "succ" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ApplyPolicy failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    // â”€â”€ Provider â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public async Task<IActionResult> GetProviders()
    {
        try
        {
            var providers = await repo.GetProvidersAsync(EmpRoleId, CountryId);
            return Json(new { ProviderList = providers });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetProviders failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddProvider([FromBody] ProviderDto p)
    {
        try
        {
            await repo.AddProviderAsync(p);
            return Json(new { myMessage = "succ" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AddProvider failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProvider([FromBody] ProviderDto p)
    {
        try
        {
            await repo.UpdateProviderAsync(p);
            return Json(new { myMessage = "succ" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UpdateProvider failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteProvider(int id)
    {
        try
        {
            await repo.DeleteProviderAsync(id);
            return Json(new { myMessage = "succ" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeleteProvider failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }
}

