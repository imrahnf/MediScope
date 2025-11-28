/**************************************************************************
 * File: DepartmentRepository.cs
 * Author: Maryam Elhamidi
 *
 * Description:
 *     Repository wrapper for CRUD operations on Department entities.
 *     Inherits generic Repository<T> for standard functionality.
 *
 * Last Modified: Nov 26, 2025
 **************************************************************************/

using MediScope.Models;

namespace MediScope.Repositories
{
    /// <summary>
    /// Provides access to Department CRUD operations.
    /// Inherits standard Repository behaviour.
    /// </summary>
    public class DepartmentRepository : Repository<Department>
    {
        public DepartmentRepository(MediScopeContext context) : base(context)
        {
        }
    }
}