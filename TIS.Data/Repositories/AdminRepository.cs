using TIS.Data.Infrastructure;
using TIS.Data.Models.Admin;

namespace TIS.Data.Repositories;

public class AdminRepository(ISpRunner sp) : IAdminRepository
{
    // ── Employee ──────────────────────────────────────────────────────────

    public async Task<EmployeeDto?> GetEmployeeByUsernameAsync(string username)
    {
        var data = await GetEmployeePageDataAsync(username);
        return data.Employees.FirstOrDefault();
    }

    public Task<EmployeeInitData> GetEmployeePageDataAsync(string username) =>
        sp.QueryMultipleAsync("sp_GetEmployee", new { Username = username }, async grid =>
        {
            var employees   = await grid.ReadAsync<EmployeeDto>();
            var roles       = await grid.ReadAsync<RoleDto>();
            var countries   = await grid.ReadAsync<CountryDto>();
            var costCenters = await grid.ReadAsync<CostCenterDto>();
            return new EmployeeInitData
            {
                Employees   = employees,
                Roles       = roles,
                Countries   = countries,
                CostCenters = costCenters
            };
        });

    public Task AddEmployeeAsync(EmployeeDto emp, int countryId, int userUid, string companyId) =>
        sp.ExecuteAsync("sp_AddEmployee", new
        {
            NAME        = emp.NAME,
            EMPLOYEENO  = emp.EMPLOYEENO,
            CCNO        = emp.CCNO,
            EMAIL       = emp.EMAIL,
            USERNAME    = emp.USERNAME,
            ORG         = emp.ORG,
            DESCRIPTION = emp.DESCRIPTION,
            MANAGERID   = emp.MANAGERID,
            GRADE       = emp.GRADE,
            EXTENSION   = emp.EXTENSION,
            PAYROLL     = emp.PAYROLL,
            ROLEID      = emp.ROLEID,
            COUNTRYID   = countryId,
            Count       = 0,
            UserUid     = userUid,
            CompanyID   = companyId,
            IsActive    = emp.IsActive
        });

    public Task UpdateEmployeeAsync(EmployeeDto emp, int countryId, int userUid, string companyId) =>
        sp.ExecuteAsync("sp_UpdateEmployee", new
        {
            UID         = emp.UID,
            NAME        = emp.NAME,
            EMPLOYEENO  = emp.EMPLOYEENO,
            CCNO        = emp.CCNO,
            EMAIL       = emp.EMAIL,
            USERNAME    = emp.USERNAME,
            ORG         = emp.ORG,
            DESCRIPTION = emp.DESCRIPTION,
            MANAGERID   = emp.MANAGERID,
            GRADE       = emp.GRADE,
            EXTENSION   = emp.EXTENSION,
            PAYROLL     = emp.PAYROLL,
            ROLEID      = emp.ROLEID,
            COUNTRYID   = countryId,
            Count       = 0,
            UserUid     = userUid,
            CompanyID   = companyId,
            IsActive    = emp.IsActive
        });

    public Task DeleteEmployeeAsync(int uid, int userUid) =>
        sp.ExecuteAsync("sp_DeleteEmployee", new { UID = uid, UserUid = userUid });

    // ── Cost Center ───────────────────────────────────────────────────────

    public async Task<IEnumerable<CostCenterDto>> GetCostCentersAsync(int countryId, int roleId)
    {
        var ds = await sp.QueryMultipleAsync("sp_GetCC", new { CountryID = countryId, RoleID = roleId },
            async grid =>
            {
                var cc = await grid.ReadAsync<CostCenterDto>();
                return cc;
            });
        return ds;
    }

    public Task ManageCostCenterAsync(int command, CostCenterDto cc) =>
        sp.ExecuteAsync("sp_ManageCC", new
        {
            Command     = command,
            UID         = cc.UID,
            CCName      = cc.CCName,
            CCNum       = cc.CCNum,
            COUNTRYID   = cc.COUNTRYID
        });

    // ── Country ───────────────────────────────────────────────────────────

    public Task ManageCountryAsync(int command, CountryDto country) =>
        sp.ExecuteAsync("sp_ManageCountry", new
        {
            Command      = command,
            CountryID    = country.COUNTRYID,
            CountryName  = country.COUNTRYNAME,
            CountryCode  = country.COUNTRYCODE,
            ShayaCode    = country.SHAYACODE,
            ExchangeRate = country.EXCHANGERATE,
            Currency     = country.CURRENCY
        });

    // ── Manager ───────────────────────────────────────────────────────────

    public Task ManageManagerAsync(int command, int? uid, string name, string employeeNo) =>
        sp.ExecuteAsync("sp_ManageManager", new { Command = command, UID = uid, Name = name, EmployeeNo = employeeNo });

