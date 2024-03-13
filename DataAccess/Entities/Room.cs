namespace DataAccess.Entities;

public partial class Room
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public Guid? HouseId { get; set; }

    public string? Status { get; set; }

    public byte[]? SecondPassword { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual House? House { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
