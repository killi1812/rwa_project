using Data.Dto;
using Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class AuthController : Controller
{
    private readonly IUserServices _userServices;

    public AuthController(IUserServices userServices)
    {
        _userServices = userServices;
    }

    public IActionResult Login()
    {
        return View();
    }

    public async Task<IActionResult> LoginAction(string username, string password)
    {
        // var cookie = await _userServices.LoginCookie(username, password);
        HttpContext.Session.SetString("username", username);
        ViewData["username"] = username;
        return Redirect("/Home/Index");
    }

    public IActionResult Register()
    {
        return View();
    }

    public async Task<IActionResult> RegisterAction(string username, string password, string password2)
    {
        if (password != password2)
        {
            ViewData["error"] = "Passwords do not match";
            return Redirect(nameof(Register));
        }

        _ = await _userServices.CreateUser(new NewUserDto
        {
            Username = username,
            Password = password,
            Admin = false,
        });
        return Redirect(nameof(Login));
    }

    public IActionResult Account()
    {
        return View();
    }

    //TODO: See what exactly should this be 
    public IActionResult EditAccount()
    {
        throw new NotImplementedException();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("username");
        return Redirect("/Home/Index");
    }
}