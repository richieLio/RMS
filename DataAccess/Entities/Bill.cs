namespace DataAccess.Entities;

public partial class Bill
{
    public Guid Id { get; set; }

    public decimal? RentAmount { get; set; }

    /// <summary>
    /// Đơn giá điện
    /// </summary>
    public decimal? ElectricityUnitPrice { get; set; }

    /// <summary>
    /// Khối lượng điện đã sử dụng
    /// </summary>
    public double? ElectricityUsed { get; set; }

    /// <summary>
    /// Đơn giá nước
    /// </summary>
    public decimal? WaterUnitPrice { get; set; }

    /// <summary>
    /// Số lượng nước đã sử dụng
    /// </summary>
    public double? WaterUsed { get; set; }

    public decimal? ServicePrice { get; set; }

    public decimal? TotalPice { get; set; }

    public DateTime? Month { get; set; }

    public bool? IsPaid { get; set; }

    public DateTime? PaymentDate { get; set; }

    public Guid? CreateBy { get; set; }

    public Guid? RoomId { get; set; }

    public virtual User? CreateByNavigation { get; set; }

    public virtual Room? Room { get; set; }
}
