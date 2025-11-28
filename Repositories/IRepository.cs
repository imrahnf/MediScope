/**************************************************************************
 * File: IRepository.cs
 * Author: Omrahn Faqiri
 *
 * Description:
 *     This interface defines a generic repository pattern for managing entities
 *     in the MediScope application. It provides methods for common data operations
 *     such as retrieving all entities, getting an entity by its ID, adding a new entity
 *     deleting an entity, and saving changes to the data store.
 *
 * Last Modified: Nov 27, 2025
 **************************************************************************/
namespace MediScope.Repositories;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll();
    Task<T?> GetById(int id);
    Task Add(T entity);
    void Delete(T entity);
    Task Save();
}