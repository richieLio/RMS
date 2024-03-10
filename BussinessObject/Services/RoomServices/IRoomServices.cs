using DataAccess.Models.CustomerModel;
using DataAccess.Models.HouseModel;
using DataAccess.Models.RoomModel;
using DataAccess.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Services.RoomServices
{
    public interface IRoomServices
    {
        public Task<ResultModel> AddRangeRoom(RoomCreateReqModel roomCreateReqModel);
        public Task<ResultModel> AddCustomerToRoom(Guid userId, CustomerCreateReqModel customerCreateReqModel, HouseUpdateAvaiableRoomReqModel houseUpdateAvaiableRoom);
        public Task<ResultModel> GetCustomerByRoomId(Guid userId, Guid roomId);
        Task<ResultModel> GetRoomList(int page);
        Task<ResultModel> GetRoomInformation(Guid roomId);
        Task<ResultModel> UpdateRoom(RoomUpdateReqModel roomUpdateReqModel);
        Task<ResultModel> UpdateRoomStatus(RoomUpdateStatusReqModel roomUpdateStatusReqModel);
    }
}
