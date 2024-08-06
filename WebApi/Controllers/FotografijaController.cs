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

    public FotografijaController(IMapper mapper, IPictureServices pictureServices)
    {
        _mapper = mapper;
        _pictureServices = pictureServices;
    }

    [Authorize]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int n = 10)
    {
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
        try
        {
            var pictures = await _pictureServices.GetPicture(id);
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

    [Authorize]
    [HttpPost("[action]")]
    public async Task<IActionResult> Create([FromForm] NewPictureDto newPictureDto)
    {
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
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpDelete("[action]/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        try
        {
            await _pictureServices.DeletePicture(id);
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

    [HttpPut("[action]/{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePictureDto dto)
    {
        try
        {
            //TODO return updated picture 
            await _pictureServices.UpdatePicture(id, dto);
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