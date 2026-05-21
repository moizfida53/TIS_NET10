using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIS.Data.Repositories;

namespace TIS.Web.Controllers;

[Authorize(Roles = "Administrator,SuperAdmin")]
public class DashboardController(
    IDashboardRepository repo,
    ILogger<DashboardController> logger) : TisController
{
    public IActionResult Index() => View();

    // â”€â”€ KPI â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public async Task<IActionResult> GetKpi()
    {
        try
        {
            var kpi = await repo.GetKpiAsync();
            return Json(kpi);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetKpi failed");
            return Json(new { Message = "Fail" });
        }
    }

    // â”€â”€ Charts â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public async Task<IActionResult> GetChart1(int year) =>
        await ChartResult(() => repo.GetChart1Async(year), "GetChart1");

    [HttpGet]
    public async Task<IActionResult> GetChart2(int year, string transType = "") =>
        await ChartResult(() => repo.GetChart2Async(year, transType), "GetChart2");

    [HttpGet]
    public async Task<IActionResult> GetChart3(int year, int month) =>
        await ChartResult(() => repo.GetChart3Async(year, month), "GetChart3");

    [HttpGet]
    public async Task<IActionResult> GetChart4(int year) =>
        await ChartResult(() => repo.GetChart4Async(year), "GetChart4");

    [HttpGet]
    public async Task<IActionResult> GetChart5(int year, string callTypeName = "") =>
        await ChartResult(() => repo.GetChart5Async(year, callTypeName), "GetChart5");

    [HttpGet]
    public async Task<IActionResult> GetChart6(int year, string callType = "") =>
        await ChartResult(() => repo.GetChart6Async(year, callType), "GetChart6");

    [HttpGet]
    public async Task<IActionResult> GetChart7(int year, string callType = "", string outCountry = "") =>
        await ChartResult(() => repo.GetChart7Async(year, callType, outCountry), "GetChart7");

    // â”€â”€ Lookup data â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public async Task<IActionResult> GetTransTypes() =>
        await ChartResult(() => repo.GetTransTypesAsync(), "GetTransTypes");

    [HttpGet]
    public async Task<IActionResult> GetCallTypes(int year) =>
        await ChartResult(() => repo.GetCallTypesAsync(year), "GetCallTypes");

    [HttpGet]
    public async Task<IActionResult> GetIntCallCount(int year, string callType = "") =>
        await ChartResult(() => repo.GetIntCallCountAsync(year, callType), "GetIntCallCount");

    [HttpGet]
    public async Task<IActionResult> GetCountryGrid(int year) =>
        await ChartResult(() => repo.GetCountryGridAsync(year), "GetCountryGrid");

    [HttpGet]
    public async Task<IActionResult> GetCountryGridMonthly(int year, int month) =>
        await ChartResult(() => repo.GetCountryGridMonthlyAsync(year, month), "GetCountryGridMonthly");

    // â”€â”€ Helper â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    private async Task<IActionResult> ChartResult(Func<Task<IEnumerable<dynamic>>> fetch, string name)
    {
        try
        {
            var rows = await fetch();
            var data = rows
                .Select(r => ((IDictionary<string, object>)r)
                    .ToDictionary(kv => kv.Key, kv => kv.Value))
                .ToList();
            return Json(new { data });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Endpoint} failed", name);
            return Json(new { data = Array.Empty<object>() });
        }
    }
}

