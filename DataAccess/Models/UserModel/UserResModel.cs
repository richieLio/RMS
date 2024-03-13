using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.UserModel
{
    public class UserResModel
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime Dob { get; set; }

        public string Address { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public string Role { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string Status { get; set; } = null!;
    }
    public class UserLoginResModel
    {
        public UserResModel User { get; set; }

        public string Token { get; set; }
    }
    public class UserVerifyOTPResModel
    {
        public string Email { get; set; } = null!;
        public string OTPCode { get; set; } = null!;
    }


}
