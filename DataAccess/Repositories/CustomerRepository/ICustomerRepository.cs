using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;

namespace DataAccess.Repositories.CustomerRepository
{
    public interface ICustomerRepository : IRepository<House>
    {
        Task<User> GetCustomerByUserId(Guid? userId);
    }
}
