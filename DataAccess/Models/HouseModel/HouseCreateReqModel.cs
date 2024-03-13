using DataAccess.Models.RoomModel;
using System.Text.Json.Serialization;

namespace DataAccess.Models.HouseModel
{
    public class HouseCreateReqModel
    {
        public string? Name { get; set; }

        public string? Address { get; set; }

        /// <summary>
        /// Số lượng phòng
        /// </summary>
        public int? RoomQuantity { get; set; }
        [JsonIgnore]
        public int? AvailableRoom { get; set; }

        public string? HouseAccount { get; set; }

        public string? Password { get; set; } = null!;

    }
    public class HouseRoomCreateReqModel
    {
        public HouseCreateReqModel HouseCreateReqModel { get; set; }
        public RoomCreateReqModel RoomCreateReqModel { get; set; }
    }


}
