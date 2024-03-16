using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.BillRepository
{
    public class BillRepository : Repository<Bill>, IBillRepository
    {
        private readonly HouseManagementContext _context;
        public BillRepository(HouseManagementContext context) : base(context)
        {
            _context = context;
        }
        public async Task<bool> AddServicesToBill(Guid billId, Dictionary<Guid, decimal> serviceQuantities)
        {
            var bill = await Get(billId);
            if (bill == null)
                return false;
            decimal? totalPrice = 0;
            foreach (var (serviceId, quantity) in serviceQuantities)
            {
                var service = _context.Services.FirstOrDefault(s => s.Id == serviceId);
                if (service == null)
                    return false;

                if (bill.BillServices.Any(s => s.ServiceId == serviceId))
                    return false;

                bill.BillServices.Add(new BillService
                {
                    BillId = billId,
                    ServiceId = serviceId,
                    Quantity = quantity
                });
                totalPrice += quantity * service.Price;
            }
            bill.TotalPrice = totalPrice;

            await Update(bill);

            return true;
        }

    }
}
