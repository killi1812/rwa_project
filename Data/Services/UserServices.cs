using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Data.Dto;
using Data.Helpers;
using Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Data.Services;

public interface IUserServices
{
    public Task<User> CreateUser(NewUserDto user);
    public Task<string> LoginJwt(LoginUserDto user);

    public Task<(ClaimsIdentity claimsIdentity, AuthenticationProperties authProperties)> LoginCookie(string username,
        string password);

    public Task ChangePassword(Guid userGuid, string oldPassword, string newPassword);
}

public class UserServices : IUserServices
{
    private readonly IMapper _mapper;
    private readonly RwaContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILoggerService _loggerService;

    public UserServices(IMapper mapper, RwaContext context, IServiceProvider serviceProvider,
        ILoggerService loggerService)
    {
        _context = context;
        _mapper = mapper;
        _configuration = serviceProvider.GetRequiredService<IConfiguration>();
        _loggerService = loggerService;
    }

    public async Task<User> CreateUser(NewUserDto userDto)
    {
        var userExist = await _context.Users.FirstOrDefaultAsync(u => u.Username == userDto.Username);
        if (userExist != null)
        {
            _loggerService.Log($"User {userDto.Username} already exists", ThreatLvl.Medium);
            throw new Exception("User already exists");
        }

        var user = _mapper.Map<User>(userDto);
        user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

        _loggerService.Log($"User {user.Username} created");

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<string> LoginJwt(LoginUserDto userDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userDto.Username);
        if (user == null)
            throw new NotFoundException($"User {userDto.Username} not found");

        var result = BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password);
        if (!result)
            throw new UnauthorizedException($"Wrong password for user: {userDto.Username}");

        var tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.ASCII.GetBytes(_configuration["key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new("guid", user.Guid.ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        _loggerService.Log($"User {user.Username} logged in");
        return tokenHandler.WriteToken(token);
    }

    public async Task<(ClaimsIdentity claimsIdentity, AuthenticationProperties authProperties)> LoginCookie(
        string username,
        string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
            throw new NotFoundException($"User {username}  not found");

        var result = BCrypt.Net.BCrypt.Verify(password, user.Password);
        if (!result)
            throw new UnauthorizedException($"Wrong password for user: {username}");

        var claims = new List<Claim>();
        claims.Add(new Claim("UserGuid", user.Guid.ToString()));
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            AllowRefresh = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
            IsPersistent = true,
        };
        return (claimsIdentity, authProperties);
    }

    public async Task ChangePassword(Guid userGuid, string oldPassword, string newPassword)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Guid == userGuid);
        if (user == null)
            throw new NotFoundException($"User with guid {userGuid} not fund");

        var result = BCrypt.Net.BCrypt.Verify(oldPassword, user.Password);
        if (!result)
            throw new UnauthorizedException($"Wrong password for user {user.Username}");

        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _context.SaveChangesAsync();
    }
}