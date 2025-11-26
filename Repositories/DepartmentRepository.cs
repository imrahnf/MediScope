using MediScope.Models;

namespace MediScope.Repositories
{
    public class DepartmentRepository : Repository<Department>
    {
        public DepartmentRepository(MediScopeContext context) : base(context)
        {
        }
    }
}