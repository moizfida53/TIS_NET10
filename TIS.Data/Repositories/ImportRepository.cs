using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TIS.Data.Infrastructure;
using TIS.Data.Models.Import;

namespace TIS.Data.Repositories;

public class ImportRepository(ISpRunner sp, string connectionString) : IImportRepository
{
    public Task<(IEnumerable<UploadHistoryDto> History, bool ShowDeleteButton)> GetUploadHistoryAsync(int roleId, int countryId) =>
        sp.QueryMultipleAsync("sp_GetUploadHistory", new { RoleID = roleId, CountryID = countryId }, async grid =>
        {
            var history  = await grid.ReadAsync<UploadHistoryDto>();
            bool showDel = false;
            if (!grid.IsConsumed)
            {
                if (!grid.IsConsumed)
            {
                var cfg = await grid.ReadFirstOrDefaultAsync<dynamic>();
#pragma warning disable CS8602
                showDel = cfg != null && (bool)(cfg.DeleteBut ?? false);
#pragma warning restore CS8602
            }
            }
            return (history, showDel);
        });

    public Task<ImportNullResult> GetImportNullRowsAsync() =>
        sp.QueryMultipleAsync("sp_GetImportNullRows", null, async grid =>
        {
            var rows   = await grid.ReadAsync<ImportRowDto>();
            var totRow = await grid.ReadFirstOrDefaultAsync<dynamic>();
            double total = totRow != null ? (double)totRow.TotalAmount : 0;
            return new ImportNullResult { NullRows = rows, TotalAmount = total };
        });

    public Task UpdateImportRowAsync(int id, double amount, string subNo, DateOnly callDate) =>
        sp.ExecuteAsync("sp_UpdateImport", new { ID = id, Amount = amount, SubNo = subNo, CallDate = callDate });

    public Task<ProviderSettingDto?> GetProviderSettingAsync(int providerId) =>
        sp.QuerySingleOrDefaultAsync<ProviderSettingDto>("sp_GetProviderSetting", new { ProviderID = providerId });

    public Task SaveProviderSettingAsync(int providerId, ProviderSettingDto s, bool dbBased)
    {
        string spName = dbBased ? "sp_UploadDBSetting" : "sp_UploadSetting";
        return sp.ExecuteAsync(spName, new
        {
            Col1 = s.Col1, Col2 = s.Col2, Col3 = s.Col3, Col4 = s.Col4,
            Col5 = s.Col5, Col6 = s.Col6, Col7 = s.Col7, Col8 = s.Col8,
            Provider    = providerId,
            dbConstr    = s.DbConstr ?? "",
            dbTableName = s.DbTableName ?? ""
        });
    }

    public Task<BillDetailDto?> InsertUploadHistoryAsync(string fileName, DateTime billDate, int providerId, string status, string userId) =>
        sp.QuerySingleOrDefaultAsync<BillDetailDto>("sp_InsertUploadHistoryAndAudit", new
        {
            UploadFileName = fileName,
            BillDate       = billDate,
            Provider       = providerId,
            Status         = status,
            UserID         = userId
        });

    public async Task<int> ImportInvoiceAsync(int providerId, DateTime billDate)
    {
        await using var conn = new SqlConnection(connectionString);
        var p = new DynamicParameters();
        p.Add("@PROVIDER", providerId, DbType.Int32);
        p.Add("@BILLDATE", billDate, DbType.DateTime);
        p.Add("@retval", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await conn.ExecuteAsync("sp_ImportInvoice", p, commandType: CommandType.StoredProcedure);
        return p.Get<int>("@retval");
    }

    public Task ClearPreviousImportAsync(int providerId, DateTime billDate, string fileName) =>
        sp.ExecuteAsync("sp_ClearPreviousImport", new { Provider = providerId, BillDate = billDate, FileName = fileName });

    public Task DeleteBillAsync(int providerId, DateTime billDate) =>
        sp.ExecuteAsync("sp_DeleteBill", new { Provider = providerId, BillDate = billDate });

    public Task<IEnumerable<UnassignedBillDto>> GetUnassignedBillsAsync(int countryId, int roleId) =>
        sp.QueryAsync<UnassignedBillDto>("sp_GetUnassignedBills", new { CountryID = countryId, RoleID = roleId });

    public async Task<int> AssignInvoiceAsync()
    {
        await using var conn = new SqlConnection(connectionString);
        var p = new DynamicParameters();
        p.Add("@retval", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await conn.ExecuteAsync("sp_AssignInvoice", p, commandType: CommandType.StoredProcedure);
        return p.Get<int>("@retval");
    }
}
