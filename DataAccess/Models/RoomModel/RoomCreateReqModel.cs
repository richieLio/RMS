using DataAccess.Models.CustomerModel;
using DataAccess.Models.HouseModel;
using System.Text.Json.Serialization;

namespace DataAccess.Models.RoomModel
{
    public class RoomCreateReqModel
    {
        [JsonIgnore]
        public Guid HouseId { get; set; }

        public string Name { get; set; }
    }
    public class AddCustomerToRoomReqModel
    {
        public CustomerCreateReqModel customerCreateReqModel { get; set; }
        public HouseUpdateAvaiableRoomReqModel houseUpdateAvaiableRoom { get; set; }
    }
}
