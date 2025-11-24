using MediScope.Models;

namespace MediScope.Repositories;

public class PatientRepository : Repository<Patient>
{
    public PatientRepository(MediScopeContext context) : base(context) {}
    
}