using TIS.Data.Models.Import;

namespace TIS.Data.Repositories;

public interface IImportRepository
{
    Task<(IEnumerable<UploadHistoryDto> History, bool ShowDeleteButton)> GetUploadHistoryAsync(int roleId, int countryId);
    Task<ImportNullResult> GetImportNullRowsAsync();
    Task UpdateImportRowAsync(int id, double amount, string subNo, DateOnly callDate);
    Task<ProviderSettingDto?> GetProviderSettingAsync(int providerId);
    Task SaveProviderSettingAsync(int providerId, ProviderSettingDto setting, bool dbBased);
    Task<BillDetailDto?> InsertUploadHistoryAsync(string fileName, DateTime billDate, int providerId, string status, string userId);
    Task<int> ImportInvoiceAsync(int providerId, DateTime billDate);
    Task ClearPreviousImportAsync(int providerId, DateTime billDate, string fileName);
    Task DeleteBillAsync(int providerId, DateTime billDate);
    Task<IEnumerable<UnassignedBillDto>> GetUnassignedBillsAsync(int countryId, int roleId);
    Task<int> AssignInvoiceAsync();
}
