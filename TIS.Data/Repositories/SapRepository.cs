using TIS.Data.Infrastructure;
using TIS.Data.Models.Sap;

namespace TIS.Data.Repositories;

public class SapRepository(ISpRunner sp) : ISapRepository
{
    public Task<SapReportResultDto> GetSapReportAsync()
        => sp.QueryMultipleAsync("sp_GetSAPReport", null, async grid =>
        {
            var rows    = (await grid.ReadAsync<dynamic>()).ToList();
            var counts  = (await grid.ReadAsync<dynamic>()).ToList();
            string pending = counts.Count > 0
                ? ((IDictionary<string, object>)counts[0]).Values.FirstOrDefault()?.ToString() ?? "0"
                : "0";

            var bills = rows.Select(r =>
            {
                var d = (IDictionary<string, object>)r;
                return new SapReportRowDto
                {
                    BillId          = d.TryGetValue("Bill_ID", out var bid) ? bid?.ToString() ?? "" : "",
                    BillDate        = d.TryGetValue("BILLDATE", out var bd)  ? bd?.ToString()  ?? "" : "",
                    TelephoneNumber = d.TryGetValue("SUB_NO", out var sn)    ? sn?.ToString()  ?? "" : "",
                    EmployeeNo      = d.TryGetValue("EMPLOYEENO", out var en) ? en?.ToString() ?? "" : "",
                    EmployeeName    = d.TryGetValue("EMPLOYEENAME", out var enm) ? enm?.ToString() ?? "" : "",
                    ManagerName     = d.TryGetValue("LINEMANAGER", out var lm) ? lm?.ToString() ?? "" : "",
                    TotalAmount     = d.TryGetValue("TOTALAMOUNT", out var ta) ? ta?.ToString() ?? "" : "",
                    BusinessCharges = d.TryGetValue("BussCharged", out var bc) ? bc?.ToString() ?? "" : "",
                    PersonalCharges = d.TryGetValue("PERSONALCHARGES", out var pc) ? pc?.ToString() ?? "" : "",
                    DeductibleAmount = d.TryGetValue("DEDUCTIBLEAMOUNT", out var da) ? da?.ToString() ?? "" : "",
                    CostCenterName  = d.TryGetValue("CostCenterName", out var ccn) ? ccn?.ToString() ?? "" : "",
                    CostCenterCode  = d.TryGetValue("CostCenterCode", out var ccc) ? ccc?.ToString() ?? "" : "",
                    Department      = d.TryGetValue("Department", out var dept) ? dept?.ToString() ?? "" : ""
                };
            });

            return new SapReportResultDto { Bills = bills, PendingCount = pending };
        });

    public Task MarkAsPostedAsync(string username, int billId, decimal deductibleAmount)
        => sp.ExecuteAsync("sp_SAP_MarkAsPosted",
            new { Username = username, Bill_ID = billId, DeductibleAmount = deductibleAmount });

    public Task LogExceptionAsync(string exception, string functionName)
        => sp.ExecuteAsync("sp_Exception",
            new { Exception = exception, FunctionName = functionName });
}
