using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Services;

public interface ILoggerService
{
    public Task Log(string message);
    public Task LogMany(string[] messages);
    public Task<IList<Log>> GetLogs(int page, int n = 10);
}

public class LoggerService : ILoggerService
{
    private readonly RwaContext _context;
    private readonly object objLock = new();

    public LoggerService(RwaContext context)
    {
        _context = context;
    }

    public Task Log(string message)
    {
        var log = new Log
        {
            Message = message,
        };

        lock (log)
        {
            _context.Logs.Add(log);
            _context.SaveChanges();
        }

        return Task.CompletedTask;
    }

    public Task LogMany(string[] messages)
    {
        var logs = new List<Log>(messages.Length);
        foreach (var m in messages)
        {
            logs.Add(new()
            {
                Message = m
            });
        }

        lock (logs)
        {
            _context.Logs.AddRange(logs);
            _context.SaveChanges();
        }

        return Task.CompletedTask;
    }

    public async Task<IList<Log>> GetLogs(int page, int n = 10)
    {
        var logs = await _context.Logs
            .OrderByDescending(l => l.Id)
            .Skip((page - 1) * n)
            .Take(n)
            .ToListAsync();
        return logs;
    }
}