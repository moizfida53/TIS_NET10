using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIS.Data.Repositories;
using TIS.Web.Models.BillReport;

namespace TIS.Web.Controllers;

[Authorize(Roles = "Administrator,SuperAdmin")]
public class BillReportController(
    IBillReportRepository repo,
    ILogger<BillReportController> logger) : TisController
{
    // â”€â”€ Views â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    public IActionResult Index()     => View("BillReport");
    public IActionResult BillReport() => View(nameof(BillReport));
    public IActionResult Report()    => View(nameof(Report));

    // â”€â”€ Bill Report â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public async Task<IActionResult> GetBillReportFilters()
    {
        try
        {
            var statuses   = await repo.GetBillReportStatusesAsync(true);
            var companies  = await repo.GetCompaniesAsync();
            return Json(new { dtStatus = statuses, CompanyList = companies });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetBillReportFilters failed");
            return Json(new { Fail = true });
        }
    }

    [HttpPost]
    public async Task<IActionResult> SearchBillReport([FromBody] BillReportSearchRequest req)
    {
        try
        {
            var rows = await repo.SearchBillReportAsync(req.Month, req.Year, req.Status, req.CompanyId);
            return Json(new { dtData = rows });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SearchBillReport failed");
            return Json(new { success = false, message = ex.Message });
        }
    }

    // â”€â”€ Pending Bills Report â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public async Task<IActionResult> GetReportFilters()
    {
        try
        {
            var filters = await repo.GetReportFiltersAsync(true);
            return Json(new { ProviderList = filters.ProviderList, dtStatus = filters.StatusList });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetReportFilters failed");
            return Json(new { Fail = true });
        }
    }

    [HttpPost]
    public async Task<IActionResult> SearchPendingBills([FromBody] PendingBillSearchRequest req)
    {
        try
        {
            var rows = await repo.SearchPendingBillsAsync(req.Month, req.Year, req.Provider, req.Status);
            return Json(new { dtData = rows });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SearchPendingBills failed");
            return Json(new { success = false, message = ex.Message });
        }
    }

    // â”€â”€ Report Chart â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public async Task<IActionResult> GetReportChart()
    {
        try
        {
            var data = await repo.GetReportChartAsync();
            return Json(new { dtReportChart = data });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetReportChart failed");
            return Json(new { Fail = true });
        }
    }
}

