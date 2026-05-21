using TIS.Data.Infrastructure;
using TIS.Data.Models.Dashboard;

namespace TIS.Data.Repositories;

public class DashboardRepository(ISpRunner sp) : IDashboardRepository
{
    // sp_GetDashboardData returns 8 result sets:
    //   [0-3] single-value scalars: unidentifiedBills, unassignedAmount, billsInApproval, sapAmount
    //   [4-7] chart data tables (not used here — fetched via dedicated chart endpoints)
    public Task<DashboardKpiDto> GetKpiAsync() =>
        sp.QueryMultipleAsync("sp_GetDashboardData", null, async grid =>
        {
            string Val(IEnumerable<dynamic> rows) =>
                ((IDictionary<string, object>?)rows.FirstOrDefault())?.Values.FirstOrDefault()?.ToString() ?? "0";

            var kpi1 = Val(await grid.ReadAsync<dynamic>());
            var kpi2 = Val(await grid.ReadAsync<dynamic>());
            var kpi3 = Val(await grid.ReadAsync<dynamic>());
            var kpi4 = Val(await grid.ReadAsync<dynamic>());

            return new DashboardKpiDto
            {
                UnidentifiedBills = kpi1,
                UnassignedAmount  = kpi2,
                BillsInApproval   = kpi3,
                SapAmount         = kpi4
            };
        });

    public Task<IEnumerable<dynamic>> GetChart1Async(int year) =>
        sp.QueryAsync<dynamic>("sp_GetDashboardChart1", new { Year = year });

    public Task<IEnumerable<dynamic>> GetChart2Async(int year, string transType) =>
        sp.QueryAsync<dynamic>("sp_GetDashboardChart2", new { Year = year, TRANS_TYPE = transType });

    public Task<IEnumerable<dynamic>> GetChart3Async(int year, int month) =>
        sp.QueryAsync<dynamic>("sp_GetDashboardChart3", new { Year = year, Month = month });

    public Task<IEnumerable<dynamic>> GetChart4Async(int year) =>
        sp.QueryAsync<dynamic>("sp_GetDashboardChart4", new { Year = year });

    public Task<IEnumerable<dynamic>> GetChart5Async(int year, string callTypeName) =>
        sp.QueryAsync<dynamic>("sp_GetDashboardChart5", new { Year = year, calltypename = callTypeName });

    public Task<IEnumerable<dynamic>> GetChart6Async(int year, string callType) =>
        sp.QueryAsync<dynamic>("sp_GetDashboardChart6", new { Year = year, Call_type = callType });

    public Task<IEnumerable<dynamic>> GetChart7Async(int year, string callType, string outCountry) =>
        sp.QueryAsync<dynamic>("sp_GetDashboardChart7", new { Year = year, Call_type = callType, OUT_COUNTRY = outCountry });

    // sp names for the Chartdata model methods — adjust if actual SP names differ
    public Task<IEnumerable<dynamic>> GetTransTypesAsync() =>
        sp.QueryAsync<dynamic>("sp_GetTransTypes");

    public Task<IEnumerable<dynamic>> GetCallTypesAsync(int year) =>
        sp.QueryAsync<dynamic>("sp_GetCallTypes", new { Year = year });

    public Task<IEnumerable<dynamic>> GetIntCallCountAsync(int year, string callType) =>
        sp.QueryAsync<dynamic>("sp_GetINTCallCount", new { Year = year, Call_type = callType });

    public Task<IEnumerable<dynamic>> GetCountryGridAsync(int year) =>
        sp.QueryAsync<dynamic>("sp_GetCountryGrid", new { Year = year });

    public Task<IEnumerable<dynamic>> GetCountryGridMonthlyAsync(int year, int month) =>
        sp.QueryAsync<dynamic>("sp_GetCountryGridMonthly", new { Year = year, Month = month });

    private static List<Dictionary<string, object?>> ToRows(IEnumerable<dynamic> rows) =>
        rows.Select(r => ((IDictionary<string, object>)r)
                .ToDictionary(kv => kv.Key, kv => (object?)kv.Value))
            .ToList();
}
