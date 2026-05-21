using TIS.Data.Infrastructure;
using TIS.Data.Models.MarsaApi;

namespace TIS.Data.Repositories;

public class MarsaApiRepository(ISpRunner sp) : IMarsaApiRepository
{
    public async Task<int?> GetBillCountByUsernameAsync(string userName)
    {
        var row = await sp.QuerySingleOrDefaultAsync<dynamic>(
            "API_GetCountofBillsByUserName", new { userName });
        if (row is null) return null;
        var d = (IDictionary<string, object>)row;
        return d.TryGetValue("CountOfBill", out var v) ? Convert.ToInt32(v) : (int?)null;
    }

    public async Task<int?> GetPendingApprovalCountByUsernameAsync(string userName)
    {
        var row = await sp.QuerySingleOrDefaultAsync<dynamic>(
            "API_GetPendingApprovalCountByUserName", new { userName });
        if (row is null) return null;
        var d = (IDictionary<string, object>)row;
        return d.TryGetValue("CountOfBill", out var v) ? Convert.ToInt32(v) : (int?)null;
    }

    public async Task<IEnumerable<ApiBillDetailsDto>> GetBillDetailsByBillIdAsync(int billId)
    {
        var rows = await sp.QueryAsync<dynamic>("API_GetBillDetailsByBillId", new { billId });
        return rows.Select(r =>
        {
            var d = (IDictionary<string, object>)r;
            return new ApiBillDetailsDto
            {
                BillId           = Convert.ToInt32(d.TryGetValue("BILL_ID", out var bid) ? bid : 0),
                BillDate         = d.TryGetValue("BILLDATE", out var bd) ? bd?.ToString() ?? "" : "",
                SubNo            = d.TryGetValue("SUB_NO", out var sn)   ? sn?.ToString()  ?? "" : "",
                Name             = d.TryGetValue("NAME", out var nm)     ? nm?.ToString()  ?? "" : "",
                Org              = d.TryGetValue("ORG", out var og)      ? og?.ToString()  ?? "" : "",
                TotalAmount      = d.TryGetValue("TOTALAMOUNT", out var ta)     ? Convert.ToDecimal(ta)     : 0,
                BusinessLimit    = d.TryGetValue("BUSSINESSLIMIT", out var bl)  ? Convert.ToDecimal(bl)     : 0,
                BusinessCharges  = d.TryGetValue("BUSINESSCHARGES", out var bc) ? Convert.ToDecimal(bc)     : 0,
                DeductibleAmount = d.TryGetValue("DEDUCTIBLEAMOUNT", out var da)? Convert.ToDecimal(da)     : 0,
                WaiverAmount     = d.TryGetValue("WAIVERAMOUNT", out var wa)    ? Convert.ToDecimal(wa)     : 0,
                Comments         = d.TryGetValue("COMMENTS", out var cm)        ? cm?.ToString() ?? ""      : ""
            };
        });
    }

    public async Task<string> UpdateBillByBillIdAsync(int billId, int status, string comments)
    {
        var row = await sp.QuerySingleOrDefaultAsync<dynamic>(
            "API_UpdateBillByBillId", new { billId, status, comments });
        if (row is null) return "Not Updated";
        var d = (IDictionary<string, object>)row;
        return d.TryGetValue("Status", out var v) ? v?.ToString() ?? "Not Updated" : "Not Updated";
    }

    public async Task<IEnumerable<ApiApprovalBillDto>> GetApprovalBillListAsync(string username)
    {
        var rows = await sp.QueryAsync<dynamic>("API_ApprovalBillList", new { Username = username });
        return rows.Select(r =>
        {
            var d = (IDictionary<string, object>)r;
            return new ApiApprovalBillDto
            {
                BillId              = Convert.ToInt32(d.TryGetValue("BillID", out var bid) ? bid : 0),
                BillAmount          = d.TryGetValue("BillAmount", out var ba)          ? Convert.ToDecimal(ba)  : 0,
                BillDate            = d.TryGetValue("BillDate", out var bd)            ? bd?.ToString() ?? ""   : "",
                EmployeeName        = d.TryGetValue("EmployeeName", out var en)        ? en?.ToString() ?? ""   : "",
                MobileNumber        = d.TryGetValue("MobileNumber", out var mn)        ? mn?.ToString() ?? ""   : "",
                BusinessCharges     = d.TryGetValue("BusinessCharges", out var bc)     ? Convert.ToDecimal(bc)  : 0,
                PersonalCharges     = d.TryGetValue("PersonalCharges", out var pc)     ? Convert.ToDecimal(pc)  : 0,
                WaiverAmount        = d.TryGetValue("WaiverAmount", out var wa)        ? Convert.ToDecimal(wa)  : 0,
                WaiverComment       = d.TryGetValue("WaiverComment", out var wc)       ? wc?.ToString() ?? ""   : "",
                BusinessLimitCharges= d.TryGetValue("BussinessLimitCharges", out var blc) ? Convert.ToDecimal(blc) : 0,
                Department          = d.TryGetValue("Department", out var dept)        ? dept?.ToString() ?? "" : "",
                BusinessLimit       = d.TryGetValue("BussinessLimit", out var bl)      ? Convert.ToDecimal(bl)  : 0,
                DeductibleAmount    = d.TryGetValue("DeductibleAmount", out var da)    ? Convert.ToDecimal(da)  : 0
            };
        });
    }
}
