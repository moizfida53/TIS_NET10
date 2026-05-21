using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIS.Data.Models.Audit;
using TIS.Data.Repositories;

namespace TIS.Web.Controllers;

[Authorize(Policy = "Admin")]
public class AuditReportController(IAuditRepository repo, ILogger<AuditReportController> logger) : TisController
{
    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> GetEmp()
    {
        var employees = await repo.GetEmployeesAsync();
        return Ok(new { EmpList = employees });
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int      @event,
        [FromQuery] int      uid,
        [FromQuery] string   status = "")
    {
        var req = new AuditSearchRequest
        {
            StartDate = startDate,
            EndDate   = endDate,
            Event     = @event,
            Uid       = uid,
            Status    = status
        };
        var rows = await repo.SearchAsync(req);
        return Ok(new { dtAuditReport = rows });
    }

    [HttpGet]
    public async Task<IActionResult> Details([FromQuery] int id)
    {
        var details = await repo.GetDetailsAsync(id);
        return Ok(new { dtDetails = details });
    }
}

