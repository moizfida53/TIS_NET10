namespace TIS.Data.Models.Admin;

public class PackageMasterDto
{
    public int PkgID { get; set; }
    public int ProviderID { get; set; }
    public string ProviderName { get; set; } = "";
    public string TransType { get; set; } = "";
    public string Description { get; set; } = "";
    public bool MakeAllUnexpected { get; set; }
    public DateTime? StartDate { get; set; }
    public int CountryID { get; set; }
}

public class PackageDetailDto
{
    public int DetailID { get; set; }
    public int PkgID { get; set; }
    public string ExpectedType { get; set; } = "";
    public decimal AmountLimit { get; set; }
    public string TransType { get; set; } = "";
    public string Description { get; set; } = "";
}

public class DataRoamingDto
{
    public int RoamingID { get; set; }
    public string CountryName { get; set; } = "";
    public string OperatorName { get; set; } = "";
    public int ProviderID { get; set; }
    public int PkgID { get; set; }
}

public class CallTypeDto
{
    public int ID { get; set; }
    public string TransType { get; set; } = "";
}

public class CallDescDto
{
    public int ID { get; set; }
    public string Description { get; set; } = "";
}

public class ContactDto
{
    public int Uid { get; set; }
    public string DialledNo { get; set; } = "";
    public string Name { get; set; } = "";
    public string? ExName { get; set; }
}

public class ManagerDto
{
    public int UID { get; set; }
    public string ManagerName { get; set; } = "";
    public string EMPLOYEENO { get; set; } = "";
}
