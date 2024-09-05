using Data.Helpers;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Services;

public interface ITagService
{
    public List<Tag> CreateNewTags(IList<string> tags);
    public Task RemoveTags(int pictureID, IList<string> tags);
    public Task UpdateTags(int pictureID, IList<string> tags);

    public Task AddTagsToPicture(int pictureID, IList<string> tags);
    public Task<List<Tag>> GetTags();
    public Task DeleteTag(Guid guid);
}

public class TagService : ITagService
{
    private readonly RwaContext _context;
    private readonly ILoggerService _loggerService;

    public TagService(RwaContext context, ILoggerService loggerService)
    {
        _context = context;
        _loggerService = loggerService;
    }

    public async Task<List<Tag>> GetTags()
    {
        return await _context.Tags.Include(t => t.PictureTags).ToListAsync();
    }

    public async Task DeleteTag(Guid guid)
    {
        var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Guid == guid);
        if (tag == null) throw new NotFoundException("Tag not found");
        _context.Remove(tag);
        await _context.SaveChangesAsync();
    }

    public List<Tag> CreateNewTags(IList<string> tags)
    {
        if (tags.Count == 0) return new List<Tag>();

        var newTags = new List<Tag>(tags.Count);
        var logs = new List<string>(tags.Count);

        foreach (var tag in tags)
        {
            newTags.Add(new Tag { Name = tag.Trim() });
            logs.Add($"Created a new tag: {tag}");
        }

        _loggerService.LogMany(logs.ToArray());

        lock (newTags)
        {
            _context.Tags.AddRange(newTags);
            _context.SaveChanges();
        }

        return _context.Tags.Where(t => newTags.Contains(t)).ToList();
    }

    public async Task RemoveTags(int pictureID, IList<string> tags)
    {
        if (tags.Count == 0) return;
        lock (tags)
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
        var toAdd = tags.Where(t => !existingTags.Any(et => et.Name == t)).ToList();
        var newTags = CreateNewTags(toAdd).Concat(existingTags).DistinctBy(t => t.Name).ToList();
        List<PictureTag> pTags = new();
        foreach (var tag in newTags)
        {
            pTags.Add(new PictureTag()
            {
                PictureId = pictureID,
                TagId = tag.Id
            });
        }

        var oldTags = _context.PictureTags.Where(pt => pt.PictureId == pictureID).ToList();

        lock (tags)
        {
            _context.PictureTags.RemoveRange(oldTags);
            _context.PictureTags.AddRange(pTags);
            _context.SaveChanges();
        }
    }

    public Task AddTagsToPicture(int pictureID, IList<string> tags)
    {
        if (tags.Count == 0) return Task.CompletedTask;
        lock (tags)
        {
            //TODO sometimes it add new tags even if they exist Try checking for newLine or space 
            var existingTags = _context.Tags.Where(t => tags.Contains(t.Name)).ToList();
            List<Tag> newTags = CreateNewTags(tags.Where(t => !existingTags.Any(et => et.Name == t)).ToList());

            var tagsToAdd = existingTags.Concat(newTags);
            if (!tagsToAdd.Any()) return Task.CompletedTask;
            List<PictureTag> pictureTags = new();

            foreach (var tag in tagsToAdd)
            {
                pictureTags.Add(new PictureTag
                {
                    PictureId = pictureID,
                    TagId = tag.Id
                });
            }

            _context.PictureTags.AddRange(pictureTags);
            _context.SaveChanges();
        }

        return Task.CompletedTask;
    }
}