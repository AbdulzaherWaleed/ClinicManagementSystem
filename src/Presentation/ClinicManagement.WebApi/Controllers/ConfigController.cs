using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ClinicManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ConfigController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("features")]
    [AllowAnonymous]
    public IActionResult GetFeatures()
    {
        var features = _configuration.GetSection("Features").Get<Dictionary<string, bool>>() ?? new Dictionary<string, bool>();
        return Ok(features);
    }
}
