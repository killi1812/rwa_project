using System.Text;
using Data.Dto;
using Data.Helpers;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace Data.Services;

public interface IPictureServices
{
    Task<IList<Picture>> GetPictures(int page = 1, int n = 10);
    Task<Picture> GetPicture(Guid guid);
    Task DeletePicture(Guid guid);
    Task UpdatePicture(Guid guid, UpdatePictureDto dto);
    Task<Picture> CreatePicture(NewPictureDto newPictureDto, Guid guid);
    Task<Pagineted<Picture>> SearchPictures(string query, int page = 1, int n = 10);
    Task<Byte[]> GetPictureData(Guid guid);
    Task DownloadPicture(Guid guid);
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
            .Include(p => p.User)
            .Include(p => p.PictureTags)
            .ThenInclude(pt => pt.Tag)
            .ToListAsync();
        return pictures;
    }

    public async Task<Picture> GetPicture(Guid guid)
    {
        var picture = await _context.Pictures
            .Include(p => p.User)
            .Include(p => p.PictureTags)
            .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Guid == guid);

        if (picture == null)
            throw new NotFoundException("Picture not found");

        _loggerService.Log($"User ${picture.User.Username} gets picture with guid {guid}");
        return picture;
    }

    public async Task<Picture> CreatePicture(NewPictureDto newPictureDto, Guid guid)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Guid == guid);
        if (user == null)
            throw new NotFoundException("User not found");

        if (!user.Admin)
            throw new UnauthorizedException($"User {user.Username} is not an admin");

        var picture = new Picture
        {
            Name = $"{newPictureDto.Name}.{newPictureDto.Data.ContentType.Split("/")[1]}",
            Photographer = newPictureDto.Photographer,
            UserId = user.Id,
        };

        lock (picture)
        {
            byte[] data = new byte[newPictureDto.Data.Length];
            newPictureDto.Data.OpenReadStream().Read(data);
            picture.PictureByte = new PictureByte
            {
                Data = data,
            };
            _context.Pictures.Add(picture);
            _context.PictureBytes.Add(picture.PictureByte);
            _context.SaveChanges();
        }

        _loggerService.Log($"User {user.Username} created picture {picture.Name}");
        var pic = _context.Pictures.FirstOrDefault(p => p.Guid == picture.Guid);
        if (pic == null)
            throw new NotFoundException("Picture not found");

        await _tagService.AddTagsToPicture(pic.Id, newPictureDto.Tags);
        return pic;
    }

    public async Task<Pagineted<Picture>> SearchPictures(string query, int page = 1, int n = 10)
    {
        var pic = _context.Pictures
            .Where(p =>
                p.Name.Contains(query) ||
                p.Photographer.Contains(query) ||
                p.PictureTags.Any(pt => pt.Tag.Name.Contains(query))
            )
            .Include(p => p.PictureTags)
            .ThenInclude(pt => pt.Tag)
            .Include(p => p.User);
        var picsPaginated = new Pagineted<Picture>(pic, page, n);
        return picsPaginated;
    }

    public async Task<byte[]> GetPictureData(Guid guid)
    {
        var picture = await _context.Pictures.Where(p => p.Guid == guid)
            .Include(p => p.PictureByte)
            .FirstOrDefaultAsync();
        if (picture == null)
            throw new NotFoundException("Picture not found");
        _loggerService.Log($"Picture data requested {picture.Name}");
        return picture.PictureByte.Data;
    }

    public Task DownloadPicture(Guid guid)
    {
        throw new NotImplementedException();
    }

    public async Task DeletePicture(Guid guid)
    {
        var picture = _context.Pictures.FirstOrDefault(p => p.Guid == guid);
        if (picture == null)
            throw new NotFoundException("Picture not found");

        _loggerService.Log($"Picture {picture.Name} deleted");

        _context.Pictures.Remove(picture);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePicture(Guid guid, UpdatePictureDto dto)
    {
        var picture = _context.Pictures.FirstOrDefault(p => p.Guid == guid);
        if (picture == null)
            throw new NotFoundException("Picture not found");

        StringBuilder sb = new($"Picture {guid} updated: ");
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

        //TODO Remove old tags and add new tags 
//        var tags = await GetTags(newPictureDto.Tags);
        _loggerService.Log(sb.ToString());
        await _context.SaveChangesAsync();
    }
}