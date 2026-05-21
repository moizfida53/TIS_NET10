using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIS.Data.Models.Admin;
using TIS.Data.Repositories;
using TIS.Web.Models.Admin;

namespace TIS.Web.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class AdminController(IAdminRepository repo, ILogger<AdminController> logger) : TisController
{
    // â”€â”€ Claim helpers â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    private int EmpId     => int.Parse(User.FindFirstValue("EmpId")     ?? "0");
    private int EmpRoleId => int.Parse(User.FindFirstValue("EmpRoleId") ?? "0");
    private int CountryId => int.Parse(User.FindFirstValue("CountryId") ?? "0");
    private string EmpLoginAs => User.FindFirstValue("EmpLoginAs") ?? "";

    // â”€â”€ Pages â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [Authorize(Roles = "Administrator,SuperAdmin")]
    public IActionResult Index()
    {
        // Administrators (RoleID=3) are finance-type admins; route them to reports
        if (EmpRoleId == 3) return Redirect("/BillReport/BillReport");
        return View("ManageEmployee");
    }

    public IActionResult Telephone() => View("AddTelephone");
    public IActionResult Delegate()  => View("DelegateBills");
    public IActionResult Package()   => View();

    // â”€â”€ Employee â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public async Task<JsonResult> GetUser()
    {
        try
        {
            var data = await repo.GetEmployeePageDataAsync(EmpLoginAs);
            return Json(new
            {
                dtEmp       = data.Employees,
                RoleList    = data.Roles,
                CountryList = data.Countries,
                dtCC        = data.CostCenters
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetUser failed for {User}", EmpLoginAs);
            return Json(new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> AddEmployee([FromBody] EmployeeRequest req)
    {
        try
        {
            if (req.CountryIds.Length == 0)
                return Json(new { myMessage = "No country selected." });

            foreach (var cid in req.CountryIds)
                await repo.AddEmployeeAsync(req.Employee, cid, EmpId, req.Employee.COMPANYID);

            return Json(new { myMessage = "succ" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AddEmployee failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> UpdateEmployee([FromBody] EmployeeRequest req)
    {
        try
        {
            if (req.CountryIds.Length == 0)
                return Json(new { myMessage = "No country selected." });

            foreach (var cid in req.CountryIds)
                await repo.UpdateEmployeeAsync(req.Employee, cid, EmpId, req.Employee.COMPANYID);

            return Json(new { myMessage = "succ" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UpdateEmployee failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> DeleteEmployee(int uid)
    {
        try
        {
            await repo.DeleteEmployeeAsync(uid, EmpId);
            return Json(new { myMessage = "succ" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeleteEmployee {Uid} failed", uid);
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpGet]
    public async Task<JsonResult> GetManagers()
    {
        try
        {
            var managers = await repo.GetManagersAsync();
            return Json(new { dtManager = managers });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetManagers failed");
            return Json(new { error = ex.Message });
        }
    }

    // â”€â”€ Cost Center â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public async Task<JsonResult> GetCC()
    {
        try
        {
            var cc = await repo.GetCostCentersAsync(CountryId, EmpRoleId);
            return Json(new { dtCC = cc });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetCC failed");
            return Json(new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> AddCC([FromBody] CostCenterDto cc)
    {
        try
        {
            await repo.ManageCostCenterAsync(1, cc);
            return Json(new { myMessage = "Added Successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AddCC failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> UpdateCC([FromBody] CostCenterDto cc)
    {
        try
        {
            await repo.ManageCostCenterAsync(2, cc);
            return Json(new { myMessage = "Updated Successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UpdateCC failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> DeleteCC(int uid)
    {
        try
        {
            await repo.ManageCostCenterAsync(3, new CostCenterDto { UID = uid });
            return Json(new { myMessage = "Deleted Successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeleteCC {Uid} failed", uid);
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    // â”€â”€ Country â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpPost]
    public async Task<JsonResult> AddCountry([FromBody] CountryDto country)
    {
        try
        {
            await repo.ManageCountryAsync(1, country);
            return Json(new { Message = "Success" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AddCountry failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> UpdateCountry([FromBody] CountryDto country)
    {
        try
        {
            await repo.ManageCountryAsync(2, country);
            return Json(new { Message = "Success" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UpdateCountry failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> DeleteCountry(int countryId)
    {
        try
        {
            await repo.ManageCountryAsync(3, new CountryDto { COUNTRYID = countryId });
            return Json(new { Message = "Success" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeleteCountry {Id} failed", countryId);
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    // â”€â”€ Manager â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpPost]
    public async Task<JsonResult> AddManager([FromBody] ManagerRequest req)
    {
        try
        {
            await repo.ManageManagerAsync(1, null, req.Name, req.EmployeeNo);
            return Json(new { Message = "Success" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AddManager failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> UpdateManager([FromBody] ManagerRequest req)
    {
        try
        {
            await repo.ManageManagerAsync(2, req.Uid, req.Name, req.EmployeeNo);
            return Json(new { Message = "Success" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UpdateManager failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> DeleteManager(int uid)
    {
        try
        {
            await repo.ManageManagerAsync(3, uid, "", "");
            return Json(new { Message = "Success" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeleteManager {Uid} failed", uid);
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    // â”€â”€ Telephone â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public async Task<JsonResult> GetTelData()
    {
        try
        {
            var data = await repo.GetTelephonePageDataAsync(CountryId, EmpRoleId);
            return Json(new
            {
                dtTel      = data.Telephones,
                dtAsg      = data.Assignments,
                dtProvider = data.Providers,
                dtEmp      = data.Employees
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetTelData failed");
            return Json(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<JsonResult> GetTelNo()
    {
        try
        {
            var tels = await repo.GetUnassignedNumbersAsync(CountryId);
            return Json(new { dtTel = tels });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetTelNo failed");
            return Json(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<JsonResult> GetAsgNo()
    {
        try
        {
            var assignments = await repo.GetAssignedNumbersAsync(CountryId);
            return Json(new { dtAsg = assignments });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetAsgNo failed");
            return Json(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<JsonResult> GetProvider()
    {
        try
        {
            var providers = await repo.GetProvidersAsync(CountryId, EmpRoleId);
            return Json(new { ProviderList = providers });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetProvider failed");
            return Json(new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> AddTelephone([FromBody] TelephoneDto tel)
    {
        try
        {
            await repo.AddTelephoneAsync(tel);
            return Json(new { myMessage = "succ" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AddTelephone failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> UpdateTelephone([FromBody] TelephoneDto tel)
    {
        try
        {
            await repo.UpdateTelephoneAsync(tel);
            return Json(new { myMessage = "succ" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UpdateTelephone failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> DeleteTelephone(int id)
    {
        try
        {
            await repo.DeleteTelephoneAsync(id);
            return Json(new { myMessage = "succ" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeleteTelephone {Id} failed", id);
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> Assign([FromBody] AssignmentDto a)
    {
        try
        {
            await repo.AssignNumberAsync(a);
            return Json(new { myMessage = "succ" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Assign failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> UpdateAssign([FromBody] AssignmentDto a)
    {
        try
        {
            await repo.UpdateAssignmentAsync(a);
            return Json(new { myMessage = "succ" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UpdateAssign failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> DeleteAssign(int id)
    {
        try
        {
            await repo.DeleteAssignmentAsync(id);
            return Json(new { myMessage = "succ" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeleteAssign {Id} failed", id);
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    // â”€â”€ Delegation â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public async Task<JsonResult> GetDelegate()
    {
        try
        {
            var dlgs = await repo.GetDelegationsAsync(CountryId);
            // Employee list for the manager/secretary pickers â€” reuse GetUser data
            var empData = await repo.GetEmployeePageDataAsync(EmpLoginAs);
            return Json(new
            {
                dtSec   = dlgs,
                EmpList = empData.Employees
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetDelegate failed");
            return Json(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<JsonResult> GetSecretary()
    {
        try
        {
            var secs = await repo.GetSecretariesAsync();
            return Json(new { dtSec = secs });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetSecretary failed");
            return Json(new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> SaveDelegate([FromBody] DelegationDto dlg)
    {
        try
        {
            await repo.ManageDelegationAsync(1, dlg);
            return Json(new { myMessage = "Added Successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SaveDelegate failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> UpdateDelegate([FromBody] DelegationDto dlg)
    {
        try
        {
            await repo.ManageDelegationAsync(2, dlg);
            return Json(new { myMessage = "Updated Successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UpdateDelegate failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> DeleteDelegate(int id)
    {
        try
        {
            await repo.ManageDelegationAsync(3, new DelegationDto { DelegateID = id });
            return Json(new { myMessage = "Deleted Successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeleteDelegate {Id} failed", id);
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    // â”€â”€ Package â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public async Task<JsonResult> GetPkgData()
    {
        try
        {
            var providers = await repo.GetPackageProvidersAsync(CountryId);
            var packages  = await repo.GetPackagesAsync(CountryId);
            return Json(new { dtPro = providers, dtPkg = packages });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetPkgData failed");
            return Json(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<JsonResult> GetPackage()
    {
        try
        {
            var packages = await repo.GetPackagesAsync(CountryId);
            return Json(new { dtPkg = packages });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetPackage failed");
            return Json(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<JsonResult> FillTransType(int providerId)
    {
        try
        {
            var types = await repo.GetPkgCallTypesAsync(providerId);
            return Json(new { dtTransType = types });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "FillTransType {ProviderId} failed", providerId);
            return Json(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<JsonResult> FillDesc(int callTypeId)
    {
        try
        {
            var descs = await repo.GetPkgCallDescAsync(callTypeId);
            return Json(new { dtdesc = descs });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "FillDesc {CallTypeId} failed", callTypeId);
            return Json(new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> AddPackage([FromBody] PackageRequest req)
    {
        try
        {
            await repo.AddPackageAsync(req.Master, req.Details);
            return Json(new { myMessage = "Added Successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AddPackage failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> UpdatePackage([FromBody] PackageRequest req)
    {
        try
        {
            await repo.UpdatePackageAsync(req.Master, req.Details);
            return Json(new { myMessage = "Updated Successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UpdatePackage failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> DeletePackage(int id)
    {
        try
        {
            await repo.DeletePackageAsync(id);
            return Json(new { Message = "Deleted Successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeletePackage {Id} failed", id);
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpGet]
    public async Task<JsonResult> GetPkgDetail(int pkgId)
    {
        try
        {
            var detail = await repo.GetPackageDetailAsync(pkgId);
            return Json(new { PkgDetail = detail });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetPkgDetail {PkgId} failed", pkgId);
            return Json(new { error = ex.Message });
        }
    }

    // â”€â”€ Contact â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpPost]
    public async Task<JsonResult> SaveContact([FromBody] ContactDto contact)
    {
        try
        {
            await repo.SaveContactAsync(contact);
            return Json(new
            {
                Message = contact.ExName != null
                    ? "Contact Updated Successfully"
                    : "Contact Saved Successfully"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SaveContact failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    // â”€â”€ Data Roaming â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    [HttpGet]
    public async Task<JsonResult> GetDataRoaming()
    {
        try
        {
            var roaming = await repo.GetDataRoamingAsync(CountryId);
            return Json(new { dtCountry = roaming });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetDataRoaming failed");
            return Json(new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> AddDataRoaming([FromBody] DataRoamingDto dto)
    {
        try
        {
            await repo.AddDataRoamingAsync(dto);
            return Json(new { myMessage = "Success" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AddDataRoaming failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> UpdateDataRoaming([FromBody] DataRoamingDto dto)
    {
        try
        {
            await repo.UpdateDataRoamingAsync(dto);
            return Json(new { myMessage = "Success" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UpdateDataRoaming failed");
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> DeleteDataRoaming(int id)
    {
        try
        {
            await repo.DeleteDataRoamingAsync(id);
            return Json(new { myMessage = "Success" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeleteDataRoaming {Id} failed", id);
            return Json(new { myMessage = "Error: " + ex.Message });
        }
    }
}

