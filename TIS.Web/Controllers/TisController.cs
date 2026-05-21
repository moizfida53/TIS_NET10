using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace TIS.Web.Controllers;

public abstract class TisController : Controller
{
    private static readonly JsonSerializerOptions CamelCase = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
    };

    protected new JsonResult Json(object? data) => new JsonResult(data, CamelCase);
}
