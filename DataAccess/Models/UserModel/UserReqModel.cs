using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccess.Models.UserModel
{
    public class UserReqModel
    {
        public string? Email { get; set; }

        public string Password { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
        public string? Gender { get; set; }

        public DateTime Dob { get; set; }

        public string? FullName { get; set; }
        [JsonIgnore]

        public string? Role { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
    public class UserLoginReqModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }

    }
    public class ChangePasswordReqModel
    {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
    public class UserResetPasswordReqModel
    {
        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
    public class UserVerifyOTPReqModel
    {
        public string Email { get; set; } = null!;
        public string OTPCode { get; set; } = null!;
    }
}
