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
    Task<Picture> CreatePicture(NewPictureDto newPictureDto, int id);
    Task<List<Picture>> SearchPictures(string query);
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

    public async Task<Picture> CreatePicture(NewPictureDto newPictureDto, int id)
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

        lock (picture)
        {
            byte[] data = new byte[newPictureDto.Data.Length];
            newPictureDto.Data.OpenReadStream().Read(data);
            picture.Data = data;


            _context.Pictures.Add(picture);
            _context.SaveChanges();
        }

        _loggerService.Log($"User {user.Username} created picture {picture.Name}");
        var pic = _context.Pictures.FirstOrDefault(p => p.Guid == picture.Guid);
        if (pic == null)
            throw new NotFoundException("Picture not found");

        await _tagService.AddTagsToPicture(pic.Id, newPictureDto.Tags);
        return pic;
    }

    public async Task<List<Picture>> SearchPictures(string query)
    {
        //TODO test
        var pics = await _context.PictureTags
            .Where(pt =>
                pt.Picture.Name.Contains(query) ||
                pt.Picture.Photographer.Contains(query) ||
                pt.Tag.Name.Contains(query)
            )
            .Include(pt => pt.Tag)
            .Include(pt => pt.Picture)
            .Select(pt => pt.Picture)
            .ToListAsync();
        return pics;
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