using AutoMapper;
using Data.Models;
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
    public async Task<IActionResult> Account(string guid)
    {
        if (guid == null)
        {
            guid = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserGuid")?.Value;
            if (guid == null)
                return BadRequest("User not found");
        }

        var user = await _userServices.GetUser(Guid.Parse(guid));
        var userVm = _mapper.Map<UserVM>(user);
        return View(userVm);
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

    [Authorize]
    public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
    {
        var userGuid = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserGuid")?.Value);
        await _userServices.ChangePassword(userGuid, oldPassword, newPassword);
        return Ok();
    }

    [Authorize]
    public IActionResult EditUser(UserVM userVm)
    {
        var userGuid = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserGuid")?.Value);
        var user = _mapper.Map<User>(userVm);
        var userNew = _userServices.EditUser(userGuid, user);
        return Redirect(nameof(Account));
    }

    [Authorize]
    public async Task<IActionResult> DeleteUser(string password)
    {
        var userGuid = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserGuid")?.Value);
        // await _userServices.DeleteUser(userGuid, password);
        return RedirectToAction("Logout", "Auth");
    }

    [Authorize]
    public IActionResult Users()
    {
        //TODO check if admin
        var users = _userServices.GetUsers();
        var usersVm = _mapper.Map<List<UserVM>>(users);
        return View(usersVm);
    }
}