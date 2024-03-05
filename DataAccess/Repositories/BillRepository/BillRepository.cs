using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.BillRepository
{
    public class BillRepository : Repository<Bill>, IBillRepository
    {
        public BillRepository(HouseManagementContext context) : base(context)
        {
        }
    }
}
