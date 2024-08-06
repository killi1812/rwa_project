using Data.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    // private readonly ILogService _logServices;


    [HttpGet("[action]")]
    public async Task<IActionResult> GetLogs()
    {
        try
        {
            // var logs = await _logServices.GetLogs();
            // return Ok(logs);
            return Ok();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}