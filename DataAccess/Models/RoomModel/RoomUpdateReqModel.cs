namespace DataAccess.Models.RoomModel
{
    public class RoomUpdateReqModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }
    }

    public class RoomUpdateStatusReqModel
    {
        public Guid Id { get; set; }
        public string? Status { get; set; }
    }
}
