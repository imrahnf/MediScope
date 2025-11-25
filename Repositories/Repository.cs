using MediScope.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediScope.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly MediScopeContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(MediScopeContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAll() => await _dbSet.ToListAsync();
        public async Task<T?> GetById(int id) => await _dbSet.FindAsync(id);
        public async Task Add(T entity) => await _dbSet.AddAsync(entity);
        public void Delete(T entity) => _dbSet.Remove(entity);
        public async Task Save() => await _context.SaveChangesAsync();
    }
}