using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TIS.Data.Repositories;
using TIS.Web.Models.Bill;

namespace TIS.Web.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class BillController(
    IBillRepository repo,
    ILogger<BillController> logger) : Controller
{
    private int EmpId     => int.Parse(User.FindFirstValue("EmpId")     ?? "0");
    private int EmpRoleId => int.Parse(User.FindFirstValue("EmpRoleId") ?? "0");
    private int CountryId => int.Parse(User.FindFirstValue("CountryId") ?? "0");

    // ── Views ─────────────────────────────────────────────────────────────

    public IActionResult Index()         => View("ForceBill");
    public IActionResult ChangeStatus()  => View(nameof(ChangeStatus));
    public IActionResult ReAssignBill()  => View(nameof(ReAssignBill));
    public IActionResult ReImburseBill() => View(nameof(ReImburseBill));

    // ── Force Bill ────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> GetForceBill()
    {
        try
        {
            var bills = await repo.GetForceBillsAsync(EmpRoleId, CountryId);
            return Json(new { Bills = bills });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetForceBill failed");
            return Json(new { Fail = true, Message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ForceBill([FromBody] ForceBillRequest req)
    {
        try
        {
            if (req.BillID.Length == 0)
                return Json(new { Success = false, Message = "No bills selected." });

            await repo.ForceBillsAsync(req.BillID, req.Status, req.CallType,
                req.WavRental, req.WavBusiness, req.Train, EmpId);

            return Json(new { Success = true, Message = "Bills Successfully Processed" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ForceBill failed");
            return Json(new { Success = false, Message = "Failed to process bills." });
        }
    }

    // ── Shared search data ────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> GetSearchData(bool isStatus)
    {
        try
        {
            var data = await repo.GetSearchDataAsync(isStatus, CountryId, EmpRoleId);
            return Json(new
            {
                EmpList      = data.EmpList,
                ProviderList = data.ProviderList,
                dtStatus     = data.StatusList
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetSearchData failed");
            return Json(new { Fail = true });
        }
    }

    // ── Change Status ─────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> Search([FromBody] BillSearchRequest req)
    {
        try
        {
            var bills = await repo.SearchChangeStatusAsync(req.Month, req.Year, req.UID, req.Status, req.Provider);
            return Json(new { dtData = bills });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Search failed");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ChangeStatus(int billId)
    {
        try
        {
            await repo.ChangeStatusToOpenAsync(billId);
            return Json(new { success = true, message = "Status changed to Open successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ChangeStatus failed for bill {BillId}", billId);
            return Json(new { success = false, message = ex.Message });
        }
    }

    // ── Re-Assign Bill ────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> SearchOpenBill([FromBody] BillSearchRequest req)
    {
        try
        {
            var bills = await repo.SearchReassignAsync(req.Month, req.Year, req.UID, req.Provider);
            return Json(new { dtData = bills });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SearchOpenBill failed");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ReAssignBill_Save([FromBody] ReassignBillRequest req)
    {
        try
        {
            await repo.ReassignBillAsync(req.BillId, req.Uid);
            return Json(new { success = true, message = "Bill Re-Assigned Successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ReAssignBill_Save failed");
            return Json(new { success = false, message = ex.Message });
        }
    }

    // ── Reimburse Bill ────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> SearchCloseBill([FromBody] BillSearchRequest req)
    {
        try
        {
            var bills = await repo.SearchReimburseAsync(req.Month, req.Year, req.UID, req.Provider);
            return Json(new { dtData = bills });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SearchCloseBill failed");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ReimbursingBill([FromBody] ReimburseBillRequest req)
    {
        try
        {
            foreach (var id in req.BillID)
                await repo.ReimburseBillAsync(id);
            return Json(new { Message = "Success" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ReimbursingBill failed");
            return Json(new { Message = "Fail" });
        }
    }
}
