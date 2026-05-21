using System.Text.Json.Serialization;

namespace TIS.Data.Models.Admin;

public class EmployeeDto
{
    [JsonPropertyName("uid")]        public int    UID         { get; set; }
    [JsonPropertyName("name")]       public string NAME        { get; set; } = "";
    [JsonPropertyName("employeeNo")] public string EMPLOYEENO  { get; set; } = "";
    [JsonPropertyName("email")]      public string EMAIL       { get; set; } = "";
    [JsonPropertyName("username")]   public string USERNAME    { get; set; } = "";
    [JsonPropertyName("org")]        public string ORG         { get; set; } = "";
    [JsonPropertyName("description")]public string DESCRIPTION { get; set; } = "";
    [JsonPropertyName("grade")]      public string GRADE       { get; set; } = "";
    [JsonPropertyName("managerId")]  public int    MANAGERID   { get; set; }
    [JsonPropertyName("managerName")]public string MANAGERNAME { get; set; } = "";
    [JsonPropertyName("extension")]  public string EXTENSION   { get; set; } = "";
    [JsonPropertyName("payroll")]    public string PAYROLL     { get; set; } = "";
    [JsonPropertyName("roleId")]     public int    ROLEID      { get; set; }
    [JsonPropertyName("roleName")]   public string ROLENAME    { get; set; } = "";
    [JsonPropertyName("countryId")]  public int    COUNTRYID   { get; set; }
    [JsonPropertyName("countryName")]public string COUNTRYNAME { get; set; } = "";
    [JsonPropertyName("ccNo")]       public string CCNO        { get; set; } = "";
    [JsonPropertyName("isCostCenter")]public string ISCOSTCENTER{ get; set; } = "";
    [JsonPropertyName("company")]    public string COMPANY     { get; set; } = "";
    [JsonPropertyName("companyId")]  public string COMPANYID   { get; set; } = "";
    public bool IsActive { get; set; }
}

public class RoleDto
{
    [JsonPropertyName("roleId")]  public int    Role_ID  { get; set; }
    [JsonPropertyName("roleName")]public string RoleName { get; set; } = "";
}

public class CountryDto
{
    [JsonPropertyName("countryId")]  public int     COUNTRYID    { get; set; }
    [JsonPropertyName("countryName")]public string  COUNTRYNAME  { get; set; } = "";
    [JsonPropertyName("countryCode")]public string  COUNTRYCODE  { get; set; } = "";
    [JsonPropertyName("shayaCode")]  public string  SHAYACODE    { get; set; } = "";
    [JsonPropertyName("exchangeRate")]public decimal EXCHANGERATE { get; set; }
    [JsonPropertyName("currency")]   public string  CURRENCY     { get; set; } = "";
}

public class CostCenterDto
{
    [JsonPropertyName("uid")]      public int    UID       { get; set; }
    [JsonPropertyName("ccName")]   public string CCName    { get; set; } = "";
    [JsonPropertyName("ccNum")]    public string CCNum     { get; set; } = "";
    [JsonPropertyName("countryId")]public string COUNTRYID { get; set; } = "";
}

public class EmployeeInitData
{
    public IEnumerable<EmployeeDto>   Employees   { get; set; } = [];
    public IEnumerable<RoleDto>       Roles       { get; set; } = [];
    public IEnumerable<CountryDto>    Countries   { get; set; } = [];
    public IEnumerable<CostCenterDto> CostCenters { get; set; } = [];
}
