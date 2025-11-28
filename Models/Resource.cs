/**************************************************************************
 * File: Resource.cs
 * Author: Maryam Elhamidi
 *
 * Description:
 *     Represents a clinical resource managed by Admins.
 *     Includes medical equipment, rooms, supplies, and other assets.
 *
 * Last Modified: Nov 26, 2025
 **************************************************************************/

namespace MediScope.Models
{
    /// <summary>
    /// Represents a healthcare resource such as equipment,
    /// medical supplies, or facility rooms.
    /// Managed through the Admin → Resources module.
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// Primary key of the resource record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Resource name (e.g., "MRI Machine", "Surgical Mask").
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Category/type of resource (e.g., "Equipment", "Room", "Supplies").
        /// </summary>
        public string Type { get; set; } = "";

        /// <summary>
        /// Inventory count for this resource (numeric).
        /// </summary>
        public int Quantity { get; set; }
    }
}