﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccess.Models.RoomModel
{
    public class RoomCreateReqModel
    {
        [JsonIgnore]
        public Guid HouseId { get; set; }

        public string Name { get; set; }
    }
}
