using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class Service
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<BillService> BillServices { get; set; } = new List<BillService>();
}
