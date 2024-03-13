using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;

namespace DataAccess.Repositories.CustomerRepository
{
    public interface ICustomerRepository : IRepository<House>
    {
        Task<House> GetHouseByAccountName(string name);
        Task<User> GetCustomerByUserId(Guid? userId);
    }
}
