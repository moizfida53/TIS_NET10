namespace TIS.Data.Models.Admin;

public class DelegationDto
{
    public int DelegateID { get; set; }
    public int ManagerID { get; set; }
    public string ManagerName { get; set; } = "";
    public int SecretaryID { get; set; }
    public string SecretaryName { get; set; } = "";
    public bool CanIdentify { get; set; }
    public bool CanApprove { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
