using DataAccess.Models.RoomModel;
using System.Text.Json.Serialization;

namespace DataAccess.Models.HouseModel
{
    public class HouseCreateReqModel
    {
        public string? Name { get; set; }

        public string? Address { get; set; }


    }
    public class HouseRoomCreateReqModel
    {
        public HouseCreateReqModel HouseCreateReqModel { get; set; }
        public RoomCreateReqModel RoomCreateReqModel { get; set; }
    }


}
