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
    private readonly IUserServices _userServices;

    public UserController(IPictureServices pictureServices, IUserServices userServices, IMapper mapper)
    {
        _pictureServices = pictureServices;
        _userServices = userServices;
        _mapper = mapper;
    }

    [Authorize]
    public IActionResult Account()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> UserUploads(string? guid)
    {
        if (guid == null)
        {
            guid = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserGuid")?.Value;
            if (guid == null)
                return BadRequest("User not found");
        }

        var user = await _userServices.GetUser(Guid.Parse(guid));
        ViewData["Title"] = $"{user.Username} Uploads";
        
        var pics = await _pictureServices.GetPicturesFromUser(Guid.Parse(guid));
        var picsVm = _mapper.Map<List<PictureVM>>(pics);
        return View(picsVm);
    }
}