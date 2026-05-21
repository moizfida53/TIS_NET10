using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIS.Data.Repositories;
using TIS.Web.Models.Pivot;

namespace TIS.Web.Controllers;

[Authorize(Roles = "Administrator,SuperAdmin")]
public class PivotController(
    IPivotRepository repo,
    ILogger<PivotController> logger) : Controller
{
    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> GetPivot()
    {
        try
        {
            var rows = await repo.GetPivotAsync();
            return Json(new { dtPivot = rows });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetPivot failed");
            return Json(new { Fail = true });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] PivotSaveRequest req)
    {
        try
        {
            await repo.SavePivotAsync(req.Object);
            return Json("Success");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Pivot Save failed");
            return Json("Fail");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Restore()
    {
        try
        {
            var obj = await repo.RestorePivotAsync();
            return Json(new { dtPivot = obj });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Pivot Restore failed");
            return Json(new { Fail = true });
        }
    }
}
