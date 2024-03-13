namespace DataAccess.Entities;

public partial class Payment
{
    public Guid Id { get; set; }

    public Guid? OwnerId { get; set; }

    public string? AccountType { get; set; }

    public string? AccountNumber { get; set; }

    public byte[]? QrcodeImage { get; set; }

    public string? TransferContent { get; set; }

    public virtual User? Owner { get; set; }
}
