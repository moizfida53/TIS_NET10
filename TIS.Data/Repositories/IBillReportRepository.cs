using TIS.Data.Models.BillReport;

namespace TIS.Data.Repositories;

public interface IBillReportRepository
{
    Task<IEnumerable<BillReportStatusDto>> GetBillReportStatusesAsync(bool isStatus);
    Task<IEnumerable<BillReportCompanyDto>> GetCompaniesAsync();
    Task<IEnumerable<BillReportRowDto>> SearchBillReportAsync(int month, int year, int status, int companyId);

    Task<ReportFilterDto> GetReportFiltersAsync(bool isStatus);

    // sp_SearchPendingBills must exist in DB:
    //   SELECT BILL_ID, SUB_NO, EMPLOYEENAME, BILLDATE, TOTALAMOUNT, LMEmail
    //   FROM vwPendingBills
    //   WHERE 1=1
    //   AND (@Month=0   OR MONTH(billdate)=@Month)
    //   AND (@Year=0    OR YEAR(billdate)=@Year)
    //   AND (@Provider=0 OR provider=@Provider)
    //   AND (@Status=0  OR (@Status=1 AND status!=4) OR (@Status=4 AND status=4))
    Task<IEnumerable<PendingBillRowDto>> SearchPendingBillsAsync(int month, int year, int provider, int status);

    Task<IEnumerable<ReportChartDto>> GetReportChartAsync();
}
