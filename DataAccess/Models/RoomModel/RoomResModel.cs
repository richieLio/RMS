using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.RoomModel
{
    public class RoomResModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public Guid? HouseId { get; set; }

        public string? Status { get; set; }
    }
}
