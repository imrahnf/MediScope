/**************************************************************************
 * File: LoggingService.cs
 * Author: Omrahn Faqiri
 *
 * Description:
 *      This file contains the LoggingService class, which provides functionality
 *      to log events and actions within the MediScope application.
 *      It interacts with the MediScopeContext to store log entries in the database.
 *      The service includes methods to add new log entries asynchronously.
 *
 * Last Modified: Nov 27, 2025
 **************************************************************************/
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

