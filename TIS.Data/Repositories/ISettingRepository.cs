using TIS.Data.Models.Setting;

namespace TIS.Data.Repositories;

public interface ISettingRepository
{
    // ── Config ────────────────────────────────────────────────────────────
    Task<ConfigDto?> GetConfigAsync();
    Task SaveConfigAsync(ConfigDto cfg);

    // ── Policy ────────────────────────────────────────────────────────────
    Task<PolicyPageData> GetPolicyPageDataAsync();
    Task<IEnumerable<PolicyDto>> GetPoliciesAsync();
    Task<int> AddPolicyAsync(int providerId, string transType, string description, int callTypeId, int lineTypeId, bool isAll, bool isSupImp);
    Task UpdatePolicyAsync(int id, bool isAll, bool isSupImp);
    Task AddPolicyDetailAsync(int manageCallTypeId, int uid, int subNoId);
    Task DeletePolicyAsync(int id);
    Task<IEnumerable<PolicyDetailDto>> GetPolicyDetailAsync(int id);
    Task<IEnumerable<TransTypeDto>> GetTransTypesAsync(int providerId);
    Task<IEnumerable<DescriptionDto>> GetDescriptionsAsync(int providerId, string transType);
    Task ApplyPolicyAsync();

    // ── Provider ──────────────────────────────────────────────────────────
    Task<IEnumerable<ProviderDto>> GetProvidersAsync(int roleId, int countryId);
    Task AddProviderAsync(ProviderDto p);
    Task UpdateProviderAsync(ProviderDto p);
    Task DeleteProviderAsync(int id);
}

public class PolicyPageData
{
    public IEnumerable<ProviderDto>     Providers  { get; set; } = [];
    public IEnumerable<CallTypeListDto> CallTypes  { get; set; } = [];
    public IEnumerable<EmpSubDto>       Employees  { get; set; } = [];
    public IEnumerable<LineTypeDto>     LineTypes  { get; set; } = [];
}
