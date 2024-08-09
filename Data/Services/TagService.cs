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

    //TODO check if function needed
    public async Task<List<Tag>> GetTags(List<string> oldTags)
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
            _context.Tags.AddRange(newTags);
            _context.SaveChanges();
        }

        return await _context.Tags.Where(t => newTags.Any(tg => tg.Name == t.Name)).ToListAsync();
    }

    public async Task RemoveTags(int pictureID, IList<string> tags)
    {
        if (tags.Count == 0) return;
        lock (lockObj)
        {
            var pictureTags = _context.PictureTags.Where(pt => pt.PictureId == pictureID).ToList();
            _context.PictureTags.RemoveRange(pictureTags);
            _context.SaveChanges();
        }
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