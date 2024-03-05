using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.RoomModel
{
    public class RoomCreateReqModel
    {
        public Guid HouseId { get; set; }
        public string Name { get; set; }
    }
}
