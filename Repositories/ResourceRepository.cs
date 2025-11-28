/**************************************************************************
 * File: ResourceRepository.cs
 * Author: Maryam Elhamidi
 *
 * Description:
 *     Repository wrapper for CRUD operations on Resource entities.
 *     Used by the Admin → Resources module.
 *
 * Last Modified: Nov 26, 2025
 **************************************************************************/

using MediScope.Models;

namespace MediScope.Repositories
{
    /// <summary>
    /// Provides CRUD operations for clinical resources.
    /// Inherits standard Repository behaviour.
    /// </summary>
    public class ResourceRepository : Repository<Resource>
    {
        public ResourceRepository(MediScopeContext context) : base(context)
        {
        }
    }
}