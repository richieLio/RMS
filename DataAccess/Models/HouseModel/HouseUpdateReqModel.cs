﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.HouseModel
{
    public class HouseUpdateReqModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public string? Address { get; set; }

        /// <summary>
        /// Số lượng phòng
        /// </summary>
        public int? RoomQuantity { get; set; }

        public int? AvailableRoom { get; set; }
    }

    public class HouseUpdateAvaiableRoomReqModel
    {
        public Guid HouseId { get; set; }
        public int AvailableRoom { get; set; }

    }

}
