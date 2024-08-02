using AutoMapper;
using Data.Dto;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Services;

interface IUserServices
{
    public User GetUser(int id);
    public Task<User> CreateUser(NewUserDto user);
    public Task<string> Login(LoginUserDto user);
}

public class UserServices : IUserServices
{
    private readonly IMapper _mapper;
    private readonly RwaContext _context;

    public UserServices(IMapper mapper, RwaContext context)
    {
        _context = context;
        _mapper = mapper;
    }

    public User GetUser(int id)
    {
        throw new NotImplementedException();
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
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == userDto.Name);
        if (user == null)
            throw new Exception("User not found");

        var result = BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password);
        if (!result)
            throw new Exception("Invalid password");
        //TODO implement jwt
        return "jwt";
    }
}