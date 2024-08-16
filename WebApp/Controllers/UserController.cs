using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class UserController : Controller
{
    public IActionResult Account()
    {
        return View();
    }
}