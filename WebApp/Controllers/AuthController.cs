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

    public IActionResult LoginAction(string username, string password)
    {
        throw new NotImplementedException();
    }

    public IActionResult Register()
    {
        return View();
    }

    public IActionResult RegisterAction()
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }
}