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
        public ValidationResult ValidateDepartment(string deptName)
        {
            if (string.IsNullOrWhiteSpace(deptName))
                return ValidationResult.Fail("Department name cannot be empty.");

            var exists = _context.Departments
                .Any(d => d.Name.ToLower() == deptName.ToLower());

            if (exists)
                return ValidationResult.Fail("A department with this name already exists.");

            return ValidationResult.Ok();
        }

        // ADMIN: Validate Resource Creation 
        public ValidationResult ValidateResource(string name, string type)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ValidationResult.Fail("Resource name cannot be empty.");

            if (string.IsNullOrWhiteSpace(type))
                return ValidationResult.Fail("Resource type cannot be empty.");

            return ValidationResult.Ok();
        }
    }
}
