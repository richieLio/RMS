using DataAccess.Models.RoomModel;
using DataAccess.ResultModel;

namespace BussinessObject.Services.RoomServices
{
    public interface IRoomServices
    {
        public Task<ResultModel> AddRangeRoom(RoomCreateReqModel roomCreateReqModel);
        public Task<ResultModel> AddCustomerToRoom(Guid userId, AddCustomerToRoomReqModel addCustomerToRoomReqModel);
        public Task<ResultModel> GetCustomerByRoomId(Guid userId, Guid roomId);
        Task<ResultModel> GetRoomList(int page);
        Task<ResultModel> GetRoomInformation(Guid roomId);
        Task<ResultModel> UpdateRoom(RoomUpdateReqModel roomUpdateReqModel);
        Task<ResultModel> UpdateRoomStatus(RoomUpdateStatusReqModel roomUpdateStatusReqModel);
    }
}
