using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.ServiceFeeRepository
{
    public interface IServiceFeeRepository : IRepository<Service>
    {
        public Task<IEnumerable<Service>> GetAllServices();
    }
}
