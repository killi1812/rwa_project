using Data.Helpers;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Services;

public interface ILoggerService
{
    public Task Log(string message, ThreatLvl lvl = ThreatLvl.Low);
    public Task LogMany(string[] messages, ThreatLvl lvl = ThreatLvl.Low);
    public Task<IList<Log>> GetLogs(int page, int n = 10);
    Task<int> GetLogsCount();
}

public class LoggerService : ILoggerService
{
    private readonly RwaContext _context;

    public LoggerService(RwaContext context)
    {
        _context = context;
    }

    public Task Log(string message, ThreatLvl lvl = ThreatLvl.Low)
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

    public Task LogMany(string[] messages, ThreatLvl lvl = ThreatLvl.Low)
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

    public async Task<int> GetLogsCount()
    {
        var count = await _context.Logs.CountAsync();
        return count;
    }
}