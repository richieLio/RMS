using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;

namespace DataAccess.Repositories.HouseRepository
{
    public interface IHouseRepository : IRepository<House>
    {
        public Task<IEnumerable<House>> GetAllHouseByUserId(Guid UserId);
        public Task<House> GetHouseById(Guid? houseId);
        Task<int?> GetRoomQuantityByHouseId(Guid houseId);

        Task<int> GetAvailableRoomByHouseId(Guid houseId);
        Task<House> GetHouseByName(string name);
        Task<List<object>> GetHouseRevenueForPeriod(Guid userId, DateTime startDate, DateTime endDate);
    }
}
