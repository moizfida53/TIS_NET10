using System.Text.Json.Serialization;

namespace TIS.Data.Models.MarsaApi;

public class ApiBillDetailsDto
{
    [JsonPropertyName("billId")]           public int     BillId           { get; set; }
    [JsonPropertyName("billDate")]         public string  BillDate         { get; set; } = "";
    [JsonPropertyName("subNo")]            public string  SubNo            { get; set; } = "";
    [JsonPropertyName("name")]             public string  Name             { get; set; } = "";
    [JsonPropertyName("org")]              public string  Org              { get; set; } = "";
    [JsonPropertyName("totalAmount")]      public decimal TotalAmount      { get; set; }
    [JsonPropertyName("businessLimit")]    public decimal BusinessLimit    { get; set; }
    [JsonPropertyName("businessCharges")]  public decimal BusinessCharges  { get; set; }
    [JsonPropertyName("deductibleAmount")] public decimal DeductibleAmount { get; set; }
    [JsonPropertyName("waiverAmount")]     public decimal WaiverAmount     { get; set; }
    [JsonPropertyName("comments")]         public string  Comments         { get; set; } = "";
}

public class ApiApprovalBillDto
{
    [JsonPropertyName("billId")]              public int     BillId              { get; set; }
    [JsonPropertyName("billAmount")]          public decimal BillAmount          { get; set; }
    [JsonPropertyName("billDate")]            public string  BillDate            { get; set; } = "";
    [JsonPropertyName("employeeName")]        public string  EmployeeName        { get; set; } = "";
    [JsonPropertyName("mobileNumber")]        public string  MobileNumber        { get; set; } = "";
    [JsonPropertyName("businessCharges")]     public decimal BusinessCharges     { get; set; }
    [JsonPropertyName("personalCharges")]     public decimal PersonalCharges     { get; set; }
    [JsonPropertyName("waiverAmount")]        public decimal WaiverAmount        { get; set; }
    [JsonPropertyName("waiverComment")]       public string  WaiverComment       { get; set; } = "";
    [JsonPropertyName("businessLimitCharges")] public decimal BusinessLimitCharges { get; set; }
    [JsonPropertyName("department")]          public string  Department          { get; set; } = "";
    [JsonPropertyName("businessLimit")]       public decimal BusinessLimit       { get; set; }
    [JsonPropertyName("deductibleAmount")]    public decimal DeductibleAmount    { get; set; }
}
