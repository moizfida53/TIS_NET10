using TIS.Data.Models.Bill;

namespace TIS.Data.Repositories;

public interface IBillRepository
{
    Task<IEnumerable<ForceBillRowDto>> GetForceBillsAsync(int roleId, int countryId);

    // Uses tmp_bill_ids staging + sp_ForceBill — parameterized, no SQL injection
    Task ForceBillsAsync(int[] billIds, int status, int callType,
        bool wavRental, bool wavBusiness, bool train, int uid);

    Task<SearchDataDto> GetSearchDataAsync(bool isStatus, int countryId, int roleId);

    Task<IEnumerable<BillSearchRowDto>> SearchChangeStatusAsync(
        int month, int year, int uid, int status, int provider);

    Task ChangeStatusToOpenAsync(int billId);

    Task<IEnumerable<BillSearchRowDto>> SearchReassignAsync(
        int month, int year, int uid, int provider);

    Task ReassignBillAsync(int billId, int uid);

    // sp_SearchReimburseBill replaces SearchCloseBill inline SQL
    Task<IEnumerable<BillSearchRowDto>> SearchReimburseAsync(
        int month, int year, int uid, int provider);

    // sp_ReimburseBill replaces ReimbursingBill inline SQL
    Task ReimburseBillAsync(int billId);
}
