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
    Task UpdatePicture(int id, NewPictureDto newPictureDto);
    Task CreatePicture(NewPictureDto newPictureDto, int id);
}

public class PictureServices : IPictureServices
{
    private readonly RwaContext _context;

    public PictureServices(RwaContext context)
    {
        _context = context;
    }

    public async Task<IList<Picture>> GetPictures(int page = 1, int n = 10)
    {
        var pictures = await _context.Pictures
            .Skip(page * n)
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
        //TODO check if works
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

        await _context.Pictures.AddAsync(picture);
        //TODO check if there need to be save changes before adding tags
        await _context.SaveChangesAsync();
        //TODO add ading tags
        // var pictureTags = new List<PictureTag>();
        // var tags = await GetTags(newPictureDto.Tags);
        // foreach (var tag in tags)
        // {
        //     pictureTags.Add(new PictureTag { PictureId = picture.Id, TagId = tag.Id });
        // }
        //
        // await _context.PictureTags.AddRangeAsync(pictureTags);
        // await _context.SaveChangesAsync();
    }

    private async Task<List<Tag>> GetTags(List<string> oldTags)
    {
        var tags = _context.Tags.Where(t => oldTags.Contains(t.Name.ToLower())).ToList();

        //TODO change all to select onaly those that are not in tags
        var newTags = oldTags
            .Where(t => tags.All(tag => tag.Name.ToLower() != t.ToLower()))
            .Select(t => new Tag { Name = t.ToLower() })
            .ToList();

        await _context.Tags.AddRangeAsync(newTags);
        await _context.SaveChangesAsync();
        return tags.Concat(newTags).ToList();
    }

    public async Task DeletePicture(int id)
    {
        var picture = _context.Pictures.FirstOrDefault(p => p.Id == id);
        if (picture == null)
            throw new NotFoundException("Picture not found");

        _context.Pictures.Remove(picture);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePicture(int id, NewPictureDto newPictureDto)
    {
        var picture = _context.Pictures.FirstOrDefault(p => p.Id == id);
        if (picture == null)
            throw new NotFoundException("Picture not found");

        //TODO make null checks
        picture.Name = newPictureDto.Name;
        picture.Photographer = newPictureDto.Photographer;
        picture.UserId = id;
        var tags = await GetTags(newPictureDto.Tags);

        //TODO Remove old tags and add new tags 
        await _context.SaveChangesAsync();
    }
}