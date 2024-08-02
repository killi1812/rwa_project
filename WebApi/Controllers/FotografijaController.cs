using AutoMapper;
using Data.Dto;
using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FotografijaController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IFotografijaServices _fotografijaServices;

    public FotografijaController(IMapper mapper, IFotografijaServices fotografijaServices)
    {
        _mapper = mapper;
        _fotografijaServices = fotografijaServices;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int n = 10)
    {
        try
        {
            var pictures = await _fotografijaServices.GetPictures(page, n);
            return Ok(pictures);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("[action]/{id}")]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        try
        {
            var pictures = await _fotografijaServices.GetPicture(id);
            return Ok(pictures);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("[action]/{id}")]
    public async Task<IActionResult> Create([FromForm] NewPictureDto newPictureDto)
    {
        try
        {
            //TODO return created picture
            await _fotografijaServices.CreatePicture(newPictureDto);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("[action]/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        try
        {
            await _fotografijaServices.DeletePicture(id);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("[action]/{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] NewPictureDto newPictureDto)
    {
        try
        {
            //TODO return updated picture 
            await _fotografijaServices.UpdatePicture(id, newPictureDto);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}