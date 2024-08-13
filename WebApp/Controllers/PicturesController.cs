using AutoMapper;
using Data.Services;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class PicturesController : Controller
{
    private readonly IPictureServices _pictureServices;
    private readonly ILoggerService _loggerService;
    private readonly IMapper _mapper;

    public PicturesController(IPictureServices pictureServices, ILoggerService loggerService, IMapper mapper)
    {
        _pictureServices = pictureServices;
        _loggerService = loggerService;
        _mapper = mapper;
    }

    /// Endpoint for searching pictures
    public async Task<IActionResult> Search(string query)
    {
        try
        {
            var pictures = await _pictureServices.SearchPictures(query);
            TempData["pictures"] = _mapper.Map<List<PictureVM>>(pictures).ToJson();
            return Redirect(nameof(SearchResults));
        }
        catch (Exception e)
        {
            _loggerService.Log(e.Message);
            return BadRequest();
        }
    }

    /// Endpoint for displaying search results
    public IActionResult SearchResults()
    {
        var pics = TempData["pictures"];
        if (pics == null)
            return View(new List<PictureVM>());

        var pictures = pics.ToString().FromJson<List<PictureVM>>();
        return View(pictures);
    }

    /// Endpoint for displaying a picture
    public IActionResult Details()
    {
        throw new NotImplementedException();
    }

    /// Endpoint for downloading a picture
    [HttpGet]
    [ValidateAntiForgeryToken]
    public IActionResult Download()
    {
        throw new NotImplementedException();
    }
}