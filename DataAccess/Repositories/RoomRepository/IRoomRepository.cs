using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;

namespace DataAccess.Repositories.RoomRepository
{
    public interface IRoomRepository : IRepository<Room>
    {
        Task<bool> AddUserToRoom(Guid userId, Guid roomId);
        Task<List<User>> GetCustomersByRoomId(Guid roomId);
        Task<IEnumerable<Room>> GetRooms(Guid houseId);
        Task<Room?> GetRoomById(Guid? roomId);
        public Task<bool> IsCustomerInRoom(Guid customerId, Guid roomId);
        Task<Room> GetRoomByName(string name);

    }
}
