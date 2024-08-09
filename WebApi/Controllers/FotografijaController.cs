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
        _loggerService.Log($"User {Request.GetId()} wanted to get pictures from page: {page}, count: {n}");
        try
        {
            var pictures = await _pictureServices.GetPictures(page, n);
            return Ok(pictures);
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

    [HttpGet("[action]/{id}")]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        _loggerService.Log($"User ${Request.GetId()} wanted to get picture with id: {id}");
        try
        {
            var pictures = await _pictureServices.GetPicture(id);
            return Ok(pictures);
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
        _loggerService.Log($"User {Request.GetId()} wanted to create a new picture with name: {newPictureDto.Name}");
        try
        {
            var id = Request.GetId();
            if (id == null) return BadRequest("id is null");
            //TODO return created picture
            await _pictureServices.CreatePicture(newPictureDto, id.Value);
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

    [HttpDelete("[action]/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        _loggerService.Log($"User {Request.GetId()} wanted to delete picture with id: {id}");
        try
        {
            await _pictureServices.DeletePicture(id);
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

    [HttpPut("[action]/{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePictureDto dto)
    {
        _loggerService.Log($"User {Request.GetId()} wanted to update picture with id: {id}");
        try
        {
            //TODO return updated picture 
            await _pictureServices.UpdatePicture(id, dto);
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
    public IActionResult Search()
    {
        //TODO write a search that searches by name, photographer and tags 
        throw new NotImplementedException();
    }
}