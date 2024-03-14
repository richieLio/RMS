using DataAccess.Models.HouseModel;
using DataAccess.ResultModel;

namespace BussinessObject.Services.HouseServices
{
    public interface IHouseServices
    {
        public Task<ResultModel> GetHousesByUserId(int page, Guid UserId);
        public Task<ResultModel> AddHouse(Guid ownerId, HouseCreateReqModel formData);
        public Task<ResultModel> UpdateHouse(Guid ownerId, HouseUpdateReqModel houseUpdateReqModel);
        public Task<ResultModel> UpdateHouseStatus(Guid ownerId, HouseUpdateStatusReqModel houseUpdateStatusReqModel);
        public Task<ResultModel> GetHouseById(Guid userId, Guid houseId);
    }
}

