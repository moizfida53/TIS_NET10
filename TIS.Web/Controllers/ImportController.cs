using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TIS.Data.Models.Import;
using TIS.Data.Repositories;
using TIS.Data.Services;
using TIS.Web.Models.Import;

namespace TIS.Web.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class ImportController(
    IImportRepository repo,
    ImportService importSvc,
    IWebHostEnvironment env,
    ILogger<ImportController> logger) : TisController
{
    private int  EmpId     => int.Parse(User.FindFirstValue("EmpId")     ?? "0");
    private int  EmpRoleId => int.Parse(User.FindFirstValue("EmpRoleId") ?? "0");
    private int  CountryId => int.Parse(User.FindFirstValue("CountryId") ?? "0");

    private string BillsFolder => Path.Combine(env.ContentRootPath, "Bills");

    // â”€â”€ Views â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    public IActionResult Index()    => View("ImportInvoice");
    public IActionResult UnAssigned() => View("UnAssignedInvoice");

    // â”€â”€ Upload History â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public async Task<IActionResult> GetUploadHistory()
    {
        try
        {
            var (history, showDel) = await repo.GetUploadHistoryAsync(EmpRoleId, CountryId);
            return Json(new { UploadList = history, IsDeleteButShow = showDel ? 1 : 0 });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetUploadHistory failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    // â”€â”€ File Upload (save to disk) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile[] fileToUpload)
    {
        Directory.CreateDirectory(BillsFolder);
        foreach (var f in fileToUpload)
        {
            var path = Path.Combine(BillsFolder, Path.GetFileName(f.FileName));
            await using var stream = System.IO.File.Create(path);
            await f.CopyToAsync(stream);
        }
        return View("ImportInvoice");
    }

    // â”€â”€ Sheet Names â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public IActionResult FillSheet(string fileName)
    {
        try
        {
            string filePath = Path.Combine(BillsFolder, Path.GetFileName(fileName));
            var sheets = ImportService.GetSheetNames(filePath)
                         .Select(s => new { SheetName = s });
            return Json(new { dtSheet = sheets });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "FillSheet failed for {File}", fileName);
            return Json(new { Message = "Fail", dtSheet = "" });
        }
    }

    // â”€â”€ Column Names (for mapping screen) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public IActionResult UploadSetting(string fileName, string sheetName)
    {
        try
        {
            string filePath = Path.Combine(BillsFolder, Path.GetFileName(fileName));
            var cols = ImportService.GetColumnNames(filePath, sheetName)
                       .Select(c => new { Cols = c });
            return Json(new { Message = "Success", dtCol = cols });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UploadSetting failed");
            return Json(new { Message = "Fail" });
        }
    }

    // â”€â”€ Process Import â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpPost]
    public async Task<IActionResult> UploadFile([FromBody] UploadFileRequest req)
    {
        try
        {
            var billDate = new DateTime(req.Year, req.Month, 1).AddMonths(1).AddDays(-1);
            string filePath = Path.Combine(BillsFolder, Path.GetFileName(req.FileName));

            ImportNullResult result;
            if (req.DbBased)
            {
                var setting = await repo.GetProviderSettingAsync(req.ProviderID);
                if (setting == null) return Json(new { MyMessage = "Provider not found" });

                if (!OperatingSystem.IsWindows())
                    return Json(new { MyMessage = "DB-based import is only supported on Windows." });

                result = await importSvc.ImportDbAsync(
                    setting.DbTableName ?? "", setting.DbConstr ?? "",
                    req.Month, req.Year, req.ProviderID, billDate);
            }
            else
            {
                result = await importSvc.ImportFileAsync(filePath, req.SheetName, req.ProviderID, billDate);
            }

            return Json(new
            {
                MyMessage  = "Success",
                BillAmount = result.TotalAmount,
                GridData   = result.NullRows
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UploadFile failed");
            return Json(new { MyMessage = "Error: " + ex.Message });
        }
    }

    // â”€â”€ Commit Import â†’ Call Records â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpPost]
    public async Task<IActionResult> ProcessBill([FromBody] ProcessBillRequest req)
    {
        try
        {
            var billDate = DateTime.Parse(req.BillDate);
            int result   = await repo.ImportInvoiceAsync(req.ProviderID, billDate);

            if (result == 0)
            {
                await repo.ClearPreviousImportAsync(req.ProviderID, billDate, req.FileName);
                return Json(new { Message = "Fail" });
            }

            var details = await repo.InsertUploadHistoryAsync(
                req.FileName, billDate, req.ProviderID, "Success", EmpId.ToString());

            return Json(new { Message = "succ", BillDetails = BuildBillDetails(details) });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ProcessBill failed");
            return Json(new { Message = "Fail" });
        }
    }

    // â”€â”€ Update a null import row (fix SQL injection) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpPost]
    public async Task<IActionResult> UpdateImport([FromBody] UpdateImportRequest req)
    {
        try
        {
            await repo.UpdateImportRowAsync(req.ID, req.Amount, req.SubNo, DateOnly.Parse(req.CallDate));
            var result = await repo.GetImportNullRowsAsync();
            return Json(new { Message = "Success", dtImp = result.NullRows });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UpdateImport failed");
            return Json(new { Message = "Fail" });
        }
    }

    // â”€â”€ Provider column mapping â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public async Task<IActionResult> GetSetting(int provider)
    {
        try
        {
            var s = await repo.GetProviderSettingAsync(provider);
            if (s == null) return Json(new { Message = "Fail" });
            return s.DbBased
                ? Json(new { dtDBCol = s })
                : Json(new { dtCol   = s });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetSetting failed");
            return Json(new { Message = "Fail" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSetting([FromBody] ColumnSettingRequest req)
    {
        try
        {
            bool dbBased = !string.IsNullOrWhiteSpace(req.DbConstr);
            await repo.SaveProviderSettingAsync(req.Provider, req.ToDto(), dbBased);
            return Json(new { Message = "Settings Updated Successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UpdateSetting failed");
            return Json(new { Message = "Fail" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> CheckProvider(int provider)
    {
        var s = await repo.GetProviderSettingAsync(provider);
        return Json(new { DbBased = s?.DbBased.ToString() ?? "False" });
    }

    // â”€â”€ Delete Bill â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpPost]
    public async Task<IActionResult> DeleteBill([FromBody] DeleteBillRequest req)
    {
        try
        {
            await repo.DeleteBillAsync(req.ProviderID, req.BillDate);
            return Json(new { myMessage = "succ" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeleteBill failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    // â”€â”€ Unassigned Bills â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public async Task<IActionResult> GetUnAssignedBill()
    {
        try
        {
            var bills = await repo.GetUnassignedBillsAsync(CountryId, EmpRoleId);
            return Json(new { Bills = bills });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetUnAssignedBill failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AssignInvoice()
    {
        int count = await repo.AssignInvoiceAsync();
        return Json(new { Message = $"{count} Bills Generated" });
    }

    // â”€â”€ Private helper â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    private static object BuildBillDetails(BillDetailDto? d) => d == null
        ? new
        {
            BilledButNotInSystem           = new { CountOfBills = 0, TotalAmount = 0.0 },
            InSystemButNotAssigned         = new { CountOfBills = 0, TotalAmount = 0.0 },
            AssignedButOutsideValidDates   = new { CountOfBills = 0, TotalAmount = 0.0 }
        }
        : new
        {
            BilledButNotInSystem         = new { CountOfBills = d.BilledButNotInSystem_Count,         TotalAmount = d.BilledButNotInSystem_Amount },
            InSystemButNotAssigned       = new { CountOfBills = d.InSystemButNotAssigned_Count,       TotalAmount = d.InSystemButNotAssigned_Amount },
            AssignedButOutsideValidDates = new { CountOfBills = d.AssignedButOutsideValidDates_Count, TotalAmount = d.AssignedButOutsideValidDates_Amount }
        };
}

