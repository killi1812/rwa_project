using System.Text;
using Data.Dto;
using Data.Helpers;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Services;

public interface IPictureServices
{
    Task<IList<Picture>> GetPictures(int page = 1, int n = 10);
    Task<Picture> GetPicture(int id);
    Task DeletePicture(int id);
    Task UpdatePicture(int id, UpdatePictureDto dto);
    Task CreatePicture(NewPictureDto newPictureDto, int id);
}

public class PictureServices : IPictureServices
{
    private readonly RwaContext _context;
    private readonly ILoggerService _loggerService;
    private readonly ITagService _tagService;

    public PictureServices(RwaContext context, ILoggerService loggerService, ITagService tagService)
    {
        _context = context;
        _loggerService = loggerService;
        _tagService = tagService;
    }

    public async Task<IList<Picture>> GetPictures(int page = 1, int n = 10)
    {
        var pictures = await _context.Pictures
            .Skip((page - 1) * n)
            .Take(n)
            .ToListAsync();
        return pictures;
    }

    public async Task<Picture> GetPicture(int id)
    {
        var picture = await _context.Pictures
            .FirstOrDefaultAsync(p => p.Id == id);

        if (picture == null)
            //TODO add notfound exception
            throw new NotFoundException("Picture not found");

        return picture;
    }

    public async Task CreatePicture(NewPictureDto newPictureDto, int id)
    {
        //TODO Add guids
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.Admin);
        if (user == null)
            throw new NotFoundException("User not found");

        var picture = new Picture
        {
            Name = $"{newPictureDto.Name}.{newPictureDto.Data.ContentType.Split("/")[1]}",
            Photographer = newPictureDto.Photographer,
            UserId = id,
        };

        byte[] data = new byte[newPictureDto.Data.Length];
        newPictureDto.Data.OpenReadStream().Read(data);
        picture.Data = data;

        _loggerService.Log($"User {user.Username} created picture {picture.Name}");

        await _context.Pictures.AddAsync(picture);
        //TODO check if there need to be save changes before adding tags
        await _context.SaveChangesAsync();

        var picId = _context.Pictures.FirstOrDefaultAsync(p => p.Guid == picture.Guid).Id;
        await _tagService.AddTags(picId, newPictureDto.Tags);
    }


    public async Task DeletePicture(int id)
    {
        var picture = _context.Pictures.FirstOrDefault(p => p.Id == id);
        if (picture == null)
            throw new NotFoundException("Picture not found");

        _loggerService.Log($"Picture {picture.Name} deleted");

        _context.Pictures.Remove(picture);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePicture(int id, UpdatePictureDto dto)
    {
        var picture = _context.Pictures.FirstOrDefault(p => p.Id == id);
        if (picture == null)
            throw new NotFoundException("Picture not found");

        StringBuilder sb = new($"Picture {id} updated: ");
        if (dto.Name != null)
        {
            picture.Name = dto.Name;
            sb.Append($"Name changed to {dto.Name}");
        }

        if (dto.Photographer != null)
        {
            picture.Photographer = dto.Photographer;
            sb.Append($"Photographer changed to {dto.Photographer}");
        }

//        var tags = await GetTags(newPictureDto.Tags);
        _loggerService.Log(sb.ToString());
        //TODO Remove old tags and add new tags 
        await _context.SaveChangesAsync();
    }
}