using MediScope.Models;

namespace MediScope.Repositories;

public class AppointmentRepository : Repository<Appointment>
{
    public AppointmentRepository(MediScopeContext dbContext) : base(dbContext) { }
}