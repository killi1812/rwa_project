using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class LoginVM
{
    [Display(Name = "Username")] public string Username { get; set; }
    [Display(Name = "Password")] public string Password { get; set; }

    public string ReturnUrl { get; set; }
}