using AutoMapper.Internal;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Services;

public interface ITagService
{
    public Task<List<Tag>> CreateNewTags(IList<string> tags);
    public Task RemoveTags(int pictureID, IList<string> tags);
    public Task UpdateTags(int pictureID, IList<string> tags);

    public Task AddTags(int pictureID, IList<string> tags);
}

public class TagService : ITagService
{
    private readonly RwaContext _context;
    private readonly ILoggerService _loggerService;
    private readonly object lockObj = new object();

    public TagService(RwaContext context, ILoggerService loggerService)
    {
        _context = context;
        _loggerService = loggerService;
    }

    public async Task<List<Tag>> CreateNewTags(IList<string> tags)
    {
        if (tags.Count == 0) return new List<Tag>();

        var newTags = new List<Tag>(tags.Count);
        var logs = new List<string>(tags.Count);

        foreach (var tag in tags)
        {
            newTags.Add(new Tag { Name = tag });
            logs.Add($"Created a new tag: {tag}");
        }

        _loggerService.LogMany(logs.ToArray());

        lock (lockObj)
        {
            _context.Tags.AddRangeAsync(newTags);
            _context.SaveChangesAsync();
        }

        return await _context.Tags.Where(t => newTags.Any(tg => tg.Name == t.Name)).ToListAsync();
    }

    public async Task RemoveTags(int pictureID, IList<string> tags)
    {
        if (tags.Count == 0) return;

        var oldTags = new List<Tag>(tags.Count);
        var logs = new List<string>(tags.Count);

        var da = await _context.PictureTags.Where(pt => tags.Any(t => t == pt.Tag.Name)).ToListAsync();

        throw new NotImplementedException();
    }

    public async Task UpdateTags(int pictureID, IList<string> tags)
    {
        if (tags.Count == 0) return;
        var existingTags = await _context.Tags.Where(t => tags.Any(tg => tg == t.Name)).ToListAsync();

        throw new NotImplementedException();
    }

    public async Task AddTags(int pictureID, IList<string> tags)
    {
        if (tags.Count == 0) return;

        var existingTags = await _context.Tags.Where(t => !tags.Any(tg => tg == t.Name)).ToListAsync();
        List<Tag> newTags = await CreateNewTags(tags.Where(t => !existingTags.Any(et => et.Name == t)).ToList());

        var tagsToAdd = existingTags.Concat(newTags);
        if (!tagsToAdd.Any()) return;
        List<PictureTag> pictureTags = new();

        foreach (var tag in tagsToAdd)
        {
            pictureTags.Add(new PictureTag
            {
                PictureId = pictureID,
                TagId = tag.Id
            });
        }

        await _context.PictureTags.AddRangeAsync(pictureTags);
        await _context.SaveChangesAsync();
    }
}