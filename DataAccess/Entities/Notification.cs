namespace DataAccess.Entities;

public partial class Notification
{
    public Guid Id { get; set; }

    public string? Content { get; set; }

    public Guid? HouseId { get; set; }

    public virtual House? House { get; set; }
}
