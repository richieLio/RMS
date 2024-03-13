namespace DataAccess.Models.ContractModel
{
    public class ContractResModel
    {
        public Guid Id { get; set; }

        public Guid? OwnerId { get; set; }

        public Guid? CustomerId { get; set; }

        public Guid? RoomId { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }

        public string? ImagesUrl { get; set; }

        public string? FileUrl { get; set; }

        public string? Status { get; set; }
    }

    public class ContractInfoResModel
    {
        public Guid Id { get; set; }

        public OwnerContractDetails Owner { get; set; }

        public CustomerContractDetails CustomerDetails { get; set; }

        public RoomContractDetails RoomDetails { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }

        public string? ImagesUrl { get; set; }

        public string? FileUrl { get; set; }

        public string? Status { get; set; }
    }

    public class OwnerContractDetails
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class CustomerContractDetails
    {
        public Guid Id { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? Gender { get; set; }

        public string? Dob { get; set; }

        public string? FullName { get; set; }

        public string? LicensePlates { get; set; }

        public string? CreatedAt { get; set; }

        public string? CitizenIdNumber { get; set; }
    }

    public class RoomContractDetails
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }
    }

    public class ContractOfRoomModel
    {
        public OwnerDetailModel? Owner { get; set; }

        public CustomerDetailModel? Customer { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? ImagesUrl { get; set; }

        public string? FileUrl { get; set; }

        public string? Status { get; set; }
    }

    public class OwnerDetailModel
    {
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? FullName { get; set; }
    }

    public class CustomerDetailModel
    {
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? FullName { get; set; }
    }
}
