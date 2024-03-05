using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Enums
{
    public class UserStatus
    {
        public static readonly string ACTIVE = "Active";
        public static readonly string RESETPASSWORD = "ResetPassword";
        public static readonly string INACTIVE = "Inactive";
    }

    public class ClassStatus
    {
        public static readonly string ACTIVE = "Active";
        public static readonly string INACTIVE = "Inactive";
    }

    public class GeneralStatus
    {
        public static readonly string ACTIVE = "Active";
        public static readonly string INACTIVE = "Inactive";
    }
}
