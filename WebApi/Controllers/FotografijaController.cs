using AutoMapper;
using Data.Dto;
using Data.Helpers;
using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

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
        var pictures = await _pictureServices.GetPictures(page, n);
        _loggerService.Log(
            $"User {Request.GetGuid()} pictures from page: {page}, count: {pictures.Count}");
        return Ok(_mapper.Map<List<PictureDto>>(pictures));
    }

    [HttpGet("[action]/{guid}")]
    public async Task<IActionResult> Get([FromRoute] string guid)
    {
        var pictures = await _pictureServices.GetPicture(Guid.Parse(guid));
        return Ok(_mapper.Map<PictureDto>(pictures));
    }

    [Authorize]
    [HttpPost("[action]")]
    public async Task<IActionResult> Create([FromForm] NewPictureDto newPictureDto)
    {
        var guid = Request.GetGuid();
        if (guid == null) return BadRequest("guid is null");
        //TODO return created picture
        await _pictureServices.CreatePicture(newPictureDto, guid.Value);
        return Ok();
    }

    [Authorize]
    [HttpDelete("[action]/{guid}")]
    public async Task<IActionResult> Delete([FromRoute] string guid)
    {
        await _pictureServices.DeletePicture(Guid.Parse(guid));
        return Ok();
    }

    [Authorize]
    [HttpPut("[action]/{guid}")]
    public async Task<IActionResult> Update([FromRoute] string guid, [FromBody] UpdatePictureDto dto)
    {
        //TODO return updated picture 
        await _pictureServices.UpdatePicture(Guid.Parse(guid), dto);
        return Ok();
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int n = 10)
    {
        var pics = await _pictureServices.SearchPictures(query);
        //TODO return whole mapped object
        var picsPag = new Pagineted<PictureDto>(_mapper.Map<List<PictureDto>>(pics), page, n);
        return Ok(picsPag);
    }

    [HttpGet("[action]/{guid}")]
    public async Task<IActionResult> Picture([FromRoute] string guid)
    {
        var pic = await _pictureServices.GetPictureData(Guid.Parse(guid));
        return File(pic, "image/jpeg");
    }
}