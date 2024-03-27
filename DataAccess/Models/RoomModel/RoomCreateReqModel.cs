using DataAccess.Models.CustomerModel;
using DataAccess.Models.HouseModel;
using System.Text.Json.Serialization;

namespace DataAccess.Models.RoomModel
{
    public class RoomCreateRangeReqModel
    {
        public Guid HouseId { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
    }
    public class RoomCreateReqModel
    {
        public Guid RoomId { get; set; }
        public Guid HouseId { get; set; }
        public string Name { get; set; }
    }
    public class AddCustomerToRoomReqModel
    {
        public CustomerCreateReqModel customerCreateReqModel { get; set; }
        public HouseUpdateAvaiableRoomReqModel houseUpdateAvaiableRoom { get; set; }
    }
}
