using MediScope.Models;

namespace MediScope.Services
{
    public class ValidationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";

        public static ValidationResult Ok() => new ValidationResult { Success = true };
        public static ValidationResult Fail(string msg) => new ValidationResult { Success = false, Message = msg };
    }

    public class ValidationService
    {
        private readonly MediScopeContext _context;

        public ValidationService(MediScopeContext context)
        {
            _context = context;
        }

        // ADMIN: Validate Creating/Updating a Doctor
        public ValidationResult ValidateDoctorCreation(string name, string specialty)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ValidationResult.Fail("Doctor name cannot be empty.");

            if (string.IsNullOrWhiteSpace(specialty))
                return ValidationResult.Fail("Doctor specialty cannot be empty.");

            if (name.Length < 3)
                return ValidationResult.Fail("Doctor name must be at least 3 characters.");

            if (specialty.Length < 3)
                return ValidationResult.Fail("Specialty must be at least 3 characters.");

            return ValidationResult.Ok();
        }

        // ADMIN: Validate Department Creation 
        public ValidationResult ValidateDepartment(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ValidationResult.Fail("Department name cannot be empty.");

            if (name.Length < 3)
                return ValidationResult.Fail("Department name must be at least 3 characters.");

            bool exists = _context.Departments.Any(d => d.Name.ToLower() == name.ToLower());
            if (exists)
                return ValidationResult.Fail("A department with this name already exists.");

            return ValidationResult.Ok();
        }

        // ADMIN: Validate Resource Creation 
        public ValidationResult ValidateResource(string name, string type, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ValidationResult.Fail("Resource name cannot be empty.");

            if (string.IsNullOrWhiteSpace(type))
                return ValidationResult.Fail("Resource type cannot be empty.");

            if (quantity < 0)
                return ValidationResult.Fail("Quantity cannot be negative.");

            return ValidationResult.Ok();
        }

        // Prevent Duplicate Resource Name
        public ValidationResult ValidateUniqueResourceName(string name)
        {
            bool exists = _context.Resources.Any(r => r.Name.ToLower() == name.ToLower());

            if (exists)
                return ValidationResult.Fail("A resource with this name already exists.");

            return ValidationResult.Ok();
        }

        // Validate Doctor-to-Department assignment (used in Phase 7)
        public ValidationResult ValidateDepartmentExists(int departmentId)
        {
            bool exists = _context.Departments.Any(d => d.Id == departmentId);

            return exists
                ? ValidationResult.Ok()
                : ValidationResult.Fail("Selected department does not exist.");
        }

        // Prevent deleting departments with assigned doctors
        public ValidationResult ValidateDepartmentDeletion(int departmentId)
        {
            bool hasDoctors = _context.Doctors.Any(d => d.Specialty == departmentId.ToString());
            // NOTE: Once doctor–department relationship is added, change this condition.

            if (hasDoctors)
                return ValidationResult.Fail("Cannot delete department — doctors are assigned to it.");

            return ValidationResult.Ok();
        }

        // Validate feedback messages -- not using unless imrahn wanna use it 
        public ValidationResult ValidateFeedback(string message, int rating)
        {
            if (string.IsNullOrWhiteSpace(message))
                return ValidationResult.Fail("Feedback message cannot be empty.");

            if (rating < 1 || rating > 5)
                return ValidationResult.Fail("Rating must be between 1 and 5.");

            return ValidationResult.Ok();
        }
    }
}
