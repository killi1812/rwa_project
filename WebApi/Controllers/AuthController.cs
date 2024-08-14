using Data.Dto;
using Data.Helpers;
using Microsoft.AspNetCore.Mvc;
using Data.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserServices _userServices;

    public AuthController(IUserServices userServices)
    {
        _userServices = userServices;
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto user)
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
        var newUser = await _userServices.CreateUser(user);
        var jwt = await _userServices.Login(new LoginUserDto()
        {
            Password = user.Password,
            Username = user.Username
        });
        return Ok(jwt);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
    {
        try
        {
            //TODO
            var userGuid = Request.GetGuid();
            if (userGuid == null) return BadRequest("Guid can't be null");
            await _userServices.ChangePassword(userGuid.Value, oldPassword, newPassword);
            return Ok();
        }
        catch (WrongCredentialsException e)
        {
            return Unauthorized(e.Message);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}