using System.Security.Claims;
using Data.Dto;
using Data.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class AuthController : Controller
{
    private readonly IUserServices _userServices;

    public AuthController(IUserServices userServices)
    {
        _userServices = userServices;
    }

    public IActionResult Login(string returnUrl = "/Home/Index")
    {
        return View(new LoginVM { ReturnUrl = returnUrl });
    }

    public async Task<IActionResult> LoginAction(LoginVM loginVm)
    {
        var claims = await _userServices.LoginCookie(loginVm.Username, loginVm.Password);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claims.claimsIdentity), claims.authProperties);

        HttpContext.Session.SetString("username", loginVm.Username);

        //TODO redirect to current page 
        return Redirect(loginVm.ReturnUrl);
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


    public IActionResult Logout(string redirectUrl = "/Home/Index")
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //TODO redirect to current page or home
        return Redirect(redirectUrl);
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}