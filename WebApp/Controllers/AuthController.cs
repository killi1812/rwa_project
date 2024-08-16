using System.Security.Claims;
using Data.Dto;
using Data.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
        var claims = await _userServices.LoginCookie(username, password);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claims.claimsIdentity), claims.authProperties);

        HttpContext.Session.SetString("username", username);
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

        await _userServices.CreateUser(new NewUserDto
        {
            Username = username,
            Password = password,
            Admin = false,
        });
        return Redirect(nameof(Login));
    }


    //TODO: This should lead to a page where the user can change their password 
    public IActionResult Account(string guid)
    {
        var guidP = Guid.Parse(guid);
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/Home/Index");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}