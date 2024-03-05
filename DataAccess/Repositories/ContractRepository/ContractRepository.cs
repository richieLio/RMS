using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.ContractRepository
{
    public class ContractRepository : Repository<Contract>, IContractRepository
    {
        private readonly HouseManagementContext _context;

        public ContractRepository(HouseManagementContext context) : base(context)
        {
            _context = context;
        }
    }
}
