namespace DataAccess.Models.HouseModel
{
    public class HouseListResModel
    {
        public Guid Id { get; set; }

        public OwnerHouseModel OwnerId { get; set; }

        public string? Name { get; set; }

        public string? Address { get; set; }

        /// <summary>
        /// Số lượng phòng
        /// </summary>
        public int? RoomQuantity { get; set; }

        public int? AvailableRoom { get; set; }
    }

    public class OwnerHouseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
