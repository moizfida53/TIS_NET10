using TIS.Data.Models.Sap;

namespace TIS.Data.Repositories;

public interface ISapRepository
{
    Task<SapReportResultDto> GetSapReportAsync();
    Task MarkAsPostedAsync(string username, int billId, decimal deductibleAmount);
    Task LogExceptionAsync(string exception, string functionName);
}
