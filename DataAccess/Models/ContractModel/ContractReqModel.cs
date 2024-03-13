namespace DataAccess.Models.ContractModel
{
    public class ContractReqModel
    {
        public Guid Id { get; set; }

        public DateTime? EndDate { get; set; }

        public string? ImagesUrl { get; set; }

        public string? FileUrl { get; set; }
    }

    public class ContractUpdateStatusReqModel
    {
        public Guid Id { get; set; }
        public string? Status { get; set; }
    }
}
