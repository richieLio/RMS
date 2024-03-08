using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.RoomRepository
{
    public interface IRoomRepository : IRepository<Room>
    {
        Task<bool> AddUserToRoom(Guid userId, Guid roomId);
        Task<List<User>> GetCustomersByRoomId(Guid roomId);
        Task<IEnumerable<Room>> GetRooms();
        Task<Room?> GetRoomById(Guid roomId);
    }
}