    // ── Telephone ─────────────────────────────────────────────────────────

    public Task<TelephonePageData> GetTelephonePageDataAsync(int countryId, int roleId) =>
        sp.QueryMultipleAsync("sp_GetTelData", new { CountryID = countryId, RoleID = roleId }, async grid =>
        {
            var tels         = await grid.ReadAsync<TelephoneDto>();
            var assignments  = await grid.ReadAsync<AssignmentDto>();
            var providers    = await grid.ReadAsync<ProviderDto>();
            var employees    = await grid.ReadAsync<EmployeeDto>();
            return new TelephonePageData { Telephones = tels, Assignments = assignments, Providers = providers, Employees = employees };
        });

    public async Task<IEnumerable<TelephoneDto>> GetUnassignedNumbersAsync(int countryId)
    {
        return await sp.QueryMultipleAsync("sp_GetNumber", new { Command = 1, CountryID = countryId },
            async grid => await grid.ReadAsync<TelephoneDto>());
    }

    public async Task<IEnumerable<AssignmentDto>> GetAssignedNumbersAsync(int countryId)
    {
        return await sp.QueryMultipleAsync("sp_GetNumber", new { Command = 2, CountryID = countryId },
            async grid => await grid.ReadAsync<AssignmentDto>());
    }

    public async Task<IEnumerable<ProviderDto>> GetProvidersAsync(int countryId, int roleId)
    {
        return await sp.QueryMultipleAsync("sp_GetProvider", new { CountryID = countryId, RoleID = roleId },
            async grid => await grid.ReadAsync<ProviderDto>());
    }

    public Task AddTelephoneAsync(TelephoneDto tel) =>
        sp.ExecuteAsync("sp_AddTelephone", new
        {
            SubNo       = tel.SubNo,
            Description = tel.Description,
            ProviderID  = tel.ProviderID,
            AccountNo   = tel.AccountNo,
            LineType    = tel.LineType,
            CountryID   = tel.CountryID
        });

    public Task UpdateTelephoneAsync(TelephoneDto tel) =>
        sp.ExecuteAsync("sp_UpdateTelephone", new
        {
            SubNoId     = tel.SubNoId,
            SubNo       = tel.SubNo,
            Description = tel.Description,
            ProviderID  = tel.ProviderID,
            AccountNo   = tel.AccountNo,
            LineType    = tel.LineType
        });

    public Task DeleteTelephoneAsync(int subNoId) =>
        sp.ExecuteAsync("sp_DeleteTelephone", new { SubNoId = subNoId });

    public Task AssignNumberAsync(AssignmentDto a) =>
        sp.ExecuteAsync("sp_AssignNumber", new
        {
            UID           = a.UID,
            SubNoId       = a.SubNoId,
            StartDate     = a.StartDate,
            EndDate       = a.EndDate,
            BusinessLimit = a.BusinessLimit,
            AllowanceLimit= a.AllowanceLimit,
            LineStatus    = a.LineStatus,
            CostCenterID  = a.CostCenterID
        });

    public Task UpdateAssignmentAsync(AssignmentDto a) =>
        sp.ExecuteAsync("sp_UpdateAssignNumber", new
        {
            AssignID      = a.AssignID,
            UID           = a.UID,
            SubNoId       = a.SubNoId,
            StartDate     = a.StartDate,
            EndDate       = a.EndDate,
            BusinessLimit = a.BusinessLimit,
            AllowanceLimit= a.AllowanceLimit,
            LineStatus    = a.LineStatus,
            CostCenterID  = a.CostCenterID
        });

    public Task DeleteAssignmentAsync(int assignId) =>
        sp.ExecuteAsync("sp_DeleteAssignNumber", new { AssignID = assignId });

    // ── Delegation ────────────────────────────────────────────────────────

    public async Task<IEnumerable<DelegationDto>> GetDelegationsAsync(int countryId)
    {
        return await sp.QueryMultipleAsync("sp_GetDelegate", new { Command = 1, CountryID = countryId },
            async grid => await grid.ReadAsync<DelegationDto>());
    }

    public Task ManageDelegationAsync(int command, DelegationDto dlg) =>
        sp.ExecuteAsync("sp_ManageDelegate", new
        {
            Command     = command,
            DelegateID  = dlg.DelegateID,
            ManagerID   = dlg.ManagerID,
            SecretaryID = dlg.SecretaryID,
            CanIdentify = dlg.CanIdentify,
            CanApprove  = dlg.CanApprove,
            StartDate   = dlg.StartDate,
            EndDate     = dlg.EndDate
        });

