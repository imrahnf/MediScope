using MediScope.Models;

namespace MediScope.Services;

public class AuthenticationService
{
    private readonly MediScopeContext _context;
    
    public AuthenticationService(MediScopeContext dbContext)
    {
        _context = dbContext;
    }
}