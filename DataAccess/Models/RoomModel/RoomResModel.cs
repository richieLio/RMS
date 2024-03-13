namespace DataAccess.Models.RoomModel
{
    public class RoomResModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public Guid? HouseId { get; set; }

        public string? Status { get; set; }
    }

    public class RoomInfoResModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public HouseDetails houseDetails { get; set; }

        public string? Status { get; set; }
    }

    public class HouseDetails
    {
        public Guid Id { get; set; }

        public OwnerHouseDetailsModel Owner { get; set; }

        public string? Name { get; set; }

        public string? Address { get; set; }

        public int? RoomQuantity { get; set; }

        public int? AvailableRoom { get; set; }

        public string? HouseAccount { get; set; }

        public string? Status { get; set; }
    }

    public class OwnerHouseDetailsModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
