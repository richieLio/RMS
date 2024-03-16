using System.Text.Json.Serialization;

namespace DataAccess.Models.BillModel
{
    public class BillCreateReqModel
    {
        public Guid RoomId { get; set; }
        public Dictionary<Guid, decimal> ServiceQuantities { get; set; }
    }

}
