using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.ServiceFeeRepository
{
    public class ServiceFeeRepository : Repository<Service>, IServiceFeeRepository
    {
        private readonly HouseManagementContext _context;
        public ServiceFeeRepository(HouseManagementContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Service>> GetAllServices()
        {
            return await _context.Services.ToListAsync();
        }
    }
}
