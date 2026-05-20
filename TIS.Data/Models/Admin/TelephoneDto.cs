namespace TIS.Data.Models.Admin;

public class TelephoneDto
{
    public int SubNoId { get; set; }
    public string SubNo { get; set; } = "";
    public string Description { get; set; } = "";
    public int ProviderID { get; set; }
    public string ProviderName { get; set; } = "";
    public string AccountNo { get; set; } = "";
    public string LineType { get; set; } = "";
    public int CountryID { get; set; }
    public string CountryName { get; set; } = "";
    public bool IsAssigned { get; set; }
}

public class AssignmentDto
{
    public int AssignID { get; set; }
    public int SubNoId { get; set; }
    public string SubNo { get; set; } = "";
    public int UID { get; set; }
    public string EmployeeName { get; set; } = "";
    public string EmployeeNo { get; set; } = "";
    public int CostCenterID { get; set; }
    public string CostCenterName { get; set; } = "";
    public string LineStatus { get; set; } = "";
    public decimal BusinessLimit { get; set; }
    public decimal AllowanceLimit { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class ProviderDto
{
    public int ProviderID { get; set; }
    public string ProviderName { get; set; } = "";
    public int CountryID { get; set; }
}

public class TelephonePageData
{
    public IEnumerable<TelephoneDto> Telephones { get; set; } = [];
    public IEnumerable<AssignmentDto> Assignments { get; set; } = [];
    public IEnumerable<ProviderDto> Providers { get; set; } = [];
    public IEnumerable<EmployeeDto> Employees { get; set; } = [];
}
