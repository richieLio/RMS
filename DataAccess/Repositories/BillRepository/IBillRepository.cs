using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;

namespace DataAccess.Repositories.BillRepository
{
    public interface IBillRepository : IRepository<Bill>
    {
        Task<bool> AddServicesToBill(Guid billId, Dictionary<Guid, decimal> serviceQuantities);
        Task<Bill> GetBillDetails(Guid userId, Guid billId);
        Task<IEnumerable<Bill>> GetBillsByUserId(Guid userId);
        Task<List<BillService>> GetBillServicesForBill(Guid billId);
        }
}
