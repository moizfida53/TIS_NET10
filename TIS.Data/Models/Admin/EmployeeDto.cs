namespace TIS.Data.Models.Admin;

public class EmployeeDto
{
    public int UID { get; set; }
    public string NAME { get; set; } = "";
    public string EMPLOYEENO { get; set; } = "";
    public string EMAIL { get; set; } = "";
    public string USERNAME { get; set; } = "";
    public string ORG { get; set; } = "";
    public string DESCRIPTION { get; set; } = "";
    public string GRADE { get; set; } = "";
    public int MANAGERID { get; set; }
    public string MANAGERNAME { get; set; } = "";
    public string EXTENSION { get; set; } = "";
    public string PAYROLL { get; set; } = "";
    public int ROLEID { get; set; }
    public string ROLENAME { get; set; } = "";
    public int COUNTRYID { get; set; }
    public string COUNTRYNAME { get; set; } = "";
    public string CCNO { get; set; } = "";
    public string ISCOSTCENTER { get; set; } = "";
    public string COMPANY { get; set; } = "";
    public string COMPANYID { get; set; } = "";
    public bool IsActive { get; set; }
}

public class RoleDto
{
    public int Role_ID { get; set; }
    public string RoleName { get; set; } = "";
}

public class CountryDto
{
    public int COUNTRYID { get; set; }
    public string COUNTRYNAME { get; set; } = "";
    public string COUNTRYCODE { get; set; } = "";
    public string SHAYACODE { get; set; } = "";
    public decimal EXCHANGERATE { get; set; }
    public string CURRENCY { get; set; } = "";
}

public class CostCenterDto
{
    public int UID { get; set; }
    public string CCName { get; set; } = "";
    public string CCNum { get; set; } = "";
    public string COUNTRYID { get; set; } = "";
}

public class EmployeeInitData
{
    public IEnumerable<EmployeeDto> Employees { get; set; } = [];
    public IEnumerable<RoleDto> Roles { get; set; } = [];
    public IEnumerable<CountryDto> Countries { get; set; } = [];
    public IEnumerable<CostCenterDto> CostCenters { get; set; } = [];
}
