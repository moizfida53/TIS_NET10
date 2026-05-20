using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TIS.Data.Infrastructure;
using TIS.Data.Models.Bill;

namespace TIS.Data.Repositories;

public class BillRepository(ISpRunner sp, string connectionString) : IBillRepository
{
    public Task<IEnumerable<ForceBillRowDto>> GetForceBillsAsync(int roleId, int countryId) =>
        sp.QueryAsync<ForceBillRowDto>("sp_GetForceBills",
            new { RoleId = roleId, CountryId = countryId });

    public async Task ForceBillsAsync(int[] billIds, int status, int callType,
        bool wavRental, bool wavBusiness, bool train, int uid)
    {
        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        // Parameterized clear + insert avoids SQL injection on tmp_bill_ids
        await conn.ExecuteAsync("DELETE FROM tmp_bill_ids");
        foreach (var id in billIds)
            await conn.ExecuteAsync("INSERT INTO tmp_bill_ids VALUES (@id)", new { id });

        await conn.ExecuteAsync("sp_ForceBill", new
        {
            Status     = status,
            CallType   = callType,
            chkWavRtl  = wavRental,
            chkWavBus  = wavBusiness,
            chkTrain   = train,
            UID        = uid
        }, commandType: CommandType.StoredProcedure);
    }

    public Task<SearchDataDto> GetSearchDataAsync(bool isStatus, int countryId, int roleId) =>
        sp.QueryMultipleAsync("sp_SearchBill",
            new { IsStatus = isStatus, CountryID = countryId, RoleID = roleId },
            async grid =>
            {
                var emps      = await grid.ReadAsync<SearchEmpDto>();
                var providers = await grid.ReadAsync<SearchProviderDto>();
                IEnumerable<SearchStatusDto> statuses = [];
                if (!grid.IsConsumed)
                    statuses = await grid.ReadAsync<SearchStatusDto>();
                return new SearchDataDto
                {
                    EmpList      = emps,
                    ProviderList = providers,
                    StatusList   = statuses
                };
            });

    public Task<IEnumerable<BillSearchRowDto>> SearchChangeStatusAsync(
        int month, int year, int uid, int status, int provider) =>
        sp.QueryAsync<BillSearchRowDto>("SP_ChangeBillStatus_Search",
            new { Month = month, Year = year, UID = uid, Status = status, Provider = provider });

    public Task ChangeStatusToOpenAsync(int billId) =>
        sp.ExecuteAsync("SP_ChangeBillStatus_Update", new { Bill_ID = billId });

    public Task<IEnumerable<BillSearchRowDto>> SearchReassignAsync(
        int month, int year, int uid, int provider) =>
        sp.QueryAsync<BillSearchRowDto>("sp_ReAssignBill_Search",
            new { Month = month, Year = year, UID = uid, Provider = provider });

    public Task ReassignBillAsync(int billId, int uid) =>
        sp.ExecuteAsync("sp_ReAssignBill_Save", new { Bill_ID = billId, Uid = uid });

    // sp_SearchReimburseBill must exist in DB:
    //   SELECT BILL_ID,BILLDATE,SUB_NO,EMPLOYEENAME,Appr_Manager,TOTALAMOUNT,STATUSNAME,STATUS,UID
    //   FROM vwPendingBills_new WHERE status=4
    //   AND (@Month=0 OR MONTH(billdate)=@Month)
    //   AND (@Year=0  OR YEAR(billdate)=@Year)
    //   AND (@UID=0   OR UID=@UID)
    //   AND (@Provider=0 OR provider=@Provider)
    public Task<IEnumerable<BillSearchRowDto>> SearchReimburseAsync(
        int month, int year, int uid, int provider) =>
        sp.QueryAsync<BillSearchRowDto>("sp_SearchReimburseBill",
            new { Month = month, Year = year, UID = uid, Provider = provider });

    // sp_ReimburseBill must exist in DB:
    //   UPDATE tblBills SET
    //     ReimbursementAmount=DeductibleAmount+WaiverAmount,
    //     DeductibleAmount=DeductibleAmount+WaiverAmount,
    //     Status=1, WaiverRejection=NULL, WaiverAmount=0
    //   WHERE Bill_ID=@BillId
    public Task ReimburseBillAsync(int billId) =>
        sp.ExecuteAsync("sp_ReimburseBill", new { BillId = billId });
}
