using MediScope.Models;

namespace MediScope.Services;

public class LoggingService
{
    private readonly MediScopeContext _context;

    public LoggingService(MediScopeContext context)
    {
        _context = context;
    }

    public async Task AddAsync(string description)
    {
        var log = new Log { Description = description };
        await _context.Logs.AddAsync(log);
        await _context.SaveChangesAsync();
    }
}

