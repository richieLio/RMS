namespace DataAccess.Models.HouseModel
{
    public class HouseUpdateReqModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public string? Address { get; set; }


    }

    public class HouseUpdateAvaiableRoomReqModel
    {
        public Guid HouseId { get; set; }
        public int AvailableRoom { get; set; }

    }
    public class HouseUpdateStatusReqModel
    {
        public Guid Id { get; set; }
        public string? Status { get; set; }
    }

}
