using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class Bill
{
    public Guid Id { get; set; }

    public decimal? TotalPrice { get; set; }

    public DateTime? Month { get; set; }

    public bool? IsPaid { get; set; }

    public DateTime? PaymentDate { get; set; }

    public Guid? CreateBy { get; set; }

    public Guid? RoomId { get; set; }

    public virtual ICollection<BillService> BillServices { get; set; } = new List<BillService>();

    public virtual User? CreateByNavigation { get; set; }

    public virtual Room? Room { get; set; }
}
