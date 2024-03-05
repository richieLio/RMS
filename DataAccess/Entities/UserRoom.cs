using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class UserRoom
{
    public Guid UserId { get; set; }

    public Guid RoomId { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
