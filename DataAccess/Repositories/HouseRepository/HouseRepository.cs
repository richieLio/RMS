using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<House> GetHouseById(Guid? houseId)
        {
            return await _context.Houses.FindAsync(houseId);
        }

        public async Task<int?> GetRoomQuantityByHouseId(Guid houseId)
        {
            var house = await _context.Houses.FindAsync(houseId);
            return house?.RoomQuantity;
        }

    }
}
