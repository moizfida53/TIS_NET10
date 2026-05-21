using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIS.Data.Repositories;

namespace TIS.Web.Controllers;

[Authorize(Policy = "Admin")]
public class MarsaApiController(IMarsaApiRepository repo, ILogger<MarsaApiController> logger) : Controller
{
    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> GetCountofBillsByUserName([FromQuery] string userName)
    {
        var count = await repo.GetBillCountByUsernameAsync(userName);
        return count.HasValue
            ? Ok(new { Message = "Success", countofBills = count.Value })
            : Ok(new { Message = "No Data Found" });
    }

    [HttpGet]
    public async Task<IActionResult> GetPendingApprovalCountByUserName([FromQuery] string userName)
    {
        var count = await repo.GetPendingApprovalCountByUsernameAsync(userName);
        return count.HasValue
            ? Ok(new { Message = "Success", countofBills = count.Value })
            : Ok(new { Message = "No Data Found" });
    }

    [HttpGet]
    public async Task<IActionResult> GetBillDetailsByBillId([FromQuery] int billId)
    {
        var data = (await repo.GetBillDetailsByBillIdAsync(billId)).ToList();
        return data.Count > 0
            ? Ok(new { Message = "Success", data })
            : Ok(new { Message = "No Data Found" });
    }

    [HttpGet]
    public async Task<IActionResult> UpdateBillByBillId(
        [FromQuery] int    billId,
        [FromQuery] int    status,
        [FromQuery] string comments = "")
    {
        var result = await repo.UpdateBillByBillIdAsync(billId, status, comments);
        return result == "Success"
            ? Ok(new { Message = "Success" })
            : Ok(new { Message = result });
    }

    [HttpGet]
    public async Task<IActionResult> ApprovalBillList([FromQuery] string username)
    {
        var data = (await repo.GetApprovalBillListAsync(username)).ToList();
        return data.Count > 0
            ? Ok(new { Message = "Success", data })
            : Ok(new { Message = "No Data Found" });
    }
}
