using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.HouseModel
{
    public class HouseInfoResModel
    {
        public Guid Id { get; set; }

        public OwnerHouseInfo OwnerId { get; set; }

        public string? Name { get; set; }

        public string? Address { get; set; }

        /// <summary>
        /// Số lượng phòng
        /// </summary>
        public int? RoomQuantity { get; set; }

        public int? AvailableRoom { get; set; }

        public string? Status { get; set; }
    }

    public class OwnerHouseInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
