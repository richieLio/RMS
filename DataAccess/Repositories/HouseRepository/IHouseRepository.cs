using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.HouseRepository
{
    public interface IHouseRepository : IRepository<House>
    {
        public Task<IEnumerable<House>> GetAllHouseByUserId(Guid UserId);
        public Task<House> GetHouseById(Guid? houseId);
        Task<int?> GetRoomQuantityByHouseId(Guid houseId);

    }
}
