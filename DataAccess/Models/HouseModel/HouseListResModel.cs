using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.HouseModel
{
    public class HouseListResModel
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
    }
}
