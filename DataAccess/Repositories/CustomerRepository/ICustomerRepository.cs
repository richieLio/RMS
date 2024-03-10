using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.CustomerRepository
{
    public interface ICustomerRepository : IRepository<House>
    {
        Task<House> GetHouseByAccountName(string name);
        Task<User> GetCustomerByUserId(Guid? userId);
    }
}
