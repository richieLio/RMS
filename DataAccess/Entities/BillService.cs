    using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class BillService
{
    public Guid BillId { get; set; }

    public Guid ServiceId { get; set; }

    public decimal? Quantity { get; set; }

    public virtual Bill Bill { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
