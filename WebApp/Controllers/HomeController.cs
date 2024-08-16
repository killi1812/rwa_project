using System.Diagnostics;
using AutoMapper;
using Data.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
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

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> TopTags()
    {
        var tags = await _pictureServices.GetTopTags();
        var photographers = await _pictureServices.GetTopPhotographers();
        //TODO combine and make top10vm
        var top10Vm = new Top10VM
        {
        };
        return View(top10Vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}