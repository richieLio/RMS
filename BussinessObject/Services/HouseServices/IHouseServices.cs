using DataAccess.Entities;
using DataAccess.Models.HouseModel;
using DataAccess.Models.RoomModel;
using DataAccess.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Services.HouseServices
{
    public interface IHouseServices
    {
        public Task<ResultModel> GetHousesByUserId(Guid UserId, int page);
        public Task<ResultModel> AddHouse(Guid ownerId, HouseRoomCreateReqModel formData);
        public Task<ResultModel> UpdateHouse(Guid ownerId, HouseUpdateReqModel houseUpdateReqModel);
        public Task<ResultModel> GetHouseById(Guid userId, Guid houseId);
    }
}

