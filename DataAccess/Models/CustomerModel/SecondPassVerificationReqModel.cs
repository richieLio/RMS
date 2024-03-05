using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.CustomerModel
{
    public class SecondPassVerificationReqModel
    {
        public Guid RoomId { get; set; }
        public Guid? HouseId { get; set; }
        public string SecondPassword { get; set; }
    }
}
