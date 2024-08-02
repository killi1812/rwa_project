using Data.Dto;
using Microsoft.AspNetCore.Mvc;
using Data.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserServices _userServices;

    public UserController(IUserServices userServices)
    {
        _userServices = userServices;
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> Login([FromForm] LoginUserDto user)
    {
        try
        {
            var jwt = await _userServices.Login(user);
            return Ok(jwt);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Register([FromForm] NewUserDto user)
    {
        try
        {
            var newUser = await _userServices.CreateUser(user);
            return Ok(newUser);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}