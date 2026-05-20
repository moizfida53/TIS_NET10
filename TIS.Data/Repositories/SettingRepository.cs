using TIS.Data.Infrastructure;
using TIS.Data.Models.Setting;

namespace TIS.Data.Repositories;

public class SettingRepository(ISpRunner sp) : ISettingRepository
{
    // ── Config ────────────────────────────────────────────────────────────

    public Task<ConfigDto?> GetConfigAsync() =>
        sp.QuerySingleOrDefaultAsync<ConfigDto>("sp_GetConfig");

    public Task SaveConfigAsync(ConfigDto c) =>
        sp.ExecuteAsync("sp_SaveConfig", new
        {
            EmpReminder       = c.EmpReminder,
            FBReminder        = c.ForceBillReminder,
            MgrReminder       = c.MgrComplaintReminder,
            LMReminder        = c.LMReminder,
            SMTP              = c.SMTPSettings,
            AdminEmail        = c.AdminEmail,
            HostUrl           = c.HostUrl,
            SuperGrade        = c.SuperGrade,
            EnableGrade       = c.EnableGrade,
            DntSndEmail       = c.NotSendMail,
            HidePerCalls      = c.HidePersonalCalls,
            GMApp             = c.SkipGMApproval,
            EnableDiscrepancy = c.EnableDiscrepancy,
            SkipAppBusZero    = c.SkipApprovalBuss,
            DedBusCharges     = c.DedBussinessCharges,
            ZeroUnlimited     = c.BusinessZeroAsUnlimited,
            AlwWav            = c.AllowWaiver,
            EnableDelete      = c.DeleteBut,
            AlwTrainFB        = c.AllowTrainForceBill,
            HideAllowanceLimit = c.HideAllowanceLimit,
            HidePersonalLimit  = c.HidePersonalLimit
        });

    // ── Policy ────────────────────────────────────────────────────────────

    public async Task<PolicyPageData> GetPolicyPageDataAsync()
    {
        var data = await sp.QueryMultipleAsync("sp_GetPolicyData", null, async grid =>
        {
            var providers = await grid.ReadAsync<ProviderDto>();
            var callTypes = await grid.ReadAsync<CallTypeListDto>();
            var emps      = await grid.ReadAsync<EmpSubDto>();
            return new PolicyPageData { Providers = providers, CallTypes = callTypes, Employees = emps };
        });
        data.LineTypes = await sp.QueryAsync<LineTypeDto>("sp_GetLineTypes");
        return data;
    }

    public Task<IEnumerable<PolicyDto>> GetPoliciesAsync() =>
        sp.QueryAsync<PolicyDto>("sp_GetPolicies");

    public async Task<int> AddPolicyAsync(int providerId, string transType, string description,
        int callTypeId, int lineTypeId, bool isAll, bool isSupImp)
    {
        var result = await sp.QuerySingleOrDefaultAsync<int?>("sp_AddPolicy", new
        {
            ProviderID  = providerId,
            TransType   = transType,
            Description = description,
            CallTypeID  = callTypeId,
            LineTypeID  = lineTypeId,
            IsAll       = isAll,
            IsSupImp    = isSupImp
        });
        return result ?? 0;
    }

    public Task UpdatePolicyAsync(int id, bool isAll, bool isSupImp) =>
        sp.ExecuteAsync("sp_UpdatePolicy", new { ID = id, IsAll = isAll, IsSupImp = isSupImp });

    public Task AddPolicyDetailAsync(int manageCallTypeId, int uid, int subNoId) =>
        sp.ExecuteAsync("sp_AddPolicyDetail", new { ManageCallTypeID = manageCallTypeId, UID = uid, SubNoID = subNoId });

    public Task DeletePolicyAsync(int id) =>
        sp.ExecuteAsync("sp_DeletePolicy", new { ID = id });

    public Task<IEnumerable<PolicyDetailDto>> GetPolicyDetailAsync(int id) =>
        sp.QueryAsync<PolicyDetailDto>("sp_GetPolicyDetail", new { ID = id });

    public Task<IEnumerable<TransTypeDto>> GetTransTypesAsync(int providerId) =>
        sp.QueryAsync<TransTypeDto>("sp_GetTransTypes", new { ProviderID = providerId });

    public Task<IEnumerable<DescriptionDto>> GetDescriptionsAsync(int providerId, string transType) =>
        sp.QueryAsync<DescriptionDto>("sp_GetDescriptions", new { ProviderID = providerId, TransType = transType });

    public Task ApplyPolicyAsync() =>
        sp.ExecuteAsync("sp_ApplyPolicy");

    // ── Provider ──────────────────────────────────────────────────────────

    public Task<IEnumerable<ProviderDto>> GetProvidersAsync(int roleId, int countryId) =>
        sp.QueryAsync<ProviderDto>("sp_GetProvider", new { RoleID = roleId, CountryID = countryId });

    public Task AddProviderAsync(ProviderDto p) =>
        sp.ExecuteAsync("sp_AddProvider", new { Name = p.Name, IsVoip = p.IsVoip, CountryID = p.CountryID == 0 ? (int?)null : p.CountryID });

    public Task UpdateProviderAsync(ProviderDto p) =>
        sp.ExecuteAsync("sp_UpdateProvider", new { ID = p.ID, Name = p.Name, IsVoip = p.IsVoip, CountryID = p.CountryID == 0 ? (int?)null : p.CountryID });

    public Task DeleteProviderAsync(int id) =>
        sp.ExecuteAsync("DeleteProvider", new { Provider = id });
}
