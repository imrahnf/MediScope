/**************************************************************************
 * File: Department.cs
 * Author: Maryam Elhamidi
 *
 * Description:
 *     Represents a hospital department (e.g., Cardiology, Neurology).
 *     Admins can create, edit, and delete departments from the dashboard.
 *     Doctors can be assigned to a department (via FK in Doctor.cs).
 *
 * Last Modified: Nov 26, 2025
 **************************************************************************/

namespace MediScope.Models
{
    /// <summary>
    /// Represents a hospital or clinic department.
    /// Every doctor may optionally belong to a department.
    /// </summary>
    public class Department
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The department's display name (e.g., "Cardiology").
        /// </summary>
        public string Name { get; set; } = "";
    }
}