using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;

namespace DataAccess.Repositories.BillRepository
{
    public interface IBillRepository : IRepository<Bill>
    {
        Task<bool> AddServicesToBill(Guid billId, Dictionary<Guid, decimal> serviceQuantities);
    }
}
