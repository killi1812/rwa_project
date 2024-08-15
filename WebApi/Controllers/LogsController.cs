using AutoMapper;
using Data.Dto;
using Data.Helpers;
using Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly ILoggerService _logServices;
    private readonly IMapper _mapper;

    public LogsController(ILoggerService logServices, IMapper mapper)
    {
        _logServices = logServices;
        _mapper = mapper;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Get([FromQuery] int page = 1, int n = 10)
    {
        var logs = await _logServices.GetLogs(page, n);
        return Ok(_mapper.Map<IEnumerable<LogDto>>(logs));
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Count()
    {
        var count = await _logServices.GetLogsCount();
        return Ok(count);
    }
}