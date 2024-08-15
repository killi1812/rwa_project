using AutoMapper;
using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using WebApp.Models;
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
    public async Task<IActionResult> Search(string query, int page = 1, int n = 10)
    {
        var picsSession = HttpContext.Session.GetString("pictures");
        var oldQuery = HttpContext.Session.GetString("query");
        List<PictureVM> pictures;
        if (picsSession == null || oldQuery != query)
        {
            var pics = await _pictureServices.SearchPictures(query);
            pictures = _mapper.Map<List<PictureVM>>(pics);
            HttpContext.Session.SetString("query", query);
            HttpContext.Session.SetString("pictures", pictures.ToJson());
        }
        else
        {
            pictures = picsSession.FromJson<List<PictureVM>>();
        }

        var picsPaginated = new Paginated<PictureVM>(pictures, page, n);
        var searchVm = _mapper.Map<SearchVM<PictureVM>>(picsPaginated);
        searchVm.Query = query;
        TempData["pictures"] = searchVm.ToJson();
        return Redirect(nameof(SearchResults));
    }

    /// Endpoint for displaying search results
    public IActionResult SearchResults()
    {
        var pics = TempData["pictures"];
        if (pics == null)
            return View(new SearchVM<PictureVM>());

        var pictures = pics.ToString().FromJson<SearchVM<PictureVM>>();
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

    [HttpGet]
    public async Task<IActionResult> Data(string guid)
    {
        var data = await _pictureServices.GetPictureData(Guid.Parse(guid));
        return File(data, "image/jpeg");
    }
}