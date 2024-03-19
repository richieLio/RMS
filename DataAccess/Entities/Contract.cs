using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class Contract
{
    public Guid Id { get; set; }

    public Guid? OwnerId { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? RoomId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? ImagesUrl { get; set; }

    public string? FileUrl { get; set; }

    public string? Status { get; set; }

    public virtual User? Customer { get; set; }

    public virtual User? Owner { get; set; }

    public virtual Room? Room { get; set; }
}
