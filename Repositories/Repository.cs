using MediScope.Models;
using Microsoft.EntityFrameworkCore;

namespace MediScope.Repositories;

public class Repository<T>(MediScopeContext context) : IRepository<T>  where T : class
{
    private MediScopeContext _context = context;
    private DbSet<T> _dbSet = context.Set<T>();

    public async Task<IEnumerable<T>> GetAll() => await _dbSet.ToListAsync();
    public async Task<T?> GetById(int id) => await _dbSet.FindAsync(id);
    public async Task Add(T entity) => await _dbSet.AddAsync(entity);
    public void Delete(T entity) => _dbSet.Remove(entity);
    public async Task Save() => await _context.SaveChangesAsync();
}