/**************************************************************************
 * File: Repository.cs
 * Author: Omrahn Faqiri
 *
 * Description:
 *     This class implements a generic repository pattern for managing entities
 *     in the MediScope application. It provides concrete implementations for common
 *     data operations such as retrieving all entities, getting an entity by its ID,
 *     adding a new entity, deleting an entity, and saving changes to the data store.
 *
 * Last Modified: Nov 27, 2025
 **************************************************************************/

using MediScope.Models;
using Microsoft.EntityFrameworkCore;

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