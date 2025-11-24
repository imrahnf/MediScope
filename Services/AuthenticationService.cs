using MediScope.Models;

namespace MediScope.Services;

public class AuthenticationService
{
    private readonly MediScopeContext _context;
    
    public AuthenticationService(MediScopeContext dbContext)
    {
        _context = dbContext;
    }

    public User? ValidateUser(string username, string password)
    {
        return _context.Users.FirstOrDefault(p => p.Username == username && p.Password == password);
    }
}