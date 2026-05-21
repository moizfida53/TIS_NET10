using TIS.Data.Models.Audit;

namespace TIS.Data.Repositories;

public interface IAuditRepository
{
    Task<IEnumerable<AuditEmployeeDto>> GetEmployeesAsync();
    Task<IEnumerable<AuditReportRowDto>> SearchAsync(AuditSearchRequest req);
    Task<IEnumerable<AuditDetailDto>> GetDetailsAsync(int id);
}
