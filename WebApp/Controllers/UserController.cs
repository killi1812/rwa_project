using AutoMapper;
using Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class UserController : Controller
{
    private readonly IPictureServices _pictureServices;
    private readonly IMapper _mapper;

    public UserController(IPictureServices pictureServices, IMapper mapper)
    {
        _pictureServices = pictureServices;
        _mapper = mapper;
    }

    [Authorize]
    public IActionResult Account()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> YourUploads()
    {
        var userGuid = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserGuid")?.Value;
        if (userGuid == null)
            return BadRequest("User not found");

        var pics = await _pictureServices.GetPicturesFromUser(Guid.Parse(userGuid));
        var picsVm = _mapper.Map<List<PictureVM>>(pics);
        return View(picsVm);
    }
}