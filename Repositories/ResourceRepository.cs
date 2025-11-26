using MediScope.Models;

namespace MediScope.Repositories
{
    public class ResourceRepository : Repository<Resource>
    {
        public ResourceRepository(MediScopeContext context) : base(context)
        {
        }
    }
}