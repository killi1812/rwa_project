using System.Diagnostics;
using AutoMapper;
using Data.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class HomeController : Controller
{
    private readonly IPictureServices _pictureServices;
    private readonly IMapper _mapper;

    public HomeController(IPictureServices pictureServices, IMapper mapper)
    {
        _pictureServices = pictureServices;
        _mapper = mapper;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Top10()
    {
        var tags = await _pictureServices.GetTopTags();
        var photographers = await _pictureServices.GetTopPhotographers();
        List<Top10VM> top10VMs = new();
        for (int i = 0; i < 10; i++)
        {
            Top10VM t = new();
            try
            {
                t.TagName = tags.ElementAt(i);
            }
            catch (ArgumentOutOfRangeException)
            {
                t.TagName = null;
            }

            try
            {
                t.Photographer = photographers.ElementAt(i);
            }
            catch (ArgumentOutOfRangeException)
            {
                t.Photographer = null;
            }

            if (t.TagName == null && t.Photographer == null) continue;
            top10VMs.Add(t);
        }

        return View(top10VMs);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}