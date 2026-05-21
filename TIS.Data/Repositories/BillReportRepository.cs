using TIS.Data.Infrastructure;
using TIS.Data.Models.BillReport;

namespace TIS.Data.Repositories;

public class BillReportRepository(ISpRunner sp) : IBillReportRepository
{
    public Task<IEnumerable<BillReportStatusDto>> GetBillReportStatusesAsync(bool isStatus) =>
        sp.QueryAsync<BillReportStatusDto>("sp_BillReport", new { IsStatus = isStatus });

    public Task<IEnumerable<BillReportCompanyDto>> GetCompaniesAsync() =>
        sp.QueryAsync<BillReportCompanyDto>("sp_GetSalesReportFilterData");

    public Task<IEnumerable<BillReportRowDto>> SearchBillReportAsync(
        int month, int year, int status, int companyId) =>
        sp.QueryAsync<BillReportRowDto>("sp_SearchBillReport",
            new { Month = month, Year = year, Status = status, CompanyId = companyId });

    public Task<ReportFilterDto> GetReportFiltersAsync(bool isStatus) =>
        sp.QueryMultipleAsync("sp_Report", new { IsStatus = isStatus }, async grid =>
        {
            var providers = await grid.ReadAsync<ReportProviderDto>();
            IEnumerable<ReportStatusDto> statuses = [];
            if (!grid.IsConsumed)
                statuses = await grid.ReadAsync<ReportStatusDto>();
            return new ReportFilterDto { ProviderList = providers, StatusList = statuses };
        });

    public Task<IEnumerable<PendingBillRowDto>> SearchPendingBillsAsync(
        int month, int year, int provider, int status) =>
        sp.QueryAsync<PendingBillRowDto>("sp_SearchPendingBills",
            new { Month = month, Year = year, Provider = provider, Status = status });

    public Task<IEnumerable<ReportChartDto>> GetReportChartAsync() =>
        sp.QueryAsync<ReportChartDto>("sp_ReportChart");
}
