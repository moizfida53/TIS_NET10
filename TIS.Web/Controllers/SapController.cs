using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIS.Data.Repositories;
using TIS.Web.Models.Sap;

namespace TIS.Web.Controllers;

[Authorize(Policy = "Admin")]
public class SapController(ISapRepository repo, ILogger<SapController> logger) : TisController
{
    // â”€â”€ SAP Pending â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    public IActionResult SapPending() => View();

    [HttpGet]
    public async Task<IActionResult> GetSapReport()
    {
        try
        {
            var result = await repo.GetSapReportAsync();
            return Ok(new { dtbillDetails = result.Bills, PendingBills = result.PendingCount });
        }
        catch (Exception ex)
        {
            await LogAsync(nameof(GetSapReport), ex);
            return Ok(new { dtbillDetails = Array.Empty<object>(), PendingBills = "0" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsPosted([FromBody] MarkAsPostedRequest req)
    {
        try
        {
            string username = User.Identity?.Name ?? "";
            foreach (var row in req.Value)
            {
                int.TryParse(row.BillId, out int billId);
                decimal.TryParse(row.DeductibleAmount, out decimal ded);
                await repo.MarkAsPostedAsync(username, billId, ded);
            }
            return Ok(new { Success = true });
        }
        catch (Exception ex)
        {
            await LogAsync(nameof(MarkAsPosted), ex);
            return Ok(new { Success = false });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult PostSap()
    {
        // SAP NCo (SAPMobile proxy) is not compatible with .NET 10.
        // BAPI posting requires the TIS.SapBridge shim â€” not yet implemented.
        return Ok(new { Message = "SAP posting is not available in this version. Please use the legacy system for BAPI export." });
    }

    // â”€â”€ BAPI Sync (stub â€” SAP NCo not available on .NET 10) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    public IActionResult SyncBapi() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SyncBapiRun()
        => Ok(new { Message = "SAP BAPI sync requires the SAP NCo connector which is not compatible with .NET 10. This feature is not available." });

    // â”€â”€ Helpers â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    private async Task LogAsync(string functionName, Exception ex)
    {
        logger.LogError(ex, "SAP error in {Function}", functionName);
        try { await repo.LogExceptionAsync(ex.ToString(), functionName); }
        catch { /* don't let audit logging mask the original error */ }
    }
}

