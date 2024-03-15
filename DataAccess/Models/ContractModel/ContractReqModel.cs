using Microsoft.AspNetCore.Http;

namespace DataAccess.Models.ContractModel
{
    public class ContractReqModel
    {
        public Guid Id { get; set; }

        public DateTime? EndDate { get; set; }

        public IFormFile ImagesUrl { get; set; }

    }

    public class ContractFileUploadReqModel
    {
        public Guid Id { get; set; }

    }
    public class ContractUpdateStatusReqModel
    {
        public Guid Id { get; set; }
        public string? Status { get; set; }
    }
}