    // ── Package ───────────────────────────────────────────────────────────

    public async Task<IEnumerable<PackageMasterDto>> GetPackagesAsync(int countryId)
    {
        return await sp.QueryMultipleAsync("sp_GetPkgData", new { CountryID = countryId },
            async grid => await grid.ReadAsync<PackageMasterDto>());
    }

    public async Task<IEnumerable<PackageDetailDto>> GetPackageDetailAsync(int pkgId)
    {
        return await sp.QueryMultipleAsync("sp_GetPkgDetail", new { ID = pkgId },
            async grid => await grid.ReadAsync<PackageDetailDto>());
    }

    public async Task<IEnumerable<ProviderDto>> GetPackageProvidersAsync(int countryId)
    {
        return await sp.QueryMultipleAsync("sp_GetProvider", new { CountryID = countryId, RoleID = 8 },
            async grid => await grid.ReadAsync<ProviderDto>());
    }

    public async Task AddPackageAsync(PackageMasterDto master, IEnumerable<PackageDetailDto> details)
    {
        foreach (var d in details)
            await sp.ExecuteAsync("sp_AddPackage", new
            {
                ProviderID        = master.ProviderID,
                TransType         = d.TransType,
                Description       = d.Description,
                MakeAllUnexpected = master.MakeAllUnexpected,
                ExpectedType      = d.ExpectedType,
                AmountLimit       = d.AmountLimit,
                StartDate         = master.StartDate,
                CountryID         = master.CountryID
            });
    }

    public async Task UpdatePackageAsync(PackageMasterDto master, IEnumerable<PackageDetailDto> details)
    {
        foreach (var d in details)
            await sp.ExecuteAsync("sp_UpdatePackage", new
            {
                PkgID             = master.PkgID,
                DetailID          = d.DetailID,
                MakeAllUnexpected = master.MakeAllUnexpected,
                ExpectedType      = d.ExpectedType,
                AmountLimit       = d.AmountLimit,
                StartDate         = master.StartDate
            });
    }

    public Task DeletePackageAsync(int pkgId) =>
        sp.ExecuteAsync("sp_DeletePackage", new { ID = pkgId });

    // ── Data Roaming ──────────────────────────────────────────────────────

    public async Task<IEnumerable<DataRoamingDto>> GetDataRoamingAsync(int countryId)
    {
        return await sp.QueryMultipleAsync("sp_GetDataRoaming", new { CountryID = countryId },
            async grid => await grid.ReadAsync<DataRoamingDto>());
    }

    public Task AddDataRoamingAsync(DataRoamingDto dto) =>
        sp.ExecuteAsync("sp_AddDataRoaming", new
        {
            CountryName  = dto.CountryName,
            OperatorName = dto.OperatorName,
            ProviderID   = dto.ProviderID,
            PkgID        = dto.PkgID
        });

    public Task UpdateDataRoamingAsync(DataRoamingDto dto) =>
        sp.ExecuteAsync("sp_UpdateDataRoaming", new
        {
            RoamingID    = dto.RoamingID,
            CountryName  = dto.CountryName,
            OperatorName = dto.OperatorName
        });

    public Task DeleteDataRoamingAsync(int roamingId) =>
        sp.ExecuteAsync("sp_DeleteDataRoaming", new { RoamingID = roamingId });

    // ── Package lookups ───────────────────────────────────────────────────

    public async Task<IEnumerable<CallTypeDto>> GetPkgCallTypesAsync(int providerId) =>
        await sp.QueryAsync<CallTypeDto>("sp_GetPkgCallTypes", new { ProviderID = providerId });

    public async Task<IEnumerable<CallDescDto>> GetPkgCallDescAsync(int callTypeId) =>
        await sp.QueryAsync<CallDescDto>("sp_GetPkgCallDesc", new { CallTypeID = callTypeId });

    // ── Delegation helpers ────────────────────────────────────────────────

    public async Task<IEnumerable<DelegationDto>> GetSecretariesAsync() =>
        await sp.QueryMultipleAsync("sp_GetDelegate", new { Command = 2 },
            async grid => await grid.ReadAsync<DelegationDto>());

    // ── Contact ───────────────────────────────────────────────────────────

    public Task SaveContactAsync(ContactDto contact) =>
        sp.ExecuteAsync("sp_SaveContact", new
        {
            Uid       = contact.Uid,
            DialledNo = contact.DialledNo,
            Name      = contact.Name,
            ExName    = contact.ExName
        });

    // ── Manager list ──────────────────────────────────────────────────────

    public async Task<IEnumerable<ManagerDto>> GetManagersAsync() =>
        await sp.QueryAsync<ManagerDto>("sp_GetManagers");
}
