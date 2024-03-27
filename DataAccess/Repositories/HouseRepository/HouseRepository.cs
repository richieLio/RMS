using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.HouseRepository
{
    public class HouseRepository : Repository<House>, IHouseRepository
    {
        private readonly HouseManagementContext _context;
        public HouseRepository(HouseManagementContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<House>> GetAllHouseByUserId(Guid userId)
        {
            return await _context.Houses
                .Where(h => h.OwnerId == userId)
                .ToListAsync();
        }

        public async Task<int> GetAvailableRoomByHouseId(Guid houseId)
        {
            var availableRoomCount = _context.Rooms
                 .Include(r => r.Users)
                 .Count(r => r.HouseId == houseId && !r.Users.Any());

            return availableRoomCount;
        }

        public async Task<House> GetHouseById(Guid? houseId)
        {
            return await _context.Houses.FindAsync(houseId);
        }

        public async Task<int?> GetRoomQuantityByHouseId(Guid houseId)
        {
            var house = await _context.Houses.FindAsync(houseId);
            return house?.RoomQuantity;
        }
        public async Task<House> GetHouseByName(string name)
        {
            return await _context.Houses.FirstOrDefaultAsync(h => h.Name == name);
        }
        public async Task<List<object>> GetHouseRevenueForPeriod(Guid userId, DateTime startDate, DateTime endDate)
        {
            var houseRevenueList = new List<object>();

            var houses = await GetAllHouseByUserId(userId);

            foreach (var house in houses)
            {
                var houseBills = await _context.Bills
                    .Where(b => b.Room != null && b.Room.HouseId == house.Id && b.Month >= startDate && b.Month <= endDate)
                    .ToListAsync();

                var houseRevenue = houseBills.Sum(b => b.TotalPrice ?? 0);

                var houseData = new
                {
                    HouseId = house.Id,
                    HouseName = house.Name,
                    HouseTotalRevenue = houseRevenue
                };

                houseRevenueList.Add(houseData);
            }

            return houseRevenueList;
        }

        public async Task<string> GetHouseName(Guid houseId)
        {
            return await _context.Houses
                                .Where(h => h.Id == houseId)
                                .Select(h => h.Name)
                                .FirstOrDefaultAsync();
        }

    }
}
