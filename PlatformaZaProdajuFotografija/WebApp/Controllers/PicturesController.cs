using AutoMapper;
using Data.Dto;
using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> Search(string query, int page = 1, int n = 30)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Redirect(nameof(SearchResults));

        if (page == 0) page = 1;
        //query filter=tag

        var picsSession = HttpContext.Session.GetString("pictures");
        var oldQuery = HttpContext.Session.GetString("query");
        List<PictureVM> pictures;

        if (picsSession == null || oldQuery != query)
        {
            var q = ParseQuery(query);
            var pics = await _pictureServices.SearchPictures(q.Item1, q.Item2);
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

    private static (string, FilterType) ParseQuery(string query)
    {
        try
        {
            var split = query.Split("=");
            var filter = FilterTypeExtensions.ParseFilterType(split[0]);
            return (split[1], filter);
        }
        catch (Exception)
        {
            return (query, FilterType.All);
        }
    }

    /// Endpoint for displaying search results
    public async Task<IActionResult> SearchResults()
    {
        var pics = TempData["pictures"];
        if (pics == null)
        {
            var mostPopular = await _pictureServices.GetMostPopularPictures();
            return View(_mapper.Map<SearchVM<PictureVM>>(mostPopular));
        }

        var pictures = pics.ToString().FromJson<SearchVM<PictureVM>>();
        return View(pictures);
    }

    /// Endpoint for displaying a picture
    public async Task<IActionResult> Details(string guid)
    {
        var picture = await _pictureServices.GetPicture(Guid.Parse(guid));
        var pictureVm = _mapper.Map<PictureDetailsVM>(picture);

        return View(pictureVm);
    }

    public async Task<IActionResult> Download(string guid)
    {
        var user = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserGuid")?.Value;
        if (user == null)
            return RedirectToAction("Login", "Auth", new { returnUrl = $"/Pictures/Details?guid={guid}" });

        var rez = await _pictureServices.DownloadPicture(Guid.Parse(guid), Guid.Parse(user));

        return File(rez.Data, "image/jpeg", $"{rez.pic.Name}.jpeg");
    }

    [HttpGet]
    public async Task<IActionResult> Data(string guid)
    {
        var data = await _pictureServices.GetPictureData(Guid.Parse(guid));
        return File(data, "image/jpeg");
    }

    [Authorize]
    public async Task<IActionResult> Upload()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> UploadPicture(NewPictureDto pic)
    {
        var guid = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserGuid")?.Value;
        if (guid == null)
            return RedirectToAction("Login", "Auth", new { returnUrl = Request.Path });

        var picture = await _pictureServices.CreatePicture(pic, Guid.Parse(guid));

        return RedirectToAction(nameof(Details), new { guid = picture.Guid });
    }

    [HttpGet]
    public async Task<IActionResult> LoadMoreDownloads(string guid, int page, int pageSize)
    {
        var downloads = await _pictureServices.GetDownloads(Guid.Parse(guid), page, pageSize);
        return Ok(downloads);
    }

    //TODO implement page and action for edit create EditPictureVM
    public async Task<IActionResult> Edit(string guid)
    {
        var pic = await _pictureServices.GetPicture(Guid.Parse(guid));
        var picVm = _mapper.Map<UpdatePictureVM>(pic);
        return View(picVm);
    }

    public async Task<IActionResult> EditAction(UpdatePictureVM vm)
    {
        UpdatePictureDto dto = _mapper.Map<UpdatePictureDto>(vm);
        var pic = await _pictureServices.UpdatePicture(Guid.Parse(vm.Guid), dto);
        return RedirectToAction(nameof(Details), new { guid = pic.Guid });
    }

    public async Task<IActionResult> Delete(string guid)
    {
        await _pictureServices.DeletePicture(Guid.Parse(guid));
        return Redirect(nameof(SearchResults));
    }
}