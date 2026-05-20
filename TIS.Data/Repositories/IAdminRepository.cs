using TIS.Data.Models.Admin;

namespace TIS.Data.Repositories;

public interface IAdminRepository
{
    // ── Employee ──────────────────────────────────────────────────────────
    Task<EmployeeDto?> GetEmployeeByUsernameAsync(string username);
    Task<EmployeeInitData> GetEmployeePageDataAsync(string username);
    Task AddEmployeeAsync(EmployeeDto emp, int countryId, int userUid, string companyId);
    Task UpdateEmployeeAsync(EmployeeDto emp, int countryId, int userUid, string companyId);
    Task DeleteEmployeeAsync(int uid, int userUid);

    // ── Cost Center ───────────────────────────────────────────────────────
    Task<IEnumerable<CostCenterDto>> GetCostCentersAsync(int countryId, int roleId);
    Task ManageCostCenterAsync(int command, CostCenterDto cc);

    // ── Country ───────────────────────────────────────────────────────────
    Task ManageCountryAsync(int command, CountryDto country);

    // ── Manager ───────────────────────────────────────────────────────────
    Task ManageManagerAsync(int command, int? uid, string name, string employeeNo);

    // ── Telephone ─────────────────────────────────────────────────────────
    Task<TelephonePageData> GetTelephonePageDataAsync(int countryId, int roleId);
    Task<IEnumerable<TelephoneDto>> GetUnassignedNumbersAsync(int countryId);
    Task<IEnumerable<AssignmentDto>> GetAssignedNumbersAsync(int countryId);
    Task<IEnumerable<ProviderDto>> GetProvidersAsync(int countryId, int roleId);
    Task AddTelephoneAsync(TelephoneDto tel);
    Task UpdateTelephoneAsync(TelephoneDto tel);
    Task DeleteTelephoneAsync(int subNoId);
    Task AssignNumberAsync(AssignmentDto assignment);
    Task UpdateAssignmentAsync(AssignmentDto assignment);
    Task DeleteAssignmentAsync(int assignId);

    // ── Delegation ────────────────────────────────────────────────────────
    Task<IEnumerable<DelegationDto>> GetDelegationsAsync(int countryId);
    Task ManageDelegationAsync(int command, DelegationDto dlg);

    // ── Package ───────────────────────────────────────────────────────────
    Task<IEnumerable<PackageMasterDto>> GetPackagesAsync(int countryId);
    Task<IEnumerable<PackageDetailDto>> GetPackageDetailAsync(int pkgId);
    Task<IEnumerable<ProviderDto>> GetPackageProvidersAsync(int countryId);
    Task AddPackageAsync(PackageMasterDto master, IEnumerable<PackageDetailDto> details);
    Task UpdatePackageAsync(PackageMasterDto master, IEnumerable<PackageDetailDto> details);
    Task DeletePackageAsync(int pkgId);

    // ── Package lookups ───────────────────────────────────────────────────
    Task<IEnumerable<CallTypeDto>> GetPkgCallTypesAsync(int providerId);
    Task<IEnumerable<CallDescDto>> GetPkgCallDescAsync(int callTypeId);

    // ── Data Roaming ──────────────────────────────────────────────────────
    Task<IEnumerable<DataRoamingDto>> GetDataRoamingAsync(int countryId);
    Task AddDataRoamingAsync(DataRoamingDto dto);
    Task UpdateDataRoamingAsync(DataRoamingDto dto);
    Task DeleteDataRoamingAsync(int roamingId);

    // ── Delegation helpers ────────────────────────────────────────────────
    Task<IEnumerable<DelegationDto>> GetSecretariesAsync();

    // ── Contact ───────────────────────────────────────────────────────────
    Task SaveContactAsync(ContactDto contact);

    // ── Manager list ──────────────────────────────────────────────────────
    Task<IEnumerable<ManagerDto>> GetManagersAsync();
}
