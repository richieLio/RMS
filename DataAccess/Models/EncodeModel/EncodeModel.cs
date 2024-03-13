namespace Data.Models.EncodeModel
{
    public class CreateHashPasswordModel
    {
        public byte[] Salt { get; set; }
        public byte[] HashedPassword { get; set; }
    }
}
