using TIS.Data.Models.MarsaApi;

namespace TIS.Data.Repositories;

public interface IMarsaApiRepository
{
    Task<int?> GetBillCountByUsernameAsync(string userName);
    Task<int?> GetPendingApprovalCountByUsernameAsync(string userName);
    Task<IEnumerable<ApiBillDetailsDto>> GetBillDetailsByBillIdAsync(int billId);
    Task<string> UpdateBillByBillIdAsync(int billId, int status, string comments);
    Task<IEnumerable<ApiApprovalBillDto>> GetApprovalBillListAsync(string username);
}
