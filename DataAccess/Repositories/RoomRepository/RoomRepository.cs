using Data.Enums;
using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.RoomRepository
{
    public class RoomRepository : Repository<Room>, IRoomRepository
    {
        private readonly HouseManagementContext _context;
        private readonly DbSet<Room> _rooms;
        public RoomRepository(HouseManagementContext context) : base(context)
        {
            _context = context;
            _rooms = context.Set<Room>();
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

        public async Task<IEnumerable<Room>> GetRooms(Guid houseId)
        {
            return await _rooms
                .Where(r => r.HouseId == houseId && r.Status == GeneralStatus.ACTIVE)
                .ToListAsync();
        }


        public async Task<Room?> GetRoomById(Guid roomId)
        {
            return await _rooms
                .FirstOrDefaultAsync(r => r.Id == roomId);
        }
        public async Task<bool> IsCustomerInRoom(Guid customerId, Guid roomId)
        {
            return await _context.Users
                .AnyAsync(u => u.Id == customerId && u.Rooms.Any(r => r.Id == roomId));
        }
    }
}
