using AutoMapper;
using Data.Dto;
using Data.Helpers;
using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FotografijaController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IPictureServices _pictureServices;
    private readonly ILoggerService _loggerService;

    public FotografijaController(IMapper mapper, IPictureServices pictureServices, ILoggerService loggerService)
    {
        _mapper = mapper;
        _pictureServices = pictureServices;
        _loggerService = loggerService;
    }

    [Authorize]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int n = 10)
    {
        try
        {
            var pictures = await _pictureServices.GetPictures(page, n);
            _loggerService.Log(
                $"User {Request.GetGuid()} pictures from page: {page}, count: {pictures.Count}");
            return Ok(_mapper.Map<List<PictureDto>>(pictures));
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

    [HttpGet("[action]/{guid}")]
    public async Task<IActionResult> Get([FromRoute] string guid)
    {
        try
        {
            //TODO return a dto and serve picture seperatly
            var pictures = await _pictureServices.GetPicture(Guid.Parse(guid));
            return Ok(_mapper.Map<PictureDto>(pictures));
        }
        catch (NotFoundException e)
        {
            _loggerService.Log($"Failed to get fotografija");
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [Authorize]
    [HttpPost("[action]")]
    public async Task<IActionResult> Create([FromForm] NewPictureDto newPictureDto)
    {
        try
        {
            var guid = Request.GetGuid();
            if (guid == null) return BadRequest("id is null");
            //TODO return created picture
            await _pictureServices.CreatePicture(newPictureDto, guid.Value);
            return Ok();
        }
        //TODO change all try caches to This
        catch (NotFoundException e)
        {
            _loggerService.Log($"Failed to create {e.Message}");
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _loggerService.Log($"Failed to create {e.Message}");
            return StatusCode(500, e.Message);
        }
    }

    [Authorize]
    [HttpDelete("[action]/{guid}")]
    public async Task<IActionResult> Delete([FromRoute] string guid)
    {
        try
        {
            await _pictureServices.DeletePicture(Guid.Parse(guid));
            return Ok();
        }
        catch (NotFoundException e)
        {
            _loggerService.Log($"Failed to delete {e.Message}");
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _loggerService.Log($"Failed to delete {e.Message}");
            return StatusCode(500, e.Message);
        }
    }

    [Authorize]
    [HttpPut("[action]/{guid}")]
    public async Task<IActionResult> Update([FromRoute] string guid, [FromBody] UpdatePictureDto dto)
    {
        try
        {
            //TODO return updated picture 
            await _pictureServices.UpdatePicture(Guid.Parse(guid), dto);
            return Ok();
        }
        catch (NotFoundException e)
        {
            _loggerService.Log($"Failed to update {e.Message}");
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _loggerService.Log($"Failed to update {e.Message}");
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var pics = await _pictureServices.SearchPictures(query);
        return Ok(pics);
    }
}