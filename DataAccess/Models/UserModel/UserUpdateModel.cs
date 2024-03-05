using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.UserModel
{
    public class UserUpdateModel
    {
        public Guid Id { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? Gender { get; set; }

        public DateTime? Dob { get; set; }

        public string? FullName { get; set; }

    }
    public class UserUpdatePasswordModel
    {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;

    }
}
