using System.Text.Json.Serialization;

namespace DataAccess.Models.BillModel
{
    public class BillCreateReqModel
    {

        public decimal? RentAmount { get; set; }
        /// Đơn giá điện
        public decimal? ElectricityUnitPrice { get; set; }
        /// Khối lượng điện đã sử dụng
        public double? ElectricityUsed { get; set; }
        /// Đơn giá nước
        public decimal? WaterUnitPrice { get; set; }
        /// Số lượng nước đã sử dụng
        public double? WaterUsed { get; set; }

        public decimal? ServicePrice { get; set; }
        [JsonIgnore]
        public decimal? TotalPice { get; set; }
        [JsonIgnore]
        public DateTime Month { get; set; }
        [JsonIgnore]
        public bool? IsPaid { get; set; }
        [JsonIgnore]
        public Guid? CreateBy { get; set; }

        public Guid? RoomId { get; set; }
    }
}
