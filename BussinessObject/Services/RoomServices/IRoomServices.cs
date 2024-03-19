using DataAccess.Models.RoomModel;
using DataAccess.ResultModel;

namespace BussinessObject.Services.RoomServices
{
    public interface IRoomServices
    {
        public Task<ResultModel> AddRangeRoom(Guid userId, RoomCreateRangeReqModel roomCreateReqModel);
        public Task<ResultModel> AddRoom(Guid userId, RoomCreateReqModel roomCreateReqModel);
        public Task<ResultModel> AddCustomerToRoom(Guid userId, AddCustomerToRoomReqModel addCustomerToRoomReqModel);
        public Task<ResultModel> GetCustomerByRoomId(int page,Guid userId, Guid roomId);
        Task<ResultModel> GetRoomList(int page, Guid userId, Guid houseId);
        Task<ResultModel> GetRoomInformation(Guid roomId);
        Task<ResultModel> UpdateRoom(RoomUpdateReqModel roomUpdateReqModel);
        Task<ResultModel> UpdateRoomStatus(RoomUpdateStatusReqModel roomUpdateStatusReqModel);
    }
}
