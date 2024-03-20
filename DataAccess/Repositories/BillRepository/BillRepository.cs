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



        public async Task<Bill> GetBillDetails(Guid userId, Guid billId)
        {
            var billDetails = await (from bill in _context.Bills
                                     where bill.Id == billId && bill.CreateBy == userId
                                     join room in _context.Rooms on bill.RoomId equals room.Id
                                     join user in _context.Users on bill.CreateBy equals user.Id
                                     join billService in _context.BillServices on bill.Id equals billService.BillId into services
                                     select new Bill
                                     {
                                         Id = bill.Id,
                                         Room = room,
                                         CreateBy = user.Id,
                                         Month = bill.Month,
                                         PaymentDate = bill.PaymentDate,
                                         TotalPrice = bill.TotalPrice,
                                         BillServices = services.ToList()
                                     }).FirstOrDefaultAsync();

            return billDetails;
        }



        public async Task<IEnumerable<Bill>> GetBillsByUserId(Guid userId)
        {
            return await _context.Bills.Where(b => b.CreateBy == userId).ToListAsync();
        }
        public async Task<List<BillService>> GetBillServicesForBill(Guid billId)
        {

            return await _context.BillServices
                .Where(bs => bs.BillId == billId)
                .Include(bs => bs.Service)
                .ToListAsync();
        }

        


    }
}
