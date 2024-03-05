using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using DataAccess.Repositories.HouseRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.RoomRepository
{
    public class RoomRepository : Repository<Room>, IRoomRepository
    {
        private readonly HouseManagementContext _context;
        public RoomRepository(HouseManagementContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> AddUserToRoom(Guid userId, Guid roomId)
        {
            var room = await Get(roomId);
            if (room == null)
                return false; 

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return false; 

            
            if (room.Users.Any(u => u.Id == userId))
                return false; 
            room.Users.Add(user);
            await Update(room);

            return true; 
        }
        public async Task<List<User>> GetCustomersByRoomId(Guid roomId)
        {
            return await _context.Users
                .Where(u => u.Rooms.Any(r => r.Id == roomId))
                .ToListAsync();
        }

    }
}
