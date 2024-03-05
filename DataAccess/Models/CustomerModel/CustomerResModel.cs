using DataAccess.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.CustomerModel
{
    public class CustomerResModel
    {

        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }

        public string? FullName { get; set; }
        /// Biển số xe
        public string? LicensePlates { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? CitizenIdNumber { get; set; }

    }
    public class CustomerLoginResModel
    {
        public HouseResModel User { get; set; }

        public string Token { get; set; }
    }
    public class HouseResModel
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

        public string? Status { get; set; }

    }
}
