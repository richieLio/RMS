using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;

namespace DataAccess.Repositories.BillRepository
{
    public class BillRepository : Repository<Bill>, IBillRepository
    {
        public BillRepository(HouseManagementContext context) : base(context)
        {
        }
    }
}
