using AutoMapper;
using Data.Dto;
using Data.Models;
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

    [HttpGet("[action]/{n}")]
    public async Task<IActionResult> Get([FromRoute] int n = 10)
    {
        var logs = await _logServices.GetLogs();
        var logsDto = _mapper.Map<List<LogDto>>(logs);
        var paginatedLogs = new Paginated<LogDto>(logsDto, 1, n);
        return Ok(paginatedLogs);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Get([FromQuery] int page = 1, int n = 10)
    {
        var logs = await _logServices.GetLogs();
        var logsDto = _mapper.Map<List<LogDto>>(logs);
        var paginatedLogs = new Paginated<LogDto>(logsDto, page, n);
        return Ok(paginatedLogs);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Count()
    {
        var count = await _logServices.GetLogsCount();
        return Ok(count);
    }
}