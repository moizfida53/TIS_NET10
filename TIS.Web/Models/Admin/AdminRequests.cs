using TIS.Data.Models.Admin;

namespace TIS.Web.Models.Admin;

public class EmployeeRequest
{
    public EmployeeDto Employee { get; set; } = new();
    public int[] CountryIds { get; set; } = [];
}

public class ManagerRequest
{
    public int? Uid { get; set; }
    public string Name { get; set; } = "";
    public string EmployeeNo { get; set; } = "";
}

public class PackageRequest
{
    public PackageMasterDto Master { get; set; } = new();
    public List<PackageDetailDto> Details { get; set; } = [];
}
