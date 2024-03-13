namespace DataAccess.Models.CustomerModel
{
    public class SecondPassVerificationReqModel
    {
        public Guid RoomId { get; set; }
        public Guid? HouseId { get; set; }
        public string SecondPassword { get; set; }
    }
}
