using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using DataAccess.Repositories.RoomRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.ServiceRepository
{
    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        private readonly HouseManagementContext _context;
        public ServiceRepository(HouseManagementContext context) : base(context)
        {
            _context = context;
        }

    }
}
