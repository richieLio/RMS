namespace DataAccess.Entities;

public partial class House
{
    public Guid Id { get; set; }

    public Guid? OwnerId { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    /// <summary>
    /// Số lượng phòng
    /// </summary>
    public int? RoomQuantity { get; set; }

    public int? AvailableRoom { get; set; }

    public string? HouseAccount { get; set; }

    public byte[]? Password { get; set; }

    public byte[]? Salt { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual User? Owner { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
