using TIS.Data.Models.Dashboard;

namespace TIS.Data.Repositories;

public interface IDashboardRepository
{
    Task<DashboardKpiDto> GetKpiAsync();

    Task<IEnumerable<dynamic>> GetChart1Async(int year);
    Task<IEnumerable<dynamic>> GetChart2Async(int year, string transType);
    Task<IEnumerable<dynamic>> GetChart3Async(int year, int month);
    Task<IEnumerable<dynamic>> GetChart4Async(int year);
    Task<IEnumerable<dynamic>> GetChart5Async(int year, string callTypeName);
    Task<IEnumerable<dynamic>> GetChart6Async(int year, string callType);
    Task<IEnumerable<dynamic>> GetChart7Async(int year, string callType, string outCountry);

    Task<IEnumerable<dynamic>> GetTransTypesAsync();
    Task<IEnumerable<dynamic>> GetCallTypesAsync(int year);
    Task<IEnumerable<dynamic>> GetIntCallCountAsync(int year, string callType);
    Task<IEnumerable<dynamic>> GetCountryGridAsync(int year);
    Task<IEnumerable<dynamic>> GetCountryGridMonthlyAsync(int year, int month);
}
