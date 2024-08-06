using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Data.Dto;
using Data.Helpers;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Data.Services;

public interface IUserServices
{
    public User GetUser(int id);
    public Task<User> CreateUser(NewUserDto user);
    public Task<string> Login(LoginUserDto user);
}

public class UserServices : IUserServices
{
    private readonly IMapper _mapper;
    private readonly RwaContext _context;
    private readonly IConfiguration _configuration;

    public UserServices(IMapper mapper, RwaContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        _mapper = mapper;
        _configuration = serviceProvider.GetRequiredService<IConfiguration>();
    }

    public User GetUser(int id)
    {
        var user = _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            throw new NotFoundException("User not found");

        return _mapper.Map<User>(user);
    }

    public async Task<User> CreateUser(NewUserDto userDto)
    {
        var user = _mapper.Map<User>(userDto);
        user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<string> Login(LoginUserDto userDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == userDto.Username);
        if (user == null)
            throw new NotFoundException("User not found");

        var result = BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password);
        if (!result)
            throw new Exception("Invalid password");

        var tokenHandler = new JwtSecurityTokenHandler();
        //TODO get key from appsettings.json
        byte[] key = Encoding.ASCII.GetBytes(_configuration["key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new("Id", user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}